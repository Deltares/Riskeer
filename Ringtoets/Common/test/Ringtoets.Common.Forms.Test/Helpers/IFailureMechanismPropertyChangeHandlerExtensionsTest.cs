using System;
using System.Collections.Generic;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class IFailureMechanismPropertyChangeHandlerExtensionsTest
    {
        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutHandler_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => IFailureMechanismPropertyChangeHandlerExtensions.SetPropertyValueAfterConfirmation(
                null,
                new TestFailureMechanism(),
                3,
                (f, v) => { });

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("changeHandler", paramName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<IFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation<TestFailureMechanism, int>(
                null,
                3,
                (f, v) => { });

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutValue_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<IFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation<TestFailureMechanism, int?>(
                new TestFailureMechanism(), 
                null,
                (f, v) => { });

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("value", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_WithoutSetProperty_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<IFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(
                new TestFailureMechanism(),
                3,
                null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("setValue", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationRequiredAndGiven_SetValueCalledNotificationsOnObservables()
        {
            // Setup
            var testFailureMechanism = new TestFailureMechanism();
            var propertySet = 0;

            var mocks = new MockRepository();
            var observableA = mocks.StrictMock<IObservable>();
            observableA.Expect(o => o.NotifyObservers());
            var observableB = mocks.StrictMock<IObservable>();
            observableB.Expect(o => o.NotifyObservers());

            var expectedAffectedObjects = new []
            {
                observableA,
                observableB
            };

            var changeHandler = CreateChangeHandler(mocks, testFailureMechanism, true, true, expectedAffectedObjects);
            mocks.ReplayAll();

            // Call
            changeHandler.SetPropertyValueAfterConfirmation(
                testFailureMechanism,
                3,
                (f,v) => propertySet++);

            // Assert
            Assert.AreEqual(1, propertySet);
            mocks.VerifyAll();
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationNotRequired_SetValueCalledNoNotificationsOnObservables()
        {
            // Setup
            var testFailureMechanism = new TestFailureMechanism();
            var propertySet = 0;

            var mocks = new MockRepository();
            var changeHandler = CreateChangeHandler(mocks, testFailureMechanism, false);
            mocks.ReplayAll();

            // Call
            changeHandler.SetPropertyValueAfterConfirmation(
                testFailureMechanism,
                3,
                (f,v) => propertySet++);

            // Assert
            Assert.AreEqual(1, propertySet);
            mocks.VerifyAll();
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ConfirmationRequiredButNotGiven_SetValueNotCalled()
        {
            // Setup
            var testFailureMechanism = new TestFailureMechanism();
            var propertySet = 0;

            var mocks = new MockRepository();
            var changeHandler = CreateChangeHandler(mocks, testFailureMechanism, true, false);
            mocks.ReplayAll();

            // Call
            changeHandler.SetPropertyValueAfterConfirmation(
                testFailureMechanism,
                3,
                (f,v) => propertySet++);

            // Assert
            Assert.AreEqual(0, propertySet);
            mocks.VerifyAll();
        }

        private static IFailureMechanismPropertyChangeHandler<IFailureMechanism> CreateChangeHandler(
            MockRepository mocks, 
            IFailureMechanism testFailureMechanism, 
            bool requireConfirmation, 
            bool? confirmPropertyChange = null, 
            IEnumerable<IObservable> expectedAffectedObjects = null)
        {
            var changeHandler = mocks.StrictMock<IFailureMechanismPropertyChangeHandler<IFailureMechanism>>();
            changeHandler.Expect(c => c.RequiresConfirmation(testFailureMechanism)).Return(requireConfirmation);
            if (requireConfirmation)
            {
                changeHandler.Expect(c => c.ConfirmPropertyChange()).Return(confirmPropertyChange.Value);
            }
            if (requireConfirmation && confirmPropertyChange.Value)
            {
                changeHandler.Expect(c => c.PropertyChanged(testFailureMechanism)).Return(expectedAffectedObjects);
            }
            return changeHandler;
        }
    }
}