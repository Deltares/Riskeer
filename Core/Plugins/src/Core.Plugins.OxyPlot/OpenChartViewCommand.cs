using Core.Common.Gui;
using Core.Components.OxyPlot;

namespace Core.Plugins.OxyPlot
{
    public class OpenChartViewCommand : IGuiCommand {
        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public IGui Gui { get; set; }

        public void Execute(params object[] arguments)
        {
            Gui.DocumentViewsResolver.OpenViewForData(new ChartData());
        }
    }
}