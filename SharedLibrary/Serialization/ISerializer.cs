using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.Serialization
{
    public interface ISerializer<in T>
    {
        string Serialize();
    }
}
