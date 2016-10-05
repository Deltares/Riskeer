﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Structures;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class StructuresImporterTest
    {
        private readonly ObservableList<TestStructure> testImportTarget = new ObservableList<TestStructure>();
        private readonly ReferenceLine testReferenceLine = new ReferenceLine();
        private readonly string testFilePath = string.Empty;

        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Call
            var importer = new TestStructuresImporter(testImportTarget, testReferenceLine, testFilePath);

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
        }

        [Test]
        public void Constructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestStructuresImporter(null, testReferenceLine, testFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void Constructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestStructuresImporter(testImportTarget, null, testFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestStructuresImporter(testImportTarget, testReferenceLine, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        public void Import_FromInvalidEmptyPath_FalseAndLogError(string filePath)
        {
            // Setup
            var testStructuresImporter = new TestStructuresImporter(testImportTarget, testReferenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilsResources.Error_Path_must_be_specified);
                StringAssert.StartsWith(expectedMessage, messageArray[0]);
            });
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_FromPathContainingInvalidFileCharacters_FalseAndLogError()
        {
            // Setup
            string filePath = "c:\\Invalid_Characters.shp";

            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            var invalidPath = filePath.Replace('_', invalidFileNameChars[0]);

            var testStructuresImporter = new TestStructuresImporter(testImportTarget, testReferenceLine, invalidPath);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string message = messages.First();
                string expectedMessage = new FileReaderErrorMessageBuilder(invalidPath)
                    .Build(string.Format(CoreCommonUtilsResources.Error_Path_cannot_contain_Characters_0_, string.Join(", ", invalidFileNameChars)));
                StringAssert.StartsWith(expectedMessage, message);
            });
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_FromDirectoryPath_FalseAndLogError()
        {
            // Setup
            string folderPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO) + Path.DirectorySeparatorChar;

            var testStructuresImporter = new TestStructuresImporter(testImportTarget, testReferenceLine, folderPath);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(folderPath)
                .Build(CoreCommonUtilsResources.Error_Path_must_not_point_to_empty_file_name);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
        }

        [Test]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        [TestCase("Multiple_PolyLine_with_ID.shp")]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Single_Multi-PolyLine_with_ID.shp")]
        [TestCase("Single_Polygon_with_ID.shp")]
        [TestCase("Single_PolyLine_with_ID.shp")]
        public void Import_FromFileWithNonPointFeatures_FalseAndLogError(string shapeFileName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                         shapeFileName);

            var profilesImporter = new TestStructuresImporter(testImportTarget, testReferenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = profilesImporter.Import();

            // Assert
            string expectedMessage =
                string.Format("Fout bij het lezen van bestand '{0}': Kon geen punten vinden in dit bestand.", filePath);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
        }

        [Test]
        public void Import_InvalidShapefile_ReturnsFalse()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("Structures", "StructuresWithoutKWKIDENT", "Kunstwerken.shp"));

            var profilesImporter = new TestStructuresImporter(testImportTarget, testReferenceLine, invalidFilePath);

            // Call
            var importResult = profilesImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_InvalidCsvFile_ReturnsFalse()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("Structures", "CorrectShpIncorrectCsv", "CorrectKunstwerken_IncorrectCsv.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var profilesImporter = new TestStructuresImporter(testImportTarget, referenceLine, invalidFilePath);

            // Call
            var importResult = profilesImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_ReturnsFalse()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var testStructuresImporter = new TestStructuresImporter(testImportTarget, testReferenceLine, filePath);

            testStructuresImporter.Cancel();

            // Call
            bool importResult = testStructuresImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_ReturnsTrue()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var importTarget = new ObservableList<TestStructure>();
            var testStructuresImporter = new TestStructuresImporter(importTarget, referenceLine, filePath);

            testStructuresImporter.Cancel();
            bool importResult = testStructuresImporter.Import();

            // Precondition
            Assert.IsFalse(importResult);

            // Call
            importResult = testStructuresImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_LocationOutsideReferenceLine_LogErrorAndReturnTrue()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testStructuresImporter = new TestStructuresImporter(new ObservableList<TestStructure>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = "Een kunstwerklocatie met KWKIDENT 'KUNST6' ligt niet op de referentielijn. Locatie wordt overgeslagen.";
                StringAssert.StartsWith(expectedMessage, messageArray[0]);
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_DuplicateLocation_LogWarningAndReturnTrue()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "DuplicateLocation", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testStructuresImporter = new TestStructuresImporter(new ObservableList<TestStructure>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            string expectedMessage = "Kunstwerklocatie met KWKIDENT 'KUNST3' is opnieuw ingelezen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
        }

        [Test]
        public void Import_LocationKWKIDENTNull_LogErrorAndReturnFalse()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "StructuresWithNullKWKident", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testStructuresImporter = new TestStructuresImporter(new ObservableList<TestStructure>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testStructuresImporter.Import();

            // Assert
            string expectedMessage = "Fout bij het lezen van kunstwerk op regel 1. Het kunstwerk heeft geen geldige waarde voor attribuut 'KWKIDENT'. Dit kunstwerk wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
        }

        [Test]
        public void Import_IllegalCsvFile_ReturnsFalse()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("Structures", "IllegalCsv", "Kunstwerken.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(131144.094, 549979.893),
                new Point2D(131538.705, 548316.752),
                new Point2D(135878.442, 532149.859),
                new Point2D(131225.017, 548395.948),
                new Point2D(131270.38, 548367.462),
                new Point2D(131507.119, 548322.951)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testStructuresImporter = new TestStructuresImporter(new ObservableList<TestStructure>(), referenceLine, filePath);

            // Call
            var importResult = testStructuresImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        private class TestStructuresImporter : StructuresImporter<ObservableList<TestStructure>>
        {
            public TestStructuresImporter(ObservableList<TestStructure> importTarget, ReferenceLine referenceLine, string filePath)
                : base(importTarget, referenceLine, filePath) {}

            protected override void CreateSpecificStructures(ICollection<StructureLocation> structureLocations,
                                                             Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows) {}
        }

        private class TestStructure {}
    }
}