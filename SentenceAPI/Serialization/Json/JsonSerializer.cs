using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SentenceAPI.Serialization.Json
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        private T obj;

        public JsonSerializer(T obj)
        {
            this.obj = obj;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
