using Core.Common.Controls.Commands;
using Core.Plugins.OxyPlot.Legend;

namespace Core.Plugins.OxyPlot.Commands
{
    /// <summary>
    /// This class describes commands which control the state of a <see cref="LegendView"/> and uses a <see cref="LegendController"/>
    /// to do so. The state of this command is determined by the state of the <see cref="LegendView"/>.
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