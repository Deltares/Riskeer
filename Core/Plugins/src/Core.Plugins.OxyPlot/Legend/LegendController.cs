using System;
using Core.Common.Controls.Views;
using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class controls the actions which are related to controlling visibility and updating contents of a <see cref="LegendView"/>.
    /// </summary>
    public class LegendController
    {
        private readonly IToolViewController plugin;
        private IView legendView;

        /// <summary>
        /// Creates a new instance of <see cref="LegendController"/>.
        /// </summary>
        /// <param name="plugin">The <see cref="OxyPlotGuiPlugin"/> to invoke actions upon.</param>
        public LegendController(IToolViewController plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException("plugin", "Cannot create a LegendController when the plugin is null.");
            }
            this.plugin = plugin;
        }

        /// <summary>
        /// Toggles the <see cref="LegendView"/>.
        /// </summary>
        public void ToggleLegend()
        {
            if (IsLegendViewOpen())
            {
                CloseLegendView();
            }
            else
            {
                OpenLegendView();
            }
        }

        /// <summary>
        /// Checks whether a <see cref="LegendView"/> is open.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="LegendView"/> is open, <c>false</c> otherwise.</returns>
        public bool IsLegendViewOpen()
        {
            return plugin.IsToolWindowOpen<LegendView>();
        }

        /// <summary>
        /// Open the <see cref="LegendView"/>.
        /// </summary>
        private void OpenLegendView()
        {
            legendView = new LegendView();
            plugin.OpenToolView(legendView);
        }

        /// <summary>
        /// Closes the <see cref="LegendView"/>.
        /// </summary>
        private void CloseLegendView()
        {
            plugin.CloseToolView(legendView);
            legendView.Dispose();
            legendView = null;
        }

        /// <summary>
        /// Updates the data for the <see cref="LegendView"/> if it is open.
        /// </summary>
        /// <param name="chart">The <see cref="BaseChart"/> for which to show data. If <c>null</c> the 
        /// data will be cleared.</param>
        public void UpdateForChart(BaseChart chart)
        {
            if (IsLegendViewOpen())
            {
                legendView.Data = chart;
            }
        }
    }
}