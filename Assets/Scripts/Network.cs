using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Provides thread-safe access to a message queue.
/// </summary>
public class DataStore
{
    private readonly object locker = new object();
    private readonly Queue<float[]> data = new Queue<float[]>();

    public string[] Labels { get; set; }

    public float[][] Dequeue()
    {
        float[][] result;

        lock (locker)
        {
            result = data.ToArray();
            data.Clear();
        }

        return result;
    }

    public void Enqueue(float[] row)
    {
        lock (locker)
        {
            data.Enqueue(row);
        }
    }
}

/// <summary>
/// Contains parsing methods that are common between Client and Server.
/// </summary>
/// <remarks>
/// The automatic timestamp assignment feature represents time as an integer multiple of milliseconds.  A 32-bit IEEE
/// floating-point type specifies a mantissa of 23 bits, meaning that the longest time that can be accurately
/// represented is 2^24 + 1 ms = 16,777,217 ms ~ 4.6 hours (https://stackoverflow.com/a/3793950).  Values up to
/// float.MaxValue will still be represented, but with decreasing resolution (i.e. not accurate to the millisecond).
/// </remarks>
internal class Parser
{
    private const char ROW_DELIM = '\n';
    private const char COL_DELIM = ',';

    private readonly DataStore dataStore;
    private readonly Stopwatch stopwatch;   // used to assign timestamps to CSV rows
    private string partialData = "";

    public bool firstRow = true;

    public Parser(DataStore dataStore, bool useTimestamps = false)
    {
        this.dataStore = dataStore;

        if (useTimestamps)
        {
            stopwatch = new Stopwatch();
        }
    }

    private static float ParseValue(string value)
    {
        float result;
        try
        {
            result = float.Parse(value);
        }
        catch (FormatException)
        {
            result = float.NaN;
        }
        catch (OverflowException)
        {
            result = value.IndexOf('-') == -1 ? float.MaxValue : float.MinValue;
        }

        return result;
    }

    private float[] ParseRow(string row)
    {
        var result = new List<float>();

        int next;
        int prev = 0;

        // parse fields from the row until no more delimiters are found
        while ((next = row.IndexOf(COL_DELIM, prev)) != -1)
        {
            result.Add(ParseValue(row.Substring(prev, next - prev)));
            prev = next + 1;
        }

        // parse the last field in the row unless the row ended with a delimiter
        if (prev != row.Length)
        {
            result.Add(ParseValue(row.Substring(prev, row.Length - prev)));
        }

        // if timestamps are being used, insert one as the final field in the row
        if (stopwatch != null)
        {
            if (stopwatch.IsRunning)
            {
                result.Add(stopwatch.ElapsedMilliseconds);
            }
            else
            {
                stopwatch.Start();
                result.Add(0);
            }
        }

        return result.ToArray();
    }

    private string[] ParseLabels(string row)
    {
        var result = new List<string>();

        int next;
        int prev = 0;

        // parse labels from the row until no more delimiters are found
        while ((next = row.IndexOf(COL_DELIM, prev)) != -1)
        {
            result.Add(row.Substring(prev, next - prev));
            prev = next + 1;
        }

        // parse the last label in the row unless the row ended with a delimiter
        if (prev != row.Length)
        {
            result.Add(row.Substring(prev, row.Length - prev));
        }

        // if timestamps are being used, insert the corresponding label
        if (stopwatch != null)
        {
            result.Add("timestamp");
        }

        return result.ToArray();
    }

    public void Parse(string data)
    {
        int next;
        int prev = 0;

        // parse rows until no more ROW_DELIM chars are found
        while (prev < data.Length && (next = data.IndexOf(ROW_DELIM, prev)) != -1)
        {
            var row = ParseRow(partialData + data.Substring(prev, next - prev));

            // if the first row could not be parsed as floats, treat it as column labels
            if (firstRow)
            {
                if (float.IsNaN(row[0]))
                {
                    dataStore.Labels = ParseLabels(partialData + data.Substring(prev, next - prev));
                }
                else
                {
                    string[] labels = new string[row.Length];
                    for (int i = 0; i < row.Length; i++)
                    {
                        labels[i] = "Feature " + (i + 1);
                    }
                    dataStore.Labels = labels;
                }
            }
            else
            {
                dataStore.Enqueue(row);
            }

            partialData = "";
            firstRow = false;
            prev = next + 1;
        }

        // if unparsed data remains, store it temporarily
        if (prev < data.Length)
            partialData = data.Substring(prev);
    }
}

