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
using System.Collections;
using System.Collections.Generic;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ObservablePropertyChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_WithCalculation_Expectedvalues()
        {
            // Call
            var changeHandler = new ObservablePropertyChangeHandler(new TestCalculation(), new TestCalculationInput());

            // Assert
            Assert.IsInstanceOf<IObservablePropertyChangeHandler>(changeHandler);
        }

        [Test]
        public void Constructor_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ObservablePropertyChangeHandler(null, new TestCalculationInput());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationInputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ObservablePropertyChangeHandler(new TestCalculation(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationInput", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_SetValueNull_ThrowArgumentNullException()
        {
            // Setup
            var changeHandler = new ObservablePropertyChangeHandler(new TestCalculation(), new TestCalculationInput());

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setValue", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(ChangePropertyTestCases))]
        public void SetPropertyValueAfterConfirmation_IfConfirmationRequiredThenGiven_SetValueCalledAffectedObjectsReturned(ChangePropertyTestCase testCase)
        {
            // Setup
            bool dialogBoxWillBeShown = testCase.Calculation.HasOutput;

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

            var calculationInput = new TestCalculationInput();
            var propertySet = 0;

            var changeHandler = new ObservablePropertyChangeHandler(testCase.Calculation, calculationInput);

            // Precondition
            Assert.AreEqual(dialogBoxWillBeShown, testCase.Calculation.HasOutput);

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(() => propertySet++);

            // Assert
            var expectedAffectedObjects = new List<IObservable>();

            if (dialogBoxWillBeShown)
            {
                Assert.AreEqual("Bevestigen", title);
                string expectedMessage = "Als u een parameter in deze berekening wijzigt, zal de uitvoer van deze berekening verwijderd worden."
                                         + Environment.NewLine + Environment.NewLine +
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

            ICalculation calculation = CalculationTestDataFactory.CreateCalculationWithOutput();

            var propertySet = 0;

            var changeHandler = new ObservablePropertyChangeHandler(calculation, new TestCalculationInput());

            // Precondition
            Assert.IsTrue(calculation.HasOutput);

            // Call
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(() => propertySet++);

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

            ICalculation calculation = CalculationTestDataFactory.CreateCalculationWithOutput();

            var changeHandler = new ObservablePropertyChangeHandler(calculation, new TestCalculationInput());
            var expectedException = new Exception();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(() => { throw expectedException; });

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreSame(expectedException, exception);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationNotRequiredExceptionInSetValue_ExceptionBubbled()
        {
            // Setup
            ICalculation calculation = CalculationTestDataFactory.CreateCalculationWithoutOutput();

            var changeHandler = new ObservablePropertyChangeHandler(calculation, new TestCalculationInput());
            var expectedException = new Exception();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(() => { throw expectedException; });

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreSame(expectedException, exception);
        }

        public class ChangePropertyTestCase
        {
            public ChangePropertyTestCase(ICalculation calculation)
            {
                Calculation = calculation;
            }

            public ICalculation Calculation { get; }
        }

        private static IEnumerable ChangePropertyTestCases()
        {
            yield return new TestCaseData(
                new ChangePropertyTestCase(CalculationTestDataFactory.CreateCalculationWithOutput())
            ).SetName("SetPropertyValueAfterConfirmation calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(CalculationTestDataFactory.CreateCalculationWithoutOutput())
            ).SetName("SetPropertyValueAfterConfirmation calculation without output");
        }
    }
}