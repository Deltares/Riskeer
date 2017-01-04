using System;
using Core.Common.Base;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Extension methods for the <see cref="IFailureMechanismPropertyChangeHandler{T}"/> class.
    /// </summary>
    public static class IFailureMechanismPropertyChangeHandlerExtensions
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
        /// Uses the <paramref name="changeHandler"/> to find out whether the property can be updated with or without confirmation.
        /// If confirmation is required, the <paramref name="changeHandler"/> is used to obtain the confirmation, after which the
        /// property is set if confirmation is given. If no confirmation was required, then the value will be set for the property.
        /// </summary>
        /// <typeparam name="TFailureMechanism">The type of the failure mechanism that is passed as argument.</typeparam>
        /// <typeparam name="TValue">The type of the value that is set on a property of the failure mechanism.</typeparam>
        /// <param name="changeHandler">The handler which is used to handle the property change.</param>
        /// <param name="failureMechanism">The failure mechanism for which the property is supposed to be set.</param>
        /// <param name="value">The new value of the failure mechanism property.</param>
        /// <param name="setValue">The operation which is performed to set the new property <paramref name="value"/>
        /// on the <paramref name="failureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <remarks>Let <paramref name="setValue"/> throw an <see cref="Exception"/> when the <paramref name="changeHandler"/>
        /// should not process the results of that operation.</remarks>
        public static void SetPropertyValueAfterConfirmation<TFailureMechanism, TValue>(
            this IFailureMechanismPropertyChangeHandler<IFailureMechanism> changeHandler,
            TFailureMechanism failureMechanism,
            TValue value,
            SetFailureMechanismPropertyValueDelegate<TFailureMechanism, TValue> setValue)
            where TFailureMechanism : IFailureMechanism
        {
            if (changeHandler == null)
            {
                throw new ArgumentNullException(nameof(changeHandler));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (setValue == null)
            {
                throw new ArgumentNullException(nameof(setValue));
            }

            if (changeHandler.RequiresConfirmation(failureMechanism))
            {
                if (changeHandler.ConfirmPropertyChange())
                {
                    setValue(failureMechanism, value);
                    foreach (IObservable changedObject in changeHandler.PropertyChanged(failureMechanism))
                    {
                        changedObject.NotifyObservers();
                    }
                    failureMechanism.NotifyObservers();
                }
            }
            else
            {
                setValue(failureMechanism, value);
                failureMechanism.NotifyObservers();
            }
        }
    }
}