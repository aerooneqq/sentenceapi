using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;


namespace SharedLibrary.Events.Interfaces
{
    public interface IDomainEvent
    {
        Task Handle();
    }
}
