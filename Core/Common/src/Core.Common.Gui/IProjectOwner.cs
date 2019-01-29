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
using Core.Common.Base.Data;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface declaring members related to owning a <see cref="IProject"/>.
    /// </summary>
    public interface IProjectOwner
    {
        /// <summary>
        /// Occurs just before a new instance is available at <see cref="Project"/>.
        /// </summary>
        event Action<IProject> BeforeProjectOpened;

        /// <summary>
        /// Occurs when a new instance is available at <see cref="Project"/>.
        /// </summary>
        event Action<IProject> ProjectOpened;

        /// <summary>
        /// Gets the project of the application.
        /// </summary>
        IProject Project { get; }

        /// <summary>
        /// Gets the project path of the application.
        /// </summary>
        string ProjectFilePath { get; }

        /// <summary>
        /// Sets the project and the path of the project that was used for obtaining it.
        /// </summary>
        /// <param name="project">The project that is used in the application.</param>
        /// <param name="projectPath">The file location where the <paramref name="project"/> was
        /// loaded from, or <c>null</c> if it was not loaded from a file source.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is <c>null</c>.</exception>
        void SetProject(IProject project, string projectPath);
    }
}