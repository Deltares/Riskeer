// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.IO;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Helper containing a source of invalid paths that can be used in tests as a TestCaseSource.
    /// </summary>
    public static class InvalidPathHelper
    {
        /// <summary>
        /// The amount of characters used for a path part that should always be too long.
        /// </summary>
        private const int tooLongPathLength = 33000;

        /// <summary>
        /// Gets a single path part that exceeds the supported length.
        /// </summary>
        public static string TooLongPathPart => new string('a', tooLongPathLength);

        /// <summary>
        /// Gets an absolute folder path that exceeds the supported length.
        /// </summary>
        public static string TooLongFolderPath => $@"C:{Path.DirectorySeparatorChar}{TooLongPathPart}{Path.DirectorySeparatorChar}";

        /// <summary>
        /// Creates an absolute file path that exceeds the supported length.
        /// </summary>
        /// <param name="fileName">The file name to append to the too long folder path.</param>
        /// <returns>A path that should always exceed the supported length.</returns>
        public static string CreateTooLongFilePath(string fileName)
        {
            return Path.Combine(TooLongFolderPath, fileName);
        }

        /// <summary>
        /// Returns a collection of invalid paths.
        /// </summary>
        /// <example>[TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]</example>
        public static IEnumerable<string> InvalidPaths
        {
            get
            {
                return new[]
                {
                    "",
                    "   ",
                    $@"C:{Path.DirectorySeparatorChar}>",
                    $@"C:{Path.DirectorySeparatorChar}"
                };
            }
        }
    }
}