using System;

namespace iCalendar.Exceptions
{
    public class MalformedFormatException : Exception
    {
        public MalformedFormatException() { }
        public MalformedFormatException(string message) : base(message) { }
        public MalformedFormatException(string message, Exception inner) : base(message, inner) { }
    }
}
