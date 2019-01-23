// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.IllustrationPoints;

namespace Ringtoets.Storage.Core.Read.HeightStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="StructuresCalculation{T}"/>
    /// based on the <see cref="HeightStructuresCalculationEntity"/>.
    /// </summary>
    internal static class HeightStructuresCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="HeightStructuresCalculationEntity"/> and use the
        /// information to update a <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructuresCalculationEntity"/>
        /// to create <see cref="StructuresCalculation{T}"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StructuresCalculation{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static StructuresCalculation<HeightStructuresInput> Read(this HeightStructuresCalculationEntity entity,
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

            var calculation = new StructuresCalculation<HeightStructuresInput>
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

        private static void ReadInputParameters(HeightStructuresInput inputParameters,
                                                HeightStructuresCalculationEntity entity,
                                                ReadConversionCollector collector)
        {
            if (entity.HeightStructureEntity != null)
            {
                inputParameters.Structure = entity.HeightStructureEntity.Read(collector);
            }

            entity.Read(inputParameters, collector);

            inputParameters.DeviationWaveDirection = (RoundedDouble) entity.DeviationWaveDirection.ToNullAsNaN();
            inputParameters.ModelFactorSuperCriticalFlow.Mean = (RoundedDouble) entity.ModelFactorSuperCriticalFlowMean.ToNullAsNaN();
            inputParameters.LevelCrestStructure.Mean = (RoundedDouble) entity.LevelCrestStructureMean.ToNullAsNaN();
            inputParameters.LevelCrestStructure.StandardDeviation = (RoundedDouble) entity.LevelCrestStructureStandardDeviation.ToNullAsNaN();
        }

        private static void ReadOutput(StructuresCalculation<HeightStructuresInput> calculation,
                                       HeightStructuresCalculationEntity entity)
        {
            HeightStructuresOutputEntity outputEntity = entity.HeightStructuresOutputEntities.FirstOrDefault();
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