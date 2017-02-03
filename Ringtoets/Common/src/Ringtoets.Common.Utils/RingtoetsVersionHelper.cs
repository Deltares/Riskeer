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

namespace Ringtoets.Common.Utils
{
    /// <summary>
    /// This class is capable of comparing Ringtoets database versions.
    /// </summary>
    public static class RingtoetsVersionHelper
    {
        private const string validDatabaseVersion = "5";
        private const string currentDatabaseVersion = "17.1";

        /// <summary>
        /// Gets the current database version.
        /// </summary>
        /// <returns>The database version.</returns>
        public static string GetCurrentDatabaseVersion()
        {
            return currentDatabaseVersion;
        }

        /// <summary>
        /// Returns if the <paramref name="version"/> is newer than the current database version.
        /// </summary>
        /// <param name="version">The version to compare.</param>
        /// <returns><c>true</c> if <paramref name="version"/> is newer than the current 
        /// database version, <c>false</c> otherwise.</returns>
        public static bool IsNewerThanCurrent(string version)
        {
            var versionComparer = new RingtoetsVersionComparer();
            return versionComparer.Compare(version, currentDatabaseVersion) > 0;
        }

        /// <summary>
        /// Returns if the <paramref name="version"/> is a valid database version.
        /// </summary>
        /// <param name="version">The version to compare.</param>
        /// <returns><c>true</c> if <paramref name="version"/> is a valid database version, 
        /// <c>false</c> otherwise.</returns>
        public static bool IsValidVersion(string version)
        {
            var versionComparer = new RingtoetsVersionComparer();
            return versionComparer.Compare(version, validDatabaseVersion) >= 0;
        }
    }
}