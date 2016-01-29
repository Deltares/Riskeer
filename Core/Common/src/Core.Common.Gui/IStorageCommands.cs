// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;

using Core.Common.Base.Data;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface for exposing commands/methods related to saving/loading a <see cref="Project"/>.
    /// </summary>
    public interface IStorageCommands : IDisposable
    {
        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <remarks>
        /// The creation action might be cancelled (due to user interaction).
        /// </remarks>
        void CreateNewProject();

        /// <summary>
        /// Saves the project to a new location.
        /// </summary>
        /// <returns>Returns if the project was successfully saved.</returns>
        bool SaveProjectAs();

        /// <summary>
        /// Saves the project to the currently selected location.
        /// </summary>
        /// <returns>Returns if the project was successfully saved.</returns>
        bool SaveProject();

        /// <summary>
        /// Opens an existing project.
        /// </summary>
        /// <remarks>
        /// The opening action might be cancelled (due to user interaction).
        /// </remarks>
        /// <returns>Whether or not an existing project was correctly opened.</returns>
        bool OpenExistingProject();

        /// <summary>
        /// Opens an existing project from file.
        /// </summary>
        /// <param name="filePath">The path to the existing project file.</param>
        /// <remarks>
        /// The opening action might be cancelled (due to user interaction).
        /// </remarks>
        /// <returns>Whether or not an existing project was correctly opened.</returns>
        bool OpenExistingProject(string filePath);

        /// <summary>
        /// Closes the current project.
        /// </summary>
        void CloseProject();
    }
}