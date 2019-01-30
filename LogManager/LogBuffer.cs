using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    /// <summary>
    /// Data structue for cacheing logs
    /// </summary>
    internal class LogBuffer
    {
        public bool InUse { get; set; }
        public bool Full { get; private set; }
        public delegate void BufferFilled(Log log);
        public event BufferFilled OnBufferFill;

        public int CurrentIndex { get; private set; }
        private Log[] _logs { get; set; }
        public Log[] Logs
        {
            get
            {
                return _logs.Where(l => l != null).ToArray();
            }
        }

        public LogBuffer()
        {
            InUse = false;
            Full = false;
            CurrentIndex = 0;
            _logs = new Log[ArbiterConcurrentTrace.BufferSize];
        }

        /// <summary>
        /// Adds a log to the buffer.
        /// </summary>
        /// <param name="log">The log to be added</param>
        public void Add(Log log)
        {
            if (CurrentIndex >= ArbiterConcurrentTrace.BufferSize) throw new LogBufferSizeExceededException($"Tried to add a Log into a filled buffer of size {ArbiterConcurrentTrace.BufferSize}.");

            int l = _logs.Length;
            _logs[CurrentIndex] = log;

            CurrentIndex++;

            if (CurrentIndex == ArbiterConcurrentTrace.BufferSize)
            {
                Full = true;
                if (OnBufferFill != null)
                    OnBufferFill(log);

                return;
            }

        }

        /// <summary>
        /// Cleans the buffer.
        /// </summary>
        public void Clear()
        {
            CurrentIndex = 0;
            _logs = new Log[ArbiterConcurrentTrace.BufferSize];
            Full = false;
        }
    }
}
