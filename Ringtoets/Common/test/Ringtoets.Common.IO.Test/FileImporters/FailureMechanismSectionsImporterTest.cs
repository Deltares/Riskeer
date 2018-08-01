// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.ReferenceLines;
using Ringtoets.Common.IO.TestUtil;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class FailureMechanismSectionsImporterTest
    {
        private const string sectionsTypeDescriptor = "Vakindeling";
        private const string expectedUpdateDataFailedText = "Add data to model failed {0}. Nothing imported";
        private const string expectedCancelledText = "Import canceled";
        private const string expectedAddDataToModelProgressText = "Add data to model";

        [Test]
        public void Constructor_ReferenceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsImporter(failureMechanism, null, "", new TestFailureMechanismSectionUpdateStrategy(), messageProvider);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("referenceLine", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismSectionUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsImporter(failureMechanism, new ReferenceLine(), "", null, messageProvider);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanismSectionUpdateStrategy", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsImporter(failureMechanism, new ReferenceLine(), "", new TestFailureMechanismSectionUpdateStrategy(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("messageProvider", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            mocks.ReplayAll();

            var referenceLine = new ReferenceLine();

            // Call
            var importer = new FailureMechanismSectionsImporter(failureMechanism, referenceLine, "", new TestFailureMechanismSectionUpdateStrategy(), messageProvider);

            // Assert
            Assert.IsInstanceOf<FileImporterBase<IFailureMechanism>>(importer);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("traject_1-1.shp", "traject_1-1_vakken.shp", 62)]
        [TestCase("traject_19-1.shp", "traject_19-1_vakken.shp", 17)]
        public void Import_ValidFileCorrespondingToReferenceLineAndNoSectionImportedYet_ImportSections(string referenceLineFileName, string sectionsFileName, int sectionCount)
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", referenceLineFileName));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", sectionsFileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);
            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            Assert.AreEqual(sectionsFilePath, updateStrategy.SourcePath);
            IEnumerable<FailureMechanismSection> sections = updateStrategy.ImportedFailureMechanismSections;
            Assert.AreEqual(sectionCount, sections.Count());
            AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidFileCorrespondingToReferenceLineAndHasSectionImported_ReplaceSections()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var oldSection = new FailureMechanismSection("A", importReferenceLine.Points);
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                oldSection
            });
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            IEnumerable<FailureMechanismSection> sections = updateStrategy.ImportedFailureMechanismSections;
            CollectionAssert.DoesNotContain(sections, oldSection);
            Assert.AreEqual(62, sections.Count());
            AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidArtificialFileImperfectlyCorrespondingToReferenceLineAndNoSectionImportedYet_ImportSections()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_ValidVakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            IEnumerable<FailureMechanismSection> sections = updateStrategy.ImportedFailureMechanismSections;
            Assert.AreEqual(7, sections.Count());
            AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("StartSectionReversedCoordinates")]
        [TestCase("EndSectionReversedCoordinates")]
        [TestCase("InBetweenSectionReversedCoordinates")]
        public void Import_ValidArtificialFileWithReversedSectionCoordinatesImperfectlyCorrespondingToReferenceLineAndNoSectionImportedYet_ImportSections(
            string affectedSection)
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            string fileName = $"Artificial_referencelijn_testA_ValidVakken_{affectedSection}.shp";
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", fileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            IEnumerable<FailureMechanismSection> sections = updateStrategy.ImportedFailureMechanismSections;
            Assert.AreEqual(7, sections.Count());
            AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ValidImport_GenerateExpectedProgressMessages()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_ValidVakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var progressChangeNotifications = new List<ProgressNotification>();

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);
            var expectedProgressMessages = new[]
            {
                new ProgressNotification("Inlezen vakindeling.", 1, 3),
                new ProgressNotification("Valideren ingelezen vakindeling.", 2, 3),
                new ProgressNotification(expectedAddDataToModelProgressText, 3, 3)
            };

            ProgressNotificationTestHelper.AssertProgressNotificationsAreEqual(expectedProgressMessages,
                                                                               progressChangeNotifications);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FilePathIsDirectory_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, Path.DirectorySeparatorChar.ToString());

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{sectionsFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_FileDoesNotExist_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_dont_exist.shp");

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{sectionsFilePath}': het bestand bestaat niet.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_EmptyArtificialFile_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor)).Return(expectedUpdateDataFailedText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_EmptyVakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(expectedUpdateDataFailedText, "Het bestand heeft geen vakindeling");
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("StartTooFarFromReferenceline")]
        [TestCase("EndTooFarFromReferenceline")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromReferenceLine_CancelImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor)).Return(expectedUpdateDataFailedText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string shapeFileName = $"Artificial_referencelijn_testA_InvalidVakken_Section{shapeCondition}.shp";
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", shapeFileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = String.Format(expectedUpdateDataFailedText, "De geografische ligging van ieder vak moet overeenkomen met de ligging van (een deel van) de referentielijn");
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("StartTooFarFromReferencelineStart")]
        [TestCase("EndTooFarFromReferencelineEnd")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromStartEndOfReferenceLine_CancelImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor)).Return(expectedUpdateDataFailedText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string shapeFileName = $"Artificial_referencelijn_testA_InvalidVakken_{shapeCondition}.shp";
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", shapeFileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(expectedUpdateDataFailedText,
                                                   "De geografische ligging van ieder vak moet overeenkomen met de ligging van (een deel van) de referentielijn");
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSectionsDoNotFullyCoverReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor)).Return(expectedUpdateDataFailedText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_NotCoveringWholeReferenceLine.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(expectedUpdateDataFailedText, "De opgetelde lengte van de vakken moet overeenkomen met de trajectlengte");
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseUnchainedSections_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(string.Empty);
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor)).Return(expectedUpdateDataFailedText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_UnchainedSections.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(expectedUpdateDataFailedText, "Het bestand moet vakken bevatten die allen op elkaar aansluiten");
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSomePointsNotOnReferenceLine_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor)).Return(expectedUpdateDataFailedText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_PointsTooFarFromReferenceLine.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(expectedUpdateDataFailedText, "De opgetelde lengte van de vakken moet overeenkomen met de trajectlengte");
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_MissingNameValue_CancelImportWithErrorMessage()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            mocks.ReplayAll();

            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "vakindeling_Empty_Name_Value.shp"));

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, new ReferenceLine(), sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            Action call = () => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{sectionsFilePath}': voor één of meerdere vakken is geen naam opgegeven.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelOfImportWhenReadingFailureMechanismSections_CancelsImportAndLogs()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText(sectionsTypeDescriptor)).Return(expectedCancelledText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen vakindeling."))
                {
                    importer.Cancel();
                }
            });

            var importSuccessful = true;

            // Call
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, expectedCancelledText, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelOfImportWhenValidatingImportedections_CancelsImportAndLogs()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText(sectionsTypeDescriptor)).Return(expectedCancelledText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Valideren ingelezen vakindeling."))
                {
                    importer.Cancel();
                }
            });

            var importSuccessful = true;

            // Call
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, expectedCancelledText, 1);
            Assert.IsFalse(importSuccessful);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_CancelOfImportWhenAddingDataToModel_ContinuesImportAndLogs()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(expectedAddDataToModelProgressText);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains(expectedAddDataToModelProgressText))
                {
                    importer.Cancel();
                }
            });

            var importSuccessful = true;

            // Call
            Action call = () => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Huidige actie was niet meer te annuleren en is daarom voortgezet.", 1);
            Assert.IsTrue(importSuccessful);
            CollectionAssert.IsNotEmpty(updateStrategy.ImportedFailureMechanismSections);
            mocks.VerifyAll();
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_TrueAndLogMessagesAndExpectedImportedData()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.StrictMock<IImporterMessageProvider>();
            messageProvider.Expect(mp => mp.GetCancelledLogMessageText(sectionsTypeDescriptor)).Return(string.Empty);
            messageProvider.Expect(mp => mp.GetAddDataToModelProgressText()).Return(string.Empty);
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            Assert.IsFalse(importer.Import());
            importer.SetProgressChanged(null);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);

            IEnumerable<FailureMechanismSection> sections = updateStrategy.ImportedFailureMechanismSections;
            Assert.AreEqual(62, sections.Count());
            AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void DoPostImport_AfterImport_ObserversNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            var observable = mocks.StrictMock<IObserver>();
            observable.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var updateStrategy = new TestFailureMechanismSectionUpdateStrategy();

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            importer.Import();
            failureMechanism.SectionResults.Attach(observable);

            // Call
            importer.DoPostImport();

            // Assert
            mocks.VerifyAll();
        }

        private class TestFailureMechanismSectionUpdateStrategy : IFailureMechanismSectionUpdateStrategy
        {
            public string SourcePath { get; private set; }

            public IEnumerable<FailureMechanismSection> ImportedFailureMechanismSections { get; private set; }

            public void UpdateSectionsWithImportedData(IEnumerable<FailureMechanismSection> importedFailureMechanismSections, string sourcePath)
            {
                SourcePath = sourcePath;
                ImportedFailureMechanismSections = importedFailureMechanismSections;
            }
        }

        private static ReferenceLine ImportReferenceLine(string referenceLineFilePath)
        {
            ReferenceLine importedReferenceLine = null;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var handler = mocks.Stub<IReferenceLineReplaceHandler>();
            handler.Stub(h => h.ConfirmReplace()).Return(true);
            handler.Stub(h => h.Replace(Arg<IAssessmentSection>.Is.Same(assessmentSection),
                                        Arg<ReferenceLine>.Is.NotNull))
                   .WhenCalled(invocation => { importedReferenceLine = (ReferenceLine) invocation.Arguments[1]; })
                   .Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            var referenceLineImporter = new ReferenceLineImporter(assessmentSection, handler, referenceLineFilePath);
            referenceLineImporter.Import();

            mocks.VerifyAll();

            return importedReferenceLine;
        }

        private void AssertSectionsAreValidForReferenceLine(IEnumerable<FailureMechanismSection> sections, ReferenceLine referenceLine)
        {
            Point2D[] referenceLineGeometry = referenceLine.Points.ToArray();

            // 1. Start & End coherence:
            Assert.AreEqual(referenceLineGeometry[0], sections.First().StartPoint,
                            "Start of the sections should correspond to the Start of the reference line.");
            Assert.AreEqual(referenceLineGeometry.Last(), sections.Last().EndPoint,
                            "End of the sections should correspond to the End of the reference line.");

            // 2. Total length coherence:
            double totalLengthOfSections = sections.Sum(s => GetLineSegments(s.Points).Sum(segment => segment.Length));
            double totalLengthOfReferenceLine = referenceLine.Length;
            Assert.AreEqual(totalLengthOfReferenceLine, totalLengthOfSections, 1e-6,
                            "The length of all sections should sum up to the length of the reference line.");

            // 3. Section Start and End coherence
            IEnumerable<Point2D> allStartAndEndPoints = sections.Select(s => s.StartPoint).Concat(sections.Select(s => s.EndPoint));
            foreach (Point2D point in allStartAndEndPoints)
            {
                Assert.Less(GetDistanceToReferenceLine(point, referenceLine), 1e-6,
                            "All start- and end points should be on the reference line.");
            }

            // 4. Section Start and End points coherence
            FailureMechanismSection sectionTowardsEnd = null;
            foreach (FailureMechanismSection section in sections)
            {
                FailureMechanismSection sectionTowardsStart = sectionTowardsEnd;
                sectionTowardsEnd = section;

                if (sectionTowardsStart != null)
                {
                    Assert.AreEqual(sectionTowardsStart.EndPoint, sectionTowardsEnd.StartPoint,
                                    "All sections should be connected and in order of connectedness.");
                }
            }
        }

        private double GetDistanceToReferenceLine(Point2D point, ReferenceLine referenceLine)
        {
            return GetLineSegments(referenceLine.Points)
                   .Select(segment => segment.GetEuclideanDistanceToPoint(point))
                   .Min();
        }

        private static IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            Point2D endPoint = null;
            foreach (Point2D linePoint in linePoints)
            {
                Point2D startPoint = endPoint;
                endPoint = linePoint;

                if (startPoint != null)
                {
                    yield return new Segment2D(startPoint, endPoint);
                }
            }
        }
    }
}