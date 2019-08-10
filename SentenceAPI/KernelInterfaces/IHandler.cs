using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.KernelInterfaces
{
    public interface IHandler<TIn, TOut>
    {
        IHandler<TIn, TOut> NextHandler { get; }
        Task<TOut> Handle(TIn obj);
    }
}
