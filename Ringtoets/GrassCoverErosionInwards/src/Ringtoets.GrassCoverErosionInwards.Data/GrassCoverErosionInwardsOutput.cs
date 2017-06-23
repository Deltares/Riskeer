// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// The total result of a grass cover erosion inwards assessment.
    /// </summary>
    public class GrassCoverErosionInwardsOutput : Observable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsOutput"/>.
        /// </summary>
        /// <param name="resultOutput">The output of the assessment result.</param>
        /// <param name="dikeHeightOutput">The dike height output.</param>
        /// <param name="overtoppingRateOutput">The overtopping rate output.</param>
        public GrassCoverErosionInwardsOutput(GrassCoverErosionInwardsResultOutput resultOutput,
                                              DikeHeightOutput dikeHeightOutput,
                                              OvertoppingRateOutput overtoppingRateOutput)
        {
            ResultOutput = resultOutput;
            DikeHeightOutput = dikeHeightOutput;
            OvertoppingRateOutput = overtoppingRateOutput;
        }

        /// <summary>
        /// Gets the output of the assessment result.
        /// </summary>
        public GrassCoverErosionInwardsResultOutput ResultOutput { get; }

        /// <summary>
        /// Gets the dike height output.
        /// </summary>
        public DikeHeightOutput DikeHeightOutput { get; }

        /// <summary>
        /// Gets the overtopping rate output.
        /// </summary>
        public OvertoppingRateOutput OvertoppingRateOutput { get; }
    }
}