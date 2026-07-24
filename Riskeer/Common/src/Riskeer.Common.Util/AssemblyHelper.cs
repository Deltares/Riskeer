// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;

namespace Riskeer.Common.Util
{
    /// <summary>
    /// Helper class for finding assemblies.
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// Returns a full path to the application directory, which contains all assemblies that are required for running
        /// the application.
        /// </summary>
        /// <returns>A full path to the application directory.</returns>
        public static string GetApplicationDirectory()
        {
            DirectoryInfo currentDirectoryInfo = new DirectoryInfo(AppContext.BaseDirectory);
            while (currentDirectoryInfo != null)
            {
                string applicationDirectory = Path.Combine(currentDirectoryInfo.FullName, "Application");
                if (Directory.Exists(applicationDirectory))
                {
                    return applicationDirectory;
                }

                currentDirectoryInfo = currentDirectoryInfo.Parent;
            }

            throw new DirectoryNotFoundException($"Could not find an 'Application' directory by searching parent directories of '{AppContext.BaseDirectory}'.");
        }
    }
}