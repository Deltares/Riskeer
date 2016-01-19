using Core.Common.Controls.Commands;
using Core.Plugins.OxyPlot.Legend;

namespace Core.Plugins.OxyPlot.Commands
{
    /// <summary>
    /// This class describes the command which toggles the visibility of the <see cref="LegendView"/>.
    /// </summary>
    public class ToggleLegendViewCommand : ICommand
    {
        private readonly LegendController controller;

        /// <summary>
        /// Creates a new instance of <see cref="ToggleLegendViewCommand"/>.
        /// </summary>
        /// <param name="controller">The <see cref="LegendController"/> to use to invoke actions and determine state.</param>
        public ToggleLegendViewCommand(LegendController controller)
        {
            this.controller = controller;
        }

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
                return controller.IsLegendViewOpen();
            }
        }

        public void Execute(params object[] arguments)
        {
            controller.ToggleLegend();
        }
    }
}