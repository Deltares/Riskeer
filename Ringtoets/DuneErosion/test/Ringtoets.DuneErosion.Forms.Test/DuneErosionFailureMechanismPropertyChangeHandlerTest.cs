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
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;

namespace Ringtoets.DuneErosion.Forms.Test
{
    [TestFixture]
    public class DuneErosionFailureMechanismPropertyChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new DuneErosionFailureMechanismPropertyChangeHandler();

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
            var changeHandler = new DuneErosionFailureMechanismPropertyChangeHandler();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation<int?>(
                new DuneErosionFailureMechanism(),
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
            var changeHandler = new DuneErosionFailureMechanismPropertyChangeHandler();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(
                new DuneErosionFailureMechanism(),
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
            bool dialogBoxWillBeShown = testCase.ExpectedAffectedLocations.Any();

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

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocations.AddRange(testCase.Locations);

            var propertySet = 0;

            var changeHandler = new DuneErosionFailureMechanismPropertyChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                failureMechanism,
                3,
                (f, v) => propertySet++);

            // Assert
            if (dialogBoxWillBeShown)
            {
                Assert.AreEqual("Bevestigen", title);
                string expectedMessage = "Als u deze parameter wijzigt, zal de uitvoer van alle randvoorwaarden locaties in dit toetsspoor verwijderd worden." + Environment.NewLine +
                                         Environment.NewLine +
                                         "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedMessage, message);
            }

            Assert.AreEqual(1, propertySet);
            var expectedAffectedObjects = new List<IObservable>(new[]
            {
                failureMechanism
            });
            expectedAffectedObjects.AddRange(testCase.ExpectedAffectedLocations);

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        [TestCaseSource(nameof(ChangePropertyFailureMechanismTestCases))]
        public void SetPropertyValueAfterConfirmation_IfConfirmationRequiredThenGiven_MessageDialogShownSetValueCalledAffectedObjectsReturned(ChangePropertyFailureMechanismTestCase testCase)
        {
            // Setup
            bool dialogBoxWillBeShown = testCase.ExpectedAffectedCalculations.Any();

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

            DuneErosionFailureMechanism failureMechanism = testCase.FailureMechanism;
            var propertySet = 0;

            var changeHandler = new DuneErosionFailureMechanismPropertyChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                failureMechanism,
                3,
                (f, v) => propertySet++);

            // Assert
            if (dialogBoxWillBeShown)
            {
                Assert.AreEqual("Bevestigen", title);
                string expectedMessage = "Als u deze parameter wijzigt, zal de uitvoer van alle randvoorwaarden locaties in dit toetsspoor verwijderd worden." + Environment.NewLine +
                                         Environment.NewLine +
                                         "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedMessage, message);
            }

            Assert.AreEqual(1, propertySet);
            var expectedAffectedObjects = new List<IObservable>(new[]
            {
                failureMechanism
            });
            expectedAffectedObjects.AddRange(testCase.ExpectedAffectedCalculations);

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

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocations.Add(CreateDuneLocationWithoutOutput());
            failureMechanism.DuneLocations.Add(CreateDuneLocationWithOutput());

            var propertySet = 0;

            var changeHandler = new DuneErosionFailureMechanismPropertyChangeHandler();

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
            public ChangePropertyTestCase(ICollection<DuneLocation> locations)
            {
                Locations = locations;
                ExpectedAffectedLocations = locations.Where(c => c.Calculation.Output != null).ToArray();
            }

            public ICollection<DuneLocation> Locations { get; }
            public ICollection<IObservable> ExpectedAffectedLocations { get; }
        }

        public class ChangePropertyFailureMechanismTestCase
        {
            public ChangePropertyFailureMechanismTestCase(DuneErosionFailureMechanism failureMechanism)
            {
                FailureMechanism = failureMechanism;

                ExpectedAffectedCalculations = failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.Where(HasOutput)
                                                               .Concat(failureMechanism.CalculationsForMechanismSpecificSignalingNorm.Where(HasOutput))
                                                               .Concat(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.Where(HasOutput))
                                                               .Concat(failureMechanism.CalculationsForLowerLimitNorm.Where(HasOutput))
                                                               .Concat(failureMechanism.CalculationsForFactorizedLowerLimitNorm.Where(HasOutput))
                                                               .ToArray();
            }

            public DuneErosionFailureMechanism FailureMechanism { get; }

            public IEnumerable<IObservable> ExpectedAffectedCalculations { get; }

            private static bool HasOutput(DuneLocationCalculation calculation)
            {
                return calculation.Output != null;
            }
        }

        private static IEnumerable<TestCaseData> ChangePropertyFailureMechanismTestCases()
        {
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(new DuneErosionFailureMechanism()))
                .SetName("SetPropertyValueAfterConfirmation No Calculations");

            var failureMechanismOneLocationWithoutOutput = new DuneErosionFailureMechanism();
            ConfigureFailureMechanism(failureMechanismOneLocationWithoutOutput);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanismOneLocationWithoutOutput))
                .SetName("SetPropertyValueAfterConfirmation One location and all calculations without output");

            var failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation failure mechanism specific factorized signaling norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation mechanism specific signaling norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation mechanism specific lower limit norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForLowerLimitNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation lower limit norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForFactorizedLowerLimitNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation factorized lower limit norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanism(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm);
            SetCalculationOutput(failureMechanism.CalculationsForLowerLimitNorm);
            SetCalculationOutput(failureMechanism.CalculationsForFactorizedLowerLimitNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and all calculations with output");
        }

        private static void ConfigureFailureMechanism(DuneErosionFailureMechanism failureMechanism)
        {
            var duneLocations = new[]
            {
                new TestDuneLocation()
            };

            failureMechanism.DuneLocations.AddRange(duneLocations);
            failureMechanism.SetDuneLocationCalculations(duneLocations);
        }

        private static void SetCalculationOutput(IEnumerable<DuneLocationCalculation> calculations)
        {
            var output = new TestDuneLocationOutput();
            calculations.First().Output = output;
        }

        private static IEnumerable ChangePropertyTestCases()
        {
            yield return new TestCaseData(
                new ChangePropertyTestCase(new TestDuneLocation[0])
            ).SetName("SetPropertyValueAfterConfirmation No locations");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CreateDuneLocationWithoutOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Single location without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CreateDuneLocationWithOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Single location with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CreateDuneLocationWithoutOutput(),
                    CreateDuneLocationWithoutOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Two locations without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CreateDuneLocationWithOutput(),
                    CreateDuneLocationWithoutOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Location with and location without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CreateDuneLocationWithOutput(),
                    CreateDuneLocationWithOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Two locations with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CreateDuneLocationWithOutput(),
                    CreateDuneLocationWithoutOutput(),
                    CreateDuneLocationWithOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Two locations with and one location without output");
        }

        private static DuneLocation CreateDuneLocationWithoutOutput()
        {
            return new TestDuneLocation();
        }

        private static DuneLocation CreateDuneLocationWithOutput()
        {
            return new TestDuneLocation
            {
                Calculation =
                {
                    Output = new TestDuneLocationOutput()
                }
            };
        }
    }
}