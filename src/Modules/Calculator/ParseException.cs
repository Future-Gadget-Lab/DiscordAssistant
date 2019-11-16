using System;

namespace Assistant.Modules.Calculator
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
