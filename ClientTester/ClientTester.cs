using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientTester
{
    internal static class ClientTester
    {
        private class DummyServer
        {
            private readonly Socket listenSocket;
            public readonly IPAddress ip;
            private readonly ushort port;
            private readonly StreamReader file;

            public DummyServer(ushort port, StreamReader file)
            {
                if (port < 1024)
                    throw new ArgumentOutOfRangeException();

                this.port = port;
                this.file = file;
            
                // choose an IP address based on the following criteria:
                //	- cannot be an IPv6 Link Local address
                foreach (var addr in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (addr.IsIPv6LinkLocal) continue;
                    ip = addr;
                    break;
                }
            
                listenSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(new IPEndPoint(ip, port));
            }

            public void Run()
            {
                listenSocket.Listen(0);
                var acceptSocket = listenSocket.Accept();
                string data;

                while ((data = file.ReadLine()) != null)
                {
                    acceptSocket.Send(Encoding.ASCII.GetBytes(data + "\n"));
                }
            
                acceptSocket.Shutdown(SocketShutdown.Both);
                acceptSocket.Close();
            }
        }
    
        private static void Main(string[] args)
        {
            const ushort port = 12345;
        
            var dataStore = new DataStore();
            var file = new StreamReader("../../../Assets/TestData/short.csv");
            var server = new DummyServer(port, file);
            var client = new Client(server.ip, port, dataStore);

            var serverThread = new Thread(server.Run);
            serverThread.Start();
        
            var clientThread = new Thread(client.Run);
            clientThread.Start();
        
            // retrieve the contents of the data store
            while (clientThread.IsAlive)
                Thread.Sleep(1000);
        
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
        }
    }
}