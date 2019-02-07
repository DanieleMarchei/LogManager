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
        /// <summary>
        /// Event rised when all the resouces are full.
        /// </summary>
        public event AllResourcesFilled OnAllResoucesFilled;

        private ConcurrentQueue<T> NonFullResources = null;
        private ConcurrentQueue<T> FullResources = null;
        private int ResourceNumber = 0;

        /// <summary>
        /// Initializes a new instance of the Arbiter class.
        /// </summary>
        public Arbiter(IEnumerable<T> resources)
        {
            NonFullResources = new ConcurrentQueue<T>(resources);
            FullResources = new ConcurrentQueue<T>();
            ResourceNumber = NonFullResources.Count;
        }

        /// <summary>
        /// Waits and return as soon as a resource is available.
        /// </summary>
        public T Wait()
        {
            T res = null;

            while (!NonFullResources.TryDequeue(out res))
                Thread.Sleep(2);
            return res;
        }


        /// <summary>
        /// Releases the resource.
        /// <para name="resource">The resource to be released</para>
        /// </summary>
        public void Release(T resource)
        {
            if (resource.IsFull())
                FullResources.Enqueue(resource);
            else
                NonFullResources.Enqueue(resource);

            if (NonFullResources.Count == 0)
            {
                if(OnAllResoucesFilled != null)
                    OnAllResoucesFilled();
                //ClearResources();
            } 
        }

        /// <summary>
        /// Gets all the non empty resources.
        /// </summary>
        public IEnumerable<T> GetNonEmptyResources()
        {
            List<T> resources = NonFullResources
                .Where(r => !r.IsEmpty())
                .Concat(FullResources)
                .ToList();
            return resources;
        }

        /// <summary>
        /// Empty all the resources.
        /// </summary>
        public void ClearResources()
        {
            int nonFullCount = NonFullResources.Count;
            T res = null;
            for (int i = 0; i < nonFullCount; i++)
            {
                NonFullResources.TryDequeue(out res);
                if (res == null) return;

                res.Clear();
                NonFullResources.Enqueue(res);
            }

            int fullCount = FullResources.Count;

            for (int i = 0; i < fullCount; i++)
            {
                FullResources.TryDequeue(out res);
                if (res == null) return;

                res.Clear();
                NonFullResources.Enqueue(res);
            }
        }

    }
}
