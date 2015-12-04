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

                return Gui.ToolWindowViews.Contains(CommonToolsGuiPlugin.ChartLegendView);
            }
        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        protected override void OnExecute(params object[] arguments)
        {
            var commonToolsGuiPlugin = Gui.Plugins.OfType<CommonToolsGuiPlugin>().FirstOrDefault();
            if (commonToolsGuiPlugin == null)
            {
                return;
            }

            var view = CommonToolsGuiPlugin.ChartLegendView;
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