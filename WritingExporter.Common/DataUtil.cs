using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common
{
    /// <summary>
    /// Utility class for interacting with data.
    /// </summary>
    public static class DataUtil
    {
        public static string GetEmbeddedResource(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = resourcePath;
            //var lolwut = assembly.GetManifestResourceNames();
            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    throw new ArgumentException("Embedded resource not found: " + resourcePath, "fresourcePathilename");
                }
                else
                {
                    var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
