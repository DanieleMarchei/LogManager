using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net;
using LogManager;
using MongoDB.Driver;

namespace Benchmarks
{
    class Program
    {
        private class BenchmarkTool
        {
            int ThreadNumber, MaxLogs, Resolution;
            public BenchmarkTool(int threadNumber, int maxLogs, int resolution)
            {
                ThreadNumber = threadNumber;
                MaxLogs = maxLogs;
                Resolution = resolution;
            }

            public void Start(string collectionName)
            {
                TraceLog.Connect(collectionName);

            }

            public List<double> Results()
            {
                throw new NotImplementedException();
            }

        }

        //table containing all the vaerage values
        public static Dictionary<int, double> dictionary = new Dictionary<int, double>();

        static void Main(string[] args)
        {
            TraceLog.Connect("Benchmark3");

            var client = new MongoClient($"mongodb://localhost:27017");
            var database = client.GetDatabase(Dns.GetHostName());
            var collection = database.GetCollection<Log>("Benchmark3");

            Stopwatch stopwatch = new Stopwatch();

            //number of values for the average
            int avgNumb = 5;
            double[] avgValues = new double[avgNumb];
            //we will cycle 256 times
            //each time is a multiple of 64, and each time is repeated 3 times
            //16384 = 256 * 64
            for (int i = 1; i <= 10; i++)
            {
                //how many times we need to write
                int limit = 64 * i;

                Console.Write(i);

                //run the test 3 times
                for (int k = 0; k < avgNumb; k++)
                {
                    for (int j = 0; j < limit; j++)
                    {
                        TraceLog.Write(new Log(LogLevel.DEBUG, "Benchmark 3 Benchmark 3 Benchmark 3 Benchmark 3 Benchmark 3 Benchmark 3"));
                    }

                    //measure the time
                    stopwatch.Start();
                    TraceLog.Flush();
                    stopwatch.Stop();

                    //add the time elapsed ti the average values
                    avgValues[k] = stopwatch.Elapsed.TotalMilliseconds;

                    stopwatch.Reset();

                    //clear the collection
                    collection.DeleteMany(l => true);
                    Console.Write($"...{k+1}");

                }

                //add the average to the dictionary
                dictionary.Add(limit, avgValues.Average());
                Console.WriteLine();

            }

            //save the values in a file
            using (StreamWriter writetext = new StreamWriter(@"C:\Users\Daniele\Desktop\benchmarks\benchmark3_noDelete.txt"))
            {
                writetext.WriteLine("batch size,time (ms)");
                foreach (var kv in dictionary)
                {
                    writetext.WriteLine($"{kv.Key},{kv.Value.ToString().Replace(',','.')}");
                }
            }

            Console.WriteLine("finito");
            Console.Read();
        }
    }
}
