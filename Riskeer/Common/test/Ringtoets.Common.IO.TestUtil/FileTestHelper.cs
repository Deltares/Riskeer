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

using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Ringtoets.Common.IO.TestUtil
{
    /// <summary>
    /// A helper for asserting files.
    /// </summary>
    public static class FileTestHelper
    {
        /// <summary>
        /// Asserts whether or not all essential shapefile related files exist (.shp, .shx and .dbf).
        /// </summary>
        /// <param name="directoryPath">The directory in which to search for the files.</param>
        /// <param name="baseName">The base name of the files to search for.</param>
        /// <param name="shouldExist">Whether or not the essential files should exist.</param>
        /// <exception cref="AssertionException">Thrown when the actual file existence does not meet the
        /// expected state.</exception>
        public static void AssertEssentialShapefilesExist(string directoryPath, string baseName, bool shouldExist)
        {
            string pathName = Path.Combine(directoryPath, baseName);
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shp"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shx"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".dbf"));
        }

        /// <summary>
        /// Asserts whether all essential shapefile related files (.shp, .shx and .dbf) contain the same binary
        /// content as the reference files.
        /// </summary>
        /// <param name="directoryPathWithActualFiles">The directory to obtain the actual files from.</param>
        /// <param name="baseNameOfActualFiles">The base name of the actual files.</param>
        /// <param name="directoryPathWithExpectedFiles">The directory to obtain the expected files from.</param>
        /// <param name="baseNameOfExpectedFiles">The base name of the expected files.</param>
        /// <param name="expectedShpBodyLength">The expected body length of the .shp file.</param>
        /// <param name="expectedShxBodyLength">The expected body length of the .shx file.</param>
        /// <param name="expectedDbfBodyLength">The expected body length of the .dbf file.</param>
        /// <exception cref="AssertionException">Thrown when the actual files do not contain the same binary
        /// content as the expected files.</exception>
        public static void AssertEssentialShapefileMd5Hashes(string directoryPathWithActualFiles,
                                                             string baseNameOfActualFiles,
                                                             string directoryPathWithExpectedFiles,
                                                             string baseNameOfExpectedFiles,
                                                             int expectedShpBodyLength,
                                                             int expectedShxBodyLength,
                                                             int expectedDbfBodyLength)
        {
            string pathName = Path.Combine(directoryPathWithActualFiles, baseNameOfActualFiles);
            string refPathName = Path.Combine(directoryPathWithExpectedFiles, baseNameOfExpectedFiles);

            AssertBinaryFileContent(refPathName, pathName, ".shp", 100, expectedShpBodyLength);
            AssertBinaryFileContent(refPathName, pathName, ".shx", 100, expectedShxBodyLength);
            AssertBinaryFileContent(refPathName, pathName, ".dbf", 32, expectedDbfBodyLength);
        }

        private static void AssertBinaryFileContent(string refPathName, string pathName, string extension, int headerLength, int bodyLength)
        {
            byte[] refContent = File.ReadAllBytes(refPathName + extension);
            byte[] content = File.ReadAllBytes(pathName + extension);
            Assert.AreEqual(headerLength + bodyLength, content.Length);
            Assert.AreEqual(refContent.Skip(headerLength).Take(bodyLength),
                            content.Skip(headerLength).Take(bodyLength));
        }
    }
}