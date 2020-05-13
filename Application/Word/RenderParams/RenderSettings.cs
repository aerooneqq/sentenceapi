using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Word.RenderParams
{
    public class RenderSettings
    {
        #region Properties
        public string FontFamily { get; set; }
        public string DefaultTextSize { get; set; }
        public string DefaultColor { get; set;  }
        public string FolderPath { get; set;  }
        #endregion

        #region Word render constants
        public double TabValue { get; } = 400;
        #endregion

        #region Constructors
        public RenderSettings() {}
        public RenderSettings(RenderSettings settings)
        {
            FontFamily = settings.FontFamily;
            DefaultTextSize = (int.Parse(settings.DefaultTextSize) * 2).ToString();
            DefaultColor = settings.DefaultColor;
            FolderPath = settings.FolderPath;
        }
        #endregion
    }
}
