using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Demo.Ringtoets.Ribbons;

namespace Demo.Ringtoets.GUIs
{
    /// <summary>
    /// UI plugin the provides access to the demo projects for Ringtoets.
    /// </summary>
    public class DemoProjectPlugin : PluginBase
    {
        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new RingtoetsDemoProjectRibbon(Gui, Gui);
            }
        }
    }
}