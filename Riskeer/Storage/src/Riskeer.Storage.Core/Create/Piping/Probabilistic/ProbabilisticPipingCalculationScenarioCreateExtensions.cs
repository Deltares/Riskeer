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
using Core.Common.Util.Extensions;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.Piping.Probabilistic
{
    /// <summary>
    /// Extension methods for <see cref="ProbabilisticPipingCalculationScenario"/> related to creating
    /// a <see cref="ProbabilisticPipingCalculationEntity"/>.
    /// </summary>
    internal static class ProbabilisticPipingCalculationScenarioCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="ProbabilisticPipingCalculationEntity"/> based on the information
        /// of the <see cref="ProbabilisticPipingCalculationScenario"/>.
        /// </summary>
        /// <param name="calculation">The piping calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="calculation"/> resides within its parent.</param>
        /// <returns>A new <see cref="ProbabilisticPipingCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        public static ProbabilisticPipingCalculationEntity Create(this ProbabilisticPipingCalculationScenario calculation,
                                                                  PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }
            
            var entity = new ProbabilisticPipingCalculationEntity
            {
                RelevantForScenario = Convert.ToByte(calculation.IsRelevant),
                ScenarioContribution = calculation.Contribution.ToNaNAsNull(),
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.Body.DeepClone(),
                Order = order
            };
            SetInputParametersToEntity(entity, calculation.InputParameters, registry);

            return entity;
        }
        
        private static void SetInputParametersToEntity(ProbabilisticPipingCalculationEntity entity,
                                                       ProbabilisticPipingInput inputParameters,
                                                       PersistenceRegistry registry)
        {
            if (inputParameters.SurfaceLine != null)
            {
                entity.SurfaceLineEntity = registry.Get(inputParameters.SurfaceLine);
            }

            if (inputParameters.HydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = registry.Get(inputParameters.HydraulicBoundaryLocation);
            }

            if (inputParameters.StochasticSoilProfile != null)
            {
                entity.PipingStochasticSoilProfileEntity = registry.Get(inputParameters.StochasticSoilProfile);
            }

            entity.ExitPointL = inputParameters.ExitPointL.Value.ToNaNAsNull();
            entity.EntryPointL = inputParameters.EntryPointL.Value.ToNaNAsNull();

            entity.PhreaticLevelExitMean = inputParameters.PhreaticLevelExit.Mean.ToNaNAsNull();
            entity.PhreaticLevelExitStandardDeviation = inputParameters.PhreaticLevelExit.StandardDeviation.ToNaNAsNull();

            entity.DampingFactorExitMean = inputParameters.DampingFactorExit.Mean.ToNaNAsNull();
            entity.DampingFactorExitStandardDeviation = inputParameters.DampingFactorExit.StandardDeviation.ToNaNAsNull();

            entity.ShouldProfileSpecificIllustrationPointsBeCalculated = Convert.ToByte(inputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated);
            entity.ShouldSectionSpecificIllustrationPointsBeCalculated = Convert.ToByte(inputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated);
        }
    }
}