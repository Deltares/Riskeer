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

using Core.Common.Base;

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Class that holds <see cref="HydraulicBoundaryLocationCalculation"/> instances that correspond to a specific target probability.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationsForTargetProbability : Observable
    {
        /// <summary>
        /// Gets or sets the target probability.
        /// </summary>
        public double TargetProbability { get; set; } = 0.1;

        /// <summary>
        /// Gets the list of <see cref="HydraulicBoundaryLocationCalculation"/> instances.
        /// </summary>
        public ObservableList<HydraulicBoundaryLocationCalculation> HydraulicBoundaryLocationCalculations { get; } = new ObservableList<HydraulicBoundaryLocationCalculation>();
    }
}