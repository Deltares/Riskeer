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
using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Action in which a property of the <paramref name="calculationInput"/> is set to the given <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TCalculationInput">The type of the calculation input that is passed as argument.</typeparam>
    /// <typeparam name="TValue">The type of the value that is set on a property of the calculation input.</typeparam>
    /// <param name="calculationInput">The calculation for which the property will be set.</param>
    /// <param name="value">The new value of the calculation input property.</param>
    /// <exception cref="Exception">Thrown when setting the property value results in an exception being thrown.</exception>
    public delegate void SetCalculationInputPropertyValueDelegate<in TCalculationInput, in TValue>(TCalculationInput calculationInput, TValue value)
        where TCalculationInput : ICalculationInput;

    /// <summary>
    /// Interface for an object that can properly handle data model changes due
    /// to a change of a calculation input property.
    /// </summary>
    public interface ICalculationInputPropertyChangeHandler
    {
        /// <summary>
        /// Find out whether the property can be updated with or without confirmation. If confirmation is required,
        /// the confirmation is obtained, after which the property is set if confirmation is given. If no confirmation
        /// was required, the value will be set for the property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value that is set on a property of the calculation input.</typeparam>
        /// <typeparam name="TCalculationInput">The type of the calculation input.</typeparam>
        /// <param name="calculationInput">The calculation input for which the property is supposed to be set.</param>
        /// <param name="calculation">The calculation which the <paramref name="calculationInput"/> belongs to.</param>
        /// <param name="value">The new value of the calculation input property.</param>
        /// <param name="setValue">The operation which is performed to set the new property <paramref name="value"/>
        /// on the <paramref name="calculationInput"/>.</param>
        /// <returns>All objects that are affected by setting the calculation input property.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationInput"/>, <paramref name="calculation"/>
        /// or <paramref name="setValue"/> is <c>null</c>.</exception>
        /// <exception cref="Exception">Thrown when calling <paramref name="setValue"/> results in an exception being
        /// thrown.</exception>
        IEnumerable<IObservable> SetPropertyValueAfterConfirmation<TCalculationInput, TValue>(
            TCalculationInput calculationInput,
            ICalculation calculation,
            TValue value,
            SetCalculationInputPropertyValueDelegate<TCalculationInput, TValue> setValue)
            where TCalculationInput : ICalculationInput;
    }
}