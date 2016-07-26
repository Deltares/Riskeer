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
        /// <param name="order">The index at which <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="calculation"/>
        /// does not have a corresponding entity in the database.</exception>
        internal static void Update(this PipingCalculationScenario calculation, PersistenceRegistry registry, IRingtoetsEntities context, int order)
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
            entity.ScenarioContribution = calculation.Contribution.Value.ToNaNAsNull();
            entity.Name = calculation.Name;
            entity.Comments = calculation.Comments;
            entity.Order = order;
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

            double tempQualifier = inputParameters.EntryPointL.Value;
            entity.EntryPointL = tempQualifier.ToNaNAsNull();
            double tempQualifier1 = inputParameters.ExitPointL.Value;
            entity.ExitPointL = tempQualifier1.ToNaNAsNull();

            entity.PhreaticLevelExitMean = inputParameters.PhreaticLevelExit.Mean.Value.ToNaNAsNull();
            entity.PhreaticLevelExitStandardDeviation = inputParameters.PhreaticLevelExit.StandardDeviation.Value.ToNaNAsNull();
            entity.DampingFactorExitMean = inputParameters.DampingFactorExit.Mean.Value.ToNaNAsNull();
            entity.DampingFactorExitStandardDeviation = inputParameters.DampingFactorExit.StandardDeviation.Value.ToNaNAsNull();
            entity.SaturatedVolumicWeightOfCoverageLayerMean = inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean.Value.ToNaNAsNull();
            entity.SaturatedVolumicWeightOfCoverageLayerStandardDeviation = inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation.Value.ToNaNAsNull();
            entity.SaturatedVolumicWeightOfCoverageLayerShift = inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift.Value.ToNaNAsNull();
            entity.Diameter70Mean = inputParameters.Diameter70.Mean.Value.ToNaNAsNull();
            entity.Diameter70StandardDeviation = inputParameters.Diameter70.StandardDeviation.Value.ToNaNAsNull();
            entity.DarcyPermeabilityMean = inputParameters.DarcyPermeability.Mean.Value.ToNaNAsNull();
            entity.DarcyPermeabilityStandardDeviation = inputParameters.DarcyPermeability.StandardDeviation.Value.ToNaNAsNull();
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