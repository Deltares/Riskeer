using System.Linq;
using Core.Common.Gui;

namespace Core.Plugins.CommonTools.Gui.Commands.Charting
{
    public class ShowChartLegendViewCommand : GuiCommand
    {
        public override bool Checked
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

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        public override void Execute(params object[] arguments)
        {
            var commonToolsGuiPlugin = Gui.Plugins.OfType<CommonToolsGuiPlugin>().FirstOrDefault();
            if (commonToolsGuiPlugin == null)
            {
                return;
            }

            var view = commonToolsGuiPlugin.ChartLegendView;
            var active = Gui.ToolWindowViews.Contains(view);

            if (active)
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