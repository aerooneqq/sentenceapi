using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.Serialization
{
    public interface IDeserializer<out T>
    {
        T Deserialize();
    }
}
