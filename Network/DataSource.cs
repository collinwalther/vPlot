using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DataSource
{
    public class Range
    {
        public int Min { get; }
        public int Max { get; }

        public Range(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }

    internal static class DataSource
    {
        private static void SendRandomRow(Socket socket, Random random, Range[] colProps, ushort msecSendInterval)
        {
            string data = "";

            foreach (var prop in colProps)
            {
                data += random.Next(prop.Min, prop.Max) + ",";
            }

            data = data.Substring(0, data.Length - 1);
            Console.WriteLine("sending: " + data);
            socket.Send(Encoding.ASCII.GetBytes(data + "\n"));
            Thread.Sleep(msecSendInterval);
        }
        
        public static void RunFile(Socket socket, StreamReader file, ushort msecSendInterval = 0)
        {
            if (!socket.Connected)
                throw new InvalidOperationException("socket not connected");
            
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            string data;
            while ((data = file.ReadLine()) != null)
            {
                Console.WriteLine("sending: " + data);
                socket.Send(Encoding.ASCII.GetBytes(data + "\n"));
                Thread.Sleep(msecSendInterval);
            }
            
            Console.WriteLine("\nDone");
        }

        public static void RunRandom(Socket socket, Range[] colProps, uint nRows = 0, ushort msecSendInterval = 0)
        {
            var random = new Random();
            
            if (colProps == null)
                throw new ArgumentNullException(nameof(colProps));

            if (!socket.Connected)
                throw new InvalidOperationException("socket not connected");

            // case 1: nRows == 0, never stop generating data
            if (nRows == 0)
            {
                Console.WriteLine("nRows = 0, generating data until stopped");
                
                while (true)
                {
                    SendRandomRow(socket, random, colProps, msecSendInterval);
                }
            }

            // case 2: nRows != 0, generate that many rows
            for (uint i = 0; i < nRows; i++)
            {
                SendRandomRow(socket, random, colProps, msecSendInterval);
            }
            
            Console.WriteLine("\nDone");
        }
    }

    public class Client
    {
        private readonly Socket socket;
        private readonly IPAddress ip;
        private readonly ushort port;

        public Client(IPAddress ip, ushort port)
        {
            if (port < 1024)
                throw new ArgumentOutOfRangeException(nameof(port), "port must be in range [1024, 65535]");

            this.ip = ip;
            this.port = port;
            
            socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void RunFile(StreamReader file, ushort msecSendInterval = 0)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            
            socket.Connect(new IPEndPoint(ip, port));
            DataSource.RunFile(socket, file, msecSendInterval);
            
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public void RunRandom(Range[] colProps, uint nRows = 0, ushort msecSendInterval = 0)
        {
            if (colProps == null)
                throw new ArgumentNullException(nameof(colProps));
            
            socket.Connect(new IPEndPoint(ip, port));
            DataSource.RunRandom(socket, colProps, nRows, msecSendInterval);
            
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
    
    public class Server
    {
        private readonly Socket listenSocket;
        private readonly IPAddress ip;
        private readonly ushort port;
        
        public Server(ushort port)
        {
            if (port < 1024)
                throw new ArgumentOutOfRangeException(nameof(port), "port must be in range [1024, 65535]");

            this.port = port;
            
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

        private Socket ListenAndAccept()
        {
            listenSocket.Listen(0);
            Console.WriteLine("Listening on [{0}]:{1}", ip, port);
            return listenSocket.Accept();
        }

        public void RunFile(StreamReader file, ushort msecSendInterval = 0)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            
            var socket = ListenAndAccept();
            DataSource.RunFile(socket, file, msecSendInterval);
            
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        
        public void RunRandom(Range[] colProps, uint nRows = 0, ushort msecSendInterval = 0)
        {
            if (colProps == null)
                throw new ArgumentNullException(nameof(colProps));
            
            var socket = ListenAndAccept();
            DataSource.RunRandom(socket, colProps, nRows, msecSendInterval);
                        
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}