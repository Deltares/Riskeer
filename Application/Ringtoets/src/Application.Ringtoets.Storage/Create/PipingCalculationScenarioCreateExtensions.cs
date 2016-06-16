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
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="PipingCalculationScenario"/> related to creating
    /// an <see cref="PipingCalculationEntity"/>.
    /// </summary>
    internal static class PipingCalculationScenarioCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipingCalculationEntity"/> based on the information of the <see cref="PipingCalculationScenario"/>.
        /// </summary>
        /// <param name="calculation">The piping calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="calculation"/> resides within its parent.</param>
        /// <returns>A new <see cref="PipingCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static PipingCalculationEntity Create(this PipingCalculationScenario calculation, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new PipingCalculationEntity
            {
                RelevantForScenario = Convert.ToByte(calculation.IsRelevant),
                ScenarioContribution = Convert.ToDecimal(calculation.Contribution),
                Name = calculation.Name,
                Comments = calculation.Comments,
                Order = order
            };
            SetInputParametersToEntity(entity, calculation.InputParameters, registry);
            CreatePipingOutputEntity(entity, calculation.Output, registry);
            CreatePipingSemiProbabilisticOutputEntity(entity, calculation.SemiProbabilisticOutput, registry);

            registry.Register(entity, calculation);
            
            return entity;
        }

        private static void SetInputParametersToEntity(PipingCalculationEntity entity, PipingInput inputParameters, PersistenceRegistry registry)
        {
            if (inputParameters.SurfaceLine != null)
            {
                entity.SurfaceLineEntity = inputParameters.SurfaceLine.Create(registry);
            }
            if (inputParameters.HydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = inputParameters.HydraulicBoundaryLocation.Create(registry);
            }
            if (inputParameters.StochasticSoilProfile != null)
            {
                inputParameters.StochasticSoilModel.Create(registry);
                entity.StochasticSoilProfileEntity = registry.Get(inputParameters.StochasticSoilProfile);
            }

            entity.ExitPointL = inputParameters.ExitPointL.Value.ToNullableDecimal();
            entity.EntryPointL = inputParameters.EntryPointL.Value.ToNullableDecimal();

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

        private static void CreatePipingOutputEntity(PipingCalculationEntity entity, PipingOutput output, PersistenceRegistry registry)
        {
            if (output != null)
            {
                entity.PipingCalculationOutputEntity = output.Create(registry);
            }
        }

        private static void CreatePipingSemiProbabilisticOutputEntity(PipingCalculationEntity entity, PipingSemiProbabilisticOutput output, PersistenceRegistry registry)
        {
            if (output != null)
            {
                entity.PipingSemiProbabilisticOutputEntity = output.Create(registry);
            }
        }
    }
}