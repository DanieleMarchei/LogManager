using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogManager
{
    public class ConcurrentTrace
    {
        public static readonly int BUFFER_SIZE = 200;
        public static readonly int NUMBER_OF_BUFFERS = 64;

        private volatile static Buffer[] Buffers = null;
        private static readonly object critSec = new object();
        private static IMongoCollection<Log> Collection = null;
        private static Semaphore Semaphore = null;

        /// <summary>
        /// Connects to the localhost database where the logs will be saved.
        /// </summary>
        /// <param name="collectionName">Name of the collection where the logs will be saved.</param>
        public static void Connect(string collectionName)
        {
            if (Collection != null) return;

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase(Dns.GetHostName());
            Collection = database.GetCollection<Log>(collectionName);

            Semaphore = new Semaphore(NUMBER_OF_BUFFERS, NUMBER_OF_BUFFERS);
            Buffers = new Buffer[NUMBER_OF_BUFFERS];
        }

        /// <summary>
        /// Writes the log into the buffer.
        /// </summary>
        /// <param name="log">The log to be saved.</param>
        public static void Write(Log log)
        {
            if (Collection != null) return;

            Semaphore.WaitOne();

            Buffer freeBuffer = null;
            lock (critSec)
            {
                //TODO
                //Find a way to do this quicker
                freeBuffer = Buffers.FirstOrDefault(b => !b.InUse && !b.Full);

                if(freeBuffer == null)
                {
                    //TODO
                    //if all buffers are in use, maybe push the log into a list
                    Flush();
                    freeBuffer = Buffers.FirstOrDefault(b => !b.InUse && !b.Full);
                }
                else
                {
                    freeBuffer.InUse = true;
                }
            }

            freeBuffer.Add(log);

            freeBuffer.InUse = false;

            Semaphore.Release();
        }

        /// <summary>
        /// Transfers synchronously all the logs from the buffer into the database.
        /// </summary>
        public static void Flush()
        {
            if (Collection != null) return;

            List<Log> b = new List<Log>();
            for (int i = 0; i < Buffers.Length; i++)
            {
                b.AddRange(Buffers[i].Logs());
            }

            Collection.InsertMany(b);
            Buffers = new Buffer[NUMBER_OF_BUFFERS];
        }

        /// <summary>
        /// Transfers asynchronously all the logs from the buffer into the database.
        /// </summary>
        public async static Task FlushAsync()
        {
            if (Collection != null) return;

            List<Log> b = new List<Log>();
            for (int i = 0; i < Buffers.Length; i++)
            {
                b.AddRange(Buffers[i].Logs());
            }

            await Collection.InsertManyAsync(b);
            Buffers = new Buffer[NUMBER_OF_BUFFERS];
        }
    }
}
