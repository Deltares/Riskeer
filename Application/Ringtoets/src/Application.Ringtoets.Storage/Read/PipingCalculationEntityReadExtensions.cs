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

using Core.Common.Base.Data;

using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Read
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
                StorageId = entity.PipingCalculationEntityId,
                IsRelevant = Convert.ToBoolean(entity.RelevantForScenario),
                Contribution = (RoundedDouble)Convert.ToDouble(entity.ScenarioContribution),
                Name = entity.Name,
                Comments = entity.Comments
            };
            ReadInputParameters(calculation.InputParameters, entity, collector);

            return calculation;
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
            if (entity.StochasticSoilProfileEntity != null)
            {
                inputParameters.StochasticSoilModel = entity.StochasticSoilProfileEntity.StochasticSoilModelEntity.Read(collector);
                inputParameters.StochasticSoilProfile = entity.StochasticSoilProfileEntity.Read(collector);
            }

            inputParameters.EntryPointL = GetRoundedDoubleFromNullableDecimal(entity.EntryPointL);
            inputParameters.ExitPointL = GetRoundedDoubleFromNullableDecimal(entity.ExitPointL);
            inputParameters.PhreaticLevelExit.Mean = (RoundedDouble)Convert.ToDouble(entity.PhreaticLevelExitMean);
            inputParameters.PhreaticLevelExit.StandardDeviation = (RoundedDouble)Convert.ToDouble(entity.PhreaticLevelExitStandardDeviation);
            inputParameters.DampingFactorExit.Mean = (RoundedDouble)Convert.ToDouble(entity.DampingFactorExitMean);
            inputParameters.DampingFactorExit.StandardDeviation = (RoundedDouble)Convert.ToDouble(entity.DampingFactorExitStandardDeviation);
            inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean = (RoundedDouble)Convert.ToDouble(entity.SaturatedVolumicWeightOfCoverageLayerMean);
            inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation = (RoundedDouble)Convert.ToDouble(entity.SaturatedVolumicWeightOfCoverageLayerStandardDeviation);
            inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift = (RoundedDouble)Convert.ToDouble(entity.SaturatedVolumicWeightOfCoverageLayerShift);
            inputParameters.Diameter70.Mean = (RoundedDouble)Convert.ToDouble(entity.Diameter70Mean);
            inputParameters.Diameter70.StandardDeviation = (RoundedDouble)Convert.ToDouble(entity.Diameter70StandardDeviation);
            inputParameters.DarcyPermeability.Mean = (RoundedDouble)Convert.ToDouble(entity.DarcyPermeabilityMean);
            inputParameters.DarcyPermeability.StandardDeviation = (RoundedDouble)Convert.ToDouble(entity.DarcyPermeabilityStandardDeviation);
        }

        private static RoundedDouble GetRoundedDoubleFromNullableDecimal(decimal? parameter)
        {
            if (parameter.HasValue)
            {
                return (RoundedDouble)Convert.ToDouble(parameter);
            }
            return (RoundedDouble)double.NaN;
        }
    }
}