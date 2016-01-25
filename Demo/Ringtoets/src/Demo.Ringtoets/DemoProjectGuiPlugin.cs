using Core.Common.Gui;
using Core.Common.Gui.Forms;

namespace Demo.Ringtoets
{
    /// <summary>
    /// UI plugin the provides access to the demo projects for Ringtoets.
    /// </summary>
    public class DemoProjectGuiPlugin : GuiPlugin
    {
        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new RingtoetsDemoProjectRibbon();
            }
        }
    }
}