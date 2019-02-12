using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    /// <summary>
    /// Exception rised when the TraceLog class is not in a valid state
    /// </summary>
    [Serializable]
    public class TraceLogStateException : Exception
    {
        /// <summary>
        /// Creates a new instance of the TraceStateException class.
        /// </summary>
        public TraceLogStateException() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the TraceStateException class.
        /// </summary>
        /// <param name="message">The exception's message</param>
        public TraceLogStateException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the TraceStateException class.
        /// </summary>
        /// <param name="message">The exception's message</param>
        /// <param name="innerException">The exception that caused this exception to rise</param>
        public TraceLogStateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
