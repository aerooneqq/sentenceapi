using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Services.ChangesApplier.Interfaces
{
    interface IChangesApplier
    {
        byte[] ApplyChanges();
    }
}
