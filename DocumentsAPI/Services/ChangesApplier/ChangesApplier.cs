using DocumentsAPI.Models.VersionControl;
using DocumentsAPI.Services.ChangesApplier.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentsAPI.Services.ChangesApplier
{
    /// <summary>
    /// Applies the changes which are listed in the frames list.
    /// </summary>
    class ChangesApplier : IChangesApplier
    {
        private readonly byte[] initialState;
        private IEnumerable<Frame> frames;

        public ChangesApplier(byte[] initialState, IEnumerable<Frame> frames)
        {
            this.initialState = initialState;
            this.frames = frames;
        }

        public byte[] ApplyChanges()
        {
            throw new NotImplementedException();
        }
    }
}
