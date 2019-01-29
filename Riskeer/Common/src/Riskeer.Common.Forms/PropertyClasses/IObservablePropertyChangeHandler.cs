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

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Action in which a property is set to its new value.
    /// </summary>
    /// <exception cref="Exception">Thrown when setting the property value results in an exception being thrown.</exception>
    public delegate void SetObservablePropertyValueDelegate();

    /// <summary>
    /// Interface for an object that can properly handle data model changes due
    /// to a change of an observable property.
    /// </summary>
    public interface IObservablePropertyChangeHandler
    {
        /// <summary>
        /// Find out whether the property can be updated with or without confirmation. If confirmation is required,
        /// the confirmation is obtained, after which the property is set if confirmation is given. If no confirmation
        /// was required, the value will be set for the property.
        /// </summary>
        /// <returns>All objects that are affected by setting the property.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="setValue"/> is <c>null</c>.</exception>
        /// <exception cref="Exception">Thrown when calling <paramref name="setValue"/> results in an exception being
        /// thrown.</exception>
        IEnumerable<IObservable> SetPropertyValueAfterConfirmation(SetObservablePropertyValueDelegate setValue);
    }
}