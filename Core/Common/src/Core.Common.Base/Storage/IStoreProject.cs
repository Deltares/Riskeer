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

namespace Core.Common.Base.Storage
{
    /// <summary>
    /// Interface that describes the methods that need to be implemented on classes that provide a storage for projects.
    /// </summary>
    public interface IStoreProject
    {
        /// <summary>
        /// Gets the file name filter string.
        /// </summary>
        string FileFilter { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has staged project.
        /// </summary>
        /// <seealso cref="StageProject"/>
        bool HasStagedProject { get; }

        /// <summary>
        /// Converts the staged project to a new storage entry. Upon return, the staged project is released and
        /// <see cref="HasStagedProject"/> will be <c>false</c>.
        /// </summary>
        /// <param name="connectionArguments">Arguments required to connect to the storage.</param>
        /// <exception cref="InvalidOperationException">Thrown when no project has been staged
        /// before calling this method.</exception>
        /// <exception cref="ArgumentException"><paramref name="connectionArguments"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">Thrown when no new storage was created.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item>No new storage was created.</item>
        /// <item>The storage is no valid project.</item>
        /// <item>Saving the staged project to the storage failed.</item>
        /// <item>The connection to the storage failed.</item>
        /// </list>
        /// </exception>
        void SaveProjectAs(string connectionArguments);

        /// <summary>
        /// Attempts to load the <see cref="IProject"/> from the storage.
        /// </summary>
        /// <param name="connectionArguments">Arguments required to connect to the storage.</param>
        /// <returns>Returns a new instance of <see cref="IProject"/> with the data from the storage or <c>null</c> when not found.</returns>
        /// <exception cref="ArgumentException"><paramref name="connectionArguments"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="connectionArguments"/> is invalid.</item>
        /// <item>The storage does not contain all requested tables.</item>
        /// <item>The connection to the storage failed.</item>
        /// <item>The related entity was not found in the storage.</item>
        /// <item>The storage's version is incompatible.</item>
        /// </list>
        /// </exception>
        IProject LoadProject(string connectionArguments);

        /// <summary>
        /// Stages the project (does some prep-work and validity checking) to be saved.
        /// </summary>
        /// <param name="project">The project to be prepared to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is <c>null</c>.</exception>
        void StageProject(IProject project);

        /// <summary>
        /// Unstages the project and discards all prep-work that has been done.
        /// </summary>
        void UnstageProject();

        /// <summary>
        /// Checks if the staged project differs from the <see cref="IProject"/> which can be found at
        /// <paramref name="filePath"/>, if any.
        /// </summary>
        /// <param name="filePath">The currently set path to the loaded project.</param>
        /// <returns><c>true</c> if <see cref="IProject"/> can be loaded from <paramref name="filePath"/> 
        /// and is different from the staged project, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no project has been staged.</exception>
        /// <exception cref="StorageException">Thrown when the staged project contains
        /// more than <see cref="int.MaxValue"/> unique object instances.</exception>
        bool HasStagedProjectChanges(string filePath);
    }
}