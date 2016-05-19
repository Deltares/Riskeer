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

using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil
{
    /// <summary>
    /// Factory for creating <see cref="GrassCoverErosionInwardsCalculation"/> objects.
    /// </summary>
    public static class GrassCoverErosionInwardsCalculationFactory
    {
        /// <summary>
        /// Returns a new instance of <see cref="GrassCoverErosionInwardsCalculation"/> without data.
        /// </summary>
        /// <returns>A new instance of <see cref="GrassCoverErosionInwardsCalculation"/> without data.</returns>
        public static GrassCoverErosionInwardsCalculation CreateCalculationWithInvalidData()
        {
            return new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                           new NormProbabilityGrassCoverErosionInwardsInput());
        }

        /// <summary>
        /// Returns a new instance of <see cref="GrassCoverErosionInwardsCalculation"/> with data.
        /// </summary>
        /// <returns>A new instance of <see cref="GrassCoverErosionInwardsCalculation"/> with data.</returns>
        public static GrassCoverErosionInwardsCalculation CreateCalculationWithValidInput()
        {
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0.0, 0.0)
            {
                DesignWaterLevel = (RoundedDouble) 1.0
            };

            return new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                           new NormProbabilityGrassCoverErosionInwardsInput())
            {
                InputParameters =
                {
                    Orientation = (RoundedDouble) 1.0,
                    CriticalFlowRate = new LognormalDistribution(1),
                    DikeHeight = (RoundedDouble) 1.0,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    UseBreakWater = false,
                    UseForeshore = false
                }
            };
        }
    }
}