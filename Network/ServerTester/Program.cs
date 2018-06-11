using System;
using System.Threading;

namespace ServerTester
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ServerTester [port]");
                return;
            }

            Server server;
            var dataStore = new DataStore();

            // attempt to parse the port number
            try
            {
                var port = ushort.Parse(args[0]);
                server = new Server(port, dataStore);
                Console.WriteLine("Listening on {0}:{1}", server.GetIPAddress(), port);
            }
            catch (Exception e) when (e is FormatException || e is OverflowException ||
                                      e is ArgumentOutOfRangeException)
            {
                Console.WriteLine("Error: port must be an integer in range [1024, 65535]");
                return;
            }
            
            // run the server asynchronously
            var thread = new Thread(server.Run);
            thread.Start();
            
            // retrieve the contents of the data store
            while (true)
            {
                var data = dataStore.Dequeue();
                
                if (data.Length == 0)
                {
                    if (!thread.IsAlive)
                        break;
                    
                    Thread.Sleep(1000);
                }

                foreach (var row in data)
                {
                    foreach (var field in row)
                    {
                        Console.Write("{0} ", field);
                    }
                    Console.Write("\n");
                }
            }
        }
    }
}