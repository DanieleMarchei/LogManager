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
    public static class ArbiterConcurrentTrace
    {
        public static int BufferSize = 200;
        public static int NumberOfBuffers = 64;

        private volatile static LogBuffer[] Buffers = null;
        private static readonly object critSec = new object();
        private static IMongoCollection<Log> Collection = null;
        private static Arbiter Arbiter = null;

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

            Buffers = new LogBuffer[NumberOfBuffers];
            for (int i = 0; i < NumberOfBuffers; i++)
            {
                Buffers[i] = new LogBuffer();
            }

            Arbiter = new Arbiter(Buffers);
            Arbiter.OnAllBuffersFilled += Flush;

        }

        /// <summary>
        /// Writes the log into the buffer.
        /// </summary>
        /// <param name="log">The log to be saved.</param>
        public static void Write(Log log)
        {
            if (Collection == null) new TraceStateException("No connection has been created.");

            LogBuffer freeBuffer = Arbiter.Wait();

            freeBuffer.Add(log);

            Arbiter.Release(freeBuffer);
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
