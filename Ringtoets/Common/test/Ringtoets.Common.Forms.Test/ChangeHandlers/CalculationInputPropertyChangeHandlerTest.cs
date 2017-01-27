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
using System.Collections;
using System.Collections.Generic;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class CalculationInputPropertyChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_Expectedvalues()
        {
            // Call
            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Assert
            Assert.IsInstanceOf<ICalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>>(changeHandler);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_CalculationInputNull_ThrowArgumentNullException()
        {
            // Setup
            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(null, new TestCalculation(), 3, (input, value) => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationInput", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(new TestCalculationInput(), null, 3, (input, value) => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_SetValueNull_ThrowArgumentNullException()
        {
            // Setup
            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(new TestCalculationInput(),
                                                                                      new TestCalculation(),
                                                                                      3,
                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setValue", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(ChangePropertyTestCases))]
        public void SetPropertyValueAfterConfirmation_IfConfirmationRequiredThenGiven_SetValueCalledAffectedObjectsReturned(ChangePropertyTestCase testCase)
        {
            // Setup
            var dialogBoxWillBeShown = testCase.Calculation.HasOutput;

            string title = "";
            string message = "";
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

            var calculationInput = new TestCalculationInput();
            var propertySet = 0;

            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Precondition
            Assert.AreEqual(dialogBoxWillBeShown, testCase.Calculation.HasOutput);

            // Call
            var affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                calculationInput,
                testCase.Calculation,
                3,
                (f, v) => propertySet++);

            // Assert
            var expectedAffectedObjects = new List<IObservable>();

            if (dialogBoxWillBeShown)
            {
                Assert.AreEqual("Bevestigen", title);
                string expectedMessage = "Als u een parameter in deze berekening wijzigt, zal de uitvoer van deze berekening verwijderd worden." + Environment.NewLine +
                                         Environment.NewLine +
                                         "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedMessage, message);

                expectedAffectedObjects.Add(testCase.Calculation);
            }
            expectedAffectedObjects.Add(calculationInput);
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
            Assert.AreEqual(1, propertySet);
            Assert.IsFalse(testCase.Calculation.HasOutput);
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

            var calculation = CalculationTestHelper.CreateCalculationWithOutput();

            var propertySet = 0;

            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Precondition
            Assert.IsTrue(calculation.HasOutput);

            // Call
            var affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                new TestCalculationInput(),
                calculation,
                3,
                (f, v) => propertySet++);

            // Assert
            Assert.AreEqual(0, propertySet);
            CollectionAssert.IsEmpty(affectedObjects);
            Assert.IsTrue(calculation.HasOutput);
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

            var calculation = CalculationTestHelper.CreateCalculationWithOutput();

            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();
            var expectedException = new Exception();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(
                new TestCalculationInput(),
                calculation,
                3,
                (f, v) => { throw expectedException; });

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreSame(expectedException, exception);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationNotRequiredExceptionInSetValue_ExceptionBubbled()
        {
            // Setup
            var calculation = CalculationTestHelper.CreateCalculationWithoutOutput();

            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();
            var expectedException = new Exception();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(
                new TestCalculationInput(),
                calculation,
                3,
                (f, v) => { throw expectedException; });

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreSame(expectedException, exception);
        }

        public class ChangePropertyTestCase
        {
            public ChangePropertyTestCase(TestCalculation calculation)
            {
                Calculation = calculation;
            }

            public TestCalculation Calculation { get; }
        }

        private static IEnumerable ChangePropertyTestCases()
        {
            yield return new TestCaseData(
                new ChangePropertyTestCase(CalculationTestHelper.CreateCalculationWithOutput())
            ).SetName("SetPropertyValueAfterConfirmation calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(CalculationTestHelper.CreateCalculationWithoutOutput())
            ).SetName("SetPropertyValueAfterConfirmation calculation without output");
        }
    }
}