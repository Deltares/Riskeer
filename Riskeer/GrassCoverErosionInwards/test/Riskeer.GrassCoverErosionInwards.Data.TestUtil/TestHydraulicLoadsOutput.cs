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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;

namespace Riskeer.GrassCoverErosionInwards.Data.TestUtil
{
    /// <summary>
    /// Simple implementation of a <see cref="HydraulicLoadsOutput"/>.
    /// </summary>
    public class TestHydraulicLoadsOutput : HydraulicLoadsOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicLoadsOutput"/>.
        /// </summary>
        /// <param name="targetProbability">The norm used during the calculation.</param>
        /// <param name="targetReliability">The reliability index used during the calculation.</param>
        /// <param name="calculatedProbability">The calculated probability.</param>
        /// <param name="calculatedReliability">The calculated reliability index.</param>
        /// <param name="calculationConvergence">The convergence status of the calculation.</param>
        /// <param name="generalResult">The general result with the fault tree illustration points.</param> 
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or <paramref name="calculatedProbability"/> falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        public TestHydraulicLoadsOutput(double targetProbability, double targetReliability,
                                        double calculatedProbability, double calculatedReliability, CalculationConvergence calculationConvergence,
                                        GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
            : base(targetProbability, targetReliability, calculatedProbability,
                   calculatedReliability, calculationConvergence, generalResult) {}
    }
}