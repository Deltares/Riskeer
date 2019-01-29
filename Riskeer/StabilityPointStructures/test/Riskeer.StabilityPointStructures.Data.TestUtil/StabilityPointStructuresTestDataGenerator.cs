// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Riskeer.StabilityPointStructures.Data.TestUtil
{
    /// <summary>
    /// Class responsible for generating test data configurations.
    /// </summary>
    public static class StabilityPointStructuresTestDataGenerator
    {
        /// <summary>
        /// This method sets random data values to all properties of <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input to set the random data values to.</param>
        public static void SetRandomDataToStabilityPointStructuresInput(StabilityPointStructuresInput input)
        {
            var random = new Random(21);

            input.Structure = new TestStabilityPointStructure();

            input.InsideWaterLevelFailureConstruction = new NormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            input.InsideWaterLevel = new NormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            input.DrainCoefficient = new NormalDistribution
            {
                Mean = random.NextRoundedDouble()
            };

            input.LevelCrestStructure = new NormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            input.ThresholdHeightOpenWeir = new NormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            input.AreaFlowApertures = new LogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            input.ConstructiveStrengthLinearLoadModel = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            input.ConstructiveStrengthQuadraticLoadModel = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            input.StabilityLinearLoadModel = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            input.StabilityQuadraticLoadModel = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            input.FailureCollisionEnergy = new VariationCoefficientLogNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            input.ShipMass = new VariationCoefficientNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            input.ShipVelocity = new VariationCoefficientNormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            input.BankWidth = new NormalDistribution
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            input.FlowVelocityStructureClosable = new VariationCoefficientNormalDistribution
            {
                Mean = random.NextRoundedDouble()
            };

            input.VolumicWeightWater = random.NextRoundedDouble();
            input.FactorStormDurationOpenStructure = random.NextRoundedDouble();
            input.EvaluationLevel = random.NextRoundedDouble();
            input.VerticalDistance = random.NextRoundedDouble();
            input.FailureProbabilityRepairClosure = random.NextDouble();
            input.ProbabilityCollisionSecondaryStructure = random.NextDouble();
            input.InflowModelType = random.NextEnumValue<StabilityPointStructureInflowModelType>();
            input.LoadSchematizationType = random.NextEnumValue<LoadSchematizationType>();
            input.LevellingCount = random.Next();

            CommonTestDataGenerator.SetRandomDataToStructuresInput(input);
        }
    }
}