using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// This class ties all the components together to enable charting interaction.
    /// </summary>
    public class OxyPlotGuiPlugin : GuiPlugin, IToolViewController
    {
        private ChartingRibbon chartingRibbon;

        private LegendController legendController;

        private bool activated;

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
            activated = true;
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<ICollection<ChartData>, ChartDataView>
            {
                Image = Resources.ChartIcon,
                GetViewName = (v, o) => "Diagram"
            };
        }

        public override void Dispose()
        {
            if (activated)
            {
                Gui.ActiveViewChanged -= GuiOnActiveViewChanged;
            }
            base.Dispose();
        }

        #region IToolWindowController

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

        #endregion

        /// <summary>
        /// Creates a new <see cref="LegendController"/>.
        /// </summary>
        /// <param name="toolViewController">The <see cref="IToolViewController"/> to use for the controller
        /// <see cref="LegendController"/>.</param>
        /// <returns>A new <see cref="LegendController"/> instance.</returns>
        private static LegendController CreateLegendController(IToolViewController toolViewController)
        {
            var controller = new LegendController(toolViewController);
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
                chartingRibbon.Chart = chartView.Chart;
                legendController.UpdateForChart(chartView.Chart);
            }
            else
            {
                chartingRibbon.Chart = null;
                legendController.UpdateForChart(null);
            }
        }
    }
}