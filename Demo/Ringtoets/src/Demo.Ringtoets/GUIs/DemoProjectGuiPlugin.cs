﻿using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;

namespace Demo.Ringtoets.GUIs
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
                return new Ribbons.RingtoetsDemoProjectRibbon(Gui, Gui);
            }
        }
    }
}