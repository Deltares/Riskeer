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

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class StabilityPointStructureTest
    {
        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);

            // Call
            var structure = new StabilityPointStructure(
                new StabilityPointStructure.ConstructionProperties
                {
                    Name = "aName", Id = "anId", Location = location,
                    StructureNormalOrientation = 123.456,
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 234.567,
                        CoefficientOfVariation = (RoundedDouble) 0.234
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 345.678,
                        StandardDeviation = (RoundedDouble) 0.345
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 456.789,
                        CoefficientOfVariation = (RoundedDouble) 0.456
                    },
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) 567.890,
                        StandardDeviation = (RoundedDouble) 0.567
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) 678.901,
                        StandardDeviation = (RoundedDouble) 0.678
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 789.012,
                        CoefficientOfVariation = (RoundedDouble) 0.789
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 890.123,
                        StandardDeviation = (RoundedDouble) 0.890
                    },
                    ConstructiveStrengthLinearLoadModel =
                    {
                        Mean = (RoundedDouble) 901.234,
                        CoefficientOfVariation = (RoundedDouble) 0.901
                    },
                    ConstructiveStrengthQuadraticLoadModel =
                    {
                        Mean = (RoundedDouble) 123.456,
                        CoefficientOfVariation = (RoundedDouble) 0.123
                    },
                    BankWidth =
                    {
                        Mean = (RoundedDouble) 234.567,
                        StandardDeviation = (RoundedDouble) 0.234
                    },
                    InsideWaterLevelFailureConstruction =
                    {
                        Mean = (RoundedDouble) 345.678,
                        StandardDeviation = (RoundedDouble) 0.345
                    },
                    EvaluationLevel = 555.555,
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) 456.789,
                        StandardDeviation = (RoundedDouble) 0.456
                    },
                    VerticalDistance = 555.55,
                    FailureProbabilityRepairClosure = 0.55,
                    FailureCollisionEnergy =
                    {
                        Mean = (RoundedDouble) 567.890,
                        CoefficientOfVariation = (RoundedDouble) 0.567
                    },
                    ShipMass =
                    {
                        Mean = (RoundedDouble) 7777777.777,
                        CoefficientOfVariation = (RoundedDouble) 0.777
                    },
                    ShipVelocity =
                    {
                        Mean = (RoundedDouble) 567.890,
                        CoefficientOfVariation = (RoundedDouble) 0.567
                    },
                    LevellingCount = 42,
                    ProbabilityCollisionSecondaryStructure = 0.55,
                    FlowVelocityStructureClosable =
                    {
                        Mean = (RoundedDouble) 678.901,
                        StandardDeviation = (RoundedDouble) 0.678
                    },
                    StabilityLinearLoadModel =
                    {
                        Mean = (RoundedDouble) 789.012,
                        CoefficientOfVariation = (RoundedDouble) 0.789
                    },
                    StabilityQuadraticLoadModel =
                    {
                        Mean = (RoundedDouble) 890.123,
                        CoefficientOfVariation = (RoundedDouble) 0.890
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) 901.234,
                        StandardDeviation = (RoundedDouble) 0.901
                    },
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert
                });

            // Assert
            Assert.IsInstanceOf<StructureBase>(structure);

            VariationCoefficientLogNormalDistribution storageStructureArea = structure.StorageStructureArea;
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(234.57, storageStructureArea.Mean, storageStructureArea.Mean.GetAccuracy());
            Assert.AreEqual(2, storageStructureArea.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, storageStructureArea.CoefficientOfVariation, storageStructureArea.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution allowedLevelIncreaseStorage = structure.AllowedLevelIncreaseStorage;
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(345.68, allowedLevelIncreaseStorage.Mean, allowedLevelIncreaseStorage.Mean.GetAccuracy());
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.35, allowedLevelIncreaseStorage.StandardDeviation, allowedLevelIncreaseStorage.StandardDeviation.GetAccuracy());

            VariationCoefficientNormalDistribution widthFlowApertures = structure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(456.79, widthFlowApertures.Mean, widthFlowApertures.Mean.GetAccuracy());
            Assert.AreEqual(2, widthFlowApertures.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.46, widthFlowApertures.CoefficientOfVariation, widthFlowApertures.CoefficientOfVariation.GetAccuracy());

            NormalDistribution insideWaterLevel = structure.InsideWaterLevel;
            Assert.AreEqual(2, insideWaterLevel.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, insideWaterLevel.Mean, insideWaterLevel.Mean.GetAccuracy());
            Assert.AreEqual(2, insideWaterLevel.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, insideWaterLevel.StandardDeviation, insideWaterLevel.StandardDeviation.GetAccuracy());

            NormalDistribution thresholdHeightOpenWeir = structure.ThresholdHeightOpenWeir;
            Assert.AreEqual(2, thresholdHeightOpenWeir.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(678.90, thresholdHeightOpenWeir.Mean, thresholdHeightOpenWeir.Mean.GetAccuracy());
            Assert.AreEqual(2, thresholdHeightOpenWeir.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.68, thresholdHeightOpenWeir.StandardDeviation, thresholdHeightOpenWeir.StandardDeviation.GetAccuracy());

            VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge = structure.CriticalOvertoppingDischarge;
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(789.01, criticalOvertoppingDischarge.Mean, criticalOvertoppingDischarge.Mean.GetAccuracy());
            Assert.AreEqual(2, criticalOvertoppingDischarge.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.79, criticalOvertoppingDischarge.CoefficientOfVariation, criticalOvertoppingDischarge.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution flowWidthAtBottomProtection = structure.FlowWidthAtBottomProtection;
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(890.12, flowWidthAtBottomProtection.Mean, flowWidthAtBottomProtection.Mean.GetAccuracy());
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.89, flowWidthAtBottomProtection.StandardDeviation, flowWidthAtBottomProtection.StandardDeviation.GetAccuracy());

            VariationCoefficientLogNormalDistribution constructiveStrengthLinearLoadModel = structure.ConstructiveStrengthLinearLoadModel;
            Assert.AreEqual(2, constructiveStrengthLinearLoadModel.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(901.23, constructiveStrengthLinearLoadModel.Mean, constructiveStrengthLinearLoadModel.Mean.GetAccuracy());
            Assert.AreEqual(2, constructiveStrengthLinearLoadModel.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.90, constructiveStrengthLinearLoadModel.CoefficientOfVariation, constructiveStrengthLinearLoadModel.CoefficientOfVariation.GetAccuracy());

            VariationCoefficientLogNormalDistribution constructiveStrengthQuadraticLoadModel = structure.ConstructiveStrengthQuadraticLoadModel;
            Assert.AreEqual(2, constructiveStrengthQuadraticLoadModel.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(123.46, constructiveStrengthQuadraticLoadModel.Mean, constructiveStrengthQuadraticLoadModel.Mean.GetAccuracy());
            Assert.AreEqual(2, constructiveStrengthQuadraticLoadModel.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.12, constructiveStrengthQuadraticLoadModel.CoefficientOfVariation, constructiveStrengthQuadraticLoadModel.CoefficientOfVariation.GetAccuracy());

            NormalDistribution bankWidth = structure.BankWidth;
            Assert.AreEqual(2, bankWidth.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(234.57, bankWidth.Mean, bankWidth.Mean.GetAccuracy());
            Assert.AreEqual(2, bankWidth.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, bankWidth.StandardDeviation, bankWidth.StandardDeviation.GetAccuracy());

            NormalDistribution insideWaterLevelFailureConstruction = structure.InsideWaterLevelFailureConstruction;
            Assert.AreEqual(2, insideWaterLevelFailureConstruction.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(345.68, insideWaterLevelFailureConstruction.Mean, insideWaterLevelFailureConstruction.Mean.GetAccuracy());
            Assert.AreEqual(2, insideWaterLevelFailureConstruction.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.35, insideWaterLevelFailureConstruction.StandardDeviation, insideWaterLevelFailureConstruction.StandardDeviation.GetAccuracy());

            Assert.IsInstanceOf<RoundedDouble>(structure.EvaluationLevel);
            Assert.AreEqual(2, structure.EvaluationLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(555.55, structure.EvaluationLevel, structure.EvaluationLevel.GetAccuracy());

            NormalDistribution levelCrestStructure = structure.LevelCrestStructure;
            Assert.AreEqual(2, levelCrestStructure.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(456.79, levelCrestStructure.Mean, levelCrestStructure.Mean.GetAccuracy());
            Assert.AreEqual(2, levelCrestStructure.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.46, levelCrestStructure.StandardDeviation, levelCrestStructure.StandardDeviation.GetAccuracy());

            Assert.IsInstanceOf<RoundedDouble>(structure.VerticalDistance);
            Assert.AreEqual(2, structure.VerticalDistance.NumberOfDecimalPlaces);
            Assert.AreEqual(555.55, structure.VerticalDistance, structure.VerticalDistance.GetAccuracy());

            Assert.IsInstanceOf<RoundedDouble>(structure.FailureProbabilityRepairClosure);
            Assert.AreEqual(2, structure.FailureProbabilityRepairClosure.NumberOfDecimalPlaces);
            Assert.AreEqual(0.55, structure.FailureProbabilityRepairClosure, structure.FailureProbabilityRepairClosure.GetAccuracy());

            VariationCoefficientLogNormalDistribution failureCollisionEnergy = structure.FailureCollisionEnergy;
            Assert.AreEqual(2, failureCollisionEnergy.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, failureCollisionEnergy.Mean, failureCollisionEnergy.Mean.GetAccuracy());
            Assert.AreEqual(2, failureCollisionEnergy.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, failureCollisionEnergy.CoefficientOfVariation, failureCollisionEnergy.CoefficientOfVariation.GetAccuracy());

            VariationCoefficientNormalDistribution shipMass = structure.ShipMass;
            Assert.AreEqual(2, shipMass.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(7777777.78, shipMass.Mean, shipMass.Mean.GetAccuracy());
            Assert.AreEqual(2, shipMass.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.78, shipMass.CoefficientOfVariation, shipMass.CoefficientOfVariation.GetAccuracy());

            VariationCoefficientNormalDistribution shipVelocity = structure.ShipVelocity;
            Assert.AreEqual(2, shipVelocity.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, shipVelocity.Mean, shipVelocity.Mean.GetAccuracy());
            Assert.AreEqual(2, shipVelocity.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, shipVelocity.CoefficientOfVariation, shipVelocity.CoefficientOfVariation.GetAccuracy());

            Assert.AreEqual(42, structure.LevellingCount);

            Assert.IsInstanceOf<RoundedDouble>(structure.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(2, structure.ProbabilityCollisionSecondaryStructure.NumberOfDecimalPlaces);
            Assert.AreEqual(0.55, structure.ProbabilityCollisionSecondaryStructure, structure.ProbabilityCollisionSecondaryStructure.GetAccuracy());

            NormalDistribution flowVelocityStructureClosable = structure.FlowVelocityStructureClosable;
            Assert.AreEqual(2, flowVelocityStructureClosable.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(678.90, flowVelocityStructureClosable.Mean, flowVelocityStructureClosable.Mean.GetAccuracy());
            Assert.AreEqual(2, flowVelocityStructureClosable.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.68, flowVelocityStructureClosable.StandardDeviation, flowVelocityStructureClosable.StandardDeviation.GetAccuracy());

            VariationCoefficientLogNormalDistribution stabilityLinearLoadModel = structure.StabilityLinearLoadModel;
            Assert.AreEqual(2, stabilityLinearLoadModel.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(789.01, stabilityLinearLoadModel.Mean, stabilityLinearLoadModel.Mean.GetAccuracy());
            Assert.AreEqual(2, stabilityLinearLoadModel.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.79, stabilityLinearLoadModel.CoefficientOfVariation, stabilityLinearLoadModel.CoefficientOfVariation.GetAccuracy());

            VariationCoefficientLogNormalDistribution stabilityQuadraticLoadModel = structure.StabilityQuadraticLoadModel;
            Assert.AreEqual(2, stabilityQuadraticLoadModel.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(890.12, stabilityQuadraticLoadModel.Mean, stabilityQuadraticLoadModel.Mean.GetAccuracy());
            Assert.AreEqual(2, stabilityQuadraticLoadModel.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.89, stabilityQuadraticLoadModel.CoefficientOfVariation, stabilityQuadraticLoadModel.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution areaFlowApertures = structure.AreaFlowApertures;
            Assert.AreEqual(2, areaFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(901.23, areaFlowApertures.Mean, areaFlowApertures.Mean.GetAccuracy());
            Assert.AreEqual(2, areaFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.90, areaFlowApertures.StandardDeviation, areaFlowApertures.StandardDeviation.GetAccuracy());

            Assert.AreEqual(StabilityPointStructureInflowModelType.FloodedCulvert, structure.InflowModelType);
        }
    }
}