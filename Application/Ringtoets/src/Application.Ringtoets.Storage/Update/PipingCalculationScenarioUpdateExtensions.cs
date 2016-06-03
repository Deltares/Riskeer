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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;

using Core.Common.Base.Data;

using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Update
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
        /// does not have a corresponding entity in <paramref name="context"/>.</exception>
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

            PipingCalculationEntity entity = GetCorrespondingPipingCalculationEntity(calculation, context);
            entity.RelevantForScenario = Convert.ToByte(calculation.IsRelevant);
            entity.ScenarioContribution = Convert.ToDecimal(calculation.Contribution);
            entity.Name = calculation.Name;
            entity.Comments = calculation.Comments;
            SetInputParameters(entity, calculation.InputParameters, registry);
            

            registry.Register(entity, calculation);
        }

        private static void SetInputParameters(PipingCalculationEntity entity, PipingInput inputParameters, PersistenceRegistry registry)
        {
            entity.EntryPointL = GetNullableDecimal(inputParameters.EntryPointL);
            entity.ExitPointL = GetNullableDecimal(inputParameters.ExitPointL);

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

            entity.SurfaceLineEntity = inputParameters.SurfaceLine == null ?
                                           null :
                                           registry.Get(inputParameters.SurfaceLine);
            entity.HydraulicLocationEntity = inputParameters.HydraulicBoundaryLocation == null ?
                                                 null :
                                                 registry.Get(inputParameters.HydraulicBoundaryLocation);
            entity.StochasticSoilProfileEntity = inputParameters.StochasticSoilProfile == null ?
                                                     null :
                                                     registry.Get(inputParameters.StochasticSoilProfile);
        }

        /// <summary>
        /// Gets the <see cref="PipingCalculationEntity"/> based on the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        /// <param name="calculation">The piping calculation corresponding with the failure mechanism entity.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <returns>The stored <see cref="PipingCalculationEntity"/>.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when either:
        /// <list type="bullet">
        /// <item>the <see cref="PipingCalculationEntity"/> couldn't be found in the <paramref name="context"/></item>
        /// <item>more than one <see cref="PipingCalculationEntity"/> was found in the <paramref name="context"/></item>
        /// </list></exception>
        private static PipingCalculationEntity GetCorrespondingPipingCalculationEntity(this PipingCalculationScenario calculation, IRingtoetsEntities context)
        {
            try
            {
                return context.PipingCalculationEntities.Single(pc => pc.PipingCalculationEntityId == calculation.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(FailureMechanismEntity).Name, calculation.StorageId), exception);
            }
        }

        private static decimal? GetNullableDecimal(RoundedDouble value)
        {
            if (double.IsNaN(value))
            {
                return null;
            }
            return Convert.ToDecimal(value);
        }
    }
}