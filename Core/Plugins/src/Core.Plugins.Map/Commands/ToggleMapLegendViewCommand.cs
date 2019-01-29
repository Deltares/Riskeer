// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Plugins.Map.Legend;

namespace Core.Plugins.Map.Commands
{
    /// <summary>
    /// The command which toggles the visibility of the <see cref="MapLegendView"/>.
    /// </summary>
    public class ToggleMapLegendViewCommand : ICommand
    {
        private readonly MapLegendController controller;

        /// <summary>
        /// Creates a new instance of <see cref="ToggleMapLegendViewCommand"/>.
        /// </summary>
        /// <param name="controller">The <see cref="MapLegendController"/> to use to invoke actions and determine state.</param>
        public ToggleMapLegendViewCommand(MapLegendController controller)
        {
            this.controller = controller;
        }

        public bool Checked
        {
            get
            {
                return controller.IsMapLegendViewOpen;
            }
        }

        public void Execute()
        {
            controller.ToggleView();
        }
    }
}