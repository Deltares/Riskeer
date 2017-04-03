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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.StabilityPointStructures.IO.Test
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StabilityPointStructuresCalculationConfiguration(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "some name";

            // Call
            var configuration = new StabilityPointStructuresCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<StructuresCalculationConfiguration>(configuration);
            Assert.AreEqual(name, configuration.Name);
            Assert.IsNull(configuration.AreaFlowApertures);
            Assert.IsNull(configuration.BankWidth);
            Assert.IsNull(configuration.ConstructiveStrengthLinearLoadModel);
            Assert.IsNull(configuration.ConstructiveStrengthQuadraticLoadModel);
            Assert.IsNull(configuration.EvaluationLevel);
            Assert.IsNull(configuration.FailureCollisionEnergy);
            Assert.IsNull(configuration.FailureProbabilityRepairClosure);
            Assert.IsNull(configuration.FlowVelocityStructureClosable);
            Assert.IsNull(configuration.InflowModelType);
            Assert.IsNull(configuration.InsideWaterLevel);
            Assert.IsNull(configuration.InsideWaterLevelFailureConstruction);
            Assert.IsNull(configuration.LevelCrestStructure);
            Assert.IsNull(configuration.LevellingCount);
            Assert.IsNull(configuration.ProbabilityCollisionSecondaryStructure);
            Assert.IsNull(configuration.ShipMass);
            Assert.IsNull(configuration.ShipVelocity);
            Assert.IsNull(configuration.StabilityLinearLoadModel);
            Assert.IsNull(configuration.StabilityQuadraticLoadModel);
            Assert.IsNull(configuration.ThresholdHeightOpenWeir);
            Assert.IsNull(configuration.VerticalDistance);
        }

        [Test]
        public void Properties_SetNewValues_NewValuesSet()
        {
            // Setup
            var configuration = new StabilityPointStructuresCalculationConfiguration("some name");
            var random = new Random(5432);
            var areaFlowApertures = new MeanStandardDeviationStochastConfiguration();
            var bankWidth = new MeanStandardDeviationStochastConfiguration();
            var constructiveStrengthLinearLoadModel = new MeanVariationCoefficientStochastConfiguration();
            var constructiveStrengthQuadraticLoadModel = new MeanVariationCoefficientStochastConfiguration();
            double evaluationLevel = random.NextDouble();
            var failureCollisionEnergy = new MeanVariationCoefficientStochastConfiguration();
            double failureProbabilityRepairClosure = random.NextDouble();
            var flowVelocityStructureClosable = new MeanVariationCoefficientStochastConfiguration();
            var inflowModelType = random.NextEnumValue<ConfigurationInflowModelType>();
            var insideWaterLevel = new MeanStandardDeviationStochastConfiguration();
            var insideWaterLevelFailureConstruction = new MeanStandardDeviationStochastConfiguration();
            var levelCrestStructure = new MeanStandardDeviationStochastConfiguration();
            int levellingCount = random.Next();
            double probabilityCollisionSecondaryStructure = random.NextDouble();
            var shipMass = new MeanVariationCoefficientStochastConfiguration();
            var shipVelocity = new MeanVariationCoefficientStochastConfiguration();
            var stabilityLinearLoadModel = new MeanVariationCoefficientStochastConfiguration();
            var stabilityQuadraticLoadModel = new MeanVariationCoefficientStochastConfiguration();
            var thresholdHeightOpenWeir = new MeanStandardDeviationStochastConfiguration();
            double verticalDistance = random.NextDouble();

            // Call
            configuration.AreaFlowApertures = areaFlowApertures;
            configuration.BankWidth = bankWidth;
            configuration.ConstructiveStrengthLinearLoadModel = constructiveStrengthLinearLoadModel;
            configuration.ConstructiveStrengthQuadraticLoadModel = constructiveStrengthQuadraticLoadModel;
            configuration.EvaluationLevel = evaluationLevel;
            configuration.FailureCollisionEnergy = failureCollisionEnergy;
            configuration.FailureProbabilityRepairClosure = failureProbabilityRepairClosure;
            configuration.FlowVelocityStructureClosable = flowVelocityStructureClosable;
            configuration.InflowModelType = inflowModelType;
            configuration.InsideWaterLevel = insideWaterLevel;
            configuration.InsideWaterLevelFailureConstruction = insideWaterLevelFailureConstruction;
            configuration.LevelCrestStructure = levelCrestStructure;
            configuration.LevellingCount = levellingCount;
            configuration.ProbabilityCollisionSecondaryStructure = probabilityCollisionSecondaryStructure;
            configuration.ShipMass = shipMass;
            configuration.ShipVelocity = shipVelocity;
            configuration.StabilityLinearLoadModel = stabilityLinearLoadModel;
            configuration.StabilityQuadraticLoadModel = stabilityQuadraticLoadModel;
            configuration.ThresholdHeightOpenWeir = thresholdHeightOpenWeir;
            configuration.VerticalDistance = verticalDistance;

            // Assert
            Assert.AreSame(areaFlowApertures, configuration.AreaFlowApertures);
            Assert.AreSame(bankWidth, configuration.BankWidth);
            Assert.AreSame(constructiveStrengthLinearLoadModel, configuration.ConstructiveStrengthLinearLoadModel);
            Assert.AreSame(constructiveStrengthQuadraticLoadModel, configuration.ConstructiveStrengthQuadraticLoadModel);
            Assert.AreEqual(evaluationLevel, configuration.EvaluationLevel);
            Assert.AreSame(failureCollisionEnergy, configuration.FailureCollisionEnergy);
            Assert.AreEqual(failureProbabilityRepairClosure, configuration.FailureProbabilityRepairClosure);
            Assert.AreSame(flowVelocityStructureClosable, configuration.FlowVelocityStructureClosable);
            Assert.AreEqual(inflowModelType, configuration.InflowModelType);
            Assert.AreEqual(insideWaterLevel, configuration.InsideWaterLevel);
            Assert.AreEqual(insideWaterLevelFailureConstruction, configuration.InsideWaterLevelFailureConstruction);
            Assert.AreEqual(levelCrestStructure, configuration.LevelCrestStructure);
            Assert.AreEqual(levellingCount, configuration.LevellingCount);
            Assert.AreEqual(probabilityCollisionSecondaryStructure, configuration.ProbabilityCollisionSecondaryStructure);
            Assert.AreSame(shipMass, configuration.ShipMass);
            Assert.AreSame(shipVelocity, configuration.ShipVelocity);
            Assert.AreSame(stabilityLinearLoadModel, configuration.StabilityLinearLoadModel);
            Assert.AreSame(stabilityQuadraticLoadModel, configuration.StabilityQuadraticLoadModel);
            Assert.AreSame(thresholdHeightOpenWeir, configuration.ThresholdHeightOpenWeir);
            Assert.AreEqual(verticalDistance, configuration.VerticalDistance);
        }
    }
}