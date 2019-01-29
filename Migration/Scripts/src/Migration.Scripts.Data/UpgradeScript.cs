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
using Core.Common.Util;
using Migration.Scripts.Data.Exceptions;

namespace Migration.Scripts.Data
{
    /// <summary>
    /// Class that provides methods for the upgrading a <see cref="IVersionedFile"/> for a specific version.
    /// </summary>
    public abstract class UpgradeScript
    {
        private readonly string fromVersion;
        private readonly string toVersion;

        /// <summary>
        /// Creates a new instance of the <see cref="UpgradeScript"/> class.
        /// </summary>
        /// <param name="fromVersion">The source version.</param>
        /// <param name="toVersion">The target version.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="fromVersion"/> is empty or <c>null</c>,</item>
        /// <item><paramref name="toVersion"/> is empty or <c>null</c>.</item>
        /// </list></exception>
        protected UpgradeScript(string fromVersion, string toVersion)
        {
            if (string.IsNullOrEmpty(fromVersion))
            {
                throw new ArgumentException(@"FromVersion must have a value.", nameof(fromVersion));
            }

            if (string.IsNullOrEmpty(toVersion))
            {
                throw new ArgumentException(@"ToVersion must have a value.", nameof(toVersion));
            }

            this.fromVersion = fromVersion;
            this.toVersion = toVersion;
        }

        /// <summary>
        /// The source version of <see cref="UpgradeScript"/>.
        /// </summary>
        /// <returns>The version.</returns>
        public string FromVersion()
        {
            return fromVersion;
        }

        /// <summary>
        /// The target version of <see cref="UpgradeScript"/>.
        /// </summary>
        /// <returns>The version.</returns>
        public string ToVersion()
        {
            return toVersion;
        }

        /// <summary>
        /// Uses <paramref name="sourceLocation"/> to upgrade to <paramref name="targetLocation"/>.
        /// </summary>
        /// <param name="sourceLocation">The source file to upgrade from.</param>
        /// <param name="targetLocation">The target file to upgrade to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sourceLocation"/> or 
        /// <paramref name="targetLocation"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        /// <exception cref="CriticalMigrationException">Thrown when upgrading failed.</exception>
        public void Upgrade(string sourceLocation, string targetLocation)
        {
            if (!IOUtils.IsValidFilePath(sourceLocation))
            {
                throw new ArgumentException($@"'{sourceLocation}' is not a valid file path.", nameof(sourceLocation));
            }

            if (!IOUtils.IsValidFilePath(targetLocation))
            {
                throw new ArgumentException($@"'{targetLocation}' is not a valid file path.", nameof(targetLocation));
            }

            PerformUpgrade(sourceLocation, targetLocation);
        }

        /// <summary>
        /// Performs the upgrade on <paramref name="targetLocation"/>.
        /// </summary>
        /// <param name="sourceLocation">The source file to upgrade from.</param>
        /// <param name="targetLocation">The target file to upgrade to.</param>
        /// <remarks>The <paramref name="sourceLocation"/> and <paramref name="targetLocation"/> has been verified in <see cref="Upgrade"/>.</remarks>
        /// <exception cref="CriticalMigrationException">Thrown when upgrading failed.</exception>
        protected abstract void PerformUpgrade(string sourceLocation, string targetLocation);
    }
}