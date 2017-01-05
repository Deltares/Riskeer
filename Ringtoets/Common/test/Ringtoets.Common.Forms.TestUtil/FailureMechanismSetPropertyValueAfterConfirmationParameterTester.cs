using System;
using System.Collections.Generic;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// This class can be used in tests to verify that the correct arguments are passed to the <see cref="IFailureMechanismPropertyChangeHandler{T}.SetPropertyValueAfterConfirmation{TValue}"/>
    /// method.
    /// </summary>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism that is expected to be passed to the method.</typeparam>
    /// <typeparam name="TExpectedValue">The type of the value that is expected to be passed to the method.</typeparam>
    public class FailureMechanismSetPropertyValueAfterConfirmationParameterTester<TFailureMechanism, TExpectedValue> : IFailureMechanismPropertyChangeHandler<TFailureMechanism>
        where TFailureMechanism : IFailureMechanism
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSetPropertyValueAfterConfirmationParameterTester{TFailureMechanism,TExpectedValue}"/>.
        /// </summary>
        /// <param name="expectedFailureMechanism">The failure mechanism that is expected to be passed to the <see cref="SetPropertyValueAfterConfirmation{TValue}"/>.</param>
        /// <param name="expectedValue">The value that is expected to be passed to the <see cref="SetPropertyValueAfterConfirmation{TValue}"/>.</param>
        /// <param name="returnedAffectedObjects">The affected object that are returned by <see cref="SetPropertyValueAfterConfirmation{TValue}"/>.</param>
        public FailureMechanismSetPropertyValueAfterConfirmationParameterTester(TFailureMechanism expectedFailureMechanism, TExpectedValue expectedValue, IEnumerable<IObservable> returnedAffectedObjects)
        {
            ExpectedFailureMechanism = expectedFailureMechanism;
            ExpectedValue = expectedValue;
            ReturnedAffectedObjects = returnedAffectedObjects;
        }

        /// <summary>
        /// Gets a value representing whether <see cref="SetPropertyValueAfterConfirmation{TValue}"/> was called.
        /// </summary>
        public bool Called { get; private set; }

        /// <summary>
        /// Gets the failure mechanism that is expected to be passed to the <see cref="SetPropertyValueAfterConfirmation{TValue}"/>.
        /// </summary>
        public TFailureMechanism ExpectedFailureMechanism { get; }

        /// <summary>
        /// Gets the value that is expected to be passed to the <see cref="SetPropertyValueAfterConfirmation{TValue}"/>.
        /// </summary>
        public TExpectedValue ExpectedValue { get; }

        /// <summary>
        /// Gets the affected object that are returned by <see cref="SetPropertyValueAfterConfirmation{TValue}"/>.
        /// </summary>
        public IEnumerable<IObservable> ReturnedAffectedObjects { get; }

        public bool ConfirmPropertyChange()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IObservable> PropertyChanged(TFailureMechanism failureMechanism)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IObservable> SetPropertyValueAfterConfirmation<TValue>(TFailureMechanism failureMechanism, TValue value, SetFailureMechanismPropertyValueDelegate<TFailureMechanism, TValue> setValue)
        {
            Called = true;
            Assert.AreSame(ExpectedFailureMechanism, failureMechanism);
            Assert.AreEqual(ExpectedValue, value);
            setValue(failureMechanism, value);
            return ReturnedAffectedObjects;
        }
    }
}