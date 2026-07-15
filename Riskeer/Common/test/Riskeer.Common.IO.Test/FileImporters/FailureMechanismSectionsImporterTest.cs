// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.ReferenceLines;
using Riskeer.Common.IO.TestUtil;

namespace Riskeer.Common.IO.Test.FileImporters
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
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();

            // Call
            void Call() => new FailureMechanismSectionsImporter(failureMechanism, null, "", updateStrategy, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();

            // Call
            void Call() => new FailureMechanismSectionsImporter(failureMechanism, new ReferenceLine(), "", null, messageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionUpdateStrategy", exception.ParamName);
        }

        [Test]
        public void Constructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();

            // Call
            void Call() => new FailureMechanismSectionsImporter(failureMechanism, new ReferenceLine(), "", updateStrategy, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var referenceLine = new ReferenceLine();

            // Call
            var importer = new FailureMechanismSectionsImporter(failureMechanism, referenceLine, "", updateStrategy, messageProvider);

            // Assert
            Assert.IsInstanceOf<FileImporterBase<IFailureMechanism>>(importer);
        }

        [Test]
        [TestCase("traject_1-1.shp", "traject_1-1_vakken.shp", 62)]
        [TestCase("traject_19-1.shp", "traject_19-1_vakken.shp", 17)]
        public void Import_ValidFileCorrespondingToReferenceLine_CallsUpdateStrategy(string referenceLineFileName, string sectionsFileName, int sectionCount)
        {
            // Setup
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", sectionsFileName));
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", referenceLineFileName));
            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            updateStrategy.UpdateSectionsWithImportedData(Arg.Any<IEnumerable<FailureMechanismSection>>(), Arg.Any<string>())
                          .Returns(callInfo =>
                          {
                              var sections = (IEnumerable<FailureMechanismSection>) callInfo.Args()[0];
                              Assert.AreEqual(sectionCount, sections.Count());
                              AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
                              Assert.AreEqual(sectionsFilePath, callInfo.Args()[1]);
                              return Enumerable.Empty<IObservable>();
                          });

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);
        }

        [Test]
        [TestCase("StartSectionReversedCoordinates")]
        [TestCase("EndSectionReversedCoordinates")]
        [TestCase("InBetweenSectionReversedCoordinates")]
        public void Import_ValidArtificialFileWithReversedSectionCoordinatesImperfectlyCorrespondingToReferenceLine_CallsUpdateStrategy(
            string affectedSection)
        {
            // Setup
            var fileName = $"Artificial_referencelijn_testA_ValidVakken_{affectedSection}.shp";
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", fileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            updateStrategy.UpdateSectionsWithImportedData(Arg.Any<IEnumerable<FailureMechanismSection>>(), Arg.Any<string>())
                          .Returns(callInfo =>
                          {
                              var sections = (IEnumerable<FailureMechanismSection>) callInfo.Args()[0];
                              Assert.AreEqual(7, sections.Count());
                              AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
                              Assert.AreEqual(sectionsFilePath, callInfo.Args()[1]);
                              return Enumerable.Empty<IObservable>();
                          });
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            bool importSuccessful = importer.Import();

            // Assert
            Assert.IsTrue(importSuccessful);
        }

        [Test]
        public void Import_ValidImport_GenerateExpectedProgressMessages()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            updateStrategy.UpdateSectionsWithImportedData(Arg.Any<IEnumerable<FailureMechanismSection>>(), Arg.Any<string>()).Returns(Enumerable.Empty<IObservable>());
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText().Returns(expectedAddDataToModelProgressText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_ValidVakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var progressChangeNotifications = new List<ProgressNotification>();

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
        }

        [Test]
        public void Import_FilePathIsDirectory_AbortImportWithErrorMessage()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor).Returns(expectedUpdateDataFailedText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, Path.DirectorySeparatorChar.ToString());

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(
                expectedUpdateDataFailedText, $@"Fout bij het lezen van bestand '{sectionsFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.");
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_FileDoesNotExist_AbortImportWithErrorMessage()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor).Returns(expectedUpdateDataFailedText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, "I_dont_exist.shp");

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(
                expectedUpdateDataFailedText, $@"Fout bij het lezen van bestand '{sectionsFilePath}': het bestand bestaat niet.");
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_EmptyArtificialFile_AbortImportWithErrorMessage()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor).Returns(expectedUpdateDataFailedText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_EmptyVakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(expectedUpdateDataFailedText, "Het bestand heeft geen vakindeling");
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        [TestCase("StartTooFarFromReferenceline")]
        [TestCase("EndTooFarFromReferenceline")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromReferenceLine_AbortImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor).Returns(expectedUpdateDataFailedText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string shapeFileName = $"Artificial_referencelijn_testA_InvalidVakken_Section{shapeCondition}.shp";
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", shapeFileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(
                expectedUpdateDataFailedText, "De geografische ligging van ieder vak moet overeenkomen met de ligging van (een deel van) de referentielijn");
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        [TestCase("StartTooFarFromReferencelineStart")]
        [TestCase("EndTooFarFromReferencelineEnd")]
        public void Import_InvalidArtificialFileBecauseOfStartEndPointsTooFarFromStartEndOfReferenceLine_AbortImportWithErrorMessage(string shapeCondition)
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor).Returns(expectedUpdateDataFailedText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string shapeFileName = $"Artificial_referencelijn_testA_InvalidVakken_{shapeCondition}.shp";
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", shapeFileName));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(
                expectedUpdateDataFailedText, "De geografische ligging van ieder vak moet overeenkomen met de ligging van (een deel van) de referentielijn");
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSectionsDoNotFullyCoverReferenceLine_AbortImportWithErrorMessage()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor).Returns(expectedUpdateDataFailedText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_NotCoveringWholeReferenceLine.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(expectedUpdateDataFailedText, "De opgetelde lengte van de vakken moet overeenkomen met de trajectlengte");
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseUnchainedSections_AbortImportWithErrorMessage()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor).Returns(expectedUpdateDataFailedText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_UnchainedSections.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(expectedUpdateDataFailedText, "Het bestand moet vakken bevatten die allen op elkaar aansluiten");
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_InvalidArtificialFileBecauseSomePointsNotOnReferenceLine_AbortImportWithErrorMessage()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetUpdateDataFailedLogMessageText(sectionsTypeDescriptor).Returns(expectedUpdateDataFailedText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "Artificial_referencelijn_testA.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "Artificial_referencelijn_testA_InvalidVakken_PointsTooFarFromReferenceLine.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            string expectedMessage = string.Format(expectedUpdateDataFailedText, "De opgetelde lengte van de vakken moet overeenkomen met de trajectlengte");
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_MissingNameValue_AbortImportWithErrorMessage()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "vakindeling_Empty_Name_Value.shp"));

            var importer = new FailureMechanismSectionsImporter(failureMechanism, new ReferenceLine(), sectionsFilePath, updateStrategy, messageProvider);

            // Call
            var importSuccessful = true;
            void Call() => importSuccessful = importer.Import();

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{sectionsFilePath}': voor een of meerdere vakken is geen naam opgegeven.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingFailureMechanismSections_CancelsImportAndLogs()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetCancelledLogMessageText(sectionsTypeDescriptor).Returns(expectedCancelledText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

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
            void Call() => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, expectedCancelledText, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_CancelOfImportWhenValidatingImportedSections_CancelsImportAndLogs()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetCancelledLogMessageText(sectionsTypeDescriptor).Returns(expectedCancelledText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

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
            void Call() => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, expectedCancelledText, 1);
            Assert.IsFalse(importSuccessful);
        }

        [Test]
        public void Import_CancelOfImportWhenAddingDataToModel_ContinuesImportAndLogs()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();

            updateStrategy.UpdateSectionsWithImportedData(Arg.Any<IEnumerable<FailureMechanismSection>>(), Arg.Any<string>())
                          .Returns(callInfo =>
                          {
                              CollectionAssert.IsNotEmpty((IEnumerable<FailureMechanismSection>) callInfo.Args()[0]);
                              return Enumerable.Empty<IObservable>();
                          });

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            messageProvider.GetAddDataToModelProgressText().Returns(expectedAddDataToModelProgressText);
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

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
            void Call() => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, "Huidige actie was niet meer te annuleren en is daarom voortgezet.", 2);
            Assert.IsTrue(importSuccessful);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_TrueAndExpectedImportedData()
        {
            // Setup
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            updateStrategy.UpdateSectionsWithImportedData(Arg.Any<IEnumerable<FailureMechanismSection>>(), Arg.Any<string>())
                          .Returns(callInfo =>
                          {
                              var sections = (IEnumerable<FailureMechanismSection>) callInfo.Args()[0];
                              Assert.AreEqual(62, sections.Count());
                              AssertSectionsAreValidForReferenceLine(sections, importReferenceLine);
                              Assert.AreEqual(sectionsFilePath, callInfo.Args()[1]);
                              return Enumerable.Empty<IObservable>();
                          });

            var messageProvider = Substitute.For<IImporterMessageProvider>();
            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            Assert.IsFalse(importer.Import());
            importer.SetProgressChanged(null);

            // Call
            var importSuccessful = false;
            void Call() => importSuccessful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{sectionsFilePath}'.", 1);
            Assert.IsTrue(importSuccessful);
        }

        [Test]
        public void DoPostImport_AfterImport_CallUpdateStrategyAndObserversNotified()
        {
            // Setup
            var updateSectionsWithImportedDataObservable = new TestObservable();
            var doPostUpdateActionsObservable = new TestObservable();
            var updateStrategy = Substitute.For<IFailureMechanismSectionUpdateStrategy>();
            updateStrategy.UpdateSectionsWithImportedData(Arg.Any<IEnumerable<FailureMechanismSection>>(), Arg.Any<string>())
                          .Returns(new[]
                          {
                              updateSectionsWithImportedDataObservable
                          });
            updateStrategy.DoPostUpdateActions()
                          .Returns(new[]
                          {
                              doPostUpdateActionsObservable
                          });
            var messageProvider = Substitute.For<IImporterMessageProvider>();

            var updateSectionsWithImportedDataObserver = Substitute.For<IObserver>();

            var doPostUpdateActionsObserver = Substitute.For<IObserver>();
            string referenceLineFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                      Path.Combine("ReferenceLine", "traject_1-1.shp"));
            string sectionsFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                 Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            ReferenceLine importReferenceLine = ImportReferenceLine(referenceLineFilePath);

            var failureMechanism = new TestFailureMechanism();
            var importer = new FailureMechanismSectionsImporter(failureMechanism, importReferenceLine, sectionsFilePath, updateStrategy, messageProvider);

            importer.Import();
            updateSectionsWithImportedDataObservable.Attach(updateSectionsWithImportedDataObserver);
            doPostUpdateActionsObservable.Attach(doPostUpdateActionsObserver);

            // Call
            importer.DoPostImport();

            // Assert
            updateSectionsWithImportedDataObserver.Received().UpdateObserver();
            doPostUpdateActionsObserver.Received().UpdateObserver();
            updateStrategy.Received().DoPostUpdateActions();
        }

        private class TestObservable : Observable {}

        private static ReferenceLine ImportReferenceLine(string referenceLineFilePath)
        {
            var referenceLine = new ReferenceLine();
            var handler = Substitute.For<IReferenceLineUpdateHandler>();
            handler.ConfirmUpdate().Returns(true);

            handler.Update(Arg.Is<ReferenceLine>(x => x != null),
                           Arg.Is<ReferenceLine>(x => x != null))
                   .Returns(callInfo =>
                   {
                       referenceLine = (ReferenceLine) callInfo.Args()[1];
                       return Enumerable.Empty<IObservable>();
                   });

            var referenceLineImporter = new ReferenceLineImporter(referenceLine, handler, referenceLineFilePath);
            referenceLineImporter.Import();
            return referenceLine;
        }

        private static void AssertSectionsAreValidForReferenceLine(IEnumerable<FailureMechanismSection> sections, ReferenceLine referenceLine)
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

        private static double GetDistanceToReferenceLine(Point2D point, ReferenceLine referenceLine)
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