// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Storage;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Interface for exposing commands/methods related to saving/loading a <see cref="IProject"/>.
    /// </summary>
    public interface IStorageCommands
    {
        /// <summary>
        /// Closes the current project and creates a new one.
        /// </summary>
        void CreateNewProject();

        /// <summary>
        /// Checks whether the current project has unsaved changes. If so, these unsaved changes
        /// will be handled.
        /// </summary>
        /// <returns><c>true</c> if there were no unsaved changes or when the changes were
        /// successfully handled, <c>false</c> if the unsaved changes were not handled.</returns>
        bool HandleUnsavedChanges();

        /// <summary>
        /// Asks the user for a file-location to save the current project, then proceeds
        /// to persist the data to that location.
        /// </summary>
        /// <returns>Returns <c>true</c> if the save was successful, <c>false</c> otherwise.</returns>
        bool SaveProjectAs();

        /// <summary>
        /// Saves the current project to the defined file, or asks the user for a location
        /// if one hasn't been defined yet.
        /// </summary>
        /// <returns>Returns <c>true</c> if the save was successful, <c>false</c> otherwise.</returns>
        bool SaveProject();

        /// <summary>
        /// Asks the user to select the file to load a project from, then returns the selected
        /// file path of the selected file.
        /// </summary>
        /// <returns>The file path of the selected file.</returns>
        string GetExistingProjectFilePath();

        /// <summary>
        /// Loads a project from a given file-location.
        /// </summary>
        /// <param name="filePath">Location of the storage file.</param>
        /// <returns><c>true</c> if an existing <see cref="IProject"/> has been loaded, 
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is
        /// not a valid file path.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/>
        /// is <c>null</c>.</exception> 
        /// <exception cref="CriticalFileReadException">Thrown when the file at <paramref name="filePath"/>
        /// couldn't be read.</exception>
        /// <exception cref="StorageValidationException">Thrown when the file at <paramref name="filePath"/>
        /// is not a valid project file.</exception>
        bool OpenExistingProject(string filePath);
    }
}