using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogManager
{
    internal class Arbiter2
    {
        public delegate void AllBuffersFilled();
        public event AllBuffersFilled OnAllBuffersFilled;

        private ConcurrentQueue<LogBuffer> Resources = null;
        private int bufferSize = 0;
        private ConcurrentQueue<LogBuffer> FullResources = null;

        public Arbiter2(IEnumerable<LogBuffer> resources)
        {
            Resources = new ConcurrentQueue<LogBuffer>(resources);
            FullResources = new ConcurrentQueue<LogBuffer>();
            bufferSize = Resources.Count;
        }

        public LogBuffer Wait()
        {
            LogBuffer logBuf = null;

            while (!Resources.TryDequeue(out logBuf))
                Thread.Sleep(2);
            return logBuf;
        }

        public void Release(LogBuffer logBuf)
        {
            if (logBuf.Full)
                FullResources.Enqueue(logBuf);
            else
                Resources.Enqueue(logBuf);

            if (FullResources.Count == bufferSize)
            {
                OnAllBuffersFilled();
                for (int i = 0; i < bufferSize; i++)
                {
                    LogBuffer buff = null;
                    FullResources.TryDequeue(out buff);
                    buff.Clear();
                    Resources.Enqueue(buff);
                }
            } 
        }

        public IEnumerable<LogBuffer> ToList()
        {
            List<LogBuffer> buffers = Resources.Where(b => b.CurrentIndex > 0).ToList();
            buffers.AddRange(FullResources.ToList());
            return buffers;
        }

        public void Clear()
        {
            for (int i = 0; i < bufferSize; i++)
            {
                LogBuffer buff = null;
                Resources.TryDequeue(out buff);
                if (buff == null) return;

                buff.Clear();
                Resources.Enqueue(buff);
            }
        }

    }
}
