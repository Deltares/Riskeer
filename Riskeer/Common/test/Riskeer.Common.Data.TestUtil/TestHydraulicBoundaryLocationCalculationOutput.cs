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

using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Class which creates simple instances of <see cref="HydraulicBoundaryLocationCalculationOutput"/>, 
    /// which can be used during testing.
    /// </summary>
    public class TestHydraulicBoundaryLocationCalculationOutput : HydraulicBoundaryLocationCalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocationCalculationOutput"/>.
        /// </summary>
        public TestHydraulicBoundaryLocationCalculationOutput() : this(double.NaN) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocationCalculationOutput"/>.
        /// </summary>
        /// <param name="result">The result to set in the output.</param>
        /// <param name="calculationConvergence">The <see cref="CalculationConvergence"/> to set in the output.</param>
        public TestHydraulicBoundaryLocationCalculationOutput(double result, CalculationConvergence calculationConvergence = CalculationConvergence.NotCalculated) :
            base(result, double.NaN, double.NaN, double.NaN, double.NaN, calculationConvergence, null) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocationCalculationOutput"/> with a specified general result.
        /// </summary>
        /// <param name="generalResult">The <see cref="GeneralResult{T}"/> to set in the output.</param>
        public TestHydraulicBoundaryLocationCalculationOutput(GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult) :
            base(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, CalculationConvergence.CalculatedConverged, generalResult) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocationCalculationOutput"/> with a specified general result.
        /// </summary>
        /// <param name="result">The result to set in the output.</param>
        /// <param name="generalResult">The <see cref="GeneralResult{T}"/> to set in the output.</param>
        public TestHydraulicBoundaryLocationCalculationOutput(double result, GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult) :
            base(result, double.NaN, double.NaN, double.NaN, double.NaN, CalculationConvergence.CalculatedConverged, generalResult) {}
    }
}