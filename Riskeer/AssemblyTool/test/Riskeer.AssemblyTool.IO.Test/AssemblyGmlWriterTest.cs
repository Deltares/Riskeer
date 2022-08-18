// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test
{
    [TestFixture]
    public class AssemblyGmlWriterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.AssemblyTool.IO,
                                                                          nameof(AssemblyGmlWriterTest));

        [Test]
        public void Write_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssemblyGmlWriter.Write(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Write_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = new ExportableAssessmentSection(
                string.Empty, string.Empty, Enumerable.Empty<Point2D>(), ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                Enumerable.Empty<ExportableFailureMechanism>(), Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Call
            void Call() => AssemblyGmlWriter.Write(assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void Write_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            var assessmentSection = new ExportableAssessmentSection(
                string.Empty, string.Empty, Enumerable.Empty<Point2D>(), ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                Enumerable.Empty<ExportableFailureMechanism>(), Enumerable.Empty<ExportableCombinedSectionAssembly>());

            var filePath = new string('a', 249);

            // Call
            void Call() => AssemblyGmlWriter.Write(assessmentSection, filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(Call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        [Test]
        public void Write_FullyConfiguredAssessmentSection_ReturnsExpectedFile()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath(nameof(Write_FullyConfiguredAssessmentSection_ReturnsExpectedFile));
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "actualAssembly.gml");

            ExportableAssessmentSection assessmentSection = CreateExportableAssessmentSection();

            try
            {
                // Call
                AssemblyGmlWriter.Write(assessmentSection, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));

                string pathToExpectedFile = Path.Combine(testDataPath, "configuredAssembly.gml");
                string expectedXml = File.ReadAllText(pathToExpectedFile);
                string actualXml = File.ReadAllText(filePath);
                Assert.AreEqual(expectedXml, actualXml);
            }
            finally
            {
                DirectoryHelper.TryDelete(folderPath);
            }
        }

        private ExportableAssessmentSection CreateExportableAssessmentSection()
        {
            return new ExportableAssessmentSection(
                "Traject A", "section1", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(100.0, 0.0)
                },
                ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                Enumerable.Empty<ExportableFailureMechanism>(), Enumerable.Empty<ExportableCombinedSectionAssembly>());
        }
    }
}