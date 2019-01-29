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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Helper for updating parameters of calculation input with values taken from 
    /// a <see cref="ForeshoreProfile"/>.
    /// </summary>
    public static class SynchronizeCalculationWithForeshoreProfileHelper
    {
        /// <summary>
        /// Updates the foreshore profile derived calculation input with values from the
        /// assigned <see cref="ForeshoreProfile"/>.
        /// </summary>
        /// <typeparam name="TInput">The type of input to update.</typeparam>
        /// <param name="calculation">The calculation for which to update the properties of.</param>
        /// <remarks>Objects which are affected by input changes are notified.</remarks>
        public static void UpdateForeshoreProfileDerivedCalculationInput<TInput>(ICalculation<TInput> calculation)
            where TInput : ICalculationInput, IHasForeshoreProfile
        {
            TInput input = calculation.InputParameters;
            if (input.IsForeshoreProfileInputSynchronized)
            {
                return;
            }

            input.SynchronizeForeshoreProfileInput();

            var affectedObjects = new List<IObservable>
            {
                input
            };

            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));

            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }
    }
}