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
            List<string> stringhe = new List<string>() {"Ciao", "Come va?", "Tutto bene", "Random text", "Oh bè", "Non so", "Mmmmmh",
            "asdasd" ,"MongoDB", "Test", "boh boh"};

            Random r = new Random();
            double avg = 0;

            for (int i = 0; i < 20; i++)
            {
                int rand = r.Next(stringhe.Count);

                Log l = new Log(LogLevel.DEBUG, stringhe[rand]);
                Stopwatch stopw = new Stopwatch();
                stopw.Start();

                ArbiterConcurrentTrace.Write(l);

                stopw.Stop();
                long finish = stopw.ElapsedMilliseconds;
                avg += finish;
            }

            avg = avg / 20;
            AvgDict.GetOrAdd(Guid.NewGuid(), avg);

        }

        static void Main(string[] args)
        {
            ArbiterConcurrentTrace.BufferSize = 64;
            ArbiterConcurrentTrace.NumberOfBuffers = 30;

            ArbiterConcurrentTrace.Connect("TestConcurrent2");
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Factory.StartNew(Print));
            }

            Task.WaitAll(tasks.ToArray());

            ArbiterConcurrentTrace.Flush();

            using (StreamWriter file = new StreamWriter("ArbiterConcurrentTrace_benchmark.txt"))
                foreach (var entry in AvgDict)
                    file.WriteLine("{0},{1}", entry.Key, entry.Value.ToString().Replace(',','.'));

            Console.WriteLine("finito");
            Console.ReadLine();

        }
    }
}
