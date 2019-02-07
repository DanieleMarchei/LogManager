using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LogManager
{
    public static class Trace
    {
        /// <summary>
        /// Get or set the total number of logs each buffer can hold. Default is 256.
        /// </summary>
        public static uint BufferSize = 256;

        /// <summary>
        /// Geto or set the number of buffers available. Default is 64.
        /// </summary>
        public static uint NumberOfBuffers = 64;

        /// <summary>
        /// Get or set the time that has to pass for every flush. If there is a flush before the timer elapses, is resetted. Default is 2 seconds.
        /// </summary>
        public static TimeSpan FlushTimer = TimeSpan.FromSeconds(2);

        private volatile static LogBuffer[] Buffers = null;
        private static readonly object critSec = new object();
        private static MongoClient client = null;
        private static IMongoCollection<Log> Collection = null;
        private static Arbiter<LogBuffer> Arbiter = null;
        private static Timer timer = null;

        /// <summary>
        /// Connects to the localhost database where the logs will be saved.
        /// </summary>
        /// <param name="collectionName">Name of the collection where the logs will be saved.</param>
        /// <param name="domain">IP address or domain of the db's server.</param>
        /// <param name="port">Port number of the db's server.</param>
        [Conditional("TRACE_LOG")]
        public static void Connect(string collectionName, string domain = "localhost", uint port = 27017)
        {
            if (Collection != null) throw new TraceStateException("Connection already established.");

            client = new MongoClient($"mongodb://{domain}:{port}");

            //just to update the description state
            var databases = client.ListDatabases();

            if (client.Cluster.Description.State == ClusterState.Disconnected)
                throw new TraceStateException("Local db is unreachable.");

            var database = client.GetDatabase(Dns.GetHostName());
            Collection = database.GetCollection<Log>(collectionName);

            Buffers = new LogBuffer[NumberOfBuffers];
            for (int i = 0; i < NumberOfBuffers; i++)
            {
                Buffers[i] = new LogBuffer();
            }

            Arbiter = new Arbiter<LogBuffer>(Buffers);
            //I create a new delegate in order to call a method with a Conditional Attribute
            Arbiter.OnAllResoucesFilled += delegate { Flush(); };

            timer = new Timer(FlushTimer.Seconds);
            timer.AutoReset = false;
            timer.Elapsed += delegate { Timer_Elapsed(null, null); };
            timer.Start();
        }

        [Conditional("TRACE_LOG")]
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            if (client == null || client.Cluster.Description.State == ClusterState.Disconnected)
                throw new TraceStateException("No connection to local db.");

            lock (critSec)
            {
                List<Log> b = new List<Log>();
                foreach (LogBuffer logBuff in Arbiter.GetNonEmptyResources())
                {
                    b.AddRange(logBuff.Logs);
                }

                if (b.Count == 0) return;

                Collection.InsertMany(b);
                Arbiter.ClearResources();
                timer.Start();
            }


        }

        /// <summary>
        /// Writes the log into the buffer.
        /// </summary>
        /// <param name="log">The log to be saved.</param>
        [Conditional("TRACE_LOG")]
        public static void Write(Log log)
        {
            if (client == null || client.Cluster.Description.State == ClusterState.Disconnected)
                throw new TraceStateException("No connection to local db.");

            LogBuffer freeBuffer = Arbiter.Wait();

            freeBuffer.Add(log);

            Arbiter.Release(freeBuffer);
        }

        /// <summary>
        /// Transfers synchronously all the logs from the buffer into the database.
        /// </summary>
        [Conditional("TRACE_LOG")]
        public static void Flush()
        {
            if (client == null || client.Cluster.Description.State == ClusterState.Disconnected)
                throw new TraceStateException("No connection to local db.");

            lock (critSec)
            {
                timer.Stop();
                List<Log> b = new List<Log>();
                foreach (LogBuffer logBuff in Arbiter.GetNonEmptyResources())
                {
                    b.AddRange(logBuff.Logs);
                }

                if (b.Count == 0) return;

                Collection.InsertMany(b);
                Arbiter.ClearResources();
                timer.Start();
            }
        }
    }
}
