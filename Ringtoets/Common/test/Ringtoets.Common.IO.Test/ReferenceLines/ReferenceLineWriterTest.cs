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
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.ReferenceLines;

namespace Ringtoets.Common.IO.Test.ReferenceLines
{
    [TestFixture]
    public class ReferenceLineWriterTest
    {
        [Test]
        public void WriteReferenceLine_NullReferenceLine_ThrowArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine(nameof(WriteReferenceLine_NullReferenceLine_ThrowArgumentNullException),
                                                                        "test.shp"));

            var writer = new ReferenceLineWriter();

            // Call
            TestDelegate call = () => writer.WriteReferenceLine(null, "anId", filePath);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void WriteReferenceLine_NullId_ThrowArgumentNullException()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(11.11, 22.22)
            });

            string filePath = TestHelper.GetScratchPadPath(Path.Combine(nameof(WriteReferenceLine_NullId_ThrowArgumentNullException),
                                                                        "test.shp"));

            var writer = new ReferenceLineWriter();

            // Call
            TestDelegate call = () => writer.WriteReferenceLine(referenceLine, null, filePath);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("   ")]
        public void WriteReferenceLine_InvalidId_ThrowArgumentException(string id)
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(11.11, 22.22)
            });

            string filePath = TestHelper.GetScratchPadPath(Path.Combine(nameof(WriteReferenceLine_InvalidId_ThrowArgumentException),
                                                                        "test.shp"));

            var writer = new ReferenceLineWriter();

            // Call
            TestDelegate call = () => writer.WriteReferenceLine(referenceLine, id, filePath);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void WriteReferenceLine_NullFilePath_ThrowArgumentNullException()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(11.11, 22.22)
            });

            var writer = new ReferenceLineWriter();

            // Call
            TestDelegate call = () => writer.WriteReferenceLine(referenceLine, "anId", null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void WriteReferenceLine_ValidData_WritesShapeFile()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(11.11, 22.22)
            });

            string directoryPath = TestHelper.GetScratchPadPath(nameof(WriteReferenceLine_ValidData_WritesShapeFile));
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(WriteReferenceLine_ValidData_WritesShapeFile)))
            {
                Directory.CreateDirectory(directoryPath);
                string filePath = Path.Combine(directoryPath, "test.shp");
                var baseName = "test";

                var writer = new ReferenceLineWriter();

                // Precondition
                AssertEssentialShapefileExists(directoryPath, baseName, false);

                // Call
                writer.WriteReferenceLine(referenceLine, "anId", filePath);

                // Assert
                AssertEssentialShapefileExists(directoryPath, baseName, true);
                AssertEssentialShapefileMd5Hashes(directoryPath, baseName);
            }
        }

        private static void AssertEssentialShapefileExists(string directoryPath, string baseName, bool shouldExist)
        {
            string pathName = Path.Combine(directoryPath, baseName);
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shp"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shx"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".dbf"));
        }

        private void AssertEssentialShapefileMd5Hashes(string directoryPath, string baseName)
        {
            string refPathName = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO), "LineShapefileMd5");
            string pathName = Path.Combine(directoryPath, baseName);

            AssertBinaryFileContent(refPathName, pathName, ".shp", 100, 88);
            AssertBinaryFileContent(refPathName, pathName, ".shx", 100, 8);
            AssertBinaryFileContent(refPathName, pathName, ".dbf", 32, 289);
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