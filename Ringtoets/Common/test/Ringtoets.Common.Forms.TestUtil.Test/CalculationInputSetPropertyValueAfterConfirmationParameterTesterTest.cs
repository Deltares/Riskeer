// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class CalculationInputSetPropertyValueAfterConfirmationParameterTesterTest
    {
        [Test]
        public void Constructructed_Always_PropertiesSet()
        {
            // Setup
            const double value = 3.0;
            var testCalculationInput = new TestCalculationInput();
            var returnedAffectedObjects = Enumerable.Empty<IObservable>();

            // Call
            var tester = new ObservableSetPropertyValueAfterConfirmationParameterTester<double>(
                testCalculationInput, value, returnedAffectedObjects);

            // Assert
            Assert.IsInstanceOf<IObservablePropertyChangeHandler>(tester);
            Assert.AreSame(testCalculationInput, tester.ExpectedCalculationInput);
            Assert.AreEqual(value, tester.ExpectedValue);
            Assert.AreSame(returnedAffectedObjects, tester.ReturnedAffectedObjects);
            Assert.IsFalse(tester.Called);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_CalculationInputPassedNotSame_ThrowsAssertionException()
        {
            // Setup
            const double value = 3.0;
            var testCalculationInput = new TestCalculationInput();
            var passedCalculationInput = new TestCalculationInput();
            var returnedAffectedObjects = Enumerable.Empty<IObservable>();

            var tester = new ObservableSetPropertyValueAfterConfirmationParameterTester<double>(
                testCalculationInput, value, returnedAffectedObjects);

            // Call
            TestDelegate test = () => tester.SetPropertyValueAfterConfirmation(passedCalculationInput, value, (input, v) => { });

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
            var testCalculationInput = new TestCalculationInput();
            var returnedAffectedObjects = Enumerable.Empty<IObservable>();

            var tester = new ObservableSetPropertyValueAfterConfirmationParameterTester<double>(
                testCalculationInput, expectedValue, returnedAffectedObjects);

            // Call
            TestDelegate test = () => tester.SetPropertyValueAfterConfirmation(testCalculationInput, passedValue, (input, v) => { });

            // Assert
            Assert.Throws<AssertionException>(test);
            Assert.IsTrue(tester.Called);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ParametersAreSameAndEqual_SetValueCalledReturnsGivenAffectedObjects()
        {
            // Setup
            const double value = 3.0;

            var returnedAffectedObjects = Enumerable.Empty<IObservable>();
            var testCalculationInput = new TestCalculationInput();
            var called = 0;

            var tester = new ObservableSetPropertyValueAfterConfirmationParameterTester<double>(
                testCalculationInput, value, returnedAffectedObjects);

            // Call
            var affectedObjects = tester.SetPropertyValueAfterConfirmation(testCalculationInput, value, (m, v) => called++);

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

            var returnedAffectedObjects = Enumerable.Empty<IObservable>();
            var testCalculationInput = new TestCalculationInput();

             var tester = new ObservableSetPropertyValueAfterConfirmationParameterTester<double>(
                testCalculationInput, value, returnedAffectedObjects);

            var expectedException = new Exception();

            // Call
            TestDelegate test = () => tester.SetPropertyValueAfterConfirmation(testCalculationInput, value, (input, v) => { throw expectedException; });

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreSame(expectedException, exception);
            Assert.IsTrue(tester.Called);
        }
    }
}