using System;
using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Action in which a property of the <paramref name="failureMechanism"/> is set to the given <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism that is passed as argument.</typeparam>
    /// <typeparam name="TValue">The type of the value that is set on a property of the failure mechanism.</typeparam>
    /// <param name="failureMechanism">The failure mechanism for which the property will be set.</param>
    /// <param name="value">The new value of the failure mechanism property.</param>
    public delegate void SetFailureMechanismPropertyValueDelegate<in TFailureMechanism, in TValue>(TFailureMechanism failureMechanism, TValue value)
        where TFailureMechanism : IFailureMechanism;

    /// <summary>
    /// Interface for an object that can properly handle data model changes due to a change of a
    /// failure mechanism property.
    /// </summary>
    public interface IFailureMechanismPropertyChangeHandler<T> where T : IFailureMechanism
    {
        /// <summary>
        /// Checks to see if the change of the failure mechanism property should occur or not.
        /// </summary>
        /// <returns><c>true</c> if the change should occur, <c>false</c> otherwise.</returns>
        bool ConfirmPropertyChange();

        /// <summary>
        /// Propagates the necessary changes to underlying data structure when a property has 
        /// been changed for a failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to be updated.</param>
        /// <returns>All objects that have been affected by the change.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        IEnumerable<IObservable> PropertyChanged(T failureMechanism);

        /// <summary>
        /// Find out whether the property can be updated with or without confirmation. If confirmation is required, 
        /// the confirmation is obtained, after which the property is set if confirmation is given. If no confirmation
        /// was required, then the value will be set for the property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value that is set on a property of the failure mechanism.</typeparam>
        /// <param name="failureMechanism">The failure mechanism for which the property is supposed to be set.</param>
        /// <param name="value">The new value of the failure mechanism property.</param>
        /// <param name="setValue">The operation which is performed to set the new property <paramref name="value"/>
        /// on the <paramref name="failureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <remarks>Let <paramref name="setValue"/> throw an <see cref="Exception"/> when the 
        /// <see cref="FailureMechanismPropertyChangeHandler{T}"/> should not process the results of that operation.</remarks>
        IEnumerable<IObservable> SetPropertyValueAfterConfirmation<TValue>(
            T failureMechanism,
            TValue value,
            SetFailureMechanismPropertyValueDelegate<T, TValue> setValue);
    }
}