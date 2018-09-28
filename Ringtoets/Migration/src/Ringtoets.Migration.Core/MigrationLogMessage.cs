// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Ringtoets.Migration.Core
{
    /// <summary>
    /// Class that provides properties for logging messages that occurred during migration.
    /// </summary>
    public class MigrationLogMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="MigrationLogMessage"/>.
        /// </summary>
        /// <param name="fromVersion">The version migrated from.</param>
        /// <param name="toVersion">The version migrated to.</param>
        /// <param name="message">The message during the migration.</param>
        /// <exception cref="ArgumentException">Throws when any of the input parameters is 
        /// <c>null</c> or empty.</exception>
        public MigrationLogMessage(string fromVersion, string toVersion, string message)
        {
            if (string.IsNullOrEmpty(fromVersion))
            {
                throw new ArgumentException($@"Parameter '{nameof(fromVersion)}' must contain a value",
                                            nameof(fromVersion));
            }

            if (string.IsNullOrEmpty(toVersion))
            {
                throw new ArgumentException($@"Parameter '{nameof(toVersion)}' must contain a value",
                                            nameof(toVersion));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException($@"Parameter '{nameof(message)}' must contain a value",
                                            nameof(message));
            }

            FromVersion = fromVersion;
            ToVersion = toVersion;
            Message = message;
        }

        /// <summary>
        /// Gets the version that was migrated from.
        /// </summary>
        public string FromVersion { get; }

        /// <summary>
        /// Gets the version that was migrated to.
        /// </summary>
        public string ToVersion { get; }

        /// <summary>
        /// Gets the message occurred during the migration.
        /// </summary>
        public string Message { get; }
    }
}