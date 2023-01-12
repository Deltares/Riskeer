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

using System.IO;
using System.IO.Compression;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Util.Test
{
    [TestFixture]
    public class DirectoryHelperTest
    {
        [Test]
        public void GivenDirectoryWithFilesThatAreLockedDueToSystemLatency_WhenCallingTryDelete_ThenDirectoryCorrectlyRemoved()
        {
            // Given
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(DirectoryHelperTest)}.{nameof(GivenDirectoryWithFilesThatAreLockedDueToSystemLatency_WhenCallingTryDelete_ThenDirectoryCorrectlyRemoved)}");
            string nestedFolderPath = Path.Combine(folderPath, "Nested");
            string zipPath = Path.Combine(folderPath, "Archive.zip");

            Directory.CreateDirectory(nestedFolderPath);

            using (File.Create(Path.Combine(nestedFolderPath, "File1.txt"))) {}

            using (File.Create(Path.Combine(nestedFolderPath, "File2.txt"))) {}

            using (File.Create(Path.Combine(nestedFolderPath, "File3.txt"))) {}

            ZipFile.CreateFromDirectory(nestedFolderPath, zipPath);

            // When
            DirectoryHelper.TryDelete(folderPath);

            // Then
            Assert.IsFalse(Directory.Exists(folderPath));
        }
    }
}