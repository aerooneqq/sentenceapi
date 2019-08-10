using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SentenceAPI.Serialization.Json
{
    public class JsonDeserializer<T> : IDeserializer<T>
    {
        private readonly string serializedObj;

        public JsonDeserializer(string serializedObj)
        {
            this.serializedObj = serializedObj;
        }

        public T Deserialize()
        {
            return JsonConvert.DeserializeObject<T>(serializedObj);
        }
    }
}
