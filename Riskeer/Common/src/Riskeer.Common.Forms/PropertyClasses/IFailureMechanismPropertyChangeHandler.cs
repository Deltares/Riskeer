// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
    /// <exception cref="Exception">Thrown when setting the property value results in an exception being thrown.</exception>
    public delegate void SetFailureMechanismPropertyValueDelegate<in TFailureMechanism, in TValue>(TFailureMechanism failureMechanism, TValue value)
        where TFailureMechanism : IFailureMechanism;

    /// <summary>
    /// Interface for an object that can properly handle data model changes due to a change of a
    /// failure mechanism property.
    /// </summary>
    /// <typeparam name="T">The type of the failure mechanism.</typeparam>
    public interface IFailureMechanismPropertyChangeHandler<T> where T : IFailureMechanism
    {
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
        /// <exception cref="Exception">Thrown when calling <paramref name="setValue"/> results in an exception being
        /// thrown.</exception>
        IEnumerable<IObservable> SetPropertyValueAfterConfirmation<TValue>(
            T failureMechanism,
            TValue value,
            SetFailureMechanismPropertyValueDelegate<T, TValue> setValue);
    }
}