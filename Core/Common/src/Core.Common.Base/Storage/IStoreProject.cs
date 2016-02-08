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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using Core.Common.Base.Data;

namespace Core.Common.Base.Storage
{
    /// <summary>
    /// Interface that describes the methods that need to be implemented on classes that provide a storage for Ringtoets projects.
    /// </summary>
    public interface IStoreProject
    {
        /// <summary>
        /// Converts <paramref name="project"/> to a new storage entry.
        /// </summary>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <param name="connectionArguments">Arguments required to connect to the storage.</param>
        /// <returns>Returns the number of changes that were saved.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="connectionArguments"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">Thrown when no new storage was created.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item>No new storage was created.</item>
        /// <item>The storage is no valid Ringtoets project.</item>
        /// <item>Saving the <paramref name="project"/> to the storage failed.</item>
        /// <item>The connection to the storage failed.</item>
        /// </list>
        /// </exception>
        int SaveProjectAs(string connectionArguments, Project project);

        /// <summary>
        /// Converts <paramref name="project"/> to an existing entity in the storage.
        /// </summary>
        /// <param name="connectionArguments"></param>
        /// <param name="project">The <see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes that were saved.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="connectionArguments"/> does not exist.</item>
        /// <item>The storage is no valid Ringtoets project.</item>
        /// <item>Saving the <paramref name="project"/> to the storage failed.</item>
        /// <item>The connection to the storage failed.</item>
        /// <item>The related entity was not found in the storage. Therefore, no update was possible.</item>
        /// </list>
        /// </exception>
        int SaveProject(string connectionArguments, Project project);

        /// <summary>
        /// Attempts to load the <see cref="Project"/> from the storage.
        /// </summary>
        /// <param name="connectionArguments">Arguments required to connect to the storage.</param>
        /// <returns>Returns a new instance of <see cref="Project"/> with the data from the storage or <c>null</c> when not found.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="connectionArguments"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="connectionArguments"/> is invalid.</item>
        /// <item>The storage does not contain all requested tables.</item>
        /// <item>The connection to the storage failed.</item>
        /// <item>The related entity was not found in the storage.</item>
        /// </list>
        /// </exception>
        Project LoadProject(string connectionArguments);
    }
}