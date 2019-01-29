// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;

namespace Riskeer.DuneErosion.Forms.Test
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
                string expectedMessage = "Als u deze parameter wijzigt, zal de uitvoer van alle berekeningen in dit toetsspoor verwijderd worden." + Environment.NewLine +
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
            ConfigureFailureMechanismWithOneDuneLocation(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm);

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
            ConfigureFailureMechanismWithOneDuneLocation(failureMechanismOneLocationWithoutOutput);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanismOneLocationWithoutOutput))
                .SetName("SetPropertyValueAfterConfirmation One location and all calculations without output");

            var failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanismWithOneDuneLocation(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation mechanism specific factorized signaling norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanismWithOneDuneLocation(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation mechanism specific signaling norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanismWithOneDuneLocation(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation mechanism specific lower limit norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanismWithOneDuneLocation(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForLowerLimitNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation lower limit norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanismWithOneDuneLocation(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForFactorizedLowerLimitNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and calculation factorized lower limit norm with output");

            failureMechanism = new DuneErosionFailureMechanism();
            ConfigureFailureMechanismWithOneDuneLocation(failureMechanism);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm);
            SetCalculationOutput(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm);
            SetCalculationOutput(failureMechanism.CalculationsForLowerLimitNorm);
            SetCalculationOutput(failureMechanism.CalculationsForFactorizedLowerLimitNorm);
            yield return new TestCaseData(new ChangePropertyFailureMechanismTestCase(failureMechanism))
                .SetName("SetPropertyValueAfterConfirmation One location and all calculations with output");
        }

        private static void ConfigureFailureMechanismWithOneDuneLocation(DuneErosionFailureMechanism failureMechanism)
        {
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation()
            });
        }

        private static void SetCalculationOutput(IEnumerable<DuneLocationCalculation> calculations)
        {
            calculations.First().Output = new TestDuneLocationCalculationOutput();
        }
    }
}