﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;

namespace Application.Ringtoets.Storage.Read.StabilityStoneCover
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="StabilityStoneCoverWaveConditionsCalculation"/>
    /// based on the <see cref="StabilityStoneCoverWaveConditionsCalculationEntity"/>.
    /// </summary>
    internal static class StabilityStoneCoverWaveConditionsCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="StabilityStoneCoverWaveConditionsCalculationEntity"/> and use the
        /// information to update a <see cref="StabilityStoneCoverWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityStoneCoverWaveConditionsCalculationEntity"/>
        /// to create <see cref="StabilityStoneCoverWaveConditionsCalculation"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StabilityStoneCoverWaveConditionsCalculation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static StabilityStoneCoverWaveConditionsCalculation Read(this StabilityStoneCoverWaveConditionsCalculationEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = entity.Name,
                Comments =
                {
                    Body = entity.Comments
                }
            };
            ReadCalculationInputs(calculation.InputParameters, entity, collector);
            ReadCalculationOutputs(entity, calculation);

            return calculation;
        }

        private static void ReadCalculationInputs(WaveConditionsInput inputParameters, StabilityStoneCoverWaveConditionsCalculationEntity entity, ReadConversionCollector collector)
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
        }

        private static void ReadCalculationOutputs(StabilityStoneCoverWaveConditionsCalculationEntity entity, StabilityStoneCoverWaveConditionsCalculation calculation)
        {
            if (!entity.StabilityStoneCoverWaveConditionsOutputEntities.Any())
            {
                return;
            }

            var columnsOutput = new List<WaveConditionsOutput>();
            var blocksOutput = new List<WaveConditionsOutput>();

            foreach (var conditionsOutputEntity in entity.StabilityStoneCoverWaveConditionsOutputEntities.OrderBy(oe => oe.Order))
            {
                var output = conditionsOutputEntity.Read();
                if (conditionsOutputEntity.OutputType == (byte) WaveConditionsOutputType.Columns)
                {
                    columnsOutput.Add(output);
                }
                else if (conditionsOutputEntity.OutputType == (byte) WaveConditionsOutputType.Blocks)
                {
                    blocksOutput.Add(output);
                }
            }

            calculation.Output = new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput);
        }

        private static ForeshoreProfile GetDikeProfileValue(ForeshoreProfileEntity foreshoreProfileEntity, ReadConversionCollector collector)
        {
            if (foreshoreProfileEntity != null)
            {
                return foreshoreProfileEntity.Read(collector);
            }
            return null;
        }

        private static HydraulicBoundaryLocation GetHydraulicBoundaryLocationValue(HydraulicLocationEntity hydraulicLocationEntity, ReadConversionCollector collector)
        {
            if (hydraulicLocationEntity != null)
            {
                return hydraulicLocationEntity.Read(collector);
            }
            return null;
        }
    }
}