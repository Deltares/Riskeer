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
        private ChartingRibbon chartingRibbon;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return chartingRibbon;
            }
        }

        public override void Activate()
        {
            chartingRibbon = new ChartingRibbon();
            Gui.ActiveViewChanged += GuiOnActiveViewChanged;
        }

        private void GuiOnActiveViewChanged(object sender, ActiveViewChangeEventArgs activeViewChangeEventArgs)
        {
            if (activeViewChangeEventArgs.View is IChartView)
            {
                chartingRibbon.ShowChartingTab();
            }
            else
            {
                chartingRibbon.HideChartingTab();
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