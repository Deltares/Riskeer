using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.OxyPlot.Data;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;

namespace Core.Plugins.OxyPlot
{
    public class OxyPlotGuiPlugin : GuiPlugin, IOxyPlotGuiPlugin
    {
        private ChartingRibbon chartingRibbon;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return chartingRibbon;
            }
        }

        public bool IsToolWindowOpen<T>()
        {
            return Gui.ToolWindowViews.Any(t => t.GetType() == typeof(T));
        }

        public void OpenToolView(IView toolView)
        {
            Gui.OpenToolView(toolView);
        }

        public void CloseToolView(IView toolView)
        {
            Gui.CloseToolView(toolView);
        }

        public override void Activate()
        {
            CreateRibbon();
            Gui.ActiveViewChanged += GuiOnActiveViewChanged;
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<IChartData, ChartDataView>
            {
                Image = Resources.ChartIcon,
                GetViewName = (v, o) => "Diagram"
            };
        }

        /// <summary>
        /// Creates the ribbon and the commands that will be used when clicking on the buttons.
        /// </summary>
        private void CreateRibbon()
        {
            var legendController = new LegendController(this);

            chartingRibbon = new ChartingRibbon
            {
                OpenChartViewCommand = new OpenChartViewCommand(),
                ToggleLegendViewCommand = new ToggleLegendViewCommand(legendController)
            };
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
    }
}