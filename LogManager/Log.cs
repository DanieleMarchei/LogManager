using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace LogManager
{
    /// <summary>
    /// Represents a Log.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Id of the Log
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        /// Level of the Log
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Date of creation of the Log
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Where the Log was created
        /// </summary>
        public Origin Origin { get; set; }

        /// <summary>
        /// Message of the Log
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        public Log(LogLevel Level, string Message) : this(Level, DateTime.Now, new Origin(Thread.CurrentThread.ManagedThreadId), Message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        public Log(LogLevel Level, Origin Origin, string Message) : this(Level, DateTime.Now, Origin ,Message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
        public Log(LogLevel Level, DateTime TimeStamp, Origin Origin, string Message)
        {
            this.Id = ObjectId.GenerateNewId();
            this.Level = Level;
            this.TimeStamp = TimeStamp;
            this.Origin = Origin;
            this.Message = Message;
        }

    }
}
