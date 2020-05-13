using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Word.Interfaces;
using Application.Word.RenderParams;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;


namespace ApplicationLib.Word.Commands
{
    public class TabCommand : IWordSecondaryCommand
    {
        private readonly RenderData renderData;

        public TabCommand(RenderData renderData)
        {
            this.renderData = renderData;
        }
        
        public OpenXmlCompositeElement GetElement()
        {
            Run run = new Run();
            RunProperties runProperties = new RunProperties(
                new RunFonts()
                {
                    HighAnsi = renderData.RenderSettings.FontFamily,
                    Ascii = renderData.RenderSettings.FontFamily
                });

            run.Append(runProperties);
            run.Append(new TabChar());

            return run;
        }
    }
}
