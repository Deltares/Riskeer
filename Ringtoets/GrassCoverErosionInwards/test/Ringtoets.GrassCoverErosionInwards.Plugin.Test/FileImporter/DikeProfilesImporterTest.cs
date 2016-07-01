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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin.FileImporter;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using GrassCoverErosionInwardsPluginResources = Ringtoets.GrassCoverErosionInwards.Plugin.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.FileImporter
{
    [TestFixture]
    public class DikeProfilesImporterTest
    {
        private MockRepository mockRepository;
        private int progress;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            progress = 0;
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var importer = new DikeProfilesImporter();

            // Assert
            Assert.IsInstanceOf<FileImporterBase<DikeProfilesContext>>(importer);
            Assert.AreEqual("Dijkprofiel locaties", importer.Name);
            Assert.AreEqual("Algemeen", importer.Category);
            TestHelper.AssertImagesAreEqual(GrassCoverErosionInwardsPluginResources.DikeProfile, importer.Image);
            Assert.AreEqual("Dijkprofiel locaties shape bestand (*.shp)|*.shp", importer.FileFilter);
        }

        [Test]
        public void CanImportOn_ValidContext_ReturnTrue()
        {
            // Setup
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var referencePoints = new List<Point2D>
            {
                new Point2D(131223.2, 548393.4),
                new Point2D(133854.3, 545323.1),
                new Point2D(135561.0, 541920.3),
                new Point2D(136432.1, 538235.2),
                new Point2D(136039.4, 533920.2)
            };
            assessmentSection.ReferenceLine = new ReferenceLine();
            assessmentSection.ReferenceLine.SetGeometry(referencePoints);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            mocks.ReplayAll();

            // Call
            var canImport = dikeProfilesImporter.CanImportOn(targetContext);

            // Assert
            Assert.IsTrue(canImport);
            mocks.VerifyAll();
        }

        [Test]
        public void CanImportOn_ContextWithoutReferenceLine_ReturnFalse()
        {
            // Setup
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            mocks.ReplayAll();

            // Call
            var canImport = dikeProfilesImporter.CanImportOn(targetContext);

            // Assert
            Assert.IsFalse(canImport);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Import_FromInvalidEmptyPath_FalseAndLogError(string filePath)
        {
            // Setup
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilsResources.Error_Path_must_be_specified);
                StringAssert.StartsWith(messageArray[0], message);
            });
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_FromPathContainingInvalidFileCharacters_FalseAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            var invalidPath = filePath.Replace('d', invalidFileNameChars[0]);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, invalidPath);

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
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            var folderPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils) + Path.DirectorySeparatorChar;

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, folderPath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = new FileReaderErrorMessageBuilder(folderPath)
                    .Build(CoreCommonUtilsResources.Error_Path_must_not_point_to_folder);
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
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                         shapeFileName);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

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
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", shapeFileName));

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = string.Format("Het bestand heeft geen attribuut '{0}' welke vereist is om de locaties van de dijkprofielen in te lezen.",
                                               missingColumnName);
                Assert.AreEqual(message, messageArray[0]);
            });
            Assert.IsFalse(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("Voorlanden_12-2_IdWithSymbol.shp")]
        [TestCase("Voorlanden_12-2_IdWithWhitespace.shp")]
        public void Import_FromFileWithIllegalCharactersInId_FalseAndLogError(string fileName)
        {
            // Setup
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", fileName));

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = "Fout bij het lezen van dijkprofiel op regel 1. De locatie parameter 'Id' mag uitsluitend uit letters en cijfers bestaan. Dit dijkprofiel wordt overgeslagen.";
                Assert.AreEqual(message, messageArray[0]);
            });
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_FromFileWithEmptyEntryForId_FalseAndLogError()
        {
            // Setup
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyId.shp"));

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message1 = "Fout bij het lezen van dijkprofiel op regel 1. De locatie parameter 'Id' heeft geen waarde. Dit dijkprofiel wordt overgeslagen.";
                string message2 = "Fout bij het lezen van dijkprofiel op regel 2. De locatie parameter 'Id' heeft geen waarde. Dit dijkprofiel wordt overgeslagen.";
                string message3 = "Fout bij het lezen van dijkprofiel op regel 4. De locatie parameter 'Id' heeft geen waarde. Dit dijkprofiel wordt overgeslagen.";
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
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "IpflWithUnmatchableId", "Voorlanden_12-2_UnmatchableId.shp"));

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                string message = "Kan geen voorland en dijkprofiel data vinden voor dijkprofiel locatie met Id: unmatchable";
                Assert.AreEqual(message, messageArray[0]);
            });
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Import_FromFileWithEmptyEntryForX0_FalseAndLogError()
        {
            // Setup
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyX0.shp"));

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

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
        public void Import_ContextWithoutReferenceLine_FalseAndErrorMessage()
        {
            // Setup
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var targetContextWithoutReferenceLine = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            mocks.ReplayAll();

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContextWithoutReferenceLine, filePath);

            // Assert
            var expectedMessage = "Er is geen referentielijn beschikbaar. Geen data ingelezen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(targetContextWithoutReferenceLine.WrappedData);
            Assert.AreEqual(0, progress);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_DikeProfileLocationsNotCloseEnoughToReferenceLine_TrueAndLogError()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var referencePoints = new List<Point2D>
            {
                new Point2D(141223.2, 548393.4),
                new Point2D(143854.3, 545323.1),
                new Point2D(145561.0, 541920.3),
                new Point2D(146432.1, 538235.2),
                new Point2D(146039.4, 533920.2)
            };
            assessmentSection.ReferenceLine = new ReferenceLine();
            assessmentSection.ReferenceLine.SetGeometry(referencePoints);
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var dikeProfilesImporter = new DikeProfilesImporter
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            var expectedMessages = Enumerable.Repeat("Een dijkprofiel locatie met Id 'profiel001' ligt niet op de referentielijn. Locatie wordt overgeslagen.", 5).ToArray();
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
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var referencePoints = new List<Point2D>
            {
                new Point2D(131223.2, 548393.4),
                new Point2D(133854.3, 545323.1),
                new Point2D(135561.0, 541920.3),
                new Point2D(136432.1, 538235.2),
                new Point2D(146039.4, 533920.2)
            };
            assessmentSection.ReferenceLine = new ReferenceLine();
            assessmentSection.ReferenceLine.SetGeometry(referencePoints);
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            var dikeProfilesImporter = new DikeProfilesImporter
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            //Precondition
            var importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            string expectedMessage = "Een dijkprofiel locatie met Id 'profiel005' ligt niet op de referentielijn. Locatie wordt overgeslagen.";
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Call
            bool importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            Assert.IsTrue(importResult);
            List<ProgressNotification> expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van dijkprofiel locaties uit een shape bestand.", 1, 1),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 1, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 2, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 3, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 4, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 5, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 1, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 2, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 3, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 4, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 5, 5)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications, targetContext);
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Call
            bool importResult = dikeProfilesImporter.Import(targetContext, filePath);
            DikeProfile dikeProfile = targetContext.WrappedData[4];

            // Assert
            Assert.AreEqual(new Point2D(136039.49100000039, 533920.28050000477), dikeProfile.WorldReferencePoint);
            Assert.AreEqual("profiel005", dikeProfile.Name);
            StringAssert.StartsWith("Verkenning prfl format:", dikeProfile.Memo);
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Call
            bool importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            Assert.IsTrue(importResult);
            List<ProgressNotification> expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van dijkprofiel locaties uit een shape bestand.", 1, 1),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 1, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 2, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 3, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 4, 5),
                new ProgressNotification("Inlezen van dijkprofiel locatie.", 5, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data uit een prfl bestand.", 1, 1),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 1, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 2, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 3, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 4, 5),
                new ProgressNotification("Inlezen van voorland en dijkprofiel data.", 5, 5)
            };
            ValidateProgressMessages(expectedProgressMessages, progressChangeNotifications, targetContext);
            Assert.AreEqual(5, targetContext.WrappedData.Count);
            mockRepository.VerifyAll(); // 'observer' should not be notified
        }

        [Test]
        public void Import_InvalidDamType_FalseAndLogMessage()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "InvalidDamType", "Voorlanden 12-2.shp"));

            var observer = mockRepository.StrictMock<IObserver>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            var progressChangeNotifications = new List<ProgressNotification>();
            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);
            targetContext.Attach(observer);

            // Precondition
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            Action<IEnumerable<string>> asserts = (messages) =>
            {
                bool found = messages.Any(message => message.EndsWith(": De ingelezen dam-type waarde (4) moet binnen het bereik [0, 3] vallen."));
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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            // Precondition
            dikeProfilesImporter.Cancel();
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

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

            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            dikeProfilesImporter.Cancel();
            bool importResult = dikeProfilesImporter.Import(targetContext, filePath);
            Assert.IsFalse(importResult);

            // Call
            importResult = dikeProfilesImporter.Import(targetContext, filePath);

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

            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var referencePoints = new List<Point2D>
            {
                new Point2D(130074.3, 543717.4),
                new Point2D(130084.3, 543727.4)
            };
            assessmentSection.ReferenceLine = new ReferenceLine();
            assessmentSection.ReferenceLine.SetGeometry(referencePoints);
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            // Precondition
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            Action<IEnumerable<string>> asserts = (messages) =>
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
        public void Import_PrflWithProfileNotZero_TrueAndErrorLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "PrflWithProfileNotZero", "Voorland_12-2.shp"));

            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = CreateMatchingReferenceLine();
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            // Precondition
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            Action<IEnumerable<string>> asserts = (messages) =>
            {
                bool found = messages.Any(message => message.StartsWith("Voorland en dijkprofiel data specificeert een damwand waarde ongelijk aan 0. Bestand wordt overgeslagen:"));
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified 
        }

        [Test]
        public void Import_PrflIsIncomplete_FalseAndErrorLog()
        {
            // Setup
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                         Path.Combine("DikeProfiles", "PrflIsIncomplete", "Voorland_12-2.shp"));

            DikeProfilesImporter dikeProfilesImporter = new DikeProfilesImporter();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var referencePoints = new List<Point2D>
            {
                new Point2D(130074.3, 543717.4),
                new Point2D(130084.3, 543727.4)
            };
            assessmentSection.ReferenceLine = new ReferenceLine();
            assessmentSection.ReferenceLine.SetGeometry(referencePoints);
            mockRepository.ReplayAll();

            var targetContext = new DikeProfilesContext(failureMechanism.DikeProfiles, assessmentSection);

            // Precondition
            bool importResult = true;

            // Call
            Action call = () => importResult = dikeProfilesImporter.Import(targetContext, filePath);

            // Assert
            Action<IEnumerable<string>> asserts = (messages) =>
            {
                bool found = messages.First().Contains(": De volgende parameter(s) zijn niet aanwezig in het bestand: VOORLAND, DAMWAND, KRUINHOOGTE, DIJK, MEMO");
                Assert.IsTrue(found);
            };
            TestHelper.AssertLogMessages(call, asserts);
            Assert.IsTrue(importResult);
            mockRepository.VerifyAll(); // 'observer' should not be notified 
        }

        private ReferenceLine CreateMatchingReferenceLine()
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[] {
                new Point2D(131223.2, 548393.4),
                new Point2D(133854.3, 545323.1),
                new Point2D(135561.0, 541920.3),
                new Point2D(136432.1, 538235.2),
                new Point2D(136039.4, 533920.2)
            });
            return referenceLine;
        }

        private static void ValidateProgressMessages(List<ProgressNotification> expectedProgressMessages, List<ProgressNotification> progressChangeNotifications, DikeProfilesContext targetContext)
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