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
    /// <summary>
    /// Handles concurrency between multiple threads and multiple resources.
    /// </summary>
    /// <typeparam name="T">The type of resources</typeparam>
    internal class Arbiter<T> where T : class, IClearable
    {
        public delegate void AllResourcesFilled();
        /// <summary>
        /// Event raised when there is no resource available.
        /// </summary>
        public event AllResourcesFilled OnAllResourcesFilled;

        private ConcurrentQueue<T> Resources = null;
        private int ResourcesSize = 0;
        private ConcurrentQueue<T> FullResources = null;

        /// <summary>
        /// Createes a new instances of the Arbiter class.
        /// </summary>
        /// <param name="resources">The initial resources</param>
        public Arbiter(IEnumerable<T> resources)
        {
            Resources = new ConcurrentQueue<T>(resources);
            FullResources = new ConcurrentQueue<T>();
            ResourcesSize = Resources.Count;
        }

        /// <summary>
        /// Wait until a resource becomes available and returns it.
        /// </summary>
        public T Wait()
        {
            T resource = null;

            while (!Resources.TryDequeue(out resource))
                Thread.Sleep(2);

            return resource;
        }

        /// <summary>
        /// Releases a resource.
        /// </summary>
        /// <param name="resource">The resource to be released</param>
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

        /// <summary>
        /// Get all the non empty resources.
        /// </summary>
        public IEnumerable<T> GetNonEmptyResources()
        {
            List<T> res = Resources.Where(r => !r.IsFull()).ToList();
            res.AddRange(FullResources.ToList());

            return res;
        }

    }
}
