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
        private LegendController legendController;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return chartingRibbon;
            }
        }

        public override void Activate()
        {
            legendController = CreateLegendController(this);
            chartingRibbon = CreateRibbon(legendController);

            legendController.ToggleLegend();
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

        public bool IsToolWindowOpen<T>()
        {
            return Gui.ToolWindowViews.Any(t => t.GetType() == typeof(T));
        }

        public void OpenToolView(IView toolView)
        {
            Gui.OpenToolView(toolView);
            UpdateComponentsForActiveView();
        }

        public void CloseToolView(IView toolView)
        {
            Gui.CloseToolView(toolView);
        }

        /// <summary>
        /// Creates a new <see cref="LegendController"/>.
        /// </summary>
        /// <param name="oxyPlotGuiPlugin">The <see cref="IOxyPlotGuiPlugin"/> to use for the controller
        /// <see cref="LegendController"/>.</param>
        /// <returns>A new <see cref="LegendController"/> instance.</returns>
        private static LegendController CreateLegendController(IOxyPlotGuiPlugin oxyPlotGuiPlugin)
        {
            var controller = new LegendController(oxyPlotGuiPlugin);
            return controller;
        }

        /// <summary>
        /// Creates the <see cref="ChartingRibbon"/> and the commands that will be used when clicking on the buttons.
        /// </summary>
        /// <param name="legendController">The <see cref="LegendController"/> to use for the 
        /// <see cref="ChartingRibbon"/>.</param>
        /// <returns>A new <see cref="ChartingRibbon"/> instance.</returns>
        private static ChartingRibbon CreateRibbon(LegendController legendController)
        {
            return new ChartingRibbon
            {
                OpenChartViewCommand = new OpenChartViewCommand(),
                ToggleLegendViewCommand = new ToggleLegendViewCommand(legendController)
            };
        }

        private void GuiOnActiveViewChanged(object sender, ActiveViewChangeEventArgs activeViewChangeEventArgs)
        {
            UpdateComponentsForActiveView();
        }

        /// <summary>
        /// Updates the components which the <see cref="OxyPlotGuiPlugin"/> knows about so that it reflects
        /// the currently active view.
        /// </summary>
        private void UpdateComponentsForActiveView()
        {
            var chartView = Gui.ActiveView as IChartView;
            if (chartView != null)
            {
                chartingRibbon.ShowChartingTab();
                legendController.UpdateForChart(chartView.Chart);
            }
            else
            {
                chartingRibbon.HideChartingTab();
                legendController.UpdateForChart(null);
            }
        }
    }
}