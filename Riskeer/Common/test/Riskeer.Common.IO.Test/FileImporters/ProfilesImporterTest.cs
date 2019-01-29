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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.IO.DikeProfiles;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;

namespace Riskeer.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class ProfilesImporterTest
    {
        private readonly ObservableList<object> testImportTarget = new ObservableList<object>();
        private readonly ReferenceLine testReferenceLine = new ReferenceLine();
        private readonly string testFilePath = string.Empty;

        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            var importer = new TestProfilesImporter(testImportTarget, testReferenceLine, testFilePath, messageProvider);

            // Assert
            Assert.IsInstanceOf<IFileImporter>(importer);
        }

        [Test]
        public void ParameteredConstructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestProfilesImporter(null, testReferenceLine, testFilePath, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importTarget", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_ReferenceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestProfilesImporter(testImportTarget, null, testFilePath, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestProfilesImporter(testImportTarget, testReferenceLine, null, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestProfilesImporter(testImportTarget, testReferenceLine, testFilePath, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_TypeDescriptorNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestProfilesImporter(testImportTarget, testReferenceLine, testFilePath, messageProvider, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("typeDescriptor", exception.ParamName);
        }

        [Test]
        [TestCase("", "bestandspad mag niet leeg of ongedefinieerd zijn.")]
        [TestCase("      ", "bestandspad mag niet leeg of ongedefinieerd zijn.")]
        [TestCase("c>\\Invalid_Characters.shp", "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.")]
        [TestCase("c:\\Directory\\", "bestandspad mag niet verwijzen naar een lege bestandsnaam.")]
        public void Import_FromInvalidPath_FalseAndLogError(string filePath, string errorMessage)
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': " +
                                     errorMessage;
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
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
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                         shapeFileName);

            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': kon geen punten vinden in dit bestand.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
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
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", shapeFileName));

            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            string expectedMessage = $"Het bestand heeft geen attribuut '{missingColumnName}'. Dit attribuut is vereist.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        [TestCase("Voorlanden_12-2_IdWithSymbol.shp")]
        [TestCase("Voorlanden_12-2_IdWithWhitespace.shp")]
        public void Import_FromFileWithIllegalCharactersInId_FalseAndLogError(string fileName)
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", fileName));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            const string expectedMessage = "Fout bij het lezen van profiellocatie 1. De locatie parameter 'ID' mag uitsluitend uit letters en cijfers bestaan.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_FromFileWithEmptyEntryForId_FalseAndLogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            const string expectedMessage = "Fout bij het lezen van profiellocatie 1. De locatie parameter 'ID' heeft geen waarde.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_FromFileWithEmptyEntryForX0_FalseAndLogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyX0.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            const string expectedMessage = "Fout bij het lezen van profiellocatie 1. Het profiel heeft geen geldige waarde voor attribuut 'X0'.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_DikeProfileLocationsNotCloseEnoughToReferenceLine_FalseAndLogError()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var referencePoints = new List<Point2D>
            {
                new Point2D(141223.2, 548393.4),
                new Point2D(143854.3, 545323.1),
                new Point2D(145561.0, 541920.3),
                new Point2D(146432.1, 538235.2),
                new Point2D(146039.4, 533920.2)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            const string expectedMessages = "Fout bij het lezen van profiellocatie 1. De profiellocatie met ID 'profiel001' ligt niet op de referentielijn.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessages, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_InvalidDamType_FalseAndLogMessage()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string testFileDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                  Path.Combine("DikeProfiles", "InvalidDamType"));
            string filePath = Path.Combine(testFileDirectory, "Voorlanden 12-2.shp");

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            string erroneousProfileFile = Path.Combine(testFileDirectory, "profiel005 - Ringtoets.prfl");
            string expectedMessage = $"Fout bij het lezen van bestand '{erroneousProfileFile}' op regel 6: het ingelezen damtype ('4') moet 0, 1, 2 of 3 zijn.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_TwoPrflWithSameId_FalseAndErrorLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string testFileDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                  Path.Combine("DikeProfiles", "TwoPrflWithSameId"));
            string filePath = Path.Combine(testFileDirectory, "profiel001.shp");

            var referencePoints = new List<Point2D>
            {
                new Point2D(130074.3, 543717.4),
                new Point2D(130084.3, 543727.4)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            string erroneousProfileFile = Path.Combine(testFileDirectory, "profiel001_2.prfl");
            string expectedMessage = $"Meerdere definities gevonden voor profiel 'profiel001'. Bestand '{erroneousProfileFile}' wordt overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_FromFileWithDuplicateId_FalseAndLogErrors()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_same_id_3_times.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            const string expectedMessage = "Profiellocatie met ID 'profiel001' is opnieuw ingelezen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_PrflWithProfileNotZero_FalseAndErrorLog()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "PrflWithProfileNotZero", "Voorland_12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

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
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "PrflIsIncomplete", "Voorland_12-2.shp"));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(130074.3, 543717.4),
                new Point2D(130084.3, 543727.4)
            });
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

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
        public void Import_FromFileWithUnrelatedInvalidPrflFilesInSameFolder_TrueAndIgnoresUnrelatedFiles()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "OkTestDataWithUnrelatedPrfl", "Voorland 12-2.shp"));
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

            // Call
            var importResult = true;
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhileReadingProfileLocations_ReturnsFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var testProfilesImporter = new TestProfilesImporter(testImportTarget, testReferenceLine, filePath, messageProvider);
            testProfilesImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van profiellocaties uit een shapebestand."))
                {
                    testProfilesImporter.Cancel();
                }
            });

            // Call
            bool importResult = testProfilesImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhileReadingDikeProfileLocations_ReturnsFalse()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(testImportTarget, referenceLine, filePath, messageProvider);
            testProfilesImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van profielgegevens uit een prfl bestand."))
                {
                    testProfilesImporter.Cancel();
                }
            });

            // Call
            bool importResult = testProfilesImporter.Import();

            // Assert
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_CancelOfImportWhileCreateProfiles_ContinueImportAndLogWarning()
        {
            // Setup
            const string addingDataToModel = "Adding Data to Model";
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            messageProvider.Stub(mp => mp.GetAddDataToModelProgressText()).Return(addingDataToModel);
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(testImportTarget, referenceLine, filePath, messageProvider);
            testProfilesImporter.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(addingDataToModel))
                {
                    testProfilesImporter.Cancel();
                }
            });

            var importResult = false;

            // Call
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            const string expectedMessage = "Huidige actie was niet meer te annuleren en is daarom voortgezet.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_True()
        {
            // Setup
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);
            testProfilesImporter.SetProgressChanged((description, step, steps) => testProfilesImporter.Cancel());

            bool importResult = testProfilesImporter.Import();

            // Precondition
            Assert.IsFalse(importResult);
            testProfilesImporter.SetProgressChanged(null);

            // Call
            importResult = testProfilesImporter.Import();

            // Assert
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_CreateProfilesThrowsUpdateDataException_ReturnsFalseAndLogsError()
        {
            // Setup
            const string typeDescriptor = "A typeDescriptor";

            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText())
                           .Return("");
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText(typeDescriptor))
                           .IgnoreArguments()
                           .Return("error {0}");
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(),
                                                                referenceLine,
                                                                filePath,
                                                                messageProvider,
                                                                typeDescriptor)
            {
                CreateProfileAction = () => { throw new UpdateDataException("Exception message"); }
            };

            var importResult = true;

            // Call
            Action call = () => importResult = testProfilesImporter.Import();

            // Assert
            const string expectedMessage = "error Exception message";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_AddingDataToModel_SetsProgressText()
        {
            // Setup
            const string expectedProgressText = "Adding Data to model";
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedProgressText);
            mocks.ReplayAll();

            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var testProfilesImporter = new TestProfilesImporter(new ObservableList<object>(), referenceLine, filePath, messageProvider);

            var callcount = 0;
            testProfilesImporter.SetProgressChanged((description, step, steps) =>
            {
                if (callcount == 12)
                {
                    Assert.AreEqual(expectedProgressText, description);
                }

                callcount++;
            });

            // Call
            testProfilesImporter.Import();

            // Assert
            // Assert done in TearDown
        }

        private static ReferenceLine CreateMatchingReferenceLine()
        {
            var referenceLine = new ReferenceLine();
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

        private class TestProfilesImporter : ProfilesImporter<ObservableList<object>>
        {
            public Action CreateProfileAction;

            public TestProfilesImporter(ObservableList<object> importTarget, ReferenceLine referenceLine, string filePath,
                                        IImporterMessageProvider messageProvider)
                : base(referenceLine, filePath, importTarget, messageProvider, string.Empty) {}

            public TestProfilesImporter(ObservableList<object> importTarget, ReferenceLine referenceLine, string filePath,
                                        IImporterMessageProvider messageProvider, string typeDescriptor)
                : base(referenceLine, filePath, importTarget, messageProvider, typeDescriptor) {}

            protected override void CreateProfiles(ReadResult<ProfileLocation> importProfileLocationResult,
                                                   ReadResult<DikeProfileData> importDikeProfileDataResult)
            {
                CreateProfileAction?.Invoke();
            }

            protected override bool DikeProfileDataIsValid(DikeProfileData data, string prflFilePath)
            {
                return true;
            }

            protected override void LogImportCanceledMessage() {}
        }
    }
}