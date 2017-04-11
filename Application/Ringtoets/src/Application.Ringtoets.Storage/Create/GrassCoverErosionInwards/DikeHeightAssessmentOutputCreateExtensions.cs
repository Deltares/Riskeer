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

using System;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Create.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="SubCalculationAssessmentOutput"/>
    /// related to creating a <see cref="GrassCoverErosionInwardsDikeHeightOutputEntity"/>
    /// or a <see cref="GrassCoverErosionInwardsOvertoppingRateOutputEntity"/>.
    /// </summary>
    internal static class DikeHeightAssessmentOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverErosionInwardsDikeHeightOutputEntity"/>
        /// based on the information of the <see cref="SubCalculationAssessmentOutput"/>.
        /// </summary>
        /// <param name="output">The output to create a database entity for.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsDikeHeightOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/>
        /// is <c>null</c>.</exception>
        internal static GrassCoverErosionInwardsDikeHeightOutputEntity CreateDikeHeight(this SubCalculationAssessmentOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return new GrassCoverErosionInwardsDikeHeightOutputEntity
            {
                DikeHeight = double.IsNaN(output.Result)
                                 ? (double?) null
                                 : output.Result,
                TargetProbability = double.IsNaN(output.TargetProbability)
                                        ? (double?) null
                                        : output.TargetProbability,
                TargetReliability = double.IsNaN(output.TargetReliability)
                                        ? (double?) null
                                        : output.TargetReliability,
                CalculatedProbability = double.IsNaN(output.CalculatedProbability)
                                            ? (double?) null
                                            : output.CalculatedProbability,
                CalculatedReliability = double.IsNaN(output.CalculatedReliability)
                                            ? (double?) null
                                            : output.CalculatedReliability,
                CalculationConvergence = (byte) output.CalculationConvergence
            };
        }

        /// <summary>
        /// Creates a <see cref="GrassCoverErosionInwardsOvertoppingRateOutputEntity"/>
        /// based on the information of the <see cref="SubCalculationAssessmentOutput"/>.
        /// </summary>
        /// <param name="output">The output to create a database entity for.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsOvertoppingRateOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/>
        /// is <c>null</c>.</exception>
        internal static GrassCoverErosionInwardsOvertoppingRateOutputEntity CreateOvertoppingRate(this SubCalculationAssessmentOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return new GrassCoverErosionInwardsOvertoppingRateOutputEntity
            {
                OvertoppingRate = double.IsNaN(output.Result)
                                 ? (double?) null
                                 : output.Result,
                TargetProbability = double.IsNaN(output.TargetProbability)
                                        ? (double?) null
                                        : output.TargetProbability,
                TargetReliability = double.IsNaN(output.TargetReliability)
                                        ? (double?) null
                                        : output.TargetReliability,
                CalculatedProbability = double.IsNaN(output.CalculatedProbability)
                                            ? (double?) null
                                            : output.CalculatedProbability,
                CalculatedReliability = double.IsNaN(output.CalculatedReliability)
                                            ? (double?) null
                                            : output.CalculatedReliability,
                CalculationConvergence = (byte) output.CalculationConvergence
            };
        }
    }
}