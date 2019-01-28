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

using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// This class can be used in tests to verify that the correct arguments 
    /// are passed to the <see cref="IObservablePropertyChangeHandler.SetPropertyValueAfterConfirmation"/> method.
    /// </summary>
    public class SetPropertyValueAfterConfirmationParameterTester : IObservablePropertyChangeHandler
    {
        /// <summary>
        /// Creates a new instance of <see cref="SetPropertyValueAfterConfirmationParameterTester"/>.
        /// </summary>
        /// <param name="returnedAffectedObjects">The affected object that are returned by <see cref="SetPropertyValueAfterConfirmation"/>.</param>
        public SetPropertyValueAfterConfirmationParameterTester(IEnumerable<IObservable> returnedAffectedObjects)
        {
            ReturnedAffectedObjects = returnedAffectedObjects;
        }

        /// <summary>
        /// Gets a value representing whether <see cref="SetPropertyValueAfterConfirmation"/> was called.
        /// </summary>
        public bool Called { get; private set; }

        /// <summary>
        /// Gets the affected object that are returned by <see cref="SetPropertyValueAfterConfirmation"/>.
        /// </summary>
        public IEnumerable<IObservable> ReturnedAffectedObjects { get; }

        public IEnumerable<IObservable> SetPropertyValueAfterConfirmation(SetObservablePropertyValueDelegate setValue)
        {
            Called = true;
            setValue();
            return ReturnedAffectedObjects;
        }
    }
}