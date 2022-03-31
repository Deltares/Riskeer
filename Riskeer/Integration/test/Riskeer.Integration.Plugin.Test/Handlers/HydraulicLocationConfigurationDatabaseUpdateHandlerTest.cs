﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Data;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.HydraRing.IO.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class HydraulicLocationConfigurationDatabaseUpdateHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicLocationConfigurationDatabaseUpdateHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSection());

            // Assert
            Assert.IsInstanceOf<IHydraulicLocationConfigurationDatabaseUpdateHandler>(handler);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_ClickDialog_ReturnsExpectedResult(bool clickOk)
        {
            // Setup
            string dialogTitle = null, dialogMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                dialogTitle = tester.Title;
                dialogMessage = tester.Text;
                if (clickOk)
                {
                    tester.ClickOk();
                }
                else
                {
                    tester.ClickCancel();
                }
            };

            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSection());

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(clickOk, result);

            Assert.AreEqual("Bevestigen", dialogTitle);
            Assert.AreEqual("Als u het gekoppelde HLCD bestand wijzigt, zal de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden." +
                            Environment.NewLine +
                            Environment.NewLine +
                            "Wilt u doorgaan?",
                            dialogMessage);
        }

        [Test]
        public void Update_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSection());

            // Call
            void Call() => handler.Update(null, ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create(), false, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Update_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSection());

            // Call
            void Call() => handler.Update(new HydraulicBoundaryDatabase(), ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create(), false, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseSettingsNull_SetsDefaultValuesAndLogsWarning()
        {
            // Setup
            const string hlcdFilePath = "some/file/path";
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSection());
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            // Call
            void Call() => handler.Update(hydraulicBoundaryDatabase, null, usePreprocessorClosure, hlcdFilePath);

            // Assert
            const string expectedMessage = "De tabel 'ScenarioInformation' in het HLCD bestand is niet aanwezig. Er worden standaardwaarden " +
                                           "conform WBI2017 gebruikt voor de HLCD bestandsinformatie.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);

            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(hlcdFilePath, settings.FilePath);
            Assert.AreEqual("WBI2017", settings.ScenarioName);
            Assert.AreEqual(2023, settings.Year);
            Assert.AreEqual("WBI2017", settings.Scope);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
            Assert.AreEqual("Conform WBI2017", settings.SeaLevel);
            Assert.AreEqual("Conform WBI2017", settings.RiverDischarge);
            Assert.AreEqual("Conform WBI2017", settings.LakeLevel);
            Assert.AreEqual("Conform WBI2017", settings.WindDirection);
            Assert.AreEqual("Conform WBI2017", settings.WindSpeed);
            Assert.AreEqual("Gegenereerd door Riskeer (conform WBI2017)", settings.Comment);
        }

        [Test]
        public void Update_WithReadHydraulicLocationConfigurationDatabaseSettings_SetsExpectedValuesAndDoesNotLog()
        {
            // Setup
            const string hlcdFilePath = "some/file/path";
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSection());
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            ReadHydraulicLocationConfigurationDatabaseSettings readSettings = ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create();
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            // Call
            void Call() => handler.Update(hydraulicBoundaryDatabase, readSettings, usePreprocessorClosure, hlcdFilePath);

            // Assert
            TestHelper.AssertLogMessagesCount(Call, 0);

            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(hlcdFilePath, settings.FilePath);
            Assert.AreEqual(readSettings.ScenarioName, settings.ScenarioName);
            Assert.AreEqual(readSettings.Year, settings.Year);
            Assert.AreEqual(readSettings.Scope, settings.Scope);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
            Assert.AreEqual(readSettings.SeaLevel, settings.SeaLevel);
            Assert.AreEqual(readSettings.RiverDischarge, settings.RiverDischarge);
            Assert.AreEqual(readSettings.LakeLevel, settings.LakeLevel);
            Assert.AreEqual(readSettings.WindDirection, settings.WindDirection);
            Assert.AreEqual(readSettings.WindSpeed, settings.WindSpeed);
            Assert.AreEqual(readSettings.Comment, settings.Comment);
        }

        [Test]
        public void Update_DataUpdated_ReturnsChangedObjects()
        {
            // Setup
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSection());
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            IEnumerable<IObservable> changedObjects = handler.Update(hydraulicBoundaryDatabase, null, false, "some/file/path");

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                hydraulicBoundaryDatabase
            }, changedObjects);
        }

        [Test]
        public void Update_LocationsWithOutput_ClearOutputAndReturnChangedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(assessmentSection);

            IEnumerable<HydraulicBoundaryLocationCalculation> locations = GetLocationCalculations(assessmentSection);
            IEnumerable<DuneLocationCalculation> duneLocations = GetDuneLocationCalculations(assessmentSection);
            ICalculation[] calculationsWithOutput = assessmentSection.GetFailureMechanisms()
                                                                     .OfType<ICalculatableFailureMechanism>()
                                                                     .SelectMany(fm => fm.Calculations)
                                                                     .Where(c => c.HasOutput)
                                                                     .ToArray();

            calculationsWithOutput = calculationsWithOutput.Except(calculationsWithOutput.OfType<SemiProbabilisticPipingCalculationScenario>()
                                                                                         .Where(c => c.InputParameters.UseAssessmentLevelManualInput))
                                                           .Except(calculationsWithOutput.OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                         .Where(c => c.InputParameters.UseAssessmentLevelManualInput))
                                                           .Except(calculationsWithOutput.OfType<TestPipingCalculationScenario>())
                                                           .ToArray();

            // Precondition
            Assert.IsTrue(locations.All(l => l.HasOutput));
            Assert.IsTrue(duneLocations.All(l => l.Output != null));

            // Call
            IEnumerable<IObservable> changedObjects = handler.Update(assessmentSection.HydraulicBoundaryDatabase, null, false, "some/file/path");

            // Assert
            Assert.IsTrue(locations.All(l => !l.HasOutput));
            Assert.IsTrue(duneLocations.All(l => l.Output == null));
            Assert.IsTrue(calculationsWithOutput.All(c => !c.HasOutput));

            IEnumerable<IObservable> expectedChangedObjects = new IObservable[]
            {
                assessmentSection.HydraulicBoundaryDatabase
            }.Concat(locations).Concat(duneLocations).Concat(calculationsWithOutput);

            CollectionAssert.AreEquivalent(expectedChangedObjects, changedObjects);
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            return new AssessmentSection(AssessmentSectionComposition.Dike);
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetLocationCalculations(AssessmentSection assessmentSection)
        {
            var calculations = new List<HydraulicBoundaryLocationCalculation>();

            calculations.AddRange(assessmentSection.WaterLevelCalculationsForSignalFloodingProbability);
            calculations.AddRange(assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability);

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                calculations.AddRange(element.HydraulicBoundaryLocationCalculations);
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                calculations.AddRange(element.HydraulicBoundaryLocationCalculations);
            }

            return calculations;
        }

        private static IEnumerable<DuneLocationCalculation> GetDuneLocationCalculations(AssessmentSection assessmentSection)
        {
            return assessmentSection.DuneErosion.DuneLocationCalculationsForUserDefinedTargetProbabilities.SelectMany(dlc => dlc.DuneLocationCalculations);
        }
    }
}