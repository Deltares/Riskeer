using System.Linq;
using Core.Common.Gui;

namespace Core.Plugins.Charting.Commands
{
    public class ToggleChartLegendViewCommand : IGuiCommand
    {
        public bool Checked
        {
            get
            {
                if (Gui == null || Gui.ToolWindowViews == null)
                {
                    return false;
                }
                var chartingGuiPlugin = Gui.Plugins.OfType<ChartingGuiPlugin>().FirstOrDefault();
                if (chartingGuiPlugin == null)
                {
                    return false;
                }
                return Gui.ToolWindowViews.Contains(chartingGuiPlugin.ChartLegendView);
            }
        }

        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public IGui Gui { get; set; }

        /// <summary>
        /// Opens the <see cref="Core.Plugins.Charting.Forms.ChartLegendView"/> in the UI when it was closed.
        /// Closes the <see cref="Core.Plugins.Charting.Forms.ChartLegendView"/> in the UI when it was open.
        /// </summary>
        /// <param name="arguments">No arguments are needed when calling this method.</param>
        public void Execute(params object[] arguments)
        {
            var chartingGuiPlugin = Gui.Plugins.OfType<ChartingGuiPlugin>().FirstOrDefault();
            if (chartingGuiPlugin == null)
            {
                return;
            }

            var view = chartingGuiPlugin.ChartLegendView;

            if (Gui.ToolWindowViews.Contains(view))
            {
                Gui.ToolWindowViews.Remove(view);
            }
            else
            {
                chartingGuiPlugin.InitializeChartLegendView();
            }
        }
    }
}