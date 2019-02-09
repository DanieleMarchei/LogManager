using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    /// <summary>
    /// Exception rised when the Trace class is not in a valid state
    /// </summary>
    [Serializable]
    public class TraceStateException : Exception
    {
        /// <summary>
        /// Creates a new instance of the TraceStateException class.
        /// </summary>
        public TraceStateException() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the TraceStateException class.
        /// </summary>
        /// <param name="message">The exception's message</param>
        public TraceStateException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the TraceStateException class.
        /// </summary>
        /// <param name="message">The exception's message</param>
        /// <param name="innerException">The exception that caused this exception to rise</param>
        public TraceStateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
