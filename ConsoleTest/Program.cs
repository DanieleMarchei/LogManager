using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LogManager;

namespace ConsoleTest
{
    class Program
    {
        static void Print()
        {
            List<string> stringhe = new List<string>() {"Ciao", "Come va?", "Tutto bene", "Random text", "Oh bè", "Non so", "Mmmmmh",
            "asdasd" ,"MongoDB", "Test", "boh boh"};

            Random r = new Random();
            int rand = r.Next(stringhe.Count);

            Log l = new Log(LogLevel.DEBUG, DateTime.Now, new Origin(Thread.CurrentThread.ManagedThreadId.ToString()), stringhe[rand]);
            ConcurrentTrace.Write(l);

        }

        static void Main(string[] args)
        {
            ConcurrentTrace.BufferSize = 5;
            ConcurrentTrace.NumberOfBuffers = 2;

            ConcurrentTrace.Connect("TestConcurrent2");
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Factory.StartNew(Print));
            }

            Task.WaitAll(tasks.ToArray());

            ConcurrentTrace.Flush();

        }
    }
}
