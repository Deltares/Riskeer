// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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