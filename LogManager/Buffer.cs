using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    public class Buffer
    {
        public bool InUse { get; set; }
        public bool Full { get; private set; }
        public delegate void BufferFilled(Log log);
        public event BufferFilled OnBufferFill;

        private int _index { get; set; }
        private Log[] _logs { get; set; }

        public Buffer()
        {
            InUse = false;
            Full = false;
            _index = 0;
            _logs = new Log[ConcurrentTrace.BufferSize];
        }

        public void Add(Log log)
        {
            if (_index >= ConcurrentTrace.BufferSize) throw new BufferSizeExceededException($"Tried to add a Log into a filled buffer of size {ConcurrentTrace.BufferSize}.");

            _logs[_index] = log;

            _index++;

            if (_index == ConcurrentTrace.BufferSize)
            {
                Full = true;
                if (OnBufferFill != null)
                    OnBufferFill(log);

                return;
            }

        }

        public Log[] Logs()
        {
            return _logs.Where(l => l != null).ToArray();
        }

        public void Clean()
        {
            _index = 0;
            _logs = new Log[ConcurrentTrace.BufferSize];
            Full = false;
        }
    }
}
