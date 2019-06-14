using System;
using System.Collections.Generic;
using System.Text;

namespace WritingExporter.Common.Exceptions
{
    public class WritingLoginFailed : Exception
    {
        public WritingLoginFailed()
        { }

        public WritingLoginFailed(string message)
            : base(message)
        { }

        public WritingLoginFailed(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
