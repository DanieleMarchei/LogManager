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
        public static int BufferSize = 200;
        public static int NumberOfBuffers = 64;

        private volatile static LogBuffer[] Buffers = null;
        private static readonly object critSec = new object();
        private static IMongoCollection<Log> Collection = null;
        private static Semaphore Semaphore = null;

        /// <summary>
        /// Connects to the localhost database where the logs will be saved.
        /// </summary>
        /// <param name="collectionName">Name of the collection where the logs will be saved.</param>
        public static void Connect(string collectionName)
        {
            if (Collection != null) new TraceStateException("Connection already established.");

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase(Dns.GetHostName());
            Collection = database.GetCollection<Log>(collectionName);

            Semaphore = new Semaphore(NumberOfBuffers, NumberOfBuffers);
            Buffers = new LogBuffer[NumberOfBuffers];
            for (int i = 0; i < NumberOfBuffers; i++)
            {
                Buffers[i] = new LogBuffer();
            }
        }

        /// <summary>
        /// Writes the log into the buffer.
        /// </summary>
        /// <param name="log">The log to be saved.</param>
        public static void Write(Log log)
        {
            if (Collection == null) new TraceStateException("No connection has been created.");

            Semaphore.WaitOne();

            LogBuffer freeBuffer = null;
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
            if (Collection == null) new TraceStateException("No connection has been created.");

            List<Log> b = new List<Log>();
            for (int i = 0; i < Buffers.Length; i++)
            {
                b.AddRange(Buffers[i].Logs);
            }

            if (b.Count == 0) return;

            Collection.InsertMany(b);
            Buffers = new LogBuffer[NumberOfBuffers];
            for (int i = 0; i < NumberOfBuffers; i++)
            {
                Buffers[i] = new LogBuffer();
            }
        }

        /// <summary>
        /// Transfers asynchronously all the logs from the buffer into the database.
        /// </summary>
        public async static Task FlushAsync()
        {
            if (Collection == null) new TraceStateException("No connection has been created.");

            List<Log> b = new List<Log>();
            for (int i = 0; i < Buffers.Length; i++)
            {
                b.AddRange(Buffers[i].Logs);
            }

            if (b.Count == 0) return;

            await Collection.InsertManyAsync(b);
            Buffers = new LogBuffer[NumberOfBuffers];
            for (int i = 0; i < NumberOfBuffers; i++)
            {
                Buffers[i] = new LogBuffer();
            }
        }
    }
}
