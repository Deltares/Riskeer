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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class StabilityPointStructureTest
    {
        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Call
            var structure = new StabilityPointStructure(
                new StabilityPointStructure.ConstructionProperties
                {
                    Name = "aName",
                    Id = "anId",
                    Location = new Point2D(1.22, 2.333),
                    StructureNormalOrientation = (RoundedDouble) 123.456,
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
                        StandardDeviation = (RoundedDouble) 0.456
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
                        CoefficientOfVariation = (RoundedDouble) 0.2
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

            Assert.AreEqual(2, structure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.AreEqual(123.46, structure.StructureNormalOrientation, structure.StructureNormalOrientation.GetAccuracy());

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

            NormalDistribution widthFlowApertures = structure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(456.79, widthFlowApertures.Mean, widthFlowApertures.Mean.GetAccuracy());
            Assert.AreEqual(2, widthFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.46, widthFlowApertures.StandardDeviation, widthFlowApertures.StandardDeviation.GetAccuracy());

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

            Assert.AreEqual(0.55, structure.FailureProbabilityRepairClosure);

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

            Assert.AreEqual(0.55, structure.ProbabilityCollisionSecondaryStructure);

            VariationCoefficientNormalDistribution flowVelocityStructureClosable = structure.FlowVelocityStructureClosable;
            Assert.AreEqual(2, flowVelocityStructureClosable.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(678.90, flowVelocityStructureClosable.Mean, flowVelocityStructureClosable.Mean.GetAccuracy());
            Assert.AreEqual(2, flowVelocityStructureClosable.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.2, flowVelocityStructureClosable.CoefficientOfVariation, flowVelocityStructureClosable.CoefficientOfVariation.GetAccuracy());

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

        [Test]
        public void Constructor_DefaultData_ExpectedValues()
        {
            // Call
            var structure = new StabilityPointStructure(
                new StabilityPointStructure.ConstructionProperties
                {
                    Name = "aName",
                    Id = "anId",
                    Location = new Point2D(1.22, 2.333)
                });

            // Assert
            Assert.IsInstanceOf<StructureBase>(structure);

            Assert.AreEqual(2, structure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.IsNaN(structure.StructureNormalOrientation);

            VariationCoefficientLogNormalDistribution storageStructureArea = structure.StorageStructureArea;
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(storageStructureArea.Mean);
            Assert.AreEqual(2, storageStructureArea.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, storageStructureArea.CoefficientOfVariation, storageStructureArea.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution allowedLevelIncreaseStorage = structure.AllowedLevelIncreaseStorage;
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(allowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, allowedLevelIncreaseStorage.StandardDeviation, allowedLevelIncreaseStorage.StandardDeviation.GetAccuracy());

            NormalDistribution widthFlowApertures = structure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(widthFlowApertures.Mean);
            Assert.AreEqual(2, widthFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.2, widthFlowApertures.StandardDeviation, widthFlowApertures.StandardDeviation.GetAccuracy());

            NormalDistribution insideWaterLevel = structure.InsideWaterLevel;
            Assert.AreEqual(2, insideWaterLevel.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(insideWaterLevel.Mean);
            Assert.AreEqual(2, insideWaterLevel.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, insideWaterLevel.StandardDeviation, insideWaterLevel.StandardDeviation.GetAccuracy());

            NormalDistribution thresholdHeightOpenWeir = structure.ThresholdHeightOpenWeir;
            Assert.AreEqual(2, thresholdHeightOpenWeir.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(thresholdHeightOpenWeir.Mean);
            Assert.AreEqual(2, thresholdHeightOpenWeir.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, thresholdHeightOpenWeir.StandardDeviation, thresholdHeightOpenWeir.StandardDeviation.GetAccuracy());

            VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge = structure.CriticalOvertoppingDischarge;
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(criticalOvertoppingDischarge.Mean);
            Assert.AreEqual(2, criticalOvertoppingDischarge.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.15, criticalOvertoppingDischarge.CoefficientOfVariation, criticalOvertoppingDischarge.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution flowWidthAtBottomProtection = structure.FlowWidthAtBottomProtection;
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(flowWidthAtBottomProtection.Mean);
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.05, flowWidthAtBottomProtection.StandardDeviation, flowWidthAtBottomProtection.StandardDeviation.GetAccuracy());

            VariationCoefficientLogNormalDistribution constructiveStrengthLinearLoadModel = structure.ConstructiveStrengthLinearLoadModel;
            Assert.AreEqual(2, constructiveStrengthLinearLoadModel.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(constructiveStrengthLinearLoadModel.Mean);
            Assert.AreEqual(2, constructiveStrengthLinearLoadModel.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, constructiveStrengthLinearLoadModel.CoefficientOfVariation, constructiveStrengthLinearLoadModel.CoefficientOfVariation.GetAccuracy());

            VariationCoefficientLogNormalDistribution constructiveStrengthQuadraticLoadModel = structure.ConstructiveStrengthQuadraticLoadModel;
            Assert.AreEqual(2, constructiveStrengthQuadraticLoadModel.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(constructiveStrengthQuadraticLoadModel.Mean);
            Assert.AreEqual(2, constructiveStrengthQuadraticLoadModel.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, constructiveStrengthQuadraticLoadModel.CoefficientOfVariation, constructiveStrengthQuadraticLoadModel.CoefficientOfVariation.GetAccuracy());

            NormalDistribution bankWidth = structure.BankWidth;
            Assert.AreEqual(2, bankWidth.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(bankWidth.Mean);
            Assert.AreEqual(2, bankWidth.StandardDeviation.NumberOfDecimalPlaces);
            Assert.IsNaN(bankWidth.StandardDeviation);

            NormalDistribution insideWaterLevelFailureConstruction = structure.InsideWaterLevelFailureConstruction;
            Assert.AreEqual(2, insideWaterLevelFailureConstruction.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(insideWaterLevelFailureConstruction.Mean);
            Assert.AreEqual(2, insideWaterLevelFailureConstruction.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, insideWaterLevelFailureConstruction.StandardDeviation, insideWaterLevelFailureConstruction.StandardDeviation.GetAccuracy());

            Assert.IsInstanceOf<RoundedDouble>(structure.EvaluationLevel);
            Assert.AreEqual(2, structure.EvaluationLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(0, structure.EvaluationLevel, structure.EvaluationLevel.GetAccuracy());

            NormalDistribution levelCrestStructure = structure.LevelCrestStructure;
            Assert.AreEqual(2, levelCrestStructure.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(levelCrestStructure.Mean);
            Assert.AreEqual(2, levelCrestStructure.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.05, levelCrestStructure.StandardDeviation, levelCrestStructure.StandardDeviation.GetAccuracy());

            Assert.IsInstanceOf<RoundedDouble>(structure.VerticalDistance);
            Assert.AreEqual(2, structure.VerticalDistance.NumberOfDecimalPlaces);
            Assert.IsNaN(structure.VerticalDistance);

            Assert.AreEqual(1, structure.FailureProbabilityRepairClosure);

            VariationCoefficientLogNormalDistribution failureCollisionEnergy = structure.FailureCollisionEnergy;
            Assert.AreEqual(2, failureCollisionEnergy.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(failureCollisionEnergy.Mean);
            Assert.AreEqual(2, failureCollisionEnergy.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.3, failureCollisionEnergy.CoefficientOfVariation, failureCollisionEnergy.CoefficientOfVariation.GetAccuracy());

            VariationCoefficientNormalDistribution shipMass = structure.ShipMass;
            Assert.AreEqual(2, shipMass.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(shipMass.Mean);
            Assert.AreEqual(2, shipMass.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.2, shipMass.CoefficientOfVariation, shipMass.CoefficientOfVariation.GetAccuracy());

            VariationCoefficientNormalDistribution shipVelocity = structure.ShipVelocity;
            Assert.AreEqual(2, shipVelocity.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(shipVelocity.Mean);
            Assert.AreEqual(2, shipVelocity.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.2, shipVelocity.CoefficientOfVariation, shipVelocity.CoefficientOfVariation.GetAccuracy());

            Assert.AreEqual(1, structure.LevellingCount);

            Assert.AreEqual(1, structure.ProbabilityCollisionSecondaryStructure);

            VariationCoefficientNormalDistribution flowVelocityStructureClosable = structure.FlowVelocityStructureClosable;
            Assert.AreEqual(2, flowVelocityStructureClosable.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(flowVelocityStructureClosable.Mean);
            Assert.AreEqual(2, flowVelocityStructureClosable.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.2, flowVelocityStructureClosable.CoefficientOfVariation, flowVelocityStructureClosable.CoefficientOfVariation.GetAccuracy());

            VariationCoefficientLogNormalDistribution stabilityLinearLoadModel = structure.StabilityLinearLoadModel;
            Assert.AreEqual(2, stabilityLinearLoadModel.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(stabilityLinearLoadModel.Mean);
            Assert.AreEqual(2, stabilityLinearLoadModel.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, stabilityLinearLoadModel.CoefficientOfVariation, stabilityLinearLoadModel.CoefficientOfVariation.GetAccuracy());

            VariationCoefficientLogNormalDistribution stabilityQuadraticLoadModel = structure.StabilityQuadraticLoadModel;
            Assert.AreEqual(2, stabilityQuadraticLoadModel.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(stabilityQuadraticLoadModel.Mean);
            Assert.AreEqual(2, stabilityQuadraticLoadModel.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, stabilityQuadraticLoadModel.CoefficientOfVariation, stabilityQuadraticLoadModel.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution areaFlowApertures = structure.AreaFlowApertures;
            Assert.AreEqual(2, areaFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(areaFlowApertures.Mean);
            Assert.AreEqual(2, areaFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.01, areaFlowApertures.StandardDeviation, areaFlowApertures.StandardDeviation.GetAccuracy());

            Assert.AreEqual(StabilityPointStructureInflowModelType.LowSill, structure.InflowModelType);
        }

        [Test]
        public void CopyProperties_FromStructureNull_ThrowsArgumentNullException()
        {
            // Setup
            var structure = new StabilityPointStructure(new StabilityPointStructure.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = new Point2D(0, 0)
            });

            // Call
            TestDelegate call = () => structure.CopyProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("fromStructure", paramName);
        }

        [Test]
        public void CopyProperties_FromStructure_UpdatesProperties()
        {
            // Setup
            var random = new Random(123);
            var structure = new StabilityPointStructure(new StabilityPointStructure.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = new Point2D(0, 0)
            });

            var otherStructure = new StabilityPointStructure(new StabilityPointStructure.ConstructionProperties
            {
                Name = "otherName",
                Id = "otherId",
                Location = new Point2D(1, 1),
                StorageStructureArea =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                WidthFlowApertures =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                InsideWaterLevel =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                ConstructiveStrengthLinearLoadModel =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                ConstructiveStrengthQuadraticLoadModel =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                BankWidth =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                InsideWaterLevelFailureConstruction =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                EvaluationLevel = random.NextDouble(),
                LevelCrestStructure =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                VerticalDistance = random.NextDouble(),
                FailureProbabilityRepairClosure = random.NextDouble(),
                FailureCollisionEnergy =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                ShipMass =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                ShipVelocity =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                LevellingCount = random.Next(),
                ProbabilityCollisionSecondaryStructure = random.NextDouble(),
                FlowVelocityStructureClosable =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                StabilityLinearLoadModel =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                StabilityQuadraticLoadModel =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                AreaFlowApertures =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                InflowModelType = random.NextEnumValue<StabilityPointStructureInflowModelType>()
            });

            // Call
            structure.CopyProperties(otherStructure);

            // Assert
            Assert.AreNotEqual(otherStructure.Id, structure.Id);
            Assert.AreEqual(otherStructure.Name, structure.Name);
            Assert.AreEqual(otherStructure.Location, structure.Location);
            Assert.AreEqual(otherStructure.StorageStructureArea, structure.StorageStructureArea);
            Assert.AreEqual(otherStructure.AllowedLevelIncreaseStorage, structure.AllowedLevelIncreaseStorage);
            Assert.AreEqual(otherStructure.WidthFlowApertures, structure.WidthFlowApertures);
            Assert.AreEqual(otherStructure.InsideWaterLevel, structure.InsideWaterLevel);
            Assert.AreEqual(otherStructure.ThresholdHeightOpenWeir, structure.ThresholdHeightOpenWeir);
            Assert.AreEqual(otherStructure.CriticalOvertoppingDischarge, structure.CriticalOvertoppingDischarge);
            Assert.AreEqual(otherStructure.FlowWidthAtBottomProtection, structure.FlowWidthAtBottomProtection);
            Assert.AreEqual(otherStructure.ConstructiveStrengthLinearLoadModel, structure.ConstructiveStrengthLinearLoadModel);
            Assert.AreEqual(otherStructure.ConstructiveStrengthQuadraticLoadModel, structure.ConstructiveStrengthQuadraticLoadModel);
            Assert.AreEqual(otherStructure.BankWidth, structure.BankWidth);
            Assert.AreEqual(otherStructure.InsideWaterLevelFailureConstruction, structure.InsideWaterLevelFailureConstruction);
            Assert.AreEqual(otherStructure.EvaluationLevel, structure.EvaluationLevel);
            Assert.AreEqual(otherStructure.LevelCrestStructure, structure.LevelCrestStructure);
            Assert.AreEqual(otherStructure.VerticalDistance, structure.VerticalDistance);
            Assert.AreEqual(otherStructure.FailureProbabilityRepairClosure, structure.FailureProbabilityRepairClosure);
            Assert.AreEqual(otherStructure.FailureCollisionEnergy, structure.FailureCollisionEnergy);
            Assert.AreEqual(otherStructure.ShipMass, structure.ShipMass);
            Assert.AreEqual(otherStructure.ShipVelocity, structure.ShipVelocity);
            Assert.AreEqual(otherStructure.LevellingCount, structure.LevellingCount);
            Assert.AreEqual(otherStructure.ProbabilityCollisionSecondaryStructure, structure.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(otherStructure.FlowVelocityStructureClosable, structure.FlowVelocityStructureClosable);
            Assert.AreEqual(otherStructure.StabilityLinearLoadModel, structure.StabilityLinearLoadModel);
            Assert.AreEqual(otherStructure.StabilityQuadraticLoadModel, structure.StabilityQuadraticLoadModel);
            Assert.AreEqual(otherStructure.AreaFlowApertures, structure.AreaFlowApertures);
            Assert.AreEqual(otherStructure.InflowModelType, structure.InflowModelType);
        }

        [Test]
        [TestCase(null)]
        [TestCase("string")]
        public void Equals_ToDifferentTypeOrNull_ReturnsFalse(object other)
        {
            // Setup
            StabilityPointStructure structure = new TestStabilityPointStructure();

            // Call
            bool isEqual = structure.Equals(other);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            StabilityPointStructure structure = new TestStabilityPointStructure();

            // Call
            bool isEqual = structure.Equals(structure);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_TransitivePropertyAllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            StabilityPointStructure structureX = new TestStabilityPointStructure();
            StabilityPointStructure structureY = new TestStabilityPointStructure();
            StabilityPointStructure structureZ = new TestStabilityPointStructure();

            // Call
            bool isXEqualToY = structureX.Equals(structureY);
            bool isYEqualToZ = structureY.Equals(structureZ);
            bool isXEqualToZ = structureX.Equals(structureZ);

            // Assert
            Assert.IsTrue(isXEqualToY);
            Assert.IsTrue(isYEqualToZ);
            Assert.IsTrue(isXEqualToZ);
        }

        [Test]
        [TestCaseSource(typeof(StabilityPointStructurePermutationHelper),
            nameof(StabilityPointStructurePermutationHelper.DifferentStabilityPointStructures),
            new object[]
            {
                "Equals",
                "ReturnsFalse"
            })]
        public void Equals_DifferentProperty_ReturnsFalse(StabilityPointStructure structure)
        {
            // Call
            bool isEqual = structure.Equals(new TestStabilityPointStructure());

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void GetHashCode_EqualStructures_ReturnsSameHashCode()
        {
            // Setup
            StabilityPointStructure structureOne = new TestStabilityPointStructure();
            StabilityPointStructure structureTwo = new TestStabilityPointStructure();

            // Call
            int hashCodeOne = structureOne.GetHashCode();
            int hashCodeTwo = structureTwo.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }
    }
}