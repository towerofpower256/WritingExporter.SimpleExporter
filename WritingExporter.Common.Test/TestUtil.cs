using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WritingExporter.Common.Test
{
    public static class TestUtil
    {
        public static string GetDataFile(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = string.Format("WritingExporter.Common.Test.Data.{0}", filename);
            var lolwut = assembly.GetManifestResourceNames();
            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    throw new ArgumentException("Embedded resource not found: " + resource, "filename");
                }
                else
                {
                    var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
            }
            return string.Empty;
        }

        public static void SetupLogging()
        {
            if (LogManager.IsReady()) return;

            Log4NetLogFactory lf = new Log4NetLogFactory();
            lf.AddConsoleAppender().SetLogLevel("debug").EndConfig();
            LogManager.SetLogFactory(lf);
        }

        public static bool SerializeAndCompare<T>(T objA, T objB)
        {
            string objASerialized = QuickXmlSerialize(objA);
            string objBSerialized = QuickXmlSerialize(objB);

            return string.Equals(objASerialized, objBSerialized);
        }

        public static string QuickXmlSerialize<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T), "");

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, obj);

                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