/// <summary>
/// Defines the necessary components for a class to implement a client.
/// A client initiates a connection to a data source.
/// </summary>
public abstract class ClientBase
{
    protected readonly Socket socket;
    protected readonly IPAddress ip;
    protected readonly ushort port;
    protected readonly DataStore dataStore;

    protected ClientBase(IPAddress ip, ushort port, DataStore dataStore)
    {
        if (port < 1024)
            throw new ArgumentOutOfRangeException("port", "port must be in range [1024, 65535]");

        this.ip = ip;
        this.port = port;
        this.dataStore = dataStore;
        socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    public abstract void Run();
}

public class Client : ClientBase
{
    private const uint BUFFER_SIZE = 1024;

    private readonly Parser parser;

    public Client(IPAddress ip, ushort port, DataStore dataStore) : base(ip, port, dataStore)
    {
        parser = new Parser(dataStore);
    }

    public override void Run()
    {
        var buffer = new byte[BUFFER_SIZE];

        // Connect to the given IP and port
        socket.Connect(new IPEndPoint(ip, port));

        // handle connection until the server closes it
        // or the thread is terminated
        while (true)
        {
            var nBytes = socket.Receive(buffer);

            // if Socket.Receive() returns 0, the server has closed the connection
            if (nBytes == 0)
                break;

            var msg = Encoding.ASCII.GetString(buffer, 0, nBytes);
            parser.Parse(msg);
        }
    }
}

/// <summary>
/// Defines the necessary components for a class to implement a server.
/// A server listens on a socket and waits for a data source to connect.
/// </summary>
public abstract class ServerBase
{
    protected readonly Socket listenSocket;
    protected readonly IPAddress ip;
    protected readonly ushort port;
    protected readonly DataStore dataStore;

    protected ServerBase(ushort port, DataStore dataStore)
    {
        if (port < 1024)
            throw new ArgumentOutOfRangeException("port", "port must be in range [1024, 65535]");

        this.port = port;
        this.dataStore = dataStore;

        // choose an IP address based on the following criteria:
        //	- cannot be an IPv6 Link Local address
        foreach (var addr in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (addr.IsIPv6LinkLocal) continue;
            ip = addr;
            break;
        }
        if (ip == null)
            throw new InvalidOperationException("failed to assign IP address");
        listenSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listenSocket.Bind(new IPEndPoint(ip, port));
    }

    public string GetIPAddress()
    {
        return ip.ToString();
    }

    public abstract void Run();
}

/// <summary>
/// Defines a reference implementation of a server for the vPlot network streaming interface.
/// It binds to a listen socket and awaits a connection from the data source.
/// Once a connection is established, it reads messages and attempts to parse them as CSV containing floats.
/// Overflows are replaced with MaxValue or MinValue, and invalid conversions are replaced with NaN.
/// Invalid conversions in the first row of data received from a given connection is treated as column labels.
/// Parsed values are stored in the DataStore object referenced in construction.
/// </summary>
public class Server : ServerBase
{
    private const uint BUFFER_SIZE = 1024;

    private readonly Parser parser;

    public Server(ushort port, DataStore dataStore) : base(port, dataStore)
    {
        parser = new Parser(dataStore, true);
    }

    public override void Run()
    {
        var buffer = new byte[BUFFER_SIZE];

        // begin listening on the assigned IP and port
        // do not allow any backlogged connections
        listenSocket.Listen(0);

        // handle incoming connections until the process is terminated
        while (true)
        {
            var connectionSocket = listenSocket.Accept();

            // handle the current connection
            parser.firstRow = true;
            while (true)
            {
                var nBytes = connectionSocket.Receive(buffer);

                // if Socket.Receive() returns 0, the client has closed the connection
                if (nBytes == 0)
                    break;

                var msg = Encoding.ASCII.GetString(buffer, 0, nBytes);
                parser.Parse(msg);
            }
        }
    }
}