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
    internal class Arbiter<T> where T : class, IClearable
    {
        public delegate void AllResourcesFilled();
        public event AllResourcesFilled OnAllResourcesFilled;

        private ConcurrentQueue<T> Resources = null;
        private int ResourcesSize = 0;
        private ConcurrentQueue<T> FullResources = null;

        public Arbiter(IEnumerable<T> resources)
        {
            Resources = new ConcurrentQueue<T>(resources);
            FullResources = new ConcurrentQueue<T>();
            ResourcesSize = Resources.Count;
        }

        public T Wait()
        {
            T resource = null;

            while (!Resources.TryDequeue(out resource))
                Thread.Sleep(2);
            return resource;
        }

        public void Release(T resource)
        {
            if (resource.IsFull())
                FullResources.Enqueue(resource);
            else
                Resources.Enqueue(resource);

            if (FullResources.Count == ResourcesSize)
            {

                OnAllResourcesFilled();


                for (int i = 0; i < ResourcesSize; i++)
                {
                    T res = null;
                    FullResources.TryDequeue(out res);
                    res.Clear();
                    Resources.Enqueue(res);
                }



            }
        }

        public IEnumerable<T> GetNonEmptyResources()
        {

            List<T> res = Resources.Where(r => !r.IsFull()).ToList();

            res.AddRange(FullResources.ToList());



            return res;
        }

    }
}
