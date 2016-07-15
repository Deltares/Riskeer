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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;

using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Update.Piping
{
    /// <summary>
    /// Extension methods for <see cref="PipingCalculationScenario"/> related to updating
    /// a <see cref="PipingCalculationEntity"/>.
    /// </summary>
    internal static class PipingCalculationScenarioUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="PipingCalculationEntity"/> in the database based on the
        /// information of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        /// <param name="calculation">The piping calculation to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="calculation"/>
        /// does not have a corresponding entity in the database.</exception>
        internal static void Update(this PipingCalculationScenario calculation, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            PipingCalculationEntity entity = calculation.GetCorrespondingEntity(
                context.PipingCalculationEntities,
                o => o.PipingCalculationEntityId);
            entity.RelevantForScenario = Convert.ToByte(calculation.IsRelevant);
            entity.ScenarioContribution = Convert.ToDecimal(calculation.Contribution);
            entity.Name = calculation.Name;
            entity.Comments = calculation.Comments;
            SetInputParameters(entity, calculation.InputParameters, registry);
            UpdatePipingCalculationOutputs(entity, calculation, registry);

            registry.Register(entity, calculation);
        }

        private static void SetInputParameters(PipingCalculationEntity entity, PipingInput inputParameters, PersistenceRegistry registry)
        {
            entity.SurfaceLineEntity = inputParameters.SurfaceLine == null ?
                                           null :
                                           registry.Get(inputParameters.SurfaceLine);
            entity.HydraulicLocationEntity = inputParameters.HydraulicBoundaryLocation == null ?
                                                 null :
                                                 registry.Get(inputParameters.HydraulicBoundaryLocation);
            entity.StochasticSoilProfileEntity = inputParameters.StochasticSoilProfile == null ?
                                                     null :
                                                     registry.Get(inputParameters.StochasticSoilProfile);

            entity.EntryPointL = inputParameters.EntryPointL.Value.ToNullableDecimal();
            entity.ExitPointL = inputParameters.ExitPointL.Value.ToNullableDecimal();

            entity.PhreaticLevelExitMean = Convert.ToDecimal(inputParameters.PhreaticLevelExit.Mean);
            entity.PhreaticLevelExitStandardDeviation = Convert.ToDecimal(inputParameters.PhreaticLevelExit.StandardDeviation);
            entity.DampingFactorExitMean = Convert.ToDecimal(inputParameters.DampingFactorExit.Mean);
            entity.DampingFactorExitStandardDeviation = Convert.ToDecimal(inputParameters.DampingFactorExit.StandardDeviation);
            entity.SaturatedVolumicWeightOfCoverageLayerMean = Convert.ToDecimal(inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean);
            entity.SaturatedVolumicWeightOfCoverageLayerStandardDeviation = Convert.ToDecimal(inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation);
            entity.SaturatedVolumicWeightOfCoverageLayerShift = Convert.ToDecimal(inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift);
            entity.Diameter70Mean = Convert.ToDecimal(inputParameters.Diameter70.Mean);
            entity.Diameter70StandardDeviation = Convert.ToDecimal(inputParameters.Diameter70.StandardDeviation);
            entity.DarcyPermeabilityMean = Convert.ToDecimal(inputParameters.DarcyPermeability.Mean);
            entity.DarcyPermeabilityStandardDeviation = Convert.ToDecimal(inputParameters.DarcyPermeability.StandardDeviation);
        }

        private static void UpdatePipingCalculationOutputs(PipingCalculationEntity entity, PipingCalculationScenario calculation, PersistenceRegistry registry)
        {
            if (calculation.Output != null)
            {
                PipingOutput pipingOutput = calculation.Output;
                if (pipingOutput.IsNew())
                {
                    entity.PipingCalculationOutputEntity = pipingOutput.Create(registry);
                }
                else
                {
                    registry.Register(entity.PipingCalculationOutputEntity, pipingOutput);
                }
            }
            else
            {
                entity.PipingCalculationOutputEntity = null;
            }

            if (calculation.SemiProbabilisticOutput != null)
            {
                PipingSemiProbabilisticOutput semiProbabilisticOutput = calculation.SemiProbabilisticOutput;
                if (semiProbabilisticOutput.IsNew())
                {
                    entity.PipingSemiProbabilisticOutputEntity = semiProbabilisticOutput.Create(registry);
                }
                else
                {
                    registry.Register(entity.PipingSemiProbabilisticOutputEntity, semiProbabilisticOutput);
                }
            }
            else
            {
                entity.PipingSemiProbabilisticOutputEntity = null;
            }
        }
    }
}