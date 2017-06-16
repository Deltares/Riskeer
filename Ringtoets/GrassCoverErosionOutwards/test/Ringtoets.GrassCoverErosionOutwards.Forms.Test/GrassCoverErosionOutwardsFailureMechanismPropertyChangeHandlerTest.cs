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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test
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
            bool dialogBoxWillBeShown = testCase.ExpectedAffectedCalculations.Any() || testCase.ExpectedAffectedLocations.Any();

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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in testCase.Calculations)
            {
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            }
            failureMechanism.HydraulicBoundaryLocations.AddRange(testCase.Locations);

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
                string expectedMessage = "Als u een parameter in een toetsspoor wijzigt, zal de uitvoer van alle randvoorwaarden locaties en berekeningen in dit toetsspoor verwijderd worden." + Environment.NewLine +
                                         Environment.NewLine +
                                         "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedMessage, message);
            }
            Assert.AreEqual(1, propertySet);
            var expectedAffectedObjects = new List<IObservable>(testCase.ExpectedAffectedCalculations);
            if (testCase.ExpectedAffectedLocations.Any())
            {
                expectedAffectedObjects.Add(failureMechanism.HydraulicBoundaryLocations);
            }
            expectedAffectedObjects.AddRange(testCase.ExpectedAffectedLocations);
            expectedAffectedObjects.Add(failureMechanism);
            CollectionAssert.AreEqual(expectedAffectedObjects, affectedObjects);
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
            public ChangePropertyTestCase(ICollection<HydraulicBoundaryLocation> locations, ICollection<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations)
            {
                Calculations = calculations;
                ExpectedAffectedCalculations = calculations.Where(c => c.HasOutput).ToArray();
                Locations = locations;
                ExpectedAffectedLocations = locations.Where(c => c.DesignWaterLevelCalculation.HasOutput || c.WaveHeightCalculation.HasOutput).ToArray();
            }

            public ICollection<HydraulicBoundaryLocation> Locations { get; }
            public ICollection<IObservable> ExpectedAffectedLocations { get; }

            public ICollection<GrassCoverErosionOutwardsWaveConditionsCalculation> Calculations { get; }

            public ICollection<IObservable> ExpectedAffectedCalculations { get; }
        }

        static IEnumerable ChangePropertyTestCases()
        {
            yield return new TestCaseData(
                new ChangePropertyTestCase(new TestHydraulicBoundaryLocation[0],
                                           new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation No locations, no calculations");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CreateHydraulicBoundaryLocationWithoutOutput()
                }, new GrassCoverErosionOutwardsWaveConditionsCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation Single location without output, no calculations");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new TestHydraulicBoundaryLocation[0],
                                           new[]
                                           {
                                               CreateCalculationWithOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Calculation with output, no locations");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CreateHydraulicBoundaryLocationWithoutOutput()
                }, new[]
                {
                    CreateCalculationWithOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Single location without output, calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Single location with output, single calculation without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithoutOutput(),
                                               CreateHydraulicBoundaryLocationWithoutOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithoutOutput(),
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Two locations without output, calculations without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithOutput(),
                                               CreateHydraulicBoundaryLocationWithoutOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithOutput(),
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Location with, location without, calculation without and calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithOutput(),
                                               CreateHydraulicBoundaryLocationWithOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithOutput(),
                                               CreateCalculationWithOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Two locations with output, two calculations with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                                           {
                                               CreateHydraulicBoundaryLocationWithOutput(),
                                               CreateHydraulicBoundaryLocationWithoutOutput(),
                                               CreateHydraulicBoundaryLocationWithOutput()
                                           },
                                           new[]
                                           {
                                               CreateCalculationWithOutput(),
                                               CreateCalculationWithOutput(),
                                               CreateCalculationWithoutOutput()
                                           })
            ).SetName("SetPropertyValueAfterConfirmation Two locations with, one location without, two calculations with and one calculation without output");
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateCalculationWithoutOutput()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation();
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateCalculationWithOutput()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
        }

        private static TestHydraulicBoundaryLocation CreateHydraulicBoundaryLocationWithoutOutput()
        {
            return new TestHydraulicBoundaryLocation();
        }

        private static TestHydraulicBoundaryLocation CreateHydraulicBoundaryLocationWithOutput()
        {
            return new TestHydraulicBoundaryLocation
            {
                WaveHeightCalculation =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(0.5)
                },
                DesignWaterLevelCalculation =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(2.3)
                }
            };
        }
    }
}