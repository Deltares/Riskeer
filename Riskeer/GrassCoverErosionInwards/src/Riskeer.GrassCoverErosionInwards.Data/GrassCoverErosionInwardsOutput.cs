// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.Calculation;

namespace Riskeer.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// The overall result of a grass cover erosion inwards assessment.
    /// </summary>
    public class GrassCoverErosionInwardsOutput : CloneableObservable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsOutput"/>.
        /// </summary>
        /// <param name="overtoppingOutput">The overtopping output.</param>
        /// <param name="dikeHeightOutput">The dike height output.</param>
        /// <param name="overtoppingRateOutput">The overtopping rate output.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="overtoppingOutput"/>
        /// is <c>null</c>.</exception>
        public GrassCoverErosionInwardsOutput(OvertoppingOutput overtoppingOutput,
                                              DikeHeightOutput dikeHeightOutput,
                                              OvertoppingRateOutput overtoppingRateOutput)
        {
            if (overtoppingOutput == null)
            {
                throw new ArgumentNullException(nameof(overtoppingOutput));
            }

            OvertoppingOutput = overtoppingOutput;
            DikeHeightOutput = dikeHeightOutput;
            OvertoppingRateOutput = overtoppingRateOutput;
        }

        /// <summary>
        /// Gets the overtopping output.
        /// </summary>
        public OvertoppingOutput OvertoppingOutput { get; private set; }

        /// <summary>
        /// Gets the dike height output.
        /// </summary>
        public DikeHeightOutput DikeHeightOutput { get; private set; }

        /// <summary>
        /// Gets the overtopping rate output.
        /// </summary>
        public OvertoppingRateOutput OvertoppingRateOutput { get; private set; }

        public override object Clone()
        {
            var clone = (GrassCoverErosionInwardsOutput) base.Clone();

            clone.OvertoppingOutput = (OvertoppingOutput) OvertoppingOutput.Clone();

            if (DikeHeightOutput != null)
            {
                clone.DikeHeightOutput = (DikeHeightOutput) DikeHeightOutput.Clone();
            }

            if (OvertoppingRateOutput != null)
            {
                clone.OvertoppingRateOutput = (OvertoppingRateOutput) OvertoppingRateOutput.Clone();
            }

            return clone;
        }
    }
}