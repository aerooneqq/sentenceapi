using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace SharedLibrary.Serialization.Xml
{
    public class XmlSerializer<T> : ISerializer<T>
    {
        private readonly T obj;

        public XmlSerializer(T obj)
        {
            this.obj = obj;
        }

        public string Serialize()
        {
            string result = string.Empty;
            
            using (StreamWriter sr = new StreamWriter(result))
            {
                new XmlSerializer(typeof(T)).Serialize(sr, obj);
            }

            return result;
        }
    }
}
