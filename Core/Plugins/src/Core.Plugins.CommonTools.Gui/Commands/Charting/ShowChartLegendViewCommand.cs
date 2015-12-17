using System.Linq;
using Core.Common.Gui;
using Core.Plugins.CommonTools.Gui.Forms.Charting;

namespace Core.Plugins.CommonTools.Gui.Commands.Charting
{
    public class ShowChartLegendViewCommand : IGuiCommand
    {
        public bool Checked
        {
            get
            {
                if (Gui == null || Gui.ToolWindowViews == null)
                {
                    return false;
                }
                var commonToolsGuiPlugin = Gui.Plugins.OfType<CommonToolsGuiPlugin>().FirstOrDefault();
                if (commonToolsGuiPlugin == null)
                {
                    return false;
                }
                return Gui.ToolWindowViews.Contains(commonToolsGuiPlugin.ChartLegendView);
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
        /// Opens the <see cref="ChartLegendView"/> in the UI when it was closed.
        /// Closes the <see cref="ChartLegendView"/> in the UI when it was open.
        /// </summary>
        /// <param name="arguments">No arguments are needed when calling this method.</param>
        public void Execute(params object[] arguments)
        {
            var commonToolsGuiPlugin = Gui.Plugins.OfType<CommonToolsGuiPlugin>().FirstOrDefault();
            if (commonToolsGuiPlugin == null)
            {
                return;
            }

            var view = commonToolsGuiPlugin.ChartLegendView;

            if (Gui.ToolWindowViews.Contains(view))
            {
                Gui.ToolWindowViews.Remove(view);
            }
            else
            {
                commonToolsGuiPlugin.InitializeChartLegendView();
            }
        }
    }
}