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
            InUse = true;
            Full = false;
            _index = 0;
            _logs = new Log[ConcurrentTrace.BUFFER_SIZE];
        }

        public void Add(Log log)
        {
            if (_index >= ConcurrentTrace.BUFFER_SIZE)
            {
                Full = true;
                OnBufferFill(log);
                return;
            }

            _logs[_index] = log;

            _index++;
        }

        public Log[] Logs()
        {
            return _logs.Where(l => l != null).ToArray();
        }

        public void Clean()
        {
            _index = 0;
            _logs = new Log[ConcurrentTrace.BUFFER_SIZE];
            Full = false;
        }
    }
}
