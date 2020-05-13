using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Word.Dto;
using Domain.Models.Document;

namespace Application.Word.RenderParams
{
    public class RenderData
    {
        public RenderSettings RenderSettings { get;  }
        public DocumentToRender Document { get; }
        
        //id for numbered lists in the document
        public int CurrentNumID { get; set; } = 0;



        public RenderData(RenderSettings renderSettings, DocumentToRender document)
        {
            RenderSettings = renderSettings;
            Document = document;
            CurrentNumID = 0;
        }
    }
}
