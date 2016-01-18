using System;
using Core.Common.Controls.Commands;

namespace Core.Plugins.OxyPlot.Commands
{
    /// <summary>
    /// This class describes the command which toggles the panning interaction of a chart.
    /// </summary>
    public class TogglePanningCommand : ICommand
    {
        private readonly ChartingInteractionController controller;

        /// <summary>
        /// Creates a new instance of <see cref="TogglePanningCommand"/>.
        /// </summary>
        /// <param name="controller">The <see cref="ChartingInteractionController"/> which is used to update interaction with a chart.</param>
        public TogglePanningCommand(ChartingInteractionController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller", "Cannot create TogglePanningCommand without a ChartingInteractionController.");
            }
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
                return controller.IsPanningEnabled;
            }
        }

        public void Execute(params object[] arguments)
        {
            controller.TogglePanning();
        }
    }
}