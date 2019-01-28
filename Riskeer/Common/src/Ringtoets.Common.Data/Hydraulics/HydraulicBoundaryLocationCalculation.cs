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
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// This class holds information about a calculation for a hydraulic boundary location.
    /// </summary>
    public class HydraulicBoundaryLocationCalculation : Observable, ICalculatable
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public HydraulicBoundaryLocationCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            HydraulicBoundaryLocation = hydraulicBoundaryLocation;
            InputParameters = new HydraulicBoundaryLocationCalculationInput();
        }

        /// <summary>
        /// Gets the hydraulic boundary location the calculation belongs to.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; }

        /// <summary>
        /// Gets the input of the hydraulic boundary location calculation.
        /// </summary>
        public HydraulicBoundaryLocationCalculationInput InputParameters { get; }

        /// <summary>
        /// Gets or sets the output of the hydraulic boundary location calculation.
        /// </summary>
        public HydraulicBoundaryLocationCalculationOutput Output { get; set; }

        /// <summary>
        /// Gets a value indicating whether this calculation item has output.
        /// </summary>
        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public bool ShouldCalculate
        {
            get
            {
                return !HasOutput || InputParameters.ShouldIllustrationPointsBeCalculated != Output.HasGeneralResult;
            }
        }
    }
}