﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Core.Common.Base.Storage
{
    /// <summary>
    /// Interface that describes the methods that need to be implemented on classes that provide a storage for projects.
    /// </summary>
    public interface IMigrateProject
    {
        /// <summary>
        /// Indicates if the project from <paramref name="filePath"/> needs to be 
        /// updated to the newest version.
        /// </summary>
        /// <param name="filePath">The filepath of the project which needs to be checked.</param>
        /// <returns>The indicator if migration is required or not.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is an invalid file path.</exception>
        MigrationNeeded ShouldMigrate(string filePath);

        /// <summary>
        /// Determines the target filepath to place the migrated Ringtoets project based
        /// on it's original filepath.
        /// </summary>
        /// <returns>The filepath to be used as location for the migration result. Value
        /// will be <c>null</c> if no target filepath an be provided.</returns>
        string DetermineMigrationLocation(string originalFilePath);

        /// <summary>
        /// Migrates an outdated project file from <paramref name="sourceFilePath"/>
        /// to the newest project version version at a user defined target filepath.
        /// </summary>
        /// <param name="sourceFilePath">The filepath of the project which needs to be migrated.</param>
        /// <param name="targetFilePath">The filepath where the migrated project is written to.</param>
        /// <returns>Returns <c>true</c> if the migration was successful; return <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="sourceFilePath"/>
        /// or <paramref name="targetFilePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when either <paramref name="sourceFilePath"/>
        /// or <paramref name="targetFilePath"/> is an invalid file path.</exception>
        bool Migrate(string sourceFilePath, string targetFilePath);
    }
}
