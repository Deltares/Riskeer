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
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.Piping.Probabilistic
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="ProbabilisticPipingCalculationScenario"/>
    /// based on the <see cref="ProbabilisticPipingCalculationEntity"/>.
    /// </summary>
    internal static class ProbabilisticPipingCalculationEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="ProbabilisticPipingCalculationEntity"/> and use the information to
        /// construct a <see cref="ProbabilisticPipingCalculationScenario"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ProbabilisticPipingCalculationEntity"/> to create
        /// <see cref="ProbabilisticPipingCalculationScenario"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="ProbabilisticPipingCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        public static ProbabilisticPipingCalculationScenario Read(this ProbabilisticPipingCalculationEntity entity,
                                                                  ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var calculation = new ProbabilisticPipingCalculationScenario
            {
                IsRelevant = Convert.ToBoolean(entity.RelevantForScenario),
                Contribution = (RoundedDouble) entity.ScenarioContribution.ToNullAsNaN(),
                Name = entity.Name,
                Comments =
                {
                    Body = entity.Comments
                }
            };
            ReadInputParameters(calculation.InputParameters, entity, collector);
            ReadCalculationOutputs(calculation, entity);

            return calculation;
        }

        private static void ReadInputParameters(ProbabilisticPipingInput inputParameters, ProbabilisticPipingCalculationEntity entity,
                                                ReadConversionCollector collector)
        {
            if (entity.SurfaceLineEntity != null)
            {
                inputParameters.SurfaceLine = entity.SurfaceLineEntity.ReadAsPipingSurfaceLine(collector);
            }

            if (entity.HydraulicLocationEntity != null)
            {
                inputParameters.HydraulicBoundaryLocation = entity.HydraulicLocationEntity.Read(collector);
            }

            if (entity.PipingStochasticSoilProfileEntity != null)
            {
                inputParameters.StochasticSoilModel = entity.PipingStochasticSoilProfileEntity.StochasticSoilModelEntity.ReadAsPipingStochasticSoilModel(collector);
                inputParameters.StochasticSoilProfile = entity.PipingStochasticSoilProfileEntity.Read(collector);
            }

            inputParameters.EntryPointL = (RoundedDouble) entity.EntryPointL.ToNullAsNaN();
            inputParameters.ExitPointL = (RoundedDouble) entity.ExitPointL.ToNullAsNaN();
            inputParameters.PhreaticLevelExit.Mean = (RoundedDouble) entity.PhreaticLevelExitMean.ToNullAsNaN();
            inputParameters.PhreaticLevelExit.StandardDeviation = (RoundedDouble) entity.PhreaticLevelExitStandardDeviation.ToNullAsNaN();
            inputParameters.DampingFactorExit.Mean = (RoundedDouble) entity.DampingFactorExitMean.ToNullAsNaN();
            inputParameters.DampingFactorExit.StandardDeviation = (RoundedDouble) entity.DampingFactorExitStandardDeviation.ToNullAsNaN();

            inputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = Convert.ToBoolean(entity.ShouldProfileSpecificIllustrationPointsBeCalculated);
            inputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = Convert.ToBoolean(entity.ShouldSectionSpecificIllustrationPointsBeCalculated);
        }
        
        private static void ReadCalculationOutputs(ProbabilisticPipingCalculationScenario calculation,
                                                   ProbabilisticPipingCalculationEntity entity)
        {
            ProbabilisticPipingCalculationOutputEntity calculationOutputEntity = entity.ProbabilisticPipingCalculationOutputEntities.FirstOrDefault();
            if (calculationOutputEntity != null)
            {
                calculation.Output = calculationOutputEntity.Read();
            }
        }
    }
}