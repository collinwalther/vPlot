using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerTester
{
    internal static class ServerTester
    {
        private class DummyClient
        {
            private readonly Socket socket;
            private readonly IPAddress ip;
            private readonly ushort port;
            private readonly StreamReader file;

            public DummyClient(IPAddress ip, ushort port, StreamReader file)
            {
                if (port < 1024)
                    throw new ArgumentOutOfRangeException();

                this.ip = ip;
                this.port = port;

                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                this.file = file;
            }

            public void Run()
            {
                socket.Connect(new IPEndPoint(ip, port));
            
                if (file != null)
                {
                    string data;

                    while ((data = file.ReadLine()) != null)
                    {
                        socket.Send(Encoding.ASCII.GetBytes(data + "\n"));
                    }
                }

                else
                {
                    var rand = new Random();

                    while (true)
                    {
                        Thread.Sleep((int) rand.NextDouble() * 100 + 1000);
                        string toSend = string.Format("{0}, {1}, {2}\n",
                            rand.Next(0, 100),
                            rand.Next(0, 100),
                            rand.Next(0, 100)
                        );

                        socket.Send(Encoding.ASCII.GetBytes(toSend));

                        if (rand.Next(0, 100) < 25)
                            break;
                    }
                }
            
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    
        private static void Main(string[] args)
        {
            const ushort port = 12345;

            var dataStore = new DataStore();
            var file = new StreamReader("../../../Assets/TestData/short.csv");
            var server = new Server(port, dataStore);
            var client = new DummyClient(IPAddress.Parse(server.GetIPAddress()), port, file);
            

            // run the server asynchronously
            var serverThread = new Thread(server.Run);
            serverThread.Start();
        
            // run the client asynchronously
            var clientThread = new Thread(client.Run);
            clientThread.Start();
		
            // retrieve the contents of the data store
            while (true)
            {
                var data = dataStore.Dequeue();
                if (data.Length != 0)
                {
                    foreach (var row in data)
                    {
                        foreach (var f in row)
                            Console.Write("{0} ", f);
                        Console.Write("\n");
                    }
                }
                else
                    Thread.Sleep(1000);
            }
        }
    }
}