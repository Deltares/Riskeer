using System;
using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Interface for an object that can properly handle data model changes due to a change of a
    /// failure mechanism property.
    /// </summary>
    public interface IFailureMechanismPropertyChangeHandler<in T> where T : IFailureMechanism
    {
        /// <summary>
        /// Checks whether a call to <see cref="PropertyChanged"/> would have any effect in the given
        /// <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to check for.</param>
        /// <returns><c>true</c> if <see cref="PropertyChanged"/> would result in changes, 
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        bool RequiresConfirmation(T failureMechanism);

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
    }
}