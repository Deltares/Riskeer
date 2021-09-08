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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.Forms.PresentationObjects
{
    /// <summary>
    /// Class that defines a drop down list edit-control from which the user can select a
    /// a target probability from a collection.
    /// </summary>
    public class SelectableTargetProbability
    {
        /// <summary>
        /// Creates a new instance of <see cref="SelectableTargetProbability"/>.
        /// </summary>
        /// <param name="calculationsForTargetProbability">The <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>.</param>
        /// <param name="waterLevelType">The <see cref="WaveConditionsInputWaterLevelType"/> belonging to
        /// the <paramref name="calculationsForTargetProbability"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationsForTargetProbability"/>
        /// is <c>null</c>.</exception>
        public SelectableTargetProbability(HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability,
                                           WaveConditionsInputWaterLevelType waterLevelType)
        {
            if (calculationsForTargetProbability == null)
            {
                throw new ArgumentNullException(nameof(calculationsForTargetProbability));
            }

            CalculationsForTargetProbability = calculationsForTargetProbability;
            WaterLevelType = waterLevelType;
        }

        /// <summary>
        /// Gets the <see cref="WaveConditionsInputWaterLevelType"/>.
        /// </summary>
        public WaveConditionsInputWaterLevelType WaterLevelType { get; }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>.
        /// </summary>
        public HydraulicBoundaryLocationCalculationsForTargetProbability CalculationsForTargetProbability { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SelectableTargetProbability) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = WaterLevelType.GetHashCode();
                hashCode = (hashCode * 397) ^ CalculationsForTargetProbability.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return ProbabilityFormattingHelper.Format(CalculationsForTargetProbability.TargetProbability);
        }

        private bool Equals(SelectableTargetProbability other)
        {
            return ReferenceEquals(CalculationsForTargetProbability, other.CalculationsForTargetProbability)
                   || WaterLevelType == other.WaterLevelType
                   && Math.Abs(CalculationsForTargetProbability.TargetProbability - other.CalculationsForTargetProbability.TargetProbability) < 1e-6;
        }
    }
}