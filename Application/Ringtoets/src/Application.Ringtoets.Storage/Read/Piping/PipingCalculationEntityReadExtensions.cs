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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Read.Piping
{
    internal static class PipingCalculationEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="PipingCalculationEntity"/> and use the information to
        /// construct a <see cref="PipingCalculationScenario"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingCalculationEntity"/> to create
        /// <see cref="PipingCalculationScenario"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <param name="generalInputParameters">The general input parameters that apply
        /// to all <see cref="PipingCalculationScenario"/> instances.</param>
        /// <returns>A new <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static PipingCalculationScenario Read(this PipingCalculationEntity entity, ReadConversionCollector collector,
                                                       GeneralPipingInput generalInputParameters)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var calculation = new PipingCalculationScenario(generalInputParameters)
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

        private static void ReadCalculationOutputs(PipingCalculationScenario calculation, PipingCalculationEntity entity)
        {
            PipingCalculationOutputEntity calculationOutputEntity = entity.PipingCalculationOutputEntities.FirstOrDefault();
            if (calculationOutputEntity != null)
            {
                calculation.Output = calculationOutputEntity.Read();
            }

            PipingSemiProbabilisticOutputEntity semiProbabilisticOutputEntity = entity.PipingSemiProbabilisticOutputEntities.FirstOrDefault();
            if (semiProbabilisticOutputEntity != null)
            {
                calculation.SemiProbabilisticOutput = semiProbabilisticOutputEntity.Read();
            }
        }

        private static void ReadInputParameters(PipingInput inputParameters, PipingCalculationEntity entity, ReadConversionCollector collector)
        {
            if (entity.SurfaceLineEntity != null)
            {
                inputParameters.SurfaceLine = entity.SurfaceLineEntity.Read(collector);
            }
            if (entity.HydraulicLocationEntity != null)
            {
                inputParameters.HydraulicBoundaryLocation = entity.HydraulicLocationEntity.Read(collector);
            }
            else
            {
                inputParameters.AssessmentLevel = (RoundedDouble) entity.AssessmentLevel.ToNullAsNaN();
            }
            if (entity.StochasticSoilProfileEntity != null)
            {
                inputParameters.StochasticSoilModel = entity.StochasticSoilProfileEntity.StochasticSoilModelEntity.Read(collector);
                inputParameters.StochasticSoilProfile = entity.StochasticSoilProfileEntity.Read(collector);
            }

            inputParameters.EntryPointL = (RoundedDouble) entity.EntryPointL.ToNullAsNaN();
            inputParameters.ExitPointL = (RoundedDouble) entity.ExitPointL.ToNullAsNaN();
            inputParameters.PhreaticLevelExit.Mean = (RoundedDouble) entity.PhreaticLevelExitMean.ToNullAsNaN();
            inputParameters.PhreaticLevelExit.StandardDeviation = (RoundedDouble) entity.PhreaticLevelExitStandardDeviation.ToNullAsNaN();
            inputParameters.DampingFactorExit.Mean = (RoundedDouble) entity.DampingFactorExitMean.ToNullAsNaN();
            inputParameters.DampingFactorExit.StandardDeviation = (RoundedDouble) entity.DampingFactorExitStandardDeviation.ToNullAsNaN();
        }
    }
}