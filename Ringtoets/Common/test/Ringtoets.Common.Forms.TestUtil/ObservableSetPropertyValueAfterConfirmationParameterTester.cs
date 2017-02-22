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
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// This class can be used in tests to verify that the correct arguments 
    /// are passed to the <see cref="IObservablePropertyChangeHandler.SetPropertyValueAfterConfirmation{TCalculationInput, TValue}"/> method.
    /// </summary>
    /// <typeparam name="TExpectedValue">The type of the value that is expected to be passed to the method.</typeparam>    
    public class ObservableSetPropertyValueAfterConfirmationParameterTester<TExpectedValue>
        : IObservablePropertyChangeHandler
    {
        /// <summary>
        /// Creates a new instance of <see cref="ObservableSetPropertyValueAfterConfirmationParameterTester{TExpectedValue}"/>.
        /// </summary>
        /// <param name="expectedCalculationInput">The calculation input that is expected to be passed to the <see cref="SetPropertyValueAfterConfirmation{TCalculationInput, TValue}"/>.</param>
        /// <param name="expectedValue">The value that is expected to be passed to the <see cref="SetPropertyValueAfterConfirmation{TCalculationInput, TValue}"/>.</param>
        /// <param name="returnedAffectedObjects">The affected object that are returned by <see cref="SetPropertyValueAfterConfirmation{TCalculationInput, TValue}"/>.</param>
        public ObservableSetPropertyValueAfterConfirmationParameterTester(ICalculationInput expectedCalculationInput, TExpectedValue expectedValue, IEnumerable<IObservable> returnedAffectedObjects)
        {
            ExpectedCalculationInput = expectedCalculationInput;
            ExpectedValue = expectedValue;
            ReturnedAffectedObjects = returnedAffectedObjects;
        }

        /// <summary>
        /// Gets a value representing whether <see cref="SetPropertyValueAfterConfirmation{TCalculationInput, TValue}"/> was called.
        /// </summary>
        public bool Called { get; private set; }

        /// <summary>
        /// Gets the calculation input that is expected to be passed to the <see cref="SetPropertyValueAfterConfirmation{TCalculationInput, TValue}"/>.
        /// </summary>
        public ICalculationInput ExpectedCalculationInput { get; }

        /// <summary>
        /// Gets the value that is expected to be passed to the <see cref="SetPropertyValueAfterConfirmation{TCalculationInput, TValue}"/>.
        /// </summary>
        public TExpectedValue ExpectedValue { get; }

        /// <summary>
        /// Gets the affected object that are returned by <see cref="SetPropertyValueAfterConfirmation{TCalculationInput, TValue}"/>.
        /// </summary>
        public IEnumerable<IObservable> ReturnedAffectedObjects { get; }

        public IEnumerable<IObservable> SetPropertyValueAfterConfirmation<TCalculationInput, TValue>(
            TCalculationInput calculationInput,
            TValue value,
            SetObservablePropertyValueDelegate<TCalculationInput, TValue> setValue)
            where TCalculationInput : IObservable
        {
            Called = true;
            Assert.AreSame(ExpectedCalculationInput, calculationInput);
            Assert.AreEqual(ExpectedValue, value);
            setValue(calculationInput, value);
            return ReturnedAffectedObjects;
        }
    }
}