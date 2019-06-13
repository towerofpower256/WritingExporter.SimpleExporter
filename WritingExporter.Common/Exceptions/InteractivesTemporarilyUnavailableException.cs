using System;
using System.Collections.Generic;
using System.Text;

namespace WritingExporter.Common.Exceptions
{
    public class InteractivesTemporarilyUnavailableException : Exception
    {
        public InteractivesTemporarilyUnavailableException()
        { }

        public InteractivesTemporarilyUnavailableException(string message)
            : base(message)
        { }

        public InteractivesTemporarilyUnavailableException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
