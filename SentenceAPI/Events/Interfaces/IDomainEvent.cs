using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Events.Interfaces
{
    interface IDomainEvent
    {
        Task Handle();
    }
}
