using System;
using System.Collections.Generic;
using System.Text;

namespace Kodutoo3
{
    internal class BaseLogger
    {
        public virtual void Info(string message)
        {
        }
        public virtual void Error(string message)
        {
        }
        public virtual void Debug(string message)
        {
        }

        public virtual void Warning(string message)
        {
        }
    }
}
