using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogManager
{
    /// <summary>
    /// Represents the location where the log was generated.
    /// </summary>
    public class Origin
    {
        public string IdDevice { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public int ProcId { get; private set; }
        public int ThreadId { get; private set; }

        public string MethodName { get; set; }

        public Origin() : this("") { }

        public Origin(int methodId) : this(methodId.ToString()) { }

        public Origin(string methodName)
        {
            IdDevice = Dns.GetHostName();

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPAddress =  ip;
                }
            }

            ProcId = Process.GetCurrentProcess().Id;

            ThreadId = Thread.CurrentThread.ManagedThreadId;

            MethodName = methodName;
        }
    }
}
