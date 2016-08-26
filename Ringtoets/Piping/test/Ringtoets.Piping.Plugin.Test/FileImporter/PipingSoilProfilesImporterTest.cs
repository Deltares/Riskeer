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
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;
using RingtoetsIOResources = Ringtoets.Piping.IO.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingSoilProfilesImporterTest
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
            TestDelegate call = () => new PipingSoilProfilesImporter(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var expectedFileFilter = string.Format("{0} {1} (*.soil)|*.soil",
                                                   PipingFormsResources.StochasticSoilProfileCollection_DisplayName, ApplicationResources.Soil_file_name);

            var list = new ObservableList<StochasticSoilModel>();

            // Call
            var importer = new PipingSoilProfilesImporter(list);

            // Assert
            Assert.IsInstanceOf<FileImporterBase>(importer);
        }

        [Test]
        public void Import_FromNonExistingFile_LogError()
        {
            // Setup
            var file = "nonexisting.soil";
            string validFilePath = Path.Combine(testDataPath, file);

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(validFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped, string.Empty);
                StringAssert.EndsWith(message, messageArray[0]);
            });
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.AreEqual(1, progress);
        }

        [Test]
        public void Import_FromInvalidFileName_LogError()
        {
            // Setup
            var file = "/";
            string invalidFilePath = Path.Combine(testDataPath, file);

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(invalidFilePath);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] messageArray = messages.ToArray();
                var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped, string.Empty);
                StringAssert.EndsWith(message, messageArray[0]);
            });
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.AreEqual(1, progress);
        }

        [Test]
        public void Import_ImportingToValidTargetWithValidFile_ImportSoilModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();

            var progressChangeNotifications = new List<ProgressNotification>();
            var importer = new PipingSoilProfilesImporter(pipingFailureMechanism.StochasticSoilModels)
            {
                ProgressChanged = (description, step, steps) => { progressChangeNotifications.Add(new ProgressNotification(description, step, steps)); }
            };

            // Call
            var importResult = importer.Import(validFilePath);

            // Assert
            Assert.IsTrue(importResult);
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
                expectedProgressMessages.Add(new ProgressNotification("Geïmporteerde data toevoegen aan toetsspoor.", i, expectedModels));
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

            var pipingFailureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(pipingFailureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = false;

            // Precondition
            importer.Import(validFilePath);
            var alreadyImportedSoilModelNames = pipingFailureMechanism.StochasticSoilModels.Select(ssm => ssm.Name);

            // Call
            Action call = () => importResult = importer.Import(validFilePath);

            // Assert
            var expectedLogMessages = alreadyImportedSoilModelNames.Select(name => string.Format("Het stochastische ondergrondmodel '{0}' bestaat al in het toetsspoor.", name)).ToArray();
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, expectedLogMessages.Length);

            Assert.IsTrue(importResult);
            Assert.AreEqual(35*2, progress);
        }

        [Test]
        public void Import_CancelOfImportToValidTargetWithValidFile_CancelImportAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            importer.Cancel();

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(validFilePath);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, ApplicationResources.PipingSoilProfilesImporter_Import_Import_cancelled, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.AreEqual(1, progress);
        }

        [Test]
        public void Import_ReuseOfCancelledImportToValidTargetWithValidFile_ImportSoilModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            // Setup (second part)
            importer.Cancel();
            var importResult = importer.Import(validFilePath);
            Assert.IsFalse(importResult);

            // Call
            importResult = importer.Import(validFilePath);

            // Assert
            Assert.IsTrue(importResult);
            Assert.AreEqual(36, progress);
        }

        [Test]
        public void Import_ImportingToValidTargetWithEmptyFile_AbortImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "empty.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = true;

            // Call
            Action call = () => importResult = importer.Import(corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(RingtoetsIOResources.PipingSoilProfileReader_Critical_Unexpected_value_on_column);
            var expectedLogMessage = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                                   internalErrorMessage);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.IsFalse(importResult);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels, "No items should be added to collection when import is aborted.");
            Assert.AreEqual(1, progress);
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidAtX_SkipImportAndLog()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "invalidAtX2dProperty.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(corruptPath);

            // Assert
            var internalErrorMessage = new FileReaderErrorMessageBuilder(corruptPath)
                .WithSubject("ondergrondschematisatie 'Profile'")
                .Build(string.Format(RingtoetsIOResources.PipingSoilProfileReader_Profile_has_invalid_value_on_Column_0_,
                                     "IntersectionX"));
            var expectedLogMessages = new[]
            {
                string.Format(ApplicationResources.PipingSoilProfilesImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                              internalErrorMessage),
                string.Format("Het stochastische ondergrondmodel '{0}' heeft een ongespecificeerde ondergrondschematisatie. Dit model wordt overgeslagen.", "Name")
            };
            TestHelper.AssertLogMessagesAreGenerated(call, expectedLogMessages, 2);

            Assert.IsTrue(importResult);
            Assert.AreEqual(0, failureMechanism.StochasticSoilModels.Count);
            Assert.AreEqual(7, progress);
        }

        [Test]
        public void Import_ImportingToValidTargetWithProfileContainingInvalidParameterValue_ZeroForValue()
        {
            // Setup
            string corruptPath = Path.Combine(testDataPath, "incorrectValue2dProperty.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            // Call
            var importResult = importer.Import(corruptPath);

            Assert.IsTrue(importResult);
            Assert.AreEqual(0, failureMechanism.StochasticSoilModels.Count);
        }

        [Test]
        public void Import_IncorrectProfiles_SkipModelAndLog()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "invalidStochasticSoilProfiles.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(validFilePath);

            // Assert
            var expectedLogMessage = string.Format("Fout bij het lezen van bestand '{0}': De ondergrondschematisatie verwijst naar een ongeldige waarde." +
                                                   " Dit stochastische ondergrondmodel wordt overgeslagen.", validFilePath);
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreEqual(0, failureMechanism.StochasticSoilModels.Count);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_IncorrectProbability_LogAndImportSoilModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "incorrectProbability.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(validFilePath);

            // Assert
            var expectedLogMessages = "De som van de kansen van voorkomen in het stochastich ondergrondmodel 'Name' is niet gelijk aan 100%.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessages, 1);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_TwoSoilModelsReusingSameProfile1D_ImportSoilModelsToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "reusedSoilProfile1D.soil");

            var pipingFailureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(pipingFailureMechanism.StochasticSoilModels);

            // Call
            var importResult = importer.Import(validFilePath);

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
            var importer = new PipingSoilProfilesImporter(pipingFailureMechanism.StochasticSoilModels);

            // Call
            var importResult = importer.Import(validFilePath);

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
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels);

            var importResult = false;

            // Call
            Action call = () => importResult = importer.Import(validFilePath);

            // Assert
            var expectedLogMessage = @"Er zijn geen ondergrondschematisaties gevonden in het stochastische ondergrondmodel 'Model'. Dit model wordt overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage, 1);
            Assert.AreEqual(0, failureMechanism.StochasticSoilModels.Count);
            Assert.IsTrue(importResult);
        }

        [Test]
        public void Import_ModelWithOneStochasticSoilProfile2DWithoutLayerPropertiesSet_ImportModelToCollection()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "SingleSoilProfile2D_noLayerProperties.soil");

            var failureMechanism = new PipingFailureMechanism();
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            bool importResult = importer.Import(validFilePath);

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
            var importer = new PipingSoilProfilesImporter(failureMechanism.StochasticSoilModels)
            {
                ProgressChanged = IncrementProgress
            };

            // Precondition
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsTrue(File.Exists(validFilePath));

            // Call
            bool importResult = importer.Import(validFilePath);

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

            public string Text { get; private set; }
            public int CurrentStep { get; private set; }
            public int TotalSteps { get; private set; }
        }
    }
}