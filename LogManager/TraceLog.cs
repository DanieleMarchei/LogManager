using MongoDB.Bson;
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
    /// <summary>
    /// Allows to connect to a database and save logs concurrently.
    /// </summary>
    public static class TraceLog
    {
        /// <summary>
        /// Get and set the buffer size. Default = 256.
        /// </summary>
        public static int BufferSize = 256;

        /// <summary>
        /// Get and set the number of buffers to be used. Default = 64.
        /// </summary>
        public static int NumberOfBuffers = 64;

        /// <summary>
        /// Get and set the time interval between each flush. Default = 10 seconds.
        /// </summary>
        public static TimeSpan FlushInterval = TimeSpan.FromSeconds(10);

        private static readonly object critSec = new object();
        private static MongoClient client = null;
        private static IMongoCollection<Log> Collection = null;
        private static Arbiter<LogBuffer> Arbiter = null;

        private static Timer timer = null;

        /// <summary>
        /// Connects to the localhost database where the logs will be saved.
        /// </summary>
        /// <param name="collectionName">Name of the collection where the logs will be saved.</param>
        /// <param name="domain">Domain or URL of the db's server.</param>
        /// <param name="port">Port number of the db's server.</param>
        [Conditional("TRACE_LOG")]
        public static void Connect(string collectionName, string domain = "localhost", uint port = 27017)
        {
            if (Collection != null) throw new TraceLogStateException("Connection already established.");

            client = new MongoClient($"mongodb://{domain}:{port}");

            //just to update the description state
            var databases = client.ListDatabases();

            if (client.Cluster.Description.State == ClusterState.Disconnected)
                throw new TraceLogStateException("Local db is unreachable.");

            var database = client.GetDatabase(Dns.GetHostName());
            Collection = database.GetCollection<Log>(collectionName);

            LogBuffer[] Buffers = new LogBuffer[NumberOfBuffers];
            for (int i = 0; i < NumberOfBuffers; i++)
            {
                Buffers[i] = new LogBuffer();
            }

            Arbiter = new Arbiter<LogBuffer>(Buffers);
            Arbiter.OnAllResourcesFilled += Arbiter_OnAllResourcesFilled;

            timer = new Timer(FlushInterval.TotalMilliseconds);
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private static void Arbiter_OnAllResourcesFilled(object sender, EventArgs e)
        {
            Flush();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Flush();
        }

        /// <summary>
        /// Writes a log into the buffer.
        /// </summary>
        /// <param name="log">The log to be saved.</param>
        [Conditional("TRACE_LOG")]
        public static void Write(Log log)
        {
            if (client == null || client.Cluster.Description.State == ClusterState.Disconnected)
                throw new TraceLogStateException("No connection to local db.");

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
                throw new TraceLogStateException("No connection to local db.");

            lock (critSec)
            {
                timer.Stop();
                List<Log> b = new List<Log>();
                IEnumerable<LogBuffer> logs = Arbiter.GetNonEmptyResources();

                foreach (LogBuffer logBuff in logs)
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
        /// Sends all the logs in the local db to a specified server. When all the logs are sent, the database is cleared.
        /// </summary>
        /// <param name="domain">Domain or URL of the server</param>
        /// <param name="port">Port number of the server</param>
        /// <param name="database">Name of the database where the logs will be sent</param>
        /// <param name="collection">Name of the collection where the logs will be sent</param>
        [Conditional("TRACE_LOG")]
        public static void SendToServer(string domain, uint port, string database, string collection)
        {

            var serverInstance = new MongoClient($"mongodb://{domain}:{port}");

            serverInstance.ListDatabases();

            if (serverInstance.Cluster.Description.State == ClusterState.Disconnected)
                throw new TraceLogStateException("Server db is unreachable.");

            var db = serverInstance.GetDatabase(database);
            var coll = db.GetCollection<Log>(collection);

            List<Log> tmpDocuments = new List<Log>();
            List<string> collectionsName = new List<string>();
            List<BsonDocument> collections = new List<BsonDocument>();
            var cursor = Collection.Database.ListCollections();
            while (cursor.MoveNext())
            {
                collections.AddRange(cursor.Current);
            }
            //tmpDocuments will have the name of all the collections present inside the database
            //to pick the name of a collection navigate the bson document with "name"
            collections.ForEach(_ =>
            {
                collectionsName.Add(_["name"].ToString());
            });
            foreach (string s in collectionsName)
            {
                tmpDocuments.AddRange(Collection.Database.GetCollection<Log>(s).Find(_ => true).ToList<Log>());
            }

            if (tmpDocuments.Count == 0) return;

            coll.InsertMany(tmpDocuments);
            //clear the collection once it has sent all the logs to the server
            foreach (string s in collectionsName)
            {
                Collection.Database.GetCollection<Log>(s).DeleteMany(_ => true);
            }
        }
    }
}
