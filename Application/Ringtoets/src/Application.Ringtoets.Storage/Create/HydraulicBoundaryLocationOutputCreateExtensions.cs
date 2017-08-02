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

using Application.Ringtoets.Storage.Create.IllustrationPoints;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Hydraulics;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="HydraulicBoundaryLocationOutput"/> related to creating a <see cref="IHydraulicLocationOutputEntity"/>.
    /// </summary>
    internal static class HydraulicBoundaryLocationOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="THydraulicLocationOutputEntity"/> based on the information of the <see cref="HydraulicBoundaryLocationOutput"/>.
        /// </summary>
        /// <typeparam name="THydraulicLocationOutputEntity">The output entity type to create.</typeparam>
        /// <param name="output">The output to create a database entity for.</param>
        /// <param name="outputType">The calculation output type.</param>
        /// <returns>A new <typeparamref name="THydraulicLocationOutputEntity"/> of output type <paramref name="outputType"/>.</returns>
        internal static THydraulicLocationOutputEntity Create<THydraulicLocationOutputEntity>(this HydraulicBoundaryLocationOutput output,
                                                                                              HydraulicLocationOutputType outputType)
            where THydraulicLocationOutputEntity : IHydraulicLocationOutputEntity, new()
        {
            return new THydraulicLocationOutputEntity
            {
                HydraulicLocationOutputType = (byte) outputType,
                Result = double.IsNaN(output.Result)
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
                CalculationConvergence = (byte) output.CalculationConvergence,
                GeneralResultSubMechanismIllustrationPointEntity = output.GeneralResult?.CreateGeneralResultSubMechanismIllustrationPointEntity()
            };
        }
    }
}