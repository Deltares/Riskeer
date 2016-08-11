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
using System.Security.AccessControl;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class ReferenceLineExporterTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "test.shp");

            // Call
            var referenceLineExporter = new ReferenceLineExporter(referenceLine, filePath, "anId");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(referenceLineExporter);
        }

        [Test]
        public void ParameteredConstructor_NullFilePath_ThrowArgumentException()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => new ReferenceLineExporter(referenceLine, null, "anId");

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void Export_ValidData_ReturnTrue()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(11.11, 22.22)
            });

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "Export_ValidData_ReturnTrue");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");

            var exporter = new ReferenceLineExporter(referenceLine, filePath, "anId");

            bool isExported;
            try
            {
                // Call
                isExported = exporter.Export();
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }

            // Assert
            Assert.IsTrue(isExported);
        }

        [Test]
        public void Export_InvalidDirectoryRights_LogErrorAndReturnFalse()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(11.11, 22.22)
            });

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "Export_InvalidDirectoryRights_LogErrorAndReturnFalse");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");

            var exporter = new ReferenceLineExporter(referenceLine, filePath, "anId");

            bool isExported = true;
            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    isExported = exporter.Export();
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }

            // Assert
            Assert.IsFalse(isExported);
        }
    }
}