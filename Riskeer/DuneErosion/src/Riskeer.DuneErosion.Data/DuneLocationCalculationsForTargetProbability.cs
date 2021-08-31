// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Helpers;

namespace Riskeer.DuneErosion.Data
{
    /// <summary>
    /// Class that holds <see cref="DuneLocationCalculation"/> instances that correspond to a specific target probability.
    /// </summary>
    public class DuneLocationCalculationsForTargetProbability : Observable
    {
        private double targetProbability;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsForTargetProbability"/>.
        /// </summary>
        /// <param name="targetProbability">The target probability.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/>
        /// is not in the interval {0.0, 0.1] or is <see cref="double.NaN"/>.</exception>
        public DuneLocationCalculationsForTargetProbability(double targetProbability)
        {
            TargetProbability = targetProbability;
        }

        /// <summary>
        /// Gets or sets the target probability.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the new value
        /// is not in the interval {0.0, 0.1] or is <see cref="double.NaN"/>.</exception>
        public double TargetProbability
        {
            get => targetProbability;
            set
            {
                TargetProbabilityHelper.ValidateTargetProbability(value);
                targetProbability = value;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="DuneLocationCalculation"/> instances.
        /// </summary>
        public ObservableList<DuneLocationCalculation> DuneLocationCalculations { get; } = new ObservableList<DuneLocationCalculation>();
    }
}