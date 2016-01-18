using System;
using Core.Plugins.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// This class controls the interaction on a chart.
    /// </summary>
    public class ChartingInteractionController
    {
        private readonly IDocumentViewController controller;

        /// <summary>
        /// Creates a new instance of <see cref="ChartingInteractionController"/>.
        /// </summary>
        /// <param name="controller">The <see cref="IDocumentViewController"/> to perform view lookups on.</param>
        public ChartingInteractionController(IDocumentViewController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller", "Cannot create a ChartingInteractionController without an IDocumentViewController.");
            }
            this.controller = controller;
        }

        /// <summary>
        /// Gets a value representing whether the active view has a chart which is in panning mode.
        /// </summary>
        /// <returns><c>true</c> if the active view has a chart which has panning enabled, <c>false</c> otherwise.</returns>
        public bool IsPanningEnabled
        {
            get
            {
                var chartView = controller.ActiveView as IChartView;
                return chartView != null && chartView.Chart.IsPanning;
            }
        }

        /// <summary>
        /// Toggles the panning interaction for the chart on the active view, if there is one.
        /// </summary>
        public void TogglePanning()
        {
            var chartView = controller.ActiveView as IChartView;
            if (chartView != null)
            {
                chartView.Chart.TogglePanning();
            }
        }
    }
}