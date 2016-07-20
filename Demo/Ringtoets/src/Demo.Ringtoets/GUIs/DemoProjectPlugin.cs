using System.Collections.Generic;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.Charting.Data;
using Core.Components.Gis.Data;
using Demo.Ringtoets.Ribbons;
using Demo.Ringtoets.Views;

using ChartResources = Core.Plugins.Chart.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

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
                Image = ChartResources.ChartIcon,
                GetViewName = (v, o) => ChartResources.OxyPlotPlugin_GetViewInfos_Diagram
            };

            yield return new ViewInfo<MapData, MapDataView>
            {
                Image = CoreCommonGuiResources.DocumentHS,
                GetViewName = (v, o) => CoreCommonGuiResources.DotSpatialPlugin_GetViewInfoObjects_Map
            };
        }
    }
}