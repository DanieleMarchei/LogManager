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
        /// <summary>
        /// Event raised when the buffer becomes full.
        /// </summary>
        public event BufferFilled OnBufferFill;

        private int CurrentIndex = 0;
        private bool Full;
        private Log[] _logs { get; set; }

        /// <summary>
        /// Get all the logs stored in the buffer.
        /// </summary>
        public Log[] Logs
        {
            get
            {
                return _logs.Where(l => l != null).ToArray();
            }
        }

        /// <summary>
        /// Creates a new instance of the LogBuffer class.
        /// </summary>
        public LogBuffer()
        {
            Full = false;
            CurrentIndex = 0;
            _logs = new Log[TraceLog.BufferSize];
        }

        /// <summary>
        /// Adds a log to the buffer.
        /// </summary>
        /// <param name="log">The log to be added</param>
        public void Add(Log log)
        {
            if (CurrentIndex >= TraceLog.BufferSize) throw new LogBufferSizeExceededException($"Tried to add a Log into a filled buffer of size {TraceLog.BufferSize}.");

            _logs[CurrentIndex] = log;

            CurrentIndex++;

            if (CurrentIndex == TraceLog.BufferSize)
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
            _logs = new Log[TraceLog.BufferSize];
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
