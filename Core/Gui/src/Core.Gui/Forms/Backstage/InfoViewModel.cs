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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Common.Base.Data;

namespace Core.Gui.Forms.Backstage
{
    /// <summary>
    /// ViewModel for <see cref="InfoBackstagePage"/>.
    /// </summary>
    public class InfoViewModel : IBackstagePageViewModel, INotifyPropertyChanged
    {
        private IProject project;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        public string ProjectName => project?.Name;

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string ProjectDescription
        {
            get => project?.Description;
            set
            {
                project.Description = value;
                OnPropertyChanged(nameof(ProjectDescription));
            }
        }

        /// <summary>
        /// Sets the project.
        /// </summary>
        /// <param name="projectToSet">The project to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when<paramref name="projectToSet"/>
        /// is <c>null</c>.</exception>
        public void SetProject(IProject projectToSet)
        {
            if (projectToSet == null)
            {
                throw new ArgumentNullException(nameof(projectToSet));
            }

            project = projectToSet;
            OnPropertyChanged(nameof(ProjectName));
            OnPropertyChanged(nameof(ProjectDescription));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}