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
using System.Linq;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class FailureMechanismPropertyChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new FailureMechanismPropertyChangeHandler<IFailureMechanism>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation<int>(
                null,
                3,
                (f, v) => { });

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutValue_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new FailureMechanismPropertyChangeHandler<IFailureMechanism>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation<int?>(
                new TestFailureMechanism(),
                null,
                (f, v) => { });

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutSetProperty_ThrowsArgumentNullException()
        {
            // Setup
            var changeHandler = new FailureMechanismPropertyChangeHandler<IFailureMechanism>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(
                new TestFailureMechanism(),
                3,
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("setValue", paramName);
        }

        [Test]
        [TestCaseSource(nameof(ChangePropertyTestCases))]
        public void SetPropertyValueAfterConfirmation_IfConfirmationRequiredThenGiven_SetValueCalledAffectedObjectsReturned(ChangePropertyTestCase testCase)
        {
            // Setup
            var dialogBoxWillBeShown = testCase.ExpectedAffectedCalculations.Any();

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

            var testFailureMechanism = new TestFailureMechanism(testCase.Calculations);
            var propertySet = 0;

            var changeHandler = new FailureMechanismPropertyChangeHandler<IFailureMechanism>();
            
            // Call
            var affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                testFailureMechanism,
                3,
                (f, v) => propertySet++);

            // Assert
            if (dialogBoxWillBeShown)
            {
                Assert.AreEqual("Bevestigen", title);
                string expectedMessage = "Als u een parameter in een toetsspoor wijzigt, zal de uitvoer van alle berekeningen in dit toetsspoor verwijderd worden." + Environment.NewLine +
                                         Environment.NewLine +
                                         "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedMessage, message);
            }
            Assert.AreEqual(1, propertySet);
            var expectedAffectedObjects = new List<IObservable>(testCase.ExpectedAffectedCalculations);
            expectedAffectedObjects.Add(testFailureMechanism);
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

            var calculationWithOutput = CreateCalculationWithOutput();
            var calculationWithoutOutput = CreateCalculationWithoutOutput();

            var testFailureMechanism = new TestFailureMechanism(
                new[]
                {
                    calculationWithOutput,
                    calculationWithoutOutput
                });
            var propertySet = 0;

            var changeHandler = new FailureMechanismPropertyChangeHandler<IFailureMechanism>();
            
            // Call
            var affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                testFailureMechanism,
                3,
                (f, v) => propertySet++);

            // Assert
            Assert.AreEqual(0, propertySet);
            CollectionAssert.IsEmpty(affectedObjects);
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

            var testFailureMechanism = new TestFailureMechanism(
                new[]
                {
                    CreateCalculationWithOutput(),
                });

            var changeHandler = new FailureMechanismPropertyChangeHandler<IFailureMechanism>();
            var expectedException = new Exception();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(
                testFailureMechanism,
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
            var testFailureMechanism = new TestFailureMechanism();
            var changeHandler = new FailureMechanismPropertyChangeHandler<IFailureMechanism>();
            var expectedException = new Exception();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(
                testFailureMechanism,
                3,
                (f, v) => { throw expectedException; });

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreSame(expectedException, exception);
        }

        public class ChangePropertyTestCase
        {
            public ChangePropertyTestCase(ICollection<TestCalculation> calculations)
            {
                Calculations = calculations;
                ExpectedAffectedCalculations = calculations.Where(c => c.HasOutput).ToArray();
            }

            public ICollection<TestCalculation> Calculations { get; private set; }
            public ICollection<TestCalculation> ExpectedAffectedCalculations { get; private set; }
        }

        static IEnumerable ChangePropertyTestCases()
        {
            yield return new TestCaseData(
                new ChangePropertyTestCase(new TestCalculation[0])

            ).SetName("SetPropertyValueAfterConfirmation No calculations");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithOutput()
                             })
            ).SetName("SetPropertyValueAfterConfirmation Single calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithoutOutput()
                             })
            ).SetName("SetPropertyValueAfterConfirmation Single calculation without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithoutOutput(),
                                 CreateCalculationWithoutOutput()
                             })
            ).SetName("SetPropertyValueAfterConfirmation Two calculations without output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithOutput(),
                                 CreateCalculationWithoutOutput()
                             })
            ).SetName("SetPropertyValueAfterConfirmation Calculation without and calculation with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithOutput(),
                                 CreateCalculationWithOutput()
                             })
            ).SetName("SetPropertyValueAfterConfirmation Two calculations with output");

            yield return new TestCaseData(
                new ChangePropertyTestCase(new[]
                             {
                                 CreateCalculationWithOutput(),
                                 CreateCalculationWithOutput(),
                                 CreateCalculationWithoutOutput()
                             })
            ).SetName("SetPropertyValueAfterConfirmation Two calculations with and one calculation without output");
        }

        private static TestCalculation CreateCalculationWithoutOutput()
        {
            return new TestCalculation();
        }

        private static TestCalculation CreateCalculationWithOutput()
        {
            return new TestCalculation
            {
                Output = new object()
            };
        }
    }
}