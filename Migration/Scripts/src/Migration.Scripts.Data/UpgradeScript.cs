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
        /// <item><paramref name="toVersion"/> is empty or <c>null</c>,</item>
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
        /// Uses <paramref name="source"/> to upgrade to <paramref name="target"/>.
        /// </summary>
        /// <param name="source">The source file to upgrade from.</param>
        /// <param name="target">The target file to upgrade to.</param>
        /// <exception cref="ArgumentNullException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="source"/> is <c>null</c>,</item>
        /// <item><paramref name="target"/> is <c>null</c>.</item>
        /// </list></exception>
        /// <exception cref="CriticalMigrationException">Thrown when upgrading failed.</exception>
        public void Upgrade(IVersionedFile source, IVersionedFile target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            PerformUpgrade(source, target);
        }

        /// <summary>
        /// Performs the upgrade on <paramref name="target"/>.
        /// </summary>
        /// <param name="source">The source file to upgrade from.</param>
        /// <param name="target">The target file to upgrade to.</param>
        /// <remarks>The <paramref name="source"/> and <paramref name="target"/> has been verified in <see cref="Upgrade"/>.</remarks>
        /// <exception cref="CriticalMigrationException">Thrown when upgrading failed.</exception>
        protected abstract void PerformUpgrade(IVersionedFile source, IVersionedFile target);
    }
}