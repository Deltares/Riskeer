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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Revetment.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.GrassCoverErosionOutwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>
    /// based on the <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionOutwardsWaveConditionsCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionInwardsCalculationEntity"/> and use the
        /// information to update a <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsCalculationEntity"/>
        /// to create <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static GrassCoverErosionOutwardsWaveConditionsCalculation Read(this GrassCoverErosionOutwardsWaveConditionsCalculationEntity entity,
                                                                                ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = entity.Name,
                Comments =
                {
                    Body = entity.Comments
                }
            };
            ReadCalculationInputs(calculation.InputParameters, entity, collector);
            ReadCalculationOutputs(calculation, entity);

            return calculation;
        }

        private static void ReadCalculationInputs(GrassCoverErosionOutwardsWaveConditionsInput inputParameters,
                                                  GrassCoverErosionOutwardsWaveConditionsCalculationEntity entity,
                                                  ReadConversionCollector collector)
        {
            inputParameters.ForeshoreProfile = GetDikeProfileValue(entity.ForeshoreProfileEntity, collector);
            inputParameters.HydraulicBoundaryLocation = GetHydraulicBoundaryLocationValue(entity.HydraulicLocationEntity, collector);
            inputParameters.Orientation = (RoundedDouble) entity.Orientation.ToNullAsNaN();
            inputParameters.UseForeshore = Convert.ToBoolean(entity.UseForeshore);
            inputParameters.UseBreakWater = Convert.ToBoolean(entity.UseBreakWater);
            inputParameters.BreakWater.Height = (RoundedDouble) entity.BreakWaterHeight.ToNullAsNaN();
            inputParameters.BreakWater.Type = (BreakWaterType) entity.BreakWaterType;
            inputParameters.UpperBoundaryRevetment = (RoundedDouble) entity.UpperBoundaryRevetment.ToNullAsNaN();
            inputParameters.LowerBoundaryRevetment = (RoundedDouble) entity.LowerBoundaryRevetment.ToNullAsNaN();
            inputParameters.UpperBoundaryWaterLevels = (RoundedDouble) entity.UpperBoundaryWaterLevels.ToNullAsNaN();
            inputParameters.LowerBoundaryWaterLevels = (RoundedDouble) entity.LowerBoundaryWaterLevels.ToNullAsNaN();
            inputParameters.StepSize = (WaveConditionsInputStepSize) entity.StepSize;
            inputParameters.CategoryType = (FailureMechanismCategoryType) entity.CategoryType;
            inputParameters.CalculationType = (GrassCoverErosionOutwardsWaveConditionsCalculationType) entity.CalculationType;
        }

        private static void ReadCalculationOutputs(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                   GrassCoverErosionOutwardsWaveConditionsCalculationEntity entity)
        {
            if (!entity.GrassCoverErosionOutwardsWaveConditionsOutputEntities.Any())
            {
                return;
            }

            var waveRunUpOutput = new List<WaveConditionsOutput>();
            var waveImpactOutput = new List<WaveConditionsOutput>();
            foreach (GrassCoverErosionOutwardsWaveConditionsOutputEntity outputEntity in
                entity.GrassCoverErosionOutwardsWaveConditionsOutputEntities.OrderBy(e => e.Order))
            {
                if (outputEntity.OutputType == Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveRunUp))
                {
                    waveRunUpOutput.Add(outputEntity.Read());
                }

                if (outputEntity.OutputType == Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveImpact))
                {
                    waveImpactOutput.Add(outputEntity.Read());
                }
            }

            calculation.Output = CreateGrassCoverErosionOutwardsWaveConditionsOutput(waveRunUpOutput, waveImpactOutput);
        }

        private static GrassCoverErosionOutwardsWaveConditionsOutput CreateGrassCoverErosionOutwardsWaveConditionsOutput(IEnumerable<WaveConditionsOutput> waveRunUpOutput,
                                                                                                                         IEnumerable<WaveConditionsOutput> waveImpactOutput)
        {
            if (!waveImpactOutput.Any())
            {
                return GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUp(waveRunUpOutput);
            }

            if (!waveRunUpOutput.Any())
            {
                return GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveImpact(waveImpactOutput);
            }

            return GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpAndWaveImpact(waveRunUpOutput, waveImpactOutput);
        }

        private static ForeshoreProfile GetDikeProfileValue(ForeshoreProfileEntity foreshoreProfileEntity,
                                                            ReadConversionCollector collector)
        {
            return foreshoreProfileEntity?.Read(collector);
        }

        private static HydraulicBoundaryLocation GetHydraulicBoundaryLocationValue(
            HydraulicLocationEntity hydraulicLocationEntity,
            ReadConversionCollector collector)
        {
            return hydraulicLocationEntity != null
                       ? collector.Get(hydraulicLocationEntity)
                       : null;
        }
    }
}