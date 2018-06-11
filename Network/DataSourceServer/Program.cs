using System;
using System.Collections.Generic;
using System.IO;

namespace DataSourceServer
{
    internal static class Program
    {
        private const int NUM_COLS = 4;
        private const int RANGE_MIN = 0;
        private const int RANGE_MAX = 100;
        
        private static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: DataSourceServer <port> <msecSendInterval> (-r <# rows> | -f <filepath>)");
                return;
            }

            var server = new DataSource.Server(ushort.Parse(args[0]));

            switch (args[2])
            {
                case "-f":
                    using (var file = new StreamReader(args[4]))
                    {
                        server.RunFile(file, ushort.Parse(args[1]));
                    }

                    break;
                
                case "-r":
                    var colProps = new List<DataSource.Range>();

                    for (int i = 0; i < NUM_COLS; i++)
                    {
                        colProps.Add(new DataSource.Range(RANGE_MIN, RANGE_MAX));
                    }
                    
                    server.RunRandom(colProps.ToArray(), uint.Parse(args[3]), ushort.Parse(args[1]));
                    break;
                
                default:
                    Console.WriteLine("Usage: DataSourceServer <port> <msecSendInterval> (-r <# rows> | -f <filepath>)");
                    break;
            }
        }
    }
}