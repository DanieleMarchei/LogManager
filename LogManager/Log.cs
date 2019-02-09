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
        /// Get the log ID.
        /// </summary>
        public ObjectId Id { get; private set; }

        /// <summary>
        /// Get or set the log level.
        /// </summary>
        public LogLevel Level { get; set; }
        
        /// <summary>
        /// Get or set the log creation time.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Get or set the log origin.
        /// </summary>
        public Origin Origin { get; set; }

        /// <summary>
        /// Get or set the log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a new instance of the Log class.
        /// </summary>
        /// <param name="Level">The log level</param>
        /// <param name="Message">The message of the log</param>
        public Log(LogLevel Level, string Message) : this(Level, DateTime.Now, new Origin(Thread.CurrentThread.ManagedThreadId), Message)
        {

        }

        /// <summary>
        /// Creates a new instance of the Log class.
        /// </summary>
        /// <param name="Level">The log level</param>
        /// <param name="Origin">The location where the log was created</param>
        /// <param name="Message">The message of the log</param>
        public Log(LogLevel Level, Origin Origin, string Message) : this(Level, DateTime.Now, Origin ,Message)
        {

        }

        /// <summary>
        /// Creates a new instance of the Log class.
        /// </summary>
        /// <param name="Level">The log level</param>
        /// <param name="TimeStamp">The time of creation of the log</param>
        /// <param name="Origin">The location where the log was created</param>
        /// <param name="Message">The message of the log</param>
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
