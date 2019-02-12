using System;
using System.Collections.Generic;
using LogManager;

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
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
