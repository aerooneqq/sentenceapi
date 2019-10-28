using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.KernelInterfaces
{
    public interface IHandler<TIn, TOut>
    {
        IHandler<TIn, TOut> NextHandler { get; }
        Task<TOut> Handle(TIn obj);
    }
}
