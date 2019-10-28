using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.KernelInterfaces
{
    public interface IValidator
    {
        (bool result, string errorMessage) Validate();
    }
}
