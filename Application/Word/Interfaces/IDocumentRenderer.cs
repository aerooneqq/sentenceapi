    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Models.Document;

    namespace Application.Word.Interfaces
    {
        public interface IDocumentRenderer
        { 
            Task<byte[]> Render();
        }
    }
