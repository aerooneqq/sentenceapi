using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.KernelInterfaces
{
    interface IModelValidator
    {
        (bool, IEnumerable<string>) Validate();
    }
}
