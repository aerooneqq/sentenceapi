using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Serialization
{
    interface ISerializer<in T>
    {
        string Serialize();
    }
}
