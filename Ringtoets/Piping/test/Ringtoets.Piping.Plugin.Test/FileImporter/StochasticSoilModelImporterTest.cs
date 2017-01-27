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
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Plugin.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class StochasticSoilModelImporterTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, "PipingSoilProfilesReader");
        private int progress;

        [SetUp]
        public void SetUp()
        {
            progress = 0;
        }

        [Test]
        public void Constructor_ObservableListNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StochasticSoilModelImporter(null, "", new StochasticSoilModelReplaceData());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", paramName);
        }

        [Test]
        public void Constructor_ModelUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StochasticSoilModelImporter(new StochasticSoilModelCollection(), "", null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("modelUpdateStrategy", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            var list = new StochasticSoilModelCollection();

            // Call
            var importer = new StochasticSoilModelImporter(list, "", new StochasticSoilModelReplaceData());

            // Assert
            Assert.IsInstanceOf<FileImporterBase<StochasticSoilModelCollection>>(importer);
        }

        [Test]
        public void Import_FromNonExistingFile_LogError()
        {
            // Setup
            var file = "nonexisting.soil";
            string validFilePath = Path.Combine(testDataPath, file);

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessages(call,
                                         messages =>
                                         {
                                             string[] messageArray = messages.ToArray();
                                             var message = $"{string.Empty} \r\nHet bestand wordt overgeslagen.";
                                             StringAssert.EndsWith(message, messageArray[0]);
                                         });
            Assert.AreEqual(1, progress);
            AssertUnsuccessfulImport(importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_FromInvalidFileName_LogError()
        {
            // Setup
            var file = "/";
            string invalidFilePath = Path.Combine(testDataPath, file);

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, invalidFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessages(call,
                                         messages =>
                                         {
                                             string[] messageArray = messages.ToArray();
                                             var message = $"{string.Empty} \r\nHet bestand wordt overgeslagen.";
                                             StringAssert.EndsWith(message, messageArray[0]);
                                         });
            Assert.AreEqual(1, progress);
            AssertUnsuccessfulImport(importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSoilModelToCollectionAndSourcePathSet()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();

            var progressChangeNotifications = new List<ProgressNotification>();
            var importer = new StochasticSoilModelImporter(pipingFailureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged((description, step, steps) => progressChangeNotifications.Add(new ProgressNotification(description, step, steps)));

            // Call
            var importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(validFilePath, pipingFailureMechanism.StochasticSoilModels.SourcePath);
            var expectedProfiles = 26;
            var expectedModels = 3;

            var expectedProgressMessages = new List<ProgressNotification>
            {
                new ProgressNotification("Inlezen van de D-Soil Model database.", 1, 1)
            };
            for (var i = 1; i <= expectedProfiles; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Inlezen van de ondergrondschematisatie uit de D-Soil Model database.", i, expectedProfiles));
            }
            expectedProgressMessages.Add(new ProgressNotification("Inlezen van de D-Soil Model database.", 1, 1));
            for (var i = 1; i <= expectedModels; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Inlezen van de stochastische ondergrondmodellen.", i, expectedModels));
            }
            expectedProgressMessages.Add(new ProgressNotification("Controleren van ondergrondschematisaties.", 1, 1));
            for (var i = 1; i <= expectedModels; i++)
            {
                expectedProgressMessages.Add(new ProgressNotification("Geïmporteerde data toevoegen aan het toetsspoor.", i, expectedModels));
            }
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

        [Test]
        public void Import_ImportingToValidTargetWithValidFileTwice_AddsSoilModelToCollectionLogWarning()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Precondition
            importer.Import();
            var alreadyImportedSoilModelNames = failureMechanism.StochasticSoilModels.Select(ssm => ssm.Name);

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var expectedLogMessages = alreadyImportedSoilModelNames.Select(name => $"Het stochastische ondergrondmodel '{name}' bestaat al in het toetsspoor.").ToArray();
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, expectedLogMessages.Length);
            Assert.AreEqual(35*2, progress);

            AssertSuccessfulImport(6, validFilePath, importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingSoilProfiles_CancelsImportAndLogs()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van de D-Soil Model database."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Stochastische ondergrondmodellen importeren afgebroken. Geen data ingelezen.", 1);
            AssertUnsuccessfulImport(importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_CancelOfImportWhenReadingStochasticSoilModels_CancelsImportAndLogs()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Inlezen van de stochastische ondergrondmodellen."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Stochastische ondergrondmodellen importeren afgebroken. Geen data ingelezen.", 1);
            AssertUnsuccessfulImport(importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_CancelOfImportWhenAddingAndCheckingSoilProfiles_CancelsImportAndLogs()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Controleren van ondergrondschematisaties."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Stochastische ondergrondmodellen importeren afgebroken. Geen data ingelezen.", 1);
            AssertUnsuccessfulImport(importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_CancelOfImportWhenAddingDataToModel_ImportCompletedSuccesfullyNonetheless()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged((description, step, steps) =>
            {
                if (description.Contains("Geïmporteerde data toevoegen aan het toetsspoor."))
                {
                    importer.Cancel();
                }
            });

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Huidige actie was niet meer te annuleren en is daarom voortgezet.", 1);
            AssertSuccessfulImport(3, validFilePath, importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_ReuseOfCanceledImportToValidTargetWithValidFile_ImportSoilModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged((description, step, steps) => importer.Cancel());

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            // Setup (second part)
            bool importResult = importer.Import();
            Assert.IsFalse(importResult);

            importer.SetProgressChanged(null);

            // Call
            importResult = importer.Import();

            // Assert
            AssertSuccessfulImport(3, validFilePath, importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_ImportingToValidTargetWithEmptyFile_AbortImportAndLog()
        {
            // Setup
            string pathToCorruptFile = Path.Combine(testDataPath, "empty.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, pathToCorruptFile, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(pathToCorruptFile).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
            var expectedLogMessage = $"{internalErrorMessage} \r\nHet bestand wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreEqual(1, progress);

            AssertUnsuccessfulImport(importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidAtX_SkipImportAndLog()
        {
            // Setup
            string pathToCorruptFile = Path.Combine(testDataPath, "invalidAtX2dProperty.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, pathToCorruptFile, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(pathToCorruptFile)
                .WithSubject("ondergrondschematisatie 'Profile'")
                .Build("Ondergrondschematisatie bevat geen geldige waarde in kolom \'IntersectionX\'.");
            var expectedLogMessages = new[]
            {
                $"{internalErrorMessage} \r\nDeze ondergrondschematisatie wordt overgeslagen.",
                "Het stochastische ondergrondmodel \'Name\' heeft een ongespecificeerde ondergrondschematisatie. Dit model wordt overgeslagen."
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);
            Assert.AreEqual(7, progress);

            AssertSuccessfulImport(0, pathToCorruptFile, importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidParameterValue_ZeroForValue()
        {
            // Setup
            string pathToCorruptFile = Path.Combine(testDataPath, "incorrectValue2dProperty.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, pathToCorruptFile, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            // Call
            var importResult = importer.Import();

            // Assert
            AssertSuccessfulImport(0, pathToCorruptFile, importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_IncorrectProfiles_SkipModelAndLog()
        {
            // Setup
            string pathToCorruptFile = Path.Combine(testDataPath, "invalidStochasticSoilProfiles.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, pathToCorruptFile, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var expectedLogMessage =
                $"Fout bij het lezen van bestand '{pathToCorruptFile}': de ondergrondschematisatie verwijst naar een ongeldige waarde." +
                " Dit stochastische ondergrondmodel wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            AssertSuccessfulImport(0, pathToCorruptFile, importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_IncorrectProbability_LogAndImportSoilModelToCollection()
        {
            // Setup
            string pathToCorruptFile = Path.Combine(testDataPath, "incorrectProbability.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, pathToCorruptFile, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var expectedLogMessages = "De som van de kansen van voorkomen in het stochastich ondergrondmodel 'Name' is niet gelijk aan 100%.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessages, 1);
            AssertSuccessfulImport(1, pathToCorruptFile, importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_TwoSoilModelsReusingSameProfile1D_ImportSoilModelsToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "reusedSoilProfile1D.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(pipingFailureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());

            // Call
            var importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, pipingFailureMechanism.StochasticSoilModels.Count);
            StochasticSoilModel model1 = pipingFailureMechanism.StochasticSoilModels[0];
            StochasticSoilModel model2 = pipingFailureMechanism.StochasticSoilModels[1];

            Assert.AreEqual(1, model1.StochasticSoilProfiles.Count);
            Assert.AreEqual(1, model2.StochasticSoilProfiles.Count);

            StochasticSoilProfile profile1 = model1.StochasticSoilProfiles[0];
            StochasticSoilProfile profile2 = model2.StochasticSoilProfiles[0];
            Assert.AreNotSame(profile1, profile2);
            Assert.AreSame(profile1.SoilProfile, profile2.SoilProfile);

            Assert.AreEqual(SoilProfileType.SoilProfile1D, profile1.SoilProfileType,
                            "Expected database to have 1D profiles.");
            Assert.AreEqual(SoilProfileType.SoilProfile1D, profile2.SoilProfileType,
                            "Expected database to have 1D profiles.");
        }

        [Test]
        public void Import_TwoSoilModelsReusingSameProfile2D_ImportSoilModelsToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "reusedSoilProfile2D.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(pipingFailureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());

            // Call
            var importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);

            Assert.AreEqual(2, pipingFailureMechanism.StochasticSoilModels.Count);
            StochasticSoilModel model1 = pipingFailureMechanism.StochasticSoilModels[0];
            StochasticSoilModel model2 = pipingFailureMechanism.StochasticSoilModels[1];

            Assert.AreEqual(1, model1.StochasticSoilProfiles.Count);
            Assert.AreEqual(1, model2.StochasticSoilProfiles.Count);

            StochasticSoilProfile profile1 = model1.StochasticSoilProfiles[0];
            StochasticSoilProfile profile2 = model2.StochasticSoilProfiles[0];
            Assert.AreNotSame(profile1, profile2);
            Assert.AreSame(profile1.SoilProfile, profile2.SoilProfile);

            Assert.AreEqual(SoilProfileType.SoilProfile2D, profile1.SoilProfileType,
                            "Expected database to have 2D profiles.");
            Assert.AreEqual(SoilProfileType.SoilProfile2D, profile2.SoilProfileType,
                            "Expected database to have 2D profiles.");
        }

        [Test]
        public void Import_ModelWithOneInvalidStochasticSoilProfileDueToMissingProfile_SkipModelAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "EmptySoilModel.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import();

            // Assert
            var expectedLogMessage = @"Er zijn geen ondergrondschematisaties gevonden in het stochastische ondergrondmodel 'Model'. Dit model wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);

            AssertSuccessfulImport(0, validFilePath, importResult, failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Import_ModelWithTwoStochasticSoilProfileForSameProfile_ProbabilitiesAddedAndLog()
        {
            // Setup
            string pathToFile = Path.Combine(testDataPath, "multipleStochasticSoilProfileForSameProfile.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, pathToFile, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            var importResult = false;

            // Call
            Action importAction = () => importResult = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(importAction, "Ondergrondschematisatie 'Profile' is meerdere keren gevonden in ondergrondmodel 'StochasticSoilModelName'. Kansen van voorkomen worden opgeteld.", 1);
            Assert.IsTrue(importResult);
            StochasticSoilModelCollection importedModels = failureMechanism.StochasticSoilModels;
            Assert.AreEqual(pathToFile, importedModels.SourcePath);
            Assert.AreEqual(1, importedModels.Count);
            var firstModel = importedModels.First();
            Assert.AreEqual(1, firstModel.StochasticSoilProfiles.Count);
            Assert.AreEqual(1.0, firstModel.StochasticSoilProfiles[0].Probability);
        }

        [Test]
        public void Import_ModelWithOneStochasticSoilProfile2DWithoutLayerPropertiesSet_ImportModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "SingleSoilProfile2D_noLayerProperties.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);

            StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[0];
            Assert.AreEqual(1, soilModel.StochasticSoilProfiles.Count);

            StochasticSoilProfile stochasticProfile = soilModel.StochasticSoilProfiles[0];
            Assert.AreEqual(100.0, stochasticProfile.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, stochasticProfile.SoilProfileType);

            PipingSoilProfile profile = stochasticProfile.SoilProfile;
            Assert.AreEqual("AD647M30_Segment_36005_1D1", profile.Name);
            Assert.AreEqual(-45.0, profile.Bottom);
            Assert.AreEqual(9, profile.Layers.Count());
            var expectedLayerTops = new[]
            {
                4.8899864439741778,
                3.25,
                2.75,
                1.25,
                1.0,
                -2.5,
                -13.0,
                -17.0,
                -25.0
            };
            CollectionAssert.AreEqual(expectedLayerTops, profile.Layers.Select(l => l.Top));
            int expectedNumberOfLayers = expectedLayerTops.Length;
            CollectionAssert.AreEqual(Enumerable.Repeat(false, expectedNumberOfLayers),
                                      profile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(Enumerable.Repeat(double.NaN, expectedNumberOfLayers),
                                      profile.Layers.Select(l => l.BelowPhreaticLevelMean));

            Assert.AreEqual(6, progress);
        }

        [Test]
        public void Import_ModelWithOneStochasticSoilProfile2DWithLayerPropertiesSet_ImportModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "SingleSoilProfile2D_withLayerProperties.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new StochasticSoilModelImporter(failureMechanism.StochasticSoilModels, validFilePath, new StochasticSoilModelReplaceData());
            importer.SetProgressChanged(IncrementProgress);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            bool importResult = importer.Import();

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);

            StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels[0];
            Assert.AreEqual(1, soilModel.StochasticSoilProfiles.Count);

            StochasticSoilProfile stochasticProfile = soilModel.StochasticSoilProfiles[0];
            Assert.AreEqual(100.0, stochasticProfile.Probability);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, stochasticProfile.SoilProfileType);

            PipingSoilProfile profile = stochasticProfile.SoilProfile;
            Assert.AreEqual("Test 2d profile", profile.Name);
            Assert.AreEqual(-45.0, profile.Bottom);
            const int expectedNumberOfLayers = 9;
            Assert.AreEqual(expectedNumberOfLayers, profile.Layers.Count());
            var expectedLayerTops = new[]
            {
                5.0571018353300463,
                3.25,
                2.75,
                1.25,
                1.0,
                -2.5,
                -13.0,
                -17.0,
                -25.0
            };
            var expectedIsAquiferValues = new[]
            {
                false,
                false,
                false,
                true,
                false,
                false,
                true,
                false,
                false
            };
            CollectionAssert.AreEqual(expectedIsAquiferValues,
                                      profile.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(expectedLayerTops, profile.Layers.Select(l => l.Top));
            var expectedBelowPhreaticLevelValues = new[]
            {
                27.27,
                28.28,
                29.29,
                30.3,
                33.33,
                35.35,
                37.37,
                39.39,
                40.4
            };
            CollectionAssert.AreEqual(expectedBelowPhreaticLevelValues,
                                      profile.Layers.Select(l => l.BelowPhreaticLevelMean));

            Assert.AreEqual(6, progress);
        }

        private static void AssertSuccessfulImport(
            int expectedSoilModelCount,
            string expectedFilePath,
            bool actualImportResult,
            StochasticSoilModelCollection actualStochasticSoilModels)
        {
            Assert.AreEqual(expectedSoilModelCount, actualStochasticSoilModels.Count);
            Assert.AreEqual(expectedFilePath, actualStochasticSoilModels.SourcePath);
            Assert.IsTrue(actualImportResult);
        }

        private static void AssertUnsuccessfulImport(
            bool actualImportResult,
            StochasticSoilModelCollection stochasticSoilModels)
        {
            Assert.IsEmpty(stochasticSoilModels);
            Assert.IsNull(stochasticSoilModels.SourcePath);
            Assert.IsFalse(actualImportResult);
        }

        private void IncrementProgress(string a, int b, int c)
        {
            progress++;
        }

        private class ProgressNotification
        {
            public ProgressNotification(string description, int currentStep, int totalSteps)
            {
                Text = description;
                CurrentStep = currentStep;
                TotalSteps = totalSteps;
            }

            public string Text { get; }
            public int CurrentStep { get; }
            public int TotalSteps { get; }
        }
    }
}