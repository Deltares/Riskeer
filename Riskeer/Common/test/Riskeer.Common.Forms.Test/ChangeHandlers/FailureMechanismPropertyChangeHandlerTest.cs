// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class FailureMechanismPropertyChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var changeHandler = new FailureMechanismPropertyChangeHandler<ICalculatableFailureMechanism>();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismPropertyChangeHandler<ICalculatableFailureMechanism>>(changeHandler);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new FailureMechanismPropertyChangeHandler<ICalculatableFailureMechanism>();

            // Call
            void Call() => changeHandler.SetPropertyValueAfterConfirmation(null, 3, (f, v) => {});

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutValue_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new FailureMechanismPropertyChangeHandler<ICalculatableFailureMechanism>();

            // Call
            void Call() => changeHandler.SetPropertyValueAfterConfirmation<int?>(new TestCalculatableFailureMechanism(), null, (f, v) => {});

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutSetProperty_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new FailureMechanismPropertyChangeHandler<ICalculatableFailureMechanism>();

            // Call
            void Call() => changeHandler.SetPropertyValueAfterConfirmation(new TestCalculatableFailureMechanism(), 3, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("setValue", paramName);
        }

        [Test]
        [TestCaseSource(nameof(ChangePropertyTestCases))]
        public void SetPropertyValueAfterConfirmation_IfConfirmationRequiredThenGiven_SetValueCalledAffectedObjectsReturned(ChangePropertyTestCase testCase)
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

            var testFailureMechanism = new TestCalculatableFailureMechanism(testCase.Calculations);
            var propertySet = 0;

            var changeHandler = new FailureMechanismPropertyChangeHandler<ICalculatableFailureMechanism>();

            // Precondition
            Assert.IsTrue(testCase.ExpectedAffectedCalculations.All(c => c.HasOutput));

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                testFailureMechanism,
                3,
                (f, v) => propertySet++);

            // Assert
            if (dialogBoxWillBeShown)
            {
                Assert.AreEqual("Bevestigen", title);
                string expectedMessage = "Als u deze parameter wijzigt, zal de uitvoer van alle berekeningen in dit faalmechanisme verwijderd worden."
                                         + Environment.NewLine + Environment.NewLine +
                                         "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedMessage, message);
            }

            Assert.AreEqual(1, propertySet);
            var expectedAffectedObjects = new List<IObservable>(testCase.ExpectedAffectedCalculations)
            {
                testFailureMechanism
            };
            CollectionAssert.AreEqual(expectedAffectedObjects, affectedObjects);
            Assert.IsTrue(testCase.Calculations.All(c => !c.HasOutput));
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

            ICalculation calculationWithOutput = CalculationTestDataFactory.CreateCalculationWithOutput();
            ICalculation calculationWithoutOutput = CalculationTestDataFactory.CreateCalculationWithoutOutput();

            var testFailureMechanism = new TestCalculatableFailureMechanism(
                new[]
                {
                    calculationWithOutput,
                    calculationWithoutOutput
                });
            var propertySet = 0;

            var changeHandler = new FailureMechanismPropertyChangeHandler<ICalculatableFailureMechanism>();

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                testFailureMechanism,
                3,
                (f, v) => propertySet++);

            // Assert
            Assert.AreEqual(0, propertySet);
            CollectionAssert.IsEmpty(affectedObjects);
            Assert.IsTrue(calculationWithOutput.HasOutput);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationRequiredAndGivenExceptionInSetValue_ExceptionBubbled()
        {
            // Setup
            DialogBoxHandler = (name, wnd) =>
            {
                var tester = new MessageBoxTester(wnd);
                tester.ClickOk();
            };

            var testFailureMechanism = new TestCalculatableFailureMechanism(
                new[]
                {
                    CalculationTestDataFactory.CreateCalculationWithOutput()
                });

            var changeHandler = new FailureMechanismPropertyChangeHandler<ICalculatableFailureMechanism>();
            var expectedException = new Exception();

            // Call
            void Call() =>
                changeHandler.SetPropertyValueAfterConfirmation(testFailureMechanism, 3, (f, v) =>
                {
                    throw expectedException;
                });

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreSame(expectedException, exception);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationNotRequiredExceptionInSetValue_ExceptionBubbled()
        {
            // Setup
            var testFailureMechanism = new TestCalculatableFailureMechanism();
            var changeHandler = new FailureMechanismPropertyChangeHandler<ICalculatableFailureMechanism>();
            var expectedException = new Exception();

            // Call
            void Call() =>
                changeHandler.SetPropertyValueAfterConfirmation(testFailureMechanism, 3, (f, v) =>
                {
                    throw expectedException;
                });

            // Assert
            var exception = Assert.Throws<Exception>(Call);
            Assert.AreSame(expectedException, exception);
        }

        public class ChangePropertyTestCase
        {
            public ChangePropertyTestCase(IEnumerable<ICalculation> calculations)
            {
                Calculations = calculations;
                ExpectedAffectedCalculations = calculations.Where(c => c.HasOutput).ToArray();
            }

            public IEnumerable<ICalculation> Calculations { get; }
            public IEnumerable<ICalculation> ExpectedAffectedCalculations { get; }
        }

        private static IEnumerable ChangePropertyTestCases()
        {
            yield return new TestCaseData(
                new ChangePropertyTestCase(new TestCalculation[0])
            ).SetName("SetPropertyValueAfterConfirmation No calculations");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CalculationTestDataFactory.CreateCalculationWithOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Single calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CalculationTestDataFactory.CreateCalculationWithoutOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Single calculation without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CalculationTestDataFactory.CreateCalculationWithoutOutput(),
                    CalculationTestDataFactory.CreateCalculationWithoutOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Two calculations without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CalculationTestDataFactory.CreateCalculationWithOutput(),
                    CalculationTestDataFactory.CreateCalculationWithoutOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Calculation without and calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CalculationTestDataFactory.CreateCalculationWithOutput(),
                    CalculationTestDataFactory.CreateCalculationWithOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Two calculations with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                {
                    CalculationTestDataFactory.CreateCalculationWithOutput(),
                    CalculationTestDataFactory.CreateCalculationWithOutput(),
                    CalculationTestDataFactory.CreateCalculationWithoutOutput()
                })
            ).SetName("SetPropertyValueAfterConfirmation Two calculations with and one calculation without output");
        }
    }
}