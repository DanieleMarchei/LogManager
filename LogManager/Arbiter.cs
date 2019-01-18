using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogManager
{
    internal class Arbiter
    {
        public int Threads { get; private set; }

        public delegate void AllBuffersFilled();
        public event AllBuffersFilled OnAllBuffersFilled;

        private Queue<LogBuffer> Resources = null;
        private int bufferSize = 0;
        private Queue<LogBuffer> FullResources = null;
        private readonly object critSec = new object();

        public Arbiter(IEnumerable<LogBuffer> resources)
        {
            Resources = new Queue<LogBuffer>(resources);
            FullResources = new Queue<LogBuffer>();
            bufferSize = Resources.Count;
        }

        public LogBuffer Wait()
        {
            LogBuffer logBuf = null;
            lock (critSec)
            {
                while(Resources.Count == 0)
                {
                    Thread.Sleep(2);
                }
                logBuf = Resources.Dequeue();

            }
            return logBuf; 
        }

        public void Release(LogBuffer logBuf)
        {
            lock (critSec)
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
                        LogBuffer buff = FullResources.Dequeue();
                        buff.Clear();
                        Resources.Enqueue(buff);
                    }
                }
            }
        }

    }
}
