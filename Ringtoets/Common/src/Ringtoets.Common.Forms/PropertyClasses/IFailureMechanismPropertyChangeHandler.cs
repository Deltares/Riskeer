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

    public interface IFailureMechanismPropertyChangeHandler
    {
        /// <summary>
        /// Checks to see if the replacement of the norm variable of the assessment section
        /// should occur or not.
        /// </summary>
        /// <returns><c>true</c> if the change should occur, <c>false</c> otherwise.</returns>
        bool ConfirmPropertyChange();

        /// <summary>
        /// propagates the necessary changes to underlying data structure when a property has 
        /// been changed for a failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The section to be updated.</param>
        /// <returns>All objects that have been affected by the change.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        IEnumerable<IObservable> PropertyChanged(IFailureMechanism failureMechanism);
    }
}