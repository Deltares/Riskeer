using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Plugins.DotSpatial;
using Demo.Ringtoets.Commands;
using Demo.Ringtoets.Ribbons;

namespace Demo.Ringtoets.GUIs
{
    /// <summary>
    /// This class describes the Demo functionality for the <see cref="DotSpatialGuiPlugin"/>.
    /// </summary>
    public class DemoDotSpatialGuiPlugin: GuiPlugin
    {
        private IRibbonCommandHandler ribbon;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return ribbon;
            }
        }

        public override void Activate()
        {
            ribbon = CreateMapRibbon();
        }

        private static MapRibbon CreateMapRibbon()
        {
            return new MapRibbon
            {
                OpenMapViewCommand = new OpenMapViewCommand()
            };
        }
    }
}
