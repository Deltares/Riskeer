﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;

namespace Riskeer.GrassCoverErosionInwards.Data.TestUtil
{
    /// <summary>
    /// Class which creates simple instances of <see cref="OvertoppingRateOutput"/>, 
    /// which can be used during testing.
    /// </summary>
    public class TestOvertoppingRateOutput : OvertoppingRateOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestOvertoppingRateOutput"/> with a specified
        /// <see cref="GeneralResult{T}"/>.
        /// </summary>
        /// <param name="generalResult">The general result with illustration points belonging
        /// to this output.</param>
        public TestOvertoppingRateOutput(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
            : base(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, CalculationConvergence.CalculatedNotConverged, generalResult) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestOvertoppingRateOutput"/>.
        /// </summary>
        /// <param name="overtoppingRate">The overtopping rate to set to the output.</param>
        /// <param name="calculationConvergence">The <see cref="CalculationConvergence"/> to set to the output.</param>
        public TestOvertoppingRateOutput(double overtoppingRate, CalculationConvergence calculationConvergence = CalculationConvergence.NotCalculated) :
            base(overtoppingRate, double.NaN, double.NaN, double.NaN, double.NaN, calculationConvergence, null) {}
    }
}