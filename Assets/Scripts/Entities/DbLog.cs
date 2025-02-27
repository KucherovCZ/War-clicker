using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;

namespace Entities
{
    public class DbLog : DbEntity
    {
        public DbLog(LogLevel level, string message, string exception)
        {
            Level = level;
            Message = message; 
            Exception = exception;
            Timestamp = DateTime.Now;
        }

        public string Exception;
        public string Message;
        public DateTime Timestamp;
        public LogLevel Level;
    }
}