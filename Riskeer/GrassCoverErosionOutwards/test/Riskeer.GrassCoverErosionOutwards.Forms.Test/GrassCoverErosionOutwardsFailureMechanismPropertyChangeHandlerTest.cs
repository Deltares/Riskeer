// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(
                null,
                3,
                (f, v) => {});

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutValue_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation<int?>(
                new GrassCoverErosionOutwardsFailureMechanism(),
                null,
                (f, v) => {});

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutSetProperty_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(
                new GrassCoverErosionOutwardsFailureMechanism(),
                3,
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("setValue", paramName);
        }

        [Test]
        [TestCaseSource(nameof(ChangePropertyTestCases))]
        public void SetPropertyValueAfterConfirmation_IfConfirmationRequiredThenGiven_MessageDialogShownSetValueCalledAffectedObjectsReturned(ChangePropertyTestCase testCase)
        {
            // Setup
            bool dialogBoxWillBeShown = testCase.ExpectedAffectedObjects.Any();

            var title = "";
            var message = "";
            if (dialogBoxWillBeShown)
            {
                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new MessageBoxTester(wnd);
                    title = tester.Title;
                    message = tester.Text;

                    tester.ClickOk();
                };
            }

            GrassCoverErosionOutwardsFailureMechanism failureMechanism = testCase.FailureMechanism;
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in testCase.Calculations)
            {
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            }

            var propertySet = 0;

            var changeHandler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                failureMechanism,
                3,
                (f, v) => propertySet++);

            // Assert
            if (dialogBoxWillBeShown)
            {
                Assert.AreEqual("Bevestigen", title);
                string expectedMessage = "Als u deze parameter wijzigt, zal de uitvoer van alle berekeningen in dit toetsspoor verwijderd worden." + Environment.NewLine +
                                         Environment.NewLine +
                                         "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedMessage, message);
            }

            Assert.AreEqual(1, propertySet);

            var expectedAffectedObjects = new List<IObservable>
            {
                failureMechanism
            };
            expectedAffectedObjects.AddRange(testCase.ExpectedAffectedObjects);
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationRequiredButNotGiven_SetValueNotCalledNoAffectedObjectsReturned()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickCancel();
            };

            GrassCoverErosionOutwardsWaveConditionsCalculation calculationWithOutput = CreateCalculationWithOutput();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationWithoutOutput = CreateCalculationWithoutOutput();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutput);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithoutOutput);

            var propertySet = 0;

            var changeHandler = new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                failureMechanism,
                3,
                (f, v) => propertySet++);

            // Assert
            Assert.AreEqual(0, propertySet);
            CollectionAssert.IsEmpty(affectedObjects);
        }

        public class ChangePropertyTestCase
        {
            public ChangePropertyTestCase(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                          IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations)
            {
                FailureMechanism = failureMechanism;
                Calculations = calculations;
                ExpectedAffectedObjects = calculations.Where(c => c.HasOutput).Cast<IObservable>()
                                                      .Concat(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                                      .Concat(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                                      .Concat(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                                      .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                                      .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                                      .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Where(HasHydraulicBoundaryLocationCalculationOutput))
                                                      .ToArray();
            }

            public GrassCoverErosionOutwardsFailureMechanism FailureMechanism { get; }

            public IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> Calculations { get; }

            public IEnumerable<IObservable> ExpectedAffectedObjects { get; }

            private static bool HasHydraulicBoundaryLocationCalculationOutput(HydraulicBoundaryLocationCalculation calculation)
            {
                return calculation.HasOutput;
            }
        }

        private static IEnumerable<TestCaseData> ChangePropertyTestCases()
        {
            yield return new TestCaseData(
                new ChangePropertyTestCase(new GrassCoverErosionOutwardsFailureMechanism(),
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation No locations, no calculations");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation Single location all without output, no calculations");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation Single location all with output, no calculations");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new GrassCoverErosionOutwardsFailureMechanism(),
                                           new[]
                                           {
                                               CreateCalculationWithOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Calculation with output, no locations");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism, new[]
                {
                    CreateCalculationWithOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Single location all without output, calculation with output");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new[]
                                           {
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Single location all with output, single calculation without output");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new[]
                                           {
                                               CreateCalculationWithoutOutput(),
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Single location all without output, calculations without output");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new[]
                                           {
                                               CreateCalculationWithOutput(),
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Single location all without output, calculation without and calculation with output");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation Single location water level mechanism specific factorized signaling norm with output, no calculations");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation Single location water level mechanism specific signaling norm with output, no calculations");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation Single location water level mechanism specific lower limit norm with output, no calculations");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation Single location wave height mechanism specific factorized signaling norm with output, no calculations");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation Single location wave height mechanism specific signaling norm with output, no calculations");

            failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(failureMechanism);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
            yield return new TestCaseData(
                new ChangePropertyTestCase(failureMechanism,
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation Single location wave height mechanism specific lower limit norm with output, no calculations");
        }

        private static void ConfigureFailureMechanismHydraulicBoundaryLocationCalculations(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            var hydraulicBoundaryLocations = new[]
            {
                new TestHydraulicBoundaryLocation()
            };

            failureMechanism.AddHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
        }

        private static void SetHydraulicBoundaryLocationCalculationOutput(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            SetHydraulicBoundaryLocationCalculationOutput(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
        }

        private static void SetHydraulicBoundaryLocationCalculationOutput(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            calculations.First().Output = new TestHydraulicBoundaryLocationCalculationOutput();
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateCalculationWithoutOutput()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation();
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateCalculationWithOutput()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create()
            };
        }
    }
}