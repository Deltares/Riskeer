using System.Collections.Generic;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Properties;
using Demo.Ringtoets.Ribbons;
using Demo.Ringtoets.Views;

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

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<ChartDataCollection, ChartDataView>
            {
                Image = Resources.ChartIcon,
                GetViewName = (v, o) => Resources.OxyPlotPlugin_GetViewInfos_Diagram
            };
        }
    }
}