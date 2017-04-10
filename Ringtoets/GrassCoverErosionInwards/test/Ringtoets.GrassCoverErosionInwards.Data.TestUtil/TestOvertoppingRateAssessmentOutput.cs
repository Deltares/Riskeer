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

using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil
{
    /// <summary>
    /// Class which creates simple instances of <see cref="OvertoppingRateAssessmentOutput"/>, 
    /// which can be used during testing.
    /// </summary>
    public class TestOvertoppingRateAssessmentOutput : OvertoppingRateAssessmentOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestOvertoppingRateAssessmentOutput"/>.
        /// </summary>
        /// <param name="overtoppingRate">The overtopping rate to set to the output.</param>
        /// <param name="calculationConvergence">The <see cref="CalculationConvergence"/> to set to the output.</param>
        public TestOvertoppingRateAssessmentOutput(double overtoppingRate, CalculationConvergence calculationConvergence = CalculationConvergence.NotCalculated) :
            base(overtoppingRate, double.NaN, double.NaN, double.NaN, double.NaN, calculationConvergence) { }
    }
}