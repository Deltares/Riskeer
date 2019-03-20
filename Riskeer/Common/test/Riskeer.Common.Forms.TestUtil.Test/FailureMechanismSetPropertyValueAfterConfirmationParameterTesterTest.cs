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
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class FailureMechanismSetPropertyValueAfterConfirmationParameterTesterTest
    {
        [Test]
        public void Constructructed_Always_PropertiesSet()
        {
            // Setup
            const double value = 3.0;
            var testFailureMechanism = new TestFailureMechanism();
            IEnumerable<IObservable> returnedAffectedObjects = Enumerable.Empty<IObservable>();

            // Call
            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism, double?>(
                testFailureMechanism, value, returnedAffectedObjects);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismPropertyChangeHandler<IFailureMechanism>>(tester);
            Assert.AreSame(testFailureMechanism, tester.ExpectedFailureMechanism);
            Assert.AreEqual(value, tester.ExpectedValue);
            Assert.AreSame(returnedAffectedObjects, tester.ReturnedAffectedObjects);
            Assert.IsFalse(tester.Called);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_FailureMechanismPassedNotSame_ThrowsAssertionException()
        {
            // Setup
            const double value = 3.0;

            var expectedFailureMechanism = new TestFailureMechanism();
            var passedFailureMechanism = new TestFailureMechanism();

            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism, double?>(
                expectedFailureMechanism, value, Enumerable.Empty<IObservable>());

            // Call
            TestDelegate test = () => tester.SetPropertyValueAfterConfirmation(passedFailureMechanism, value, (m, v) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
            Assert.IsTrue(tester.Called);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ValuePassedNotEqual_ThrowsAssertionException()
        {
            // Setup
            const double expectedValue = 3.0;
            const double passedValue = 2.0;

            var expectedFailureMechanism = new TestFailureMechanism();

            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism, double?>(
                expectedFailureMechanism, expectedValue, Enumerable.Empty<IObservable>());

            // Call
            TestDelegate test = () => tester.SetPropertyValueAfterConfirmation(expectedFailureMechanism, passedValue, (m, v) => {});

            // Assert
            Assert.Throws<AssertionException>(test);
            Assert.IsTrue(tester.Called);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ParametersAreSameAndEqual_SetValueCalledReturnsGivenAffectedObjects()
        {
            // Setup
            const double value = 3.0;

            IEnumerable<IObservable> returnedAffectedObjects = Enumerable.Empty<IObservable>();
            var testFailureMechanism = new TestFailureMechanism();
            var called = 0;

            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism, double?>(
                testFailureMechanism, value, returnedAffectedObjects);

            // Call
            IEnumerable<IObservable> affectedObjects = tester.SetPropertyValueAfterConfirmation(testFailureMechanism, value, (m, v) => called++);

            // Assert
            Assert.AreEqual(1, called);
            Assert.AreSame(returnedAffectedObjects, affectedObjects);
            Assert.IsTrue(tester.Called);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_SetValueThrowsException_BubblesException()
        {
            // Setup
            const double value = 3.0;
            var testFailureMechanism = new TestFailureMechanism();

            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism, double?>(
                testFailureMechanism, value, Enumerable.Empty<IObservable>());

            var expectedException = new Exception();

            // Call
            TestDelegate test = () => tester.SetPropertyValueAfterConfirmation(testFailureMechanism, value, (m, v) =>
            {
                throw expectedException;
            });

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreSame(expectedException, exception);
            Assert.IsTrue(tester.Called);
        }
    }
}