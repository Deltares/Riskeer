﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
        public void Constructor_FilePathNull_ThrowsArgumentException()
        {
            // Call
            void Call() => new AssemblyGmlWriter(null);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_InvalidFilePath_ThrowsArgumentException(string filePath)
        {
            // Call
            void Call() => new AssemblyGmlWriter(filePath);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var writer = new AssemblyGmlWriter("filepath");

            // Assert
            Assert.IsInstanceOf<IDisposable>(writer);
        }

        [Test]
        public void Write_AssemblyNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var writer = new AssemblyGmlWriter("filepath"))
            {
                // Call
                void Call() => writer.Write(null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(Call);
                Assert.AreEqual("assembly", exception.ParamName);
            }
        }

        [Test]
        public void Write_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            ExportableAssembly assembly = CreateExportableAssembly();
            var filePath = new string('a', 249);

            using (var writer = new AssemblyGmlWriter(filePath))
            {
                // Call
                void Call() => writer.Write(assembly);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(Call);
                Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
                Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
            }
        }

        [Test]
        public void Write_FullyConfiguredAssessmentSection_ReturnsExpectedFile()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath(nameof(Write_FullyConfiguredAssessmentSection_ReturnsExpectedFile));
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "actualAssembly.gml");

            ExportableAssembly assembly = CreateExportableAssembly();

            try
            {
                using (var writer = new AssemblyGmlWriter(filePath))
                {
                    // Call
                    writer.Write(assembly);
                }

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

        private static ExportableAssembly CreateExportableAssembly()
        {
            var assessmentSection = new ExportableAssessmentSection(
                "Traject A", "section1", new[]
                {
                    new Point2D(0, 0),
                    new Point2D(100.0, 0.0)
                },
                ExportableAssessmentSectionAssemblyResultTestFactory.CreateResult(),
                Enumerable.Empty<ExportableFailureMechanism>(), Enumerable.Empty<ExportableCombinedSectionAssembly>());

            var assessmentProcess = new ExportableAssessmentProcess("beoordelingsproces1", 2023, 2035, assessmentSection);

            return new ExportableAssembly("assemblage_1", assessmentSection, assessmentProcess);
        }
    }
}