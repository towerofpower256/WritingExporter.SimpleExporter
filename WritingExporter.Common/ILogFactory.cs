using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common
{
    public interface ILogFactory
    {
        ILogger GetLogger(Type type);
        ILogger GetLogger(string name);
    }
}
