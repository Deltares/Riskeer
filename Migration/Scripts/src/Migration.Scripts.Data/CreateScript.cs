// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Utils;
using Migration.Scripts.Data.Exceptions;

namespace Migration.Scripts.Data
{
    /// <summary>
    /// Class that provides methods for the creating an <see cref="IVersionedFile"/> for a specific version.
    /// </summary>
    public abstract class CreateScript
    {
        protected readonly string CreateQuery;
        private readonly string version;

        /// <summary>
        /// Creates a new instance of the <see cref="CreateScript"/> class.
        /// </summary>
        /// <param name="version">The version <paramref name="query"/> was designed for.</param>
        /// <param name="query">The SQL query that belongs to <paramref name="version"/>.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="version"/> is empty or <c>null</c>,</item>
        /// <item><paramref name="query"/> is empty, <c>null</c>, or consist out of only whitespace characters.</item>
        /// </list></exception>
        protected CreateScript(string version, string query)
        {
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentException(@"Version must have a value.", nameof(version));
            }
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException(@"Query must have a value.", nameof(query));
            }
            this.version = version;
            CreateQuery = query;
        }

        /// <summary>
        /// Creates a new <see cref="IVersionedFile"/> at <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The location to store the <see cref="IVersionedFile"/>.</param>
        /// <returns>A new <see cref="IVersionedFile"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="location"/>:
        /// <list type="bullet">
        /// <item>is empty or <c>null</c>,</item>
        /// <item>consists out of only whitespace characters,</item>
        /// <item>contains an invalid character,</item>
        /// <item>ends with a directory or path separator (empty file name).</item>
        /// </list></exception>
        /// <exception cref="CriticalMigrationException">Thrown when creating <see cref="IVersionedFile"/> 
        /// failed.</exception>
        /// <remarks>Creates the file if it does not exist.</remarks>
        public IVersionedFile CreateEmptyVersionedFile(string location)
        {
            IOUtils.CreateFileIfNotExists(location);
            return GetEmptyVersionedFile(location);
        }

        /// <summary>
        /// Gets the version <see cref="CreateScript"/> was created for.
        /// </summary>
        /// <returns>The version.</returns>
        public string Version()
        {
            return version;
        }

        /// <summary>
        /// Creates a new <see cref="IVersionedFile"/> at <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The location to store the <see cref="IVersionedFile"/>.</param>
        /// <returns>A new <see cref="IVersionedFile"/>.</returns>
        /// <remarks>The <paramref name="location"/> has been verified in <see cref="CreateEmptyVersionedFile"/>.</remarks>
        /// <exception cref="CriticalMigrationException">Thrown when creating <see cref="IVersionedFile"/> 
        /// failed.</exception>
        protected abstract IVersionedFile GetEmptyVersionedFile(string location);
    }
}