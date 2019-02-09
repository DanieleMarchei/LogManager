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
        /// <summary>
        /// Get the id of the device.
        /// </summary>
        public string IdDevice { get; private set; }

        /// <summary>
        /// Get the ip address of the device.
        /// </summary>
        public IPAddress IPAddress { get; private set; }

        /// <summary>
        /// Get the pid of the process.
        /// </summary>
        public int ProcId { get; private set; }

        /// <summary>
        /// Get the thread id.
        /// </summary>
        public int ThreadId { get; private set; }

        /// <summary>
        /// Get or set the method name.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Creates a new instance of the Origin class.
        /// </summary>
        public Origin() : this("") { }

        /// <summary>
        /// Creates a new instance of the Origin class.
        /// </summary>
        /// <param name="methodId">The method id</param>
        public Origin(int methodId) : this(methodId.ToString()) { }

        /// <summary>
        /// Creates a new instance of the Origin class.
        /// </summary>
        /// <param name="methodName">The method name</param>
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
