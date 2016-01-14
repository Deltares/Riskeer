using Core.Common.Gui;
using Core.Common.Gui.Forms;

namespace Core.Plugins.DotSpatial
{
    public class DotSpatialGuiPlugin : GuiPlugin
    {
        private MapRibbon ribbon;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return ribbon;
            }
        }

        public override void Activate()
        {
            ribbon = new MapRibbon();
        }
    }
}