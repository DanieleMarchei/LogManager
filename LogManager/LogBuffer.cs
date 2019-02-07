using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    /// <summary>
    /// Data structue for caching logs
    /// </summary>
    internal class LogBuffer : IClearable
    {
        private bool Full;
        public delegate void BufferFilled(Log log);
        public event BufferFilled OnBufferFill;

        private int CurrentIndex = 0;
        private Log[] _logs { get; set; }
        /// <summary>
        /// The logs in the buffer.
        /// </summary>
        public Log[] Logs
        {
            get
            {
                return _logs.Where(l => l != null).ToArray();
            }
        }

        /// <summary>
        /// Initializes a new instance of the LogBUffer class.
        /// </summary>
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
            if (Full)
                throw new LogBufferSizeExceededException($"Tried to add a Log into a filled buffer of size {Trace.BufferSize}.");

            int l = _logs.Length;
            _logs[CurrentIndex] = log;

            CurrentIndex++;

            if (CurrentIndex == Trace.BufferSize)
            {
                Full = true;
                if (OnBufferFill != null)
                    OnBufferFill(log);
            }

        }

        /// <summary>
        /// Cleares the buffer.
        /// </summary>
        public void Clear()
        {
            CurrentIndex = 0;
            _logs = new Log[Trace.BufferSize];
            Full = false;
        }

        /// <summary>
        /// Check if the buffer is empty.
        /// </summary>
        public bool IsEmpty()
        {
            return CurrentIndex == 0;
        }

        /// <summary>
        /// Check if the buffer is full.
        /// </summary>
        public bool IsFull()
        {
            return Full;
        }
    }
}
