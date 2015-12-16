using System.Drawing;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Gui;

namespace Core.Plugins.CommonTools.Gui.Commands.Charting
{
    public abstract class ChartViewCommandBase : GuiCommand
    {
        public override bool Enabled
        {
            get
            {
                return View != null;
            }
        }

        public override bool Checked
        {
            get
            {
                return false;
            }
        }

        protected ChartView View
        {
            get
            {
                return Gui != null
                           ? Gui.DocumentViews.ActiveView as ChartView
                           : null;
            }
        }

        protected Font GetChangedFontSize(Font font, int pixels)
        {
            return new Font(font.FontFamily, font.Size + pixels, font.Style);
        }
    }
}