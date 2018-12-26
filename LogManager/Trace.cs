using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace LogManager
{
    public static class Trace
    {
        private static readonly int BUFFER_SIZE = 200;
        private static Log[] Buffer = null;
        private static int index = 0;
        private static IMongoCollection<Log> Collection = null;

        /// <summary>
        /// Connects to the localhost database where the logs will be saved.
        /// </summary>
        /// <param name="collectionName">Name of the collection where the logs will be saved.</param>
        public static void Connect(string collectionName)
        {
            if (Collection != null) return;

            Buffer = new Log[BUFFER_SIZE];
            index = 0;

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase(Dns.GetHostName());
            Collection = database.GetCollection<Log>(collectionName);
        }

        /// <summary>
        /// Writes the log into the buffer.
        /// </summary>
        /// <param name="log">The log to be saved.</param>
        public static void Write(Log log)
        {
            if (index >= BUFFER_SIZE)
                Flush();

            Buffer[index] = log;
            index++;
        }

        /// <summary>
        /// Transfers synchronously all the logs from the buffer into the database.
        /// </summary>
        public static void Flush()
        {
            Collection.InsertMany(Buffer.Take(index));
            Buffer = new Log[BUFFER_SIZE];
            index = 0;
        }

        /// <summary>
        /// Transfers asynchronously all the logs from the buffer into the database.
        /// </summary>
        public async static Task FlushAsync()
        {
            await Collection.InsertManyAsync((Log[])Buffer.Clone());
            Buffer = new Log[BUFFER_SIZE];
            index = 0;
        }


    }
}
