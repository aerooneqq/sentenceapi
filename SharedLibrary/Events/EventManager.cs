using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.Events.Interfaces;


namespace SharedLibrary.Events 
{
    /// <summary>
    /// The event manager which can be used to raise the domain event.
    /// </summary>
    public static class EventManager
    {
        public static async Task Raise(IDomainEvent @event)
        {
            await @event.Handle();
        }
    }
}
