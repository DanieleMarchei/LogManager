using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LogManager;

namespace ConsoleTest
{
    class Program
    {
        static ConcurrentDictionary<Guid, double> AvgDict = new ConcurrentDictionary<Guid, double>();

        static void Print()
        {
            const int NLOGS = 200;
            for (int i = 0; i < NLOGS; i++)
            {

                Log l = new Log(LogLevel.DEBUG, "This is a test log");
                LogManager.Trace.Write(l);
            }
        }

        static void Main(string[] args)
        {
            LogManager.Trace.Connect("TestConcurrent2");
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Factory.StartNew(Print));
            }

            Stopwatch s = new Stopwatch();
            s.Start();

            Task.WaitAll(tasks.ToArray());
            s.Stop();
            long time = s.ElapsedMilliseconds;
            Console.WriteLine(time);
            LogManager.Trace.Flush();

            using (StreamWriter file = new StreamWriter("ArbiterConcurrentTrace_benchmark.txt"))
                foreach (var entry in AvgDict)
                    file.WriteLine("{0} , {1}", entry.Key.ToString().Substring(0, 4), entry.Value.ToString().Replace(',', '.'));

            Console.WriteLine("done");
            Console.ReadLine();

        }
    }
}
