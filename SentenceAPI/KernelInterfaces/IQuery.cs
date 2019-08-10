using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.KernelInterfaces
{
    interface IQuery<Tin, TOut> : IHandler<Tin, TOut>
    {
    }
}
