// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.IllustrationPoints;

namespace Ringtoets.Storage.Core.Read.ClosingStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="StructuresCalculation{T}"/>
    /// based on the <see cref="ClosingStructuresCalculationEntity"/>.
    /// </summary>
    internal static class ClosingStructuresCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="ClosingStructuresCalculationEntity"/> and use the
        /// information to update a <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructuresCalculationEntity"/>
        /// to create <see cref="StructuresCalculation{T}"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StructuresCalculation{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static StructuresCalculation<ClosingStructuresInput> Read(this ClosingStructuresCalculationEntity entity,
                                                                           ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = entity.Name,
                Comments =
                {
                    Body = entity.Comments
                }
            };
            ReadInputParameters(calculation.InputParameters, entity, collector);
            ReadOutput(calculation, entity);

            collector.Read(entity, calculation);

            return calculation;
        }

        private static void ReadInputParameters(ClosingStructuresInput inputParameters,
                                                ClosingStructuresCalculationEntity entity,
                                                ReadConversionCollector collector)
        {
            if (entity.ClosingStructureEntity != null)
            {
                inputParameters.Structure = entity.ClosingStructureEntity.Read(collector);
            }

            entity.Read(inputParameters, collector);

            inputParameters.InflowModelType = (ClosingStructureInflowModelType) entity.InflowModelType;
            inputParameters.InsideWaterLevel.Mean = (RoundedDouble) entity.InsideWaterLevelMean.ToNullAsNaN();
            inputParameters.InsideWaterLevel.StandardDeviation = (RoundedDouble) entity.InsideWaterLevelStandardDeviation.ToNullAsNaN();
            inputParameters.DeviationWaveDirection = (RoundedDouble) entity.DeviationWaveDirection.ToNullAsNaN();
            inputParameters.DrainCoefficient.Mean = (RoundedDouble) entity.DrainCoefficientMean.ToNullAsNaN();
            inputParameters.FactorStormDurationOpenStructure = (RoundedDouble) entity.FactorStormDurationOpenStructure.ToNullAsNaN();
            inputParameters.ThresholdHeightOpenWeir.Mean = (RoundedDouble) entity.ThresholdHeightOpenWeirMean.ToNullAsNaN();
            inputParameters.ThresholdHeightOpenWeir.StandardDeviation = (RoundedDouble) entity.ThresholdHeightOpenWeirStandardDeviation.ToNullAsNaN();
            inputParameters.AreaFlowApertures.Mean = (RoundedDouble) entity.AreaFlowAperturesMean.ToNullAsNaN();
            inputParameters.AreaFlowApertures.StandardDeviation = (RoundedDouble) entity.AreaFlowAperturesStandardDeviation.ToNullAsNaN();
            inputParameters.FailureProbabilityOpenStructure = entity.FailureProbabilityOpenStructure;
            inputParameters.FailureProbabilityReparation = entity.FailureProbabilityReparation;
            inputParameters.IdenticalApertures = entity.IdenticalApertures;
            inputParameters.LevelCrestStructureNotClosing.Mean = (RoundedDouble) entity.LevelCrestStructureNotClosingMean.ToNullAsNaN();
            inputParameters.LevelCrestStructureNotClosing.StandardDeviation = (RoundedDouble) entity.LevelCrestStructureNotClosingStandardDeviation.ToNullAsNaN();
            inputParameters.ProbabilityOpenStructureBeforeFlooding = entity.ProbabilityOpenStructureBeforeFlooding;
        }

        private static void ReadOutput(StructuresCalculation<ClosingStructuresInput> calculation,
                                       ClosingStructuresCalculationEntity entity)
        {
            ClosingStructuresOutputEntity outputEntity = entity.ClosingStructuresOutputEntities.FirstOrDefault();
            if (outputEntity == null)
            {
                return;
            }

            var output = new StructuresOutput(outputEntity.Reliability.ToNullAsNaN(),
                                              outputEntity.GeneralResultFaultTreeIllustrationPointEntity?.Read());
            calculation.Output = output;
        }
    }
}