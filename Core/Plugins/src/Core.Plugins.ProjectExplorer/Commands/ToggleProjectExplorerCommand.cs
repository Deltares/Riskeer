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

using System;
using Core.Common.Controls.Commands;

namespace Core.Plugins.ProjectExplorer.Commands
{
    /// <summary>
    /// This class describes commands which are used to toggle the project explorer in the GUI.
    /// </summary>
    public class ToggleProjectExplorerCommand : ICommand
    {
        private readonly ProjectExplorerViewController viewController;

        /// <summary>
        /// Creates a new instance of <see cref="ToggleProjectExplorerCommand"/>.
        /// </summary>
        /// <param name="viewController">The <see cref="ProjectExplorerViewController"/> to use for
        /// querying and modifying the project explorer's state.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="viewController"/> is <c>null</c>.</exception>
        public ToggleProjectExplorerCommand(ProjectExplorerViewController viewController)
        {
            if (viewController == null)
            {
                throw new ArgumentNullException(nameof(viewController));
            }

            this.viewController = viewController;
        }

        public bool Checked
        {
            get
            {
                return viewController.IsProjectExplorerOpen;
            }
        }

        public void Execute()
        {
            viewController.ToggleView();
        }
    }
}