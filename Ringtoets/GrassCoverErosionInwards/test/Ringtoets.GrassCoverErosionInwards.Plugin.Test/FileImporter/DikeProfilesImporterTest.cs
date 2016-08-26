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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin.FileImporter;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using GrassCoverErosionInwardsPluginResources = Ringtoets.GrassCoverErosionInwards.Plugin.Properties.Resources;
using RingtoetsCommonIoResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.FileImporter
{
    [TestFixture]
    public class DikeProfilesImporterTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var importTarget = new ObservableList<DikeProfile>();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new DikeProfilesImporter(importTarget, referenceLine);

            // Assert
            Assert.IsInstanceOf<FileImporterBase>(importer);
        }
        
        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Import_FromInvalidEmptyPath_FalseAndLogError(string filePath)
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilsResources.Error_Path_must_be_specified);
                StringAssert.StartsWith(messageArray[0], message);
            });
            Assert.IsFalse(importResult);
        }

        [Test]
        public void Import_FromPathContainingInvalidFileCharacters_FalseAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            var invalidPath = filePath.Replace('d', invalidFileNameChars[0]);

            var referenceLine = new ReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(invalidPath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = new FileReaderErrorMessageBuilder(invalidPath)
                    .Build(string.Format(CoreCommonUtilsResources.Error_Path_cannot_contain_Characters_0_, string.Join(", ", Path.GetInvalidFileNameChars())));
                StringAssert.StartsWith(messageArray[0], message);
            });
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_FromDirectoryPath_FalseAndLogError()
        {
            // Setup
            var folderPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils) + Path.DirectorySeparatorChar;

            var referenceLine = new ReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(folderPath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = new FileReaderErrorMessageBuilder(folderPath)
                    .Build(CoreCommonUtilsResources.Error_Path_must_not_point_to_empty_file_name);
                StringAssert.StartsWith(messageArray[0], message);
            });
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll();
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

            var referenceLine = new ReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(string.Format("Fout bij het lezen van bestand '{0}': Het bestand mag uitsluitend punten bevatten.",
                                         filePath));
                StringAssert.EndsWith(messageArray[0], message);
            });
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("Voorlanden_12-2_WithoutId.shp", "ID")]
        [TestCase("Voorlanden_12-2_WithoutName.shp", "Naam")]
        [TestCase("Voorlanden_12-2_WithoutX0.shp", "X0")]
        public void Import_FromFileMissingAttributeColumn_FalseAndLogError(
            string shapeFileName, string missingColumnName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", shapeFileName));

            var referenceLine = new ReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = string.Format("Het bestand heeft geen attribuut '{0}'. Dit attribuut is vereist.",
                                               missingColumnName);
                Assert.AreEqual(message, messageArray[0]);
            });
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("Voorlanden_12-2_IdWithSymbol.shp")]
        [TestCase("Voorlanden_12-2_IdWithWhitespace.shp")]
        public void Import_FromFileWithIllegalCharactersInId_TrueAndLogError(string fileName)
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", fileName));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = "Fout bij het lezen van dijkprofiel op regel 1. De locatie parameter 'ID' mag uitsluitend uit letters en cijfers bestaan. Dit dijkprofiel wordt overgeslagen.";
                Assert.AreEqual(message, messageArray[0]);
            });
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_FromFileWithEmptyEntryForId_TrueAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message1 = "Fout bij het lezen van dijkprofiel op regel 1. De locatie parameter 'ID' heeft geen waarde. Dit dijkprofiel wordt overgeslagen.";
                string message2 = "Fout bij het lezen van dijkprofiel op regel 2. De locatie parameter 'ID' heeft geen waarde. Dit dijkprofiel wordt overgeslagen.";
                string message3 = "Fout bij het lezen van dijkprofiel op regel 4. De locatie parameter 'ID' heeft geen waarde. Dit dijkprofiel wordt overgeslagen.";
                Assert.AreEqual(message1, messageArray[0]);
                Assert.AreEqual(message2, messageArray[1]);
                Assert.AreEqual(message3, messageArray[2]);
            });
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_FromFileWithUnmatchableId_TrueAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "IpflWithUnmatchableId", "Voorlanden_12-2_UnmatchableId.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = "Kan geen geldige voorland- en dijkprofieldata vinden voor dijkprofiel locatie met ID: unmatchable";
                Assert.AreEqual(message, messageArray[0]);
            });
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_FromFileWithEmptyEntryForX0_TrueAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyX0.shp"));

            var referenceLine = new ReferenceLine();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = "Fout bij het lezen van dijkprofiel op regel 1. Het dijkprofiel heeft geen geldige waarde voor attribuut 'X0'. Dit dijkprofiel wordt overgeslagen.";
                Assert.AreEqual(message, messageArray[0]);
            });
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_DikeProfileLocationsNotCloseEnoughToReferenceLine_TrueAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
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

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            var expectedMessages = Enumerable.Repeat("Een dijkprofiel locatie met ID 'profiel001' ligt niet op de referentielijn. Locatie wordt overgeslagen.", 5).ToArray();
            TestHelper.AssertLogMessagesAreGenerated(call, expectedMessages, expectedMessages.Length);
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_OneDikeProfileLocationNotCloseEnoughToReferenceLine_TrueAndLogErrorAndFourDikeProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var referencePoints = new List<Point2D>
            {
                new Point2D(131223.2, 548393.4),
                new Point2D(133854.3, 545323.1),
                new Point2D(135561.0, 541920.3),
                new Point2D(136432.1, 538235.2),
                new Point2D(146039.4, 533920.2)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            string expectedMessage = "Een dijkprofiel locatie met ID 'profiel005' ligt niet op de referentielijn. Locatie wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsTrue(importResult);
            Assert.AreEqual(4, targetContext.WrappedData.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_AllOkTestData_TrueAndLogMessagesAndFiveDikeProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine)
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Call
            bool importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            Assert.IsTrue(importResult);
            List<ProgressNotification> expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van dijkprofiel locaties uit een shapebestand.", 1, 1),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 1, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 2, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 3, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 4, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 5, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 1, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 2, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 3, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 4, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 5, 5)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
            Assert.AreEqual(5, targetContext.WrappedData.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_AllOkTestData_CorrectDikeProfileProperties()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Call
            dikeProfilesImporter.Import(filePath);
            DikeProfile dikeProfile = targetContext.WrappedData[4];

            // Assert
            Assert.AreEqual(new Point2D(136039.49100000039, 533920.28050000477), dikeProfile.WorldReferencePoint);
            Assert.AreEqual("profiel005", dikeProfile.Name);
            Assert.AreEqual(15.56165507, dikeProfile.X0);
            Assert.AreEqual(new RoundedDouble(2, 330.0), dikeProfile.Orientation);
            Assert.IsTrue(dikeProfile.HasBreakWater);
            Assert.AreEqual(new RoundedDouble(2, 6.0), dikeProfile.DikeHeight);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_AllDamTypes_TrueAndLogMessagesAndFiveDikeProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllDamTypes", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine)
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Call
            bool importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            Assert.IsTrue(importResult);
            List<ProgressNotification> expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van dijkprofiel locaties uit een shapebestand.", 1, 1),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 1, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 2, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 3, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 4, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 5, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 1, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 2, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 3, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 4, 5),
                new ProgressNotification("Inlezen van voorland- en dijkprofieldata.", 5, 5)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications);
            Assert.AreEqual(5, targetContext.WrappedData.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_InvalidDamType_TrueAndLogMessage()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "InvalidDamType", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Precondition
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            Action<IEnumerable<string>> asserts = messages =>
            {
                bool found = messages.Any(message => message.EndsWith(": Het ingelezen damtype ('4') moet 0, 1, 2 of 3 zijn."));
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            // Precondition
            dikeProfilesImporter.Cancel();
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Dijkprofielen importeren is afgebroken. Geen data ingelezen.", 1);
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_TrueAndLogMessagesAndFiveDikeProfiles()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            dikeProfilesImporter.Cancel();
            bool importResult = dikeProfilesImporter.Import(filePath);
            Assert.IsFalse(importResult);

            // Call
            importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(5, targetContext.WrappedData.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_TwoPrflWithSameId_TrueAndErrorLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "TwoPrflWithSameId", "profiel001.shp"));

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var referencePoints = new List<Point2D>
            {
                new Point2D(130074.3, 543717.4),
                new Point2D(130084.3, 543727.4)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            // Precondition
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            Action<IEnumerable<string>> asserts = messages =>
            {
                var start = "Meerdere dijkprofieldata definities gevonden voor dijkprofiel 'profiel001'. Bestand '";
                var end = "' wordt overgeslagen.";
                bool found = messages.Any(m => m.StartsWith(start) && m.EndsWith(end));
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified 
        }

        [Test]
        public void Import_FromFileWithDupplicateId_TrueAndLogWarnings()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_same_id_3_times.shp"));

            var referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = "Dijkprofiel locatie met ID 'profiel001' is opnieuw ingelezen.";
                Assert.AreEqual(message, messageArray[0]);
                Assert.AreEqual(message, messageArray[1]);
            });
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_PrflWithProfileNotZero_TrueAndErrorLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "PrflWithProfileNotZero", "Voorland_12-2.shp"));

            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            // Precondition
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            Action<IEnumerable<string>> asserts = messages =>
            {
                bool found = messages.Any(message => message.StartsWith("Voorland- en dijkprofieldata specificeren een damwand waarde ongelijk aan 0. Bestand wordt overgeslagen:"));
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified 
        }

        [Test]
        public void Import_PrflIsIncomplete_TrueAndErrorLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "PrflIsIncomplete", "Voorland_12-2.shp"));

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var referencePoints = new List<Point2D>
            {
                new Point2D(130074.3, 543717.4),
                new Point2D(130084.3, 543727.4)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(referencePoints);

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mockRepository.ReplayAll();

            var dikeProfilesImporter = new DikeProfilesImporter(failureMechanism.DikeProfiles, referenceLine);

            // Precondition
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(filePath);

            // Assert
            Action<IEnumerable<string>> asserts = messages =>
            {
                bool found = messages.First().Contains(": De volgende parameters zijn niet aanwezig in het bestand: VOORLAND, DAMWAND, KRUINHOOGTE, DIJK, MEMO");
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified 
        }

        private ReferenceLine CreateMatchingReferenceLine()
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

        private static void ValidateProgressMessages(List<ProgressNotification> expectedProgressMessages, List<ProgressNotification> progressChangeNotifications)
        {
            Assert.AreEqual(expectedProgressMessages.Count, progressChangeNotifications.Count);
            for (var i = 0; i < expectedProgressMessages.Count; i++)
            {
                var notification = expectedProgressMessages[i];
                var actualNotification = progressChangeNotifications[i];
                Assert.AreEqual(notification.Text, actualNotification.Text);
                Assert.AreEqual(notification.CurrentStep, actualNotification.CurrentStep);
                Assert.AreEqual(notification.TotalSteps, actualNotification.TotalSteps);
            }
        }

        private class ProgressNotification
        {
            public ProgressNotification(string description, int currentStep, int totalSteps)
            {
                Text = description;
                CurrentStep = currentStep;
                TotalSteps = totalSteps;
            }

            public string Text { get; private set; }
            public int CurrentStep { get; private set; }
            public int TotalSteps { get; private set; }
        }
    }
}