﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Serialization
{
    interface IDeserializer<out T>
    {
        T Deserialize();
    }
}
