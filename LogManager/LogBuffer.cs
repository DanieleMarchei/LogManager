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
    internal class LogBuffer : IClearable
    {
        public delegate void BufferFilled(Log log);
        public event BufferFilled OnBufferFill;

        private int CurrentIndex = 0;
        private bool Full;
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
            Full = false;
            CurrentIndex = 0;
            _logs = new Log[Trace.BufferSize];
        }

        /// <summary>
        /// Adds a log to the buffer.
        /// </summary>
        /// <param name="log">The log to be added</param>
        public void Add(Log log)
        {
            if (CurrentIndex >= Trace.BufferSize) throw new LogBufferSizeExceededException($"Tried to add a Log into a filled buffer of size {Trace.BufferSize}.");

            _logs[CurrentIndex] = log;

            CurrentIndex++;

            if (CurrentIndex == Trace.BufferSize)
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
            _logs = new Log[Trace.BufferSize];
            Full = false;
        }

        /// <summary>
        /// Get if the buffer is full or not.
        /// </summary>
        public bool IsFull()
        {
            return Full;
        }

        /// <summary>
        /// Get if the buffer is empty or not.
        /// </summary>
        public bool IsEmpty()
        {
            return CurrentIndex == 0;
        }
    }
}
