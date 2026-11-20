using System;
using System.Collections.Generic;
using System.Text;

namespace Kodutoo3
{
    internal class ConsoleLogger : BaseLogger
    {
        public override void Info(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }
        public override void Error(string message)
        {
            Console.WriteLine($"ERROR: {message}");
        }
        public override void Debug(string message)
        {
            Console.WriteLine($"DEBUG: {message}");
        }
        public override void Warning(string message)
        {
            Console.WriteLine($"WARNING: {message}");
        }
 
    }
}
