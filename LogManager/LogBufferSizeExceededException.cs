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
        public LogBufferSizeExceededException() : base()
        {
        }

        public LogBufferSizeExceededException(string message) : base(message)
        {
        }

        public LogBufferSizeExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LogBufferSizeExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
