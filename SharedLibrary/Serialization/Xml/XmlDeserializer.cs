using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace SharedLibrary.Serialization.Xml
{
    public class XmlDeserializer<T> : IDeserializer<T>
    {
        private readonly string serializedObj;

        public XmlDeserializer(string serializedObj, Encoding encoding)
        {
            this.serializedObj = serializedObj;
        }

        public T Deserialize()
        {
            return (T)(new XmlSerializer(typeof(T)).Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(serializedObj))));
        }
    }
}
