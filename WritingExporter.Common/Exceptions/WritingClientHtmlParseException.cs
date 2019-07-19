using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.Exceptions
{
    /// <summary>
    /// An exception for when the WdcClient has an issue trying to parse HTML returned by Writing.com.
    /// </summary>
    class WritingClientHtmlParseException : Exception
    {
        public string HtmlResult { get; private set; }
        public string Address { get; private set; }

        public WritingClientHtmlParseException()
        { }

        public WritingClientHtmlParseException(string message)
            : base(message)
        { }

        public WritingClientHtmlParseException(string message, Exception inner)
            : base(message, inner)
        { }

        public WritingClientHtmlParseException(string message, string html, string address)
            : base(message)
        {
            HtmlResult = html;
            Address = address;
        }

        public WritingClientHtmlParseException(string message, string html, string address, Exception inner)
            : base(message, inner)
        {
            HtmlResult = html;
            Address = address;
        }
    }
}
