// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Windows.Input;
using Core.Gui.Commands;

namespace Core.Gui.Forms.MainWindow
{
    /// <summary>
    /// ViewModel for <see cref="StartScreen"/>.
    /// </summary>
    public class StartScreenViewModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="StartScreenViewModel"/>.
        /// </summary>
        /// <param name="newProjectAction">The action to perform to create a new project.</param>
        /// <param name="openProjectAction">The action to perform to open a project.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StartScreenViewModel(Action newProjectAction, Action openProjectAction)
        {
            if (newProjectAction == null)
            {
                throw new ArgumentNullException(nameof(newProjectAction));
            }

            if (openProjectAction == null)
            {
                throw new ArgumentNullException(nameof(openProjectAction));
            }
            
            NewProjectCommand = new RelayCommand(o => newProjectAction());
            OpenProjectCommand = new RelayCommand(o => openProjectAction());
        }

        /// <summary>
        /// Gets the command to create a new project.
        /// </summary>
        public ICommand NewProjectCommand { get; }
        
        /// <summary>
        /// Gets the command to open a project.
        /// </summary>
        public ICommand OpenProjectCommand { get; }
    }
}