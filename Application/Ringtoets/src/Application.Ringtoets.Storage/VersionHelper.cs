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
using System.Linq;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class is capable of comparing Ringtoets database versions.
    /// </summary>
    public static class VersionHelper
    {
        private const string validDatabaseversion = "4";
        private const string currentDatabaseVersion = "17.1";
        private const string versionSeparator = ".";

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
        /// <returns><c>true</c> is <paramref name="version"/> is newer than the current 
        /// database version, <c>false</c> otherwise.</returns>
        public static bool IsNewerThanCurrent(string version)
        {
            return CompareToVersion(version, currentDatabaseVersion) > 0;
        }

        /// <summary>
        /// Returns if the <paramref name="version"/> is a valid database version.
        /// </summary>
        /// <param name="version">The version to compare.</param>
        /// <returns><c>true</c> is <paramref name="version"/> is a valid database version, 
        /// <c>false</c> otherwise.</returns>
        public static bool IsValidVersion(string version)
        {
            return CompareToVersion(version, validDatabaseversion) >= 0;
        }

        private static int CompareToVersion(string versionString, string compareString)
        {
            var separatorArray = versionSeparator.ToCharArray();
            string[] versionArray = versionString.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);
            string[] currentVersionArray = compareString.Split(separatorArray, StringSplitOptions.RemoveEmptyEntries);

            if (versionArray.Length < 1)
            {
                return -1;
            }

            if (currentVersionArray.Length < 1)
            {
                return 1;
            }

            int version;
            int.TryParse(versionArray[0], out version);

            int currentVersion;
            int.TryParse(currentVersionArray[0], out currentVersion);

            var compareTo = version.CompareTo(currentVersion);
            if (compareTo > 0)
            {
                return compareTo;
            }
            if (compareTo == 0 && versionArray.Length > 1)
            {
                var newVersionString = string.Join(versionSeparator, versionArray.Skip(1).ToArray());
                var newCurrentVersionString = string.Join(versionSeparator, currentVersionArray.Skip(1).ToArray());
                return CompareToVersion(newVersionString, newCurrentVersionString);
            }
            return compareTo;
        }
    }
}