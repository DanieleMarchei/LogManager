using System;
using System.Collections.Generic;
using System.Text;

namespace Benchmarks
{
    class BenchmarkVariableLogFrequence : BenchmarkTool
    {
        public BenchmarkVariableLogFrequence(int threads, int maxLogs, int resolution) : base(threads, maxLogs, resolution)
        {
        }

        public override void Start(string collectionName)
        {
            throw new NotImplementedException();
        }

        public override void ThreadWork()
        {
            throw new NotImplementedException();
        }
    }
}
