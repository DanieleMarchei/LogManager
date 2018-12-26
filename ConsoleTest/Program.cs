using System;
using System.Threading;
using LogManager;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Connect("Test1");

            Log l = new Log(LogLevel.DEBUG, DateTime.Now, new Origin("MAIN"), "CIAOOONEE");

            Trace.Write(l);
            Trace.Flush();

        }
    }
}
