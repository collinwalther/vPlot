using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DataSourceClient
{
    internal static class Program
    {
        private const int NUM_COLS = 4;
        private const int RANGE_MIN = 0;
        private const int RANGE_MAX = 100;

        private static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine("Usage: DataSourceClient <ip> <port> <msecSendInterval> (-r <# rows> | -f <filepath>)");
                return;
            }

            var client = new DataSource.Client(IPAddress.Parse(args[0]), ushort.Parse(args[1]));

            switch (args[3])
            {
                case "-f":
                    using (var file = new StreamReader(args[4]))
                    {
                        client.RunFile(file, ushort.Parse(args[2]));
                    }

                    break;

                case "-r":
                    var colProps = new List<DataSource.Range>();

                    for (int i = 0; i < NUM_COLS; i++)
                    {
                        colProps.Add(new DataSource.Range(RANGE_MIN, RANGE_MAX));
                    }

                    client.RunRandom(colProps.ToArray(), uint.Parse(args[4]), ushort.Parse(args[2]));
                    break;

                default:
                    Console.WriteLine("Usage: DataSourceClient.exe <ip> <port> <msecSendInterval> (-r <# rows> | -f <filepath>)");
                    break;
            }
        }
    }
}