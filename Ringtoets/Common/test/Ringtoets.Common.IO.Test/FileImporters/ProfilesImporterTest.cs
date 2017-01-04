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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.DikeProfiles;
using Ringtoets.Common.IO.FileImporters;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class ProfilesImporterTest
    {
        private readonly ObservableList<TestProfile> testImportTarget = new ObservableList<TestProfile>();
        private readonly ReferenceLine testReferenceLine = new ReferenceLine();
        private readonly string testFilePath = string.Empty;

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Call
            var importer = new TestProfilesImporter(testImportTarget, testReferenceLine, testFilePath);

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
        }

        [Test]
        public void ParameteredConstructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestProfilesImporter(null, testReferenceLine, testFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestProfilesImporter(testImportTarget, null, testFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestProfilesImporter(testImportTarget, testReferenceLine, null);

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
            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

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

            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, invalidPath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string message = messages.First();
                string expectedMessage = new FileReaderErrorMessageBuilder(invalidPath)
                    .Build(string.Format(CoreCommonUtilsResources.Error_Path_cannot_contain_Characters_0_, string.Join(", ", Path.GetInvalidFileNameChars())));
                StringAssert.StartsWith(expectedMessage, message);
            });
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_FromDirectoryPath_FalseAndLogError()
        {
            // Setup
            string folderPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin) + Path.DirectorySeparatorChar;

            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, folderPath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = new FileReaderErrorMessageBuilder(folderPath)
                    .Build(CoreCommonUtilsResources.Error_Path_must_not_point_to_empty_file_name);
                StringAssert.StartsWith(expectedMessage, messageArray[0]);
            });
            Assert.IsFalse(importResult);
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

            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage =
                    string.Format("Fout bij het lezen van bestand '{0}': kon geen punten vinden in dit bestand.", filePath);
                StringAssert.EndsWith(expectedMessage, messageArray[0]);
            });
            Assert.IsFalse(importResult);
        }

        [Test]
        [TestCase("Voorlanden_12-2_WithoutId.shp", "ID")]
        [TestCase("Voorlanden_12-2_WithoutName.shp", "Naam")]
        [TestCase("Voorlanden_12-2_WithoutX0.shp", "X0")]
        public void Import_FromFileMissingAttributeColumn_FalseAndLogError(
            string shapeFileName, string missingColumnName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", shapeFileName));

            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = string.Format("Het bestand heeft geen attribuut '{0}'. Dit attribuut is vereist.",
                                                       missingColumnName);
                Assert.AreEqual(expectedMessage, messageArray[0]);
            });
            Assert.IsFalse(importResult);
        }

        [Test]
        [TestCase("Voorlanden_12-2_IdWithSymbol.shp")]
        [TestCase("Voorlanden_12-2_IdWithWhitespace.shp")]
        public void Import_FromFileWithIllegalCharactersInId_TrueAndLogError(string fileName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", fileName));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = "Fout bij het lezen van profiellocatie 1. De locatie parameter 'ID' mag uitsluitend uit letters en cijfers bestaan. Dit profiel wordt overgeslagen.";
                Assert.AreEqual(expectedMessage, messageArray[0]);
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_FromFileWithEmptyEntryForId_TrueAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message1 = "Fout bij het lezen van profiellocatie 1. De locatie parameter 'ID' heeft geen waarde. Dit profiel wordt overgeslagen.";
                string message2 = "Fout bij het lezen van profiellocatie 2. De locatie parameter 'ID' heeft geen waarde. Dit profiel wordt overgeslagen.";
                string message3 = "Fout bij het lezen van profiellocatie 4. De locatie parameter 'ID' heeft geen waarde. Dit profiel wordt overgeslagen.";
                Assert.AreEqual(message1, messageArray[0]);
                Assert.AreEqual(message2, messageArray[1]);
                Assert.AreEqual(message3, messageArray[2]);
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_FromFileWithUnrelatedInvalidPrflFilesInSameFolder_TrueAndIgnoresUnrelatedFiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "OkTestDataWithUnrelatedPrfl", "Voorland 12-2.shp"));
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_FromFileWithEmptyEntryForX0_TrueAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyX0.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = "Fout bij het lezen van profiellocatie 1. Het profiel heeft geen geldige waarde voor attribuut 'X0'. Dit profiel wordt overgeslagen.";
                Assert.AreEqual(expectedMessage, messageArray[0]);
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_DikeProfileLocationsNotCloseEnoughToReferenceLine_TrueAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(141223.2, 548393.4),
                new Point2D(143854.3, 545323.1),
                new Point2D(145561.0, 541920.3),
                new Point2D(146432.1, 538235.2),
                new Point2D(146039.4, 533920.2)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            var expectedMessages = Enumerable.Repeat("Fout bij het lezen van profiellocatie 1. " +
                                                     "De profiellocatie met ID 'profiel001' ligt niet op de referentielijn. " +
                                                     "Dit profiel wordt overgeslagen.", 5).ToArray();
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, expectedMessages.Length);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_InvalidDamType_TrueAndLogMessage()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "InvalidDamType", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            Action<IEnumerable<string>> asserts = messages =>
            {
                bool found = messages.Any(message => message.EndsWith(": het ingelezen damtype ('4') moet 0, 1, 2 of 3 zijn."));
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_TwoPrflWithSameId_TrueAndErrorLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "TwoPrflWithSameId", "profiel001.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(130074.3, 543717.4),
                new Point2D(130084.3, 543727.4)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            Action<IEnumerable<string>> asserts = messages =>
            {
                var start = "Meerdere definities gevonden voor profiel 'profiel001'. Bestand '";
                var end = "' wordt overgeslagen.";
                bool found = messages.Any(m => m.StartsWith(start) && m.EndsWith(end));
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_FromFileWithDupplicateId_TrueAndLogWarnings()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_same_id_3_times.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string expectedMessage = "Profiellocatie met ID 'profiel001' is opnieuw ingelezen.";
                Assert.AreEqual(expectedMessage, messageArray[0]);
                Assert.AreEqual(expectedMessage, messageArray[1]);
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_PrflWithProfileNotZero_TrueAndErrorLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "PrflWithProfileNotZero", "Voorland_12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            Action<IEnumerable<string>> asserts = messages =>
            {
                bool found = messages.Any(message => message.StartsWith("Profielgegevens definiëren een damwand waarde ongelijk aan 0. Bestand wordt overgeslagen:"));
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_PrflIsIncomplete_FalseAndErrorLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "PrflIsIncomplete", "Voorland_12-2.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(130074.3, 543717.4),
                new Point2D(130084.3, 543727.4)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            Action<IEnumerable<string>> asserts = messages =>
            {
                bool found = messages.First().Contains(": de volgende parameters zijn niet aanwezig in het bestand: VOORLAND, DAMWAND, KRUINHOOGTE, DIJK, MEMO");
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImport()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Plugin,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, filePath);

            testProfilesImporter.Cancel();

            // Call
            bool importResult = testProfilesImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_True()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(130074.3, 543717.4),
                new Point2D(130084.3, 543727.4)
            };
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<TestProfile>(), referenceLine, filePath);

            testProfilesImporter.Cancel();
            bool importResult = testProfilesImporter.Import();

            // Precondition
            Assert.IsFalse(importResult);

            // Call
            importResult = testProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
        }

        private ReferenceLine CreateMatchingReferenceLine()
        {
            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(131223.2, 548393.4),
                new Point2D(133854.3, 545323.1),
                new Point2D(135561.0, 541920.3),
                new Point2D(136432.1, 538235.2),
                new Point2D(136039.4, 533920.2)
            });
            return referenceLine;
        }

        private class TestProfilesImporter : ProfilesImporter<ObservableList<TestProfile>>
        {
            public TestProfilesImporter(ObservableList<TestProfile> importTarget, ReferenceLine referenceLine, string filePath)
                : base(referenceLine, filePath, importTarget) {}

            protected override void CreateProfiles(ReadResult<ProfileLocation> importProfileLocationResult, ReadResult<DikeProfileData> importDikeProfileDataResult) {}

            protected override bool DikeProfileDataIsValid(DikeProfileData data, string prflFilePath)
            {
                return true;
            }
        }

        private class TestProfile {}
    }
}