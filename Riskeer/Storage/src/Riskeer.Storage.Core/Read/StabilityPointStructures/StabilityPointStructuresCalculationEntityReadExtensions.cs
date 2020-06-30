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
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Structures;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.IllustrationPoints;

namespace Riskeer.Storage.Core.Read.StabilityPointStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="StructuresCalculation{T}"/>
    /// based on the <see cref="StabilityPointStructuresCalculationEntity"/>.
    /// </summary>
    internal static class StabilityPointStructuresCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="StabilityPointStructuresCalculationEntity"/> and use the
        /// information to update a <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructuresCalculationEntity"/>
        /// to create <see cref="StructuresCalculation{T}"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StructuresCalculation{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static StructuresCalculationScenario<StabilityPointStructuresInput> Read(this StabilityPointStructuresCalculationEntity entity,
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

            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
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

        private static void ReadInputParameters(StabilityPointStructuresInput inputParameters,
                                                StabilityPointStructuresCalculationEntity entity,
                                                ReadConversionCollector collector)
        {
            if (entity.StabilityPointStructureEntity != null)
            {
                inputParameters.Structure = entity.StabilityPointStructureEntity.Read(collector);
            }

            entity.Read(inputParameters, collector);

            inputParameters.InsideWaterLevel.Mean = (RoundedDouble) entity.InsideWaterLevelMean.ToNullAsNaN();
            inputParameters.InsideWaterLevel.StandardDeviation = (RoundedDouble) entity.InsideWaterLevelStandardDeviation.ToNullAsNaN();
            inputParameters.ThresholdHeightOpenWeir.Mean = (RoundedDouble) entity.ThresholdHeightOpenWeirMean.ToNullAsNaN();
            inputParameters.ThresholdHeightOpenWeir.StandardDeviation = (RoundedDouble) entity.ThresholdHeightOpenWeirStandardDeviation.ToNullAsNaN();
            inputParameters.ConstructiveStrengthLinearLoadModel.Mean = (RoundedDouble) entity.ConstructiveStrengthLinearLoadModelMean.ToNullAsNaN();
            inputParameters.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation = (RoundedDouble) entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation.ToNullAsNaN();
            inputParameters.ConstructiveStrengthQuadraticLoadModel.Mean = (RoundedDouble) entity.ConstructiveStrengthQuadraticLoadModelMean.ToNullAsNaN();
            inputParameters.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation = (RoundedDouble) entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation.ToNullAsNaN();
            inputParameters.BankWidth.Mean = (RoundedDouble) entity.BankWidthMean.ToNullAsNaN();
            inputParameters.BankWidth.StandardDeviation = (RoundedDouble) entity.BankWidthStandardDeviation.ToNullAsNaN();
            inputParameters.InsideWaterLevelFailureConstruction.Mean = (RoundedDouble) entity.InsideWaterLevelFailureConstructionMean.ToNullAsNaN();
            inputParameters.InsideWaterLevelFailureConstruction.StandardDeviation = (RoundedDouble) entity.InsideWaterLevelFailureConstructionStandardDeviation.ToNullAsNaN();
            inputParameters.EvaluationLevel = (RoundedDouble) entity.EvaluationLevel.ToNullAsNaN();
            inputParameters.LevelCrestStructure.Mean = (RoundedDouble) entity.LevelCrestStructureMean.ToNullAsNaN();
            inputParameters.LevelCrestStructure.StandardDeviation = (RoundedDouble) entity.LevelCrestStructureStandardDeviation.ToNullAsNaN();
            inputParameters.VerticalDistance = (RoundedDouble) entity.VerticalDistance.ToNullAsNaN();
            inputParameters.FailureProbabilityRepairClosure = entity.FailureProbabilityRepairClosure;
            inputParameters.FailureCollisionEnergy.Mean = (RoundedDouble) entity.FailureCollisionEnergyMean.ToNullAsNaN();
            inputParameters.FailureCollisionEnergy.CoefficientOfVariation = (RoundedDouble) entity.FailureCollisionEnergyCoefficientOfVariation.ToNullAsNaN();
            inputParameters.ShipMass.Mean = (RoundedDouble) entity.ShipMassMean.ToNullAsNaN();
            inputParameters.ShipMass.CoefficientOfVariation = (RoundedDouble) entity.ShipMassCoefficientOfVariation.ToNullAsNaN();
            inputParameters.ShipVelocity.Mean = (RoundedDouble) entity.ShipVelocityMean.ToNullAsNaN();
            inputParameters.ShipVelocity.CoefficientOfVariation = (RoundedDouble) entity.ShipVelocityCoefficientOfVariation.ToNullAsNaN();
            inputParameters.LevellingCount = entity.LevellingCount;
            inputParameters.ProbabilityCollisionSecondaryStructure = entity.ProbabilityCollisionSecondaryStructure;
            inputParameters.FlowVelocityStructureClosable.Mean = (RoundedDouble) entity.FlowVelocityStructureClosableMean.ToNullAsNaN();
            inputParameters.StabilityLinearLoadModel.Mean = (RoundedDouble) entity.StabilityLinearLoadModelMean.ToNullAsNaN();
            inputParameters.StabilityLinearLoadModel.CoefficientOfVariation = (RoundedDouble) entity.StabilityLinearLoadModelCoefficientOfVariation.ToNullAsNaN();
            inputParameters.StabilityQuadraticLoadModel.Mean = (RoundedDouble) entity.StabilityQuadraticLoadModelMean.ToNullAsNaN();
            inputParameters.StabilityQuadraticLoadModel.CoefficientOfVariation = (RoundedDouble) entity.StabilityQuadraticLoadModelCoefficientOfVariation.ToNullAsNaN();
            inputParameters.AreaFlowApertures.Mean = (RoundedDouble) entity.AreaFlowAperturesMean.ToNullAsNaN();
            inputParameters.AreaFlowApertures.StandardDeviation = (RoundedDouble) entity.AreaFlowAperturesStandardDeviation.ToNullAsNaN();
            inputParameters.InflowModelType = (StabilityPointStructureInflowModelType) entity.InflowModelType;
            inputParameters.LoadSchematizationType = (LoadSchematizationType) entity.LoadSchematizationType;
            inputParameters.VolumicWeightWater = (RoundedDouble) entity.VolumicWeightWater.ToNullAsNaN();
            inputParameters.FactorStormDurationOpenStructure = (RoundedDouble) entity.FactorStormDurationOpenStructure.ToNullAsNaN();
            inputParameters.DrainCoefficient.Mean = (RoundedDouble) entity.DrainCoefficientMean.ToNullAsNaN();
        }

        private static void ReadOutput(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                       StabilityPointStructuresCalculationEntity entity)
        {
            StabilityPointStructuresOutputEntity outputEntity = entity.StabilityPointStructuresOutputEntities.FirstOrDefault();
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