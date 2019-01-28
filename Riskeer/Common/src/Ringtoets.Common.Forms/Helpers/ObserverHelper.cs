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
using Core.Common.Base;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for dealing with <see cref="Observer"/> and <see cref="RecursiveObserver{TContainer,TObservable}"/>.
    /// </summary>
    public static class ObserverHelper
    {
        /// <summary>
        /// Creates a <see cref="RecursiveObserver{TContainer,TObservable}"/> for an enumeration of hydraulic boundary location calculations.
        /// </summary>
        /// <param name="calculations">The calculations to observe.</param>
        /// <param name="updateObserverAction">The action to perform on notifications coming from one of the calculations.</param>
        /// <returns>The created <see cref="RecursiveObserver{TContainer,TObservable}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> CreateHydraulicBoundaryLocationCalculationsObserver(
            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations, Action updateObserverAction)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (updateObserverAction == null)
            {
                throw new ArgumentNullException(nameof(updateObserverAction));
            }

            return new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>(
                updateObserverAction, calc => calc)
            {
                Observable = calculations
            };
        }
    }
}