using LogManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace Benchmarks
{
    public abstract class BenchmarkTool
    {
        public int ThreadNumber, MaxLogs, Resolution;
        private List<double> ResultsList = new List<double>();

        public BenchmarkTool(int threads, int maxLogs, int resolution)
        {
            ThreadNumber = threads;
            MaxLogs = maxLogs;
            Resolution = resolution;
        }

        public abstract void Start(string collectionName);

        public abstract void ThreadWork();

        public List<double> Results()
        {
            return ResultsList;
        }
    }
}
