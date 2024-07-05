// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSectionWithHydraulicBoundaryDatabases());

            // Assert
            Assert.IsInstanceOf<IHydraulicLocationConfigurationDatabaseUpdateHandler>(handler);
        }

        [Test]
        public void InquireConfirmation_WithoutHydraulicBoundaryDatabases_ReturnsTrue()
        {
            // Setup
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_WithHydraulicBoundaryDatabasesAndClickDialog_ReturnsExpectedResult(bool clickOk)
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

            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSectionWithHydraulicBoundaryDatabases());

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(clickOk, result);

            Assert.AreEqual("Bevestigen", dialogTitle);
            Assert.AreEqual("Als u het geselecteerde HLCD bestand wijzigt, zal de uitvoer van alle ervan afhankelijke berekeningen verwijderd worden." +
                            Environment.NewLine +
                            Environment.NewLine +
                            "Wilt u doorgaan?",
                            dialogMessage);
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSectionWithHydraulicBoundaryDatabases());

            // Call
            void Call() => handler.Update(null, new Dictionary<HydraulicBoundaryDatabase, long>(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("readHydraulicLocationConfigurationDatabase", exception.ParamName);
        }

        [Test]
        public void Update_HydraulicBoundaryDatabaseLookupNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSectionWithHydraulicBoundaryDatabases());

            // Call
            void Call() => handler.Update(ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(1), null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDatabaseLookup", exception.ParamName);
        }

        [Test]
        public void Update_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSectionWithHydraulicBoundaryDatabases());

            // Call
            void Call() => handler.Update(ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(1),
                                          new Dictionary<HydraulicBoundaryDatabase, long>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseWithoutSettings_SetsDefaultValuesAndLogsWarning()
        {
            // Setup
            const string hlcdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryDatabases();
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(assessmentSection);
            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(1);
            var lookup = new Dictionary<HydraulicBoundaryDatabase, long>();

            // Call
            void Call() => handler.Update(readHydraulicLocationConfigurationDatabase, lookup, hlcdFilePath);

            // Assert
            const string expectedMessage = "De tabel 'ScenarioInformation' in het HLCD bestand is niet aanwezig. Er worden standaardwaarden " +
                                           "conform WBI2017 gebruikt voor de HLCD bestandsinformatie.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);

            HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase = assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase;
            Assert.AreEqual(hlcdFilePath, hydraulicLocationConfigurationDatabase.FilePath);
            Assert.AreEqual("WBI2017", hydraulicLocationConfigurationDatabase.ScenarioName);
            Assert.AreEqual(2023, hydraulicLocationConfigurationDatabase.Year);
            Assert.AreEqual("WBI2017", hydraulicLocationConfigurationDatabase.Scope);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.SeaLevel);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.RiverDischarge);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.LakeLevel);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.WindDirection);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.WindSpeed);
            Assert.AreEqual("Gegenereerd door Riskeer (conform WBI2017)", hydraulicLocationConfigurationDatabase.Comment);
        }

        [Test]
        public void Update_ReadHydraulicLocationConfigurationDatabaseWithSettings_SetsExpectedValuesAndDoesNotLog()
        {
            // Setup
            const string hlcdFilePath = "some/file/path";
            AssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryDatabases();
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(assessmentSection);
            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = ReadHydraulicLocationConfigurationDatabaseTestFactory.CreateWithConfigurationSettings(1);
            var lookup = new Dictionary<HydraulicBoundaryDatabase, long>();

            // Call
            void Call() => handler.Update(readHydraulicLocationConfigurationDatabase, lookup, hlcdFilePath);

            // Assert
            TestHelper.AssertLogMessagesCount(Call, 0);

            HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase = assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase;
            ReadHydraulicLocationConfigurationSettings readSettings = readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationSettings.Single();
            Assert.AreEqual(hlcdFilePath, hydraulicLocationConfigurationDatabase.FilePath);
            Assert.AreEqual(readSettings.ScenarioName, hydraulicLocationConfigurationDatabase.ScenarioName);
            Assert.AreEqual(readSettings.Year, hydraulicLocationConfigurationDatabase.Year);
            Assert.AreEqual(readSettings.Scope, hydraulicLocationConfigurationDatabase.Scope);
            Assert.AreEqual(readSettings.SeaLevel, hydraulicLocationConfigurationDatabase.SeaLevel);
            Assert.AreEqual(readSettings.RiverDischarge, hydraulicLocationConfigurationDatabase.RiverDischarge);
            Assert.AreEqual(readSettings.LakeLevel, hydraulicLocationConfigurationDatabase.LakeLevel);
            Assert.AreEqual(readSettings.WindDirection, hydraulicLocationConfigurationDatabase.WindDirection);
            Assert.AreEqual(readSettings.WindSpeed, hydraulicLocationConfigurationDatabase.WindSpeed);
            Assert.AreEqual(readSettings.Comment, hydraulicLocationConfigurationDatabase.Comment);
        }

        [Test]
        public void Update_WithHydraulicBoundaryDatabasesInLookup_UpdatesHydraulicBoundaryDatabases()
        {
            // Setup
            const string hlcdFilePath = "some/file/path";
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(CreateAssessmentSectionWithHydraulicBoundaryDatabases());
            var hydraulicBoundaryDatabase1 = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryDatabase2 = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryDatabase3 = new HydraulicBoundaryDatabase();

            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase =
                ReadHydraulicLocationConfigurationDatabaseTestFactory.CreateWithConfigurationSettings(new Dictionary<long, bool>
                {
                    {
                        1, false
                    },
                    {
                        2, true
                    }
                });

            var lookup = new Dictionary<HydraulicBoundaryDatabase, long>
            {
                {
                    hydraulicBoundaryDatabase1, 1
                },
                {
                    hydraulicBoundaryDatabase2, 2
                },
                {
                    hydraulicBoundaryDatabase3, 3
                }
            };

            // Preconditions
            Assert.IsFalse(hydraulicBoundaryDatabase1.UsePreprocessorClosure);
            Assert.IsFalse(hydraulicBoundaryDatabase2.UsePreprocessorClosure);
            Assert.IsFalse(hydraulicBoundaryDatabase3.UsePreprocessorClosure);

            // Call
            handler.Update(readHydraulicLocationConfigurationDatabase, lookup, hlcdFilePath);

            // Assert
            Assert.IsFalse(hydraulicBoundaryDatabase1.UsePreprocessorClosure);
            Assert.IsTrue(hydraulicBoundaryDatabase2.UsePreprocessorClosure);
            Assert.IsFalse(hydraulicBoundaryDatabase3.UsePreprocessorClosure);
        }

        [Test]
        public void Update_DataUpdated_ReturnsChangedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryDatabases();
            var handler = new HydraulicLocationConfigurationDatabaseUpdateHandler(assessmentSection);
            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(1);
            var lookup = new Dictionary<HydraulicBoundaryDatabase, long>();

            // Call
            IEnumerable<IObservable> changedObjects = handler.Update(readHydraulicLocationConfigurationDatabase, lookup, "some/file/path");

            // Assert
            CollectionAssert.AreEqual(new Observable[]
            {
                assessmentSection.HydraulicBoundaryData,
                assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase
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
            IEnumerable<IObservable> changedObjects = handler.Update(
                ReadHydraulicLocationConfigurationDatabaseTestFactory.Create(1),
                new Dictionary<HydraulicBoundaryDatabase, long>(),
                "some/file/path");

            // Assert
            Assert.IsTrue(locations.All(l => !l.HasOutput));
            Assert.IsTrue(duneLocations.All(l => l.Output == null));
            Assert.IsTrue(calculationsWithOutput.All(c => !c.HasOutput));

            IEnumerable<IObservable> expectedChangedObjects = new IObservable[]
            {
                assessmentSection.HydraulicBoundaryData,
                assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase
            }.Concat(locations).Concat(duneLocations).Concat(calculationsWithOutput);

            CollectionAssert.AreEquivalent(expectedChangedObjects, changedObjects);
        }

        private static AssessmentSection CreateAssessmentSectionWithHydraulicBoundaryDatabases()
        {
            return new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryData =
                {
                    HydraulicBoundaryDatabases =
                    {
                        new HydraulicBoundaryDatabase()
                    }
                }
            };
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