using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Word.Interfaces
{
    public interface ICommandsContainer : IWordCommand
    {
        IList<IWordCommand> Commands { get; set; }

        void Add(IWordCommand command);
        void Remove(IWordCommand command);
        void Refresh();
    }
}
