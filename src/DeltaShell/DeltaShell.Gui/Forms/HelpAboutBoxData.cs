using System.Collections.Generic;
using DelftTools.Shell.Core;

namespace DeltaShell.Gui.Forms
{
    /// <summary>
    /// Data displayed in AboutBox
    /// </summary>
    public class HelpAboutBoxData
    {
        public HelpAboutBoxData()
        {
            Plugins = new List<IPlugin>();
        }

        public string ProductName { get; set; }

        public string Copyright { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }

        public string SupportEmail { get; set; }

        public string SupportPhone { get; set; }

        public IEnumerable<IPlugin> Plugins { get; set; }
    }
}