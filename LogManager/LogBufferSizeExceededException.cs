using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    /// <summary>
    /// Exception that rises when a log is added to a filled buffer
    /// </summary>
    [Serializable]
    public class LogBufferSizeExceededException : Exception
    {
        /// <summary>
        /// Creates a new instance of the LogBufferSizeExceededException class.
        /// </summary>
        public LogBufferSizeExceededException() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the LogBufferSizeExceededException class.
        /// </summary>
        /// <param name="message">The exception message</param>
        public LogBufferSizeExceededException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the LogBufferSizeExceededException class.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The exception that is the cause of this exception</param>
        public LogBufferSizeExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
