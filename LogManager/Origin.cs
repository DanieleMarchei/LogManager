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
        /// Gets the device's Id.
        /// </summary>
        public string IdDevice { get; private set; }

        /// <summary>
        /// Gets the device's IPAddress.
        /// </summary>
        public IPAddress IPAddress { get; private set; }

        /// <summary>
        /// Gets the the Process' ID.
        /// </summary>
        public int ProcId { get; private set; }

        /// <summary>
        /// Gets the the id of the thread.
        /// </summary>
        public int ThreadId { get; private set; }

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
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
