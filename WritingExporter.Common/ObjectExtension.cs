using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WritingExporter.Common
{
    public static class ObjectExtension
    {
        // Deep clone the object, by serializing and deserializing it.
        // Thanks to:
        // https://www.infoworld.com/article/3109870/my-two-cents-on-deep-copy-vs-shallow-copy-in-net.html
        // https://stackoverflow.com/questions/6569486/creating-a-copy-of-an-object-in-c-sharp
        public static T DeepClone<T>(this T obj)
        {
            if (!typeof(T).IsSerializable)
                throw new Exception("Cannot perform deep clone, object is not serializable.");

            if (Object.ReferenceEquals(obj, null))
                throw new Exception("Cannot perform deep clone, object is null.");

            T result = default(T);

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj); // Serialize
                stream.Seek(0, SeekOrigin.Begin); // Rewind the stream
                result = (T)formatter.Deserialize(stream); // Deserialize
                stream.Close();
            }

            return result;
        }
    }
}
