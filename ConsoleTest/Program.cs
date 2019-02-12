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
        static void Print()
        {
            const int NLOGS = 200;
            for (int i = 0; i < NLOGS; i++)
            {

                Log l = new Log(LogLevel.DEBUG, "This is a test log");
                LogManager.TraceLog.Write(l);
            }
        }

        static void Main(string[] args)
        {
            TraceLog.Connect("TestConcurrent2");

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

            LogManager.TraceLog.Flush();

            Console.WriteLine("done");
            Console.ReadLine();

        }
    }
}
