using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.OxyPlot.Data;
using Core.Plugins.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Properties;

namespace Core.Plugins.OxyPlot
{
    public class OxyPlotGuiPlugin : GuiPlugin
    {
        private IRibbonCommandHandler ribbon = new Ribbon();

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return ribbon;
            }
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<IChartData, ChartDataView>
            {
                Image = Resources.ChartIcon,
                GetViewName = (v, o) => "Diagram"
            };
        }
    }
}