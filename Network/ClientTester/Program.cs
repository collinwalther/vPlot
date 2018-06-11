using System;
using System.Net;
using System.Threading;

namespace ClientTester
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // ensure the user passed 2 arguments
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: ClientTester [ip] [port]");
                return;
            }

            IPAddress ip;
            Client client;
            var dataStore = new DataStore();

            // attempt to parse ip from argument list
            try
            {
                ip = IPAddress.Parse(args[0]);
            }

            // exit if the argument cannot be parsed correctly
            catch (FormatException)
            {
                Console.WriteLine("Error: invalid IP address");
                return;
            }

            // attempt to parse port from argument list
            try
            {
                var port = ushort.Parse(args[1]);
                client = new Client(ip, port, dataStore);
            }

            // exit if the argument cannot be parsed correctly
            catch (Exception e)
            {
                if (!(e is FormatException || e is OverflowException || e is ArgumentOutOfRangeException))
                    throw;

                Console.WriteLine("Error: port must be an integer in range [1024, 65535]");
                return;
            }

            // run the client asynchronously
            var thread = new Thread(client.Run);
            thread.Start();

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
                {
                    if (!thread.IsAlive)
                        break;
                    Thread.Sleep(1);
                }
            }
            
            Console.WriteLine("\nDone");
        }
    }
}