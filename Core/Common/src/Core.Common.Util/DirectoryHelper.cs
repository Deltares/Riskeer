// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.IO;
using System.Threading;

namespace Core.Common.Util
{
    /// <summary>
    /// Class containing helper methods for correctly dealing with directories.
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// Tries to delete the directory specified by <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <param name="numberOfRetries">The number of retries in case an <see cref="IOException"/> occurs.</param>
        /// <remarks>This helper method is a solution to latency issues caused by file locks, which normally cause an
        /// <see cref="IOException"/>. The file locks at stake will be released as soon as the application becomes idle,
        /// which explains retrying the delete action for <paramref name="numberOfRetries"/> times.</remarks>
        /// <exception cref="IOException">Thrown when:
        /// <list type="bullet">
        /// <item>a file with the same name and location exists;</item>
        /// <item>the directory is read-only;</item>
        /// <item>the directory is the application's current working directory;</item>
        /// <item>the directory contains a read-only file;</item>
        /// <item>the directory is being used by another process.</item>
        /// </list>
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permissions.</exception>
        /// <exception cref="ArgumentException">Thrown when the specified path is a zero-length string, contains only white spaces, or contains one or more invalid characters.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <c>null</c>.</exception>
        /// <exception cref="PathTooLongException">Thrown when the specified path exceeds the system defined maximum length.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the specified path:
        /// <list type="bullet">
        /// <item>does not exist or could not be found;</item>
        /// <item>is invalid (for example, it is on an unmapped drive).</item>
        /// </list>
        /// </exception>
        public static void TryDelete(string path, int numberOfRetries = 5)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                if (numberOfRetries != 0)
                {
                    Thread.Sleep(100);

                    TryDelete(path, numberOfRetries - 1);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}