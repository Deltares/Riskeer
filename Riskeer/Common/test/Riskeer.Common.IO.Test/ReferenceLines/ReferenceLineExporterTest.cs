// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.ReferenceLines;

namespace Riskeer.Common.IO.Test.ReferenceLines
{
    [TestFixture]
    public class ReferenceLineExporterTest
    {
        [Test]
        public void ParameteredConstructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            string filePath = TestHelper.GetScratchPadPath(nameof(ParameteredConstructor_ValidParameters_ExpectedValues));

            // Call
            var referenceLineExporter = new ReferenceLineExporter(referenceLine, "anId", filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(referenceLineExporter);
        }

        [Test]
        public void ParameteredConstructor_NullFilePath_ThrowArgumentException()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () => new ReferenceLineExporter(referenceLine, "anId", null);

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

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_ValidData_ReturnTrue));
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_ValidData_ReturnTrue)))
            {
                string filePath = Path.Combine(directoryPath, "test.shp");

                var exporter = new ReferenceLineExporter(referenceLine, "anId", filePath);

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
            }
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

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse)))
            {
                string filePath = Path.Combine(directoryPath, "test.shp");
                var exporter = new ReferenceLineExporter(referenceLine, "anId", filePath);

                disposeHelper.LockDirectory(FileSystemRights.Write);
                var isExported = true;

                // Call

                Action call = () => isExported = exporter.Export();

                // Assert
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.{Environment.NewLine}Er is geen referentielijn geëxporteerd.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
                Assert.IsFalse(isExported);
            }
        }
    }
}