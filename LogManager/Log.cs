using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace LogManager
{
    public class Log
    {
        public ObjectId Id { get; set; }
        public LogLevel Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public Origin Origin { get; set; }
        public string Message { get; set; }

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
