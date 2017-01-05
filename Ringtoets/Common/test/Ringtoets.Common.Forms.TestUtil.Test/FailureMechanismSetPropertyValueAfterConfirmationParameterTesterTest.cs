using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class FailureMechanismSetPropertyValueAfterConfirmationParameterTesterTest
    {
        [Test]
        public void Constructructed_Always_PropertiesSet()
        {
            // Setup
            var testFailureMechanism = new TestFailureMechanism();
            var value = 3.0;
            var returnedAffectedObjects = Enumerable.Empty<IObservable>();

            // Call
            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism,double?>(
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
            var value = 3.0;

            var expectedFailureMechanism = new TestFailureMechanism();
            var passedFailureMechanism = new TestFailureMechanism();

            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism,double?>(
                expectedFailureMechanism, value, Enumerable.Empty<IObservable>());
            
            // Call
            TestDelegate test = () => tester.SetPropertyValueAfterConfirmation(passedFailureMechanism, value, (m, v) => { });

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ValuePassedNotEqual_ThrowsAssertionException()
        {
            // Setup
            var expectedFailureMechanism = new TestFailureMechanism();

            var expectedValue = 3.0;
            var passedValue = 2.0;

            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism,double?>(
                expectedFailureMechanism, expectedValue, Enumerable.Empty<IObservable>());
            
            // Call
            TestDelegate test = () => tester.SetPropertyValueAfterConfirmation(expectedFailureMechanism, passedValue, (m, v) => { });

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ParametersAreSameAndEqual_SetValueCalledReturnsGivenAffectedObjects()
        {
            // Setup
            var returnedAffectedObjects = Enumerable.Empty<IObservable>();
            var testFailureMechanism = new TestFailureMechanism();
            var value = 3.0;
            var called = 0;

            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism,double?>(
                testFailureMechanism, value, returnedAffectedObjects);
            
            // Call
            var affectedObjetcs = tester.SetPropertyValueAfterConfirmation(testFailureMechanism, value, (m, v) => called++);

            // Assert
            Assert.AreEqual(1, called);
            Assert.AreSame(returnedAffectedObjects, affectedObjetcs);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_SetValueThrowsException_BubblesException()
        {
            // Setup
            var testFailureMechanism = new TestFailureMechanism();
            var value = 3.0;

            var tester = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<IFailureMechanism,double?>(
                testFailureMechanism, value, Enumerable.Empty<IObservable>());
            
            var expectedException = new Exception();

            // Call
            TestDelegate test = () => tester.SetPropertyValueAfterConfirmation(testFailureMechanism, value, (m, v) => { throw expectedException; });

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreSame(expectedException, exception);
        }
    }
}