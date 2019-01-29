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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.StabilityPointStructures.Data.Test
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
            TestHelper.AssertAreEqualButNotSame(otherStructure.Location, structure.Location);
            TestHelper.AssertAreEqualButNotSame(otherStructure.StorageStructureArea, structure.StorageStructureArea);
            TestHelper.AssertAreEqualButNotSame(otherStructure.AllowedLevelIncreaseStorage, structure.AllowedLevelIncreaseStorage);
            TestHelper.AssertAreEqualButNotSame(otherStructure.WidthFlowApertures, structure.WidthFlowApertures);
            TestHelper.AssertAreEqualButNotSame(otherStructure.InsideWaterLevel, structure.InsideWaterLevel);
            TestHelper.AssertAreEqualButNotSame(otherStructure.ThresholdHeightOpenWeir, structure.ThresholdHeightOpenWeir);
            TestHelper.AssertAreEqualButNotSame(otherStructure.CriticalOvertoppingDischarge, structure.CriticalOvertoppingDischarge);
            TestHelper.AssertAreEqualButNotSame(otherStructure.FlowWidthAtBottomProtection, structure.FlowWidthAtBottomProtection);
            TestHelper.AssertAreEqualButNotSame(otherStructure.ConstructiveStrengthLinearLoadModel, structure.ConstructiveStrengthLinearLoadModel);
            TestHelper.AssertAreEqualButNotSame(otherStructure.ConstructiveStrengthQuadraticLoadModel, structure.ConstructiveStrengthQuadraticLoadModel);
            TestHelper.AssertAreEqualButNotSame(otherStructure.BankWidth, structure.BankWidth);
            TestHelper.AssertAreEqualButNotSame(otherStructure.InsideWaterLevelFailureConstruction, structure.InsideWaterLevelFailureConstruction);
            Assert.AreEqual(otherStructure.EvaluationLevel, structure.EvaluationLevel);
            TestHelper.AssertAreEqualButNotSame(otherStructure.LevelCrestStructure, structure.LevelCrestStructure);
            Assert.AreEqual(otherStructure.VerticalDistance, structure.VerticalDistance);
            Assert.AreEqual(otherStructure.FailureProbabilityRepairClosure, structure.FailureProbabilityRepairClosure);
            TestHelper.AssertAreEqualButNotSame(otherStructure.FailureCollisionEnergy, structure.FailureCollisionEnergy);
            TestHelper.AssertAreEqualButNotSame(otherStructure.ShipMass, structure.ShipMass);
            TestHelper.AssertAreEqualButNotSame(otherStructure.ShipVelocity, structure.ShipVelocity);
            Assert.AreEqual(otherStructure.LevellingCount, structure.LevellingCount);
            Assert.AreEqual(otherStructure.ProbabilityCollisionSecondaryStructure, structure.ProbabilityCollisionSecondaryStructure);
            TestHelper.AssertAreEqualButNotSame(otherStructure.FlowVelocityStructureClosable, structure.FlowVelocityStructureClosable);
            TestHelper.AssertAreEqualButNotSame(otherStructure.StabilityLinearLoadModel, structure.StabilityLinearLoadModel);
            TestHelper.AssertAreEqualButNotSame(otherStructure.StabilityQuadraticLoadModel, structure.StabilityQuadraticLoadModel);
            TestHelper.AssertAreEqualButNotSame(otherStructure.AreaFlowApertures, structure.AreaFlowApertures);
            Assert.AreEqual(otherStructure.InflowModelType, structure.InflowModelType);
        }

        [TestFixture]
        private class StabilityPointStructureEqualsTest : EqualsTestFixture<StabilityPointStructure, DerivedStabilityPointStructure>
        {
            protected override StabilityPointStructure CreateObject()
            {
                return new StabilityPointStructure(CreateConstructionProperties());
            }

            protected override DerivedStabilityPointStructure CreateDerivedObject()
            {
                return new DerivedStabilityPointStructure(CreateConstructionProperties());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                foreach (ChangePropertyData<StabilityPointStructure.ConstructionProperties> changeSingleDataProperty in ChangeSingleDataProperties())
                {
                    StabilityPointStructure.ConstructionProperties differentConstructionProperties = CreateConstructionProperties();
                    changeSingleDataProperty.ActionToChangeProperty(differentConstructionProperties);

                    yield return new TestCaseData(new StabilityPointStructure(differentConstructionProperties))
                        .SetName(changeSingleDataProperty.PropertyName);
                }
            }

            private static IEnumerable<ChangePropertyData<StabilityPointStructure.ConstructionProperties>> ChangeSingleDataProperties()
            {
                var random = new Random(21);
                RoundedDouble offset = random.NextRoundedDouble();

                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.Name = "Different Name",
                                                                                                    "Name");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.Id = "Different Id",
                                                                                                    "Id");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.Location = new Point2D(random.NextDouble(), random.NextDouble()),
                                                                                                    "Location");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.StructureNormalOrientation = random.NextRoundedDouble(),
                                                                                                    "NormalOrientation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.StorageStructureArea.Mean = cp.StorageStructureArea.Mean + offset,
                                                                                                    "StorageStructureAreaMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.StorageStructureArea.CoefficientOfVariation = cp.StorageStructureArea.CoefficientOfVariation + offset,
                                                                                                    "StorageStructureAreaCoefficientOfVariation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.AllowedLevelIncreaseStorage.Mean = cp.AllowedLevelIncreaseStorage.Mean + offset,
                                                                                                    "AllowedLevelIncreaseStorageMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.AllowedLevelIncreaseStorage.StandardDeviation = cp.AllowedLevelIncreaseStorage.StandardDeviation + offset,
                                                                                                    "AllowedLevelIncreaseStorageStandardDeviation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.WidthFlowApertures.Mean = cp.WidthFlowApertures.Mean + offset,
                                                                                                    "WidthFlowAperturesMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.WidthFlowApertures.StandardDeviation = cp.WidthFlowApertures.StandardDeviation + offset,
                                                                                                    "WidthFlowAperturesStandardDeviation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.InsideWaterLevel.Mean = cp.InsideWaterLevel.Mean + offset,
                                                                                                    "InsideWaterLevelMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.InsideWaterLevel.StandardDeviation = cp.InsideWaterLevel.StandardDeviation + offset,
                                                                                                    "InsideWaterLevelStandardDeviation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ThresholdHeightOpenWeir.Mean = cp.ThresholdHeightOpenWeir.Mean + offset,
                                                                                                    "ThresholdHeightOpenWeirMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ThresholdHeightOpenWeir.StandardDeviation = cp.ThresholdHeightOpenWeir.StandardDeviation + offset,
                                                                                                    "ThresholdHeightOpenWeirStandardDeviation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.CriticalOvertoppingDischarge.Mean = cp.CriticalOvertoppingDischarge.Mean + offset,
                                                                                                    "CriticalOvertoppingDischargeMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.CriticalOvertoppingDischarge.CoefficientOfVariation = cp.CriticalOvertoppingDischarge.CoefficientOfVariation + offset,
                                                                                                    "CriticalOvertoppingDischargeCoefficientOfVariation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.FlowWidthAtBottomProtection.Mean = cp.FlowWidthAtBottomProtection.Mean + offset,
                                                                                                    "FlowWidthAtBottomProtectionMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.FlowWidthAtBottomProtection.StandardDeviation = cp.FlowWidthAtBottomProtection.StandardDeviation + offset,
                                                                                                    "FlowWidthAtBottomProtectionStandardDeviation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ConstructiveStrengthLinearLoadModel.Mean = cp.ConstructiveStrengthLinearLoadModel.Mean + offset,
                                                                                                    "ConstructiveStrengthLinearLoadModelMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation = cp.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation + offset,
                                                                                                    "ConstructiveStrengthLinearLoadModelCoefficientOfVariation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ConstructiveStrengthQuadraticLoadModel.Mean = cp.ConstructiveStrengthQuadraticLoadModel.Mean + offset,
                                                                                                    "ConstructiveStrengthQuadraticLoadModelMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation = cp.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation + offset,
                                                                                                    "ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.BankWidth.Mean = cp.BankWidth.Mean + offset,
                                                                                                    "BankWidthMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.BankWidth.StandardDeviation = cp.BankWidth.StandardDeviation + offset,
                                                                                                    "BankWidthStandardDeviation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.InsideWaterLevelFailureConstruction.Mean = cp.InsideWaterLevelFailureConstruction.Mean + offset,
                                                                                                    "InsideWaterLevelFailureConstructionMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.InsideWaterLevelFailureConstruction.StandardDeviation = cp.InsideWaterLevelFailureConstruction.StandardDeviation + offset,
                                                                                                    "InsideWaterLevelFailureConstructionStandardDeviation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.EvaluationLevel = cp.EvaluationLevel + offset,
                                                                                                    "EvaluationLevel");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.LevelCrestStructure.Mean = cp.LevelCrestStructure.Mean + offset,
                                                                                                    "LevelCrestStructureMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.LevelCrestStructure.StandardDeviation = cp.LevelCrestStructure.StandardDeviation + offset,
                                                                                                    "LevelCrestStructureStandardDeviation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.VerticalDistance = cp.VerticalDistance + offset,
                                                                                                    "VerticalDistance");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.FailureProbabilityRepairClosure = random.NextDouble(),
                                                                                                    "FailureProbabilityRepairClosure");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.FailureCollisionEnergy.Mean = cp.FailureCollisionEnergy.Mean + offset,
                                                                                                    "FailureCollisionEnergyMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.FailureCollisionEnergy.CoefficientOfVariation = cp.FailureCollisionEnergy.CoefficientOfVariation + offset,
                                                                                                    "FailureCollisionEnergyCoefficientOfVariation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ShipMass.Mean = cp.ShipMass.Mean + offset,
                                                                                                    "ShipMassMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ShipMass.CoefficientOfVariation = cp.ShipMass.CoefficientOfVariation + offset,
                                                                                                    "ShipMassCoefficientOfVariation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ShipVelocity.Mean = cp.ShipVelocity.Mean + offset,
                                                                                                    "ShipVelocityMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ShipVelocity.CoefficientOfVariation = cp.ShipVelocity.CoefficientOfVariation + offset,
                                                                                                    "ShipVelocityCoefficientOfVariation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.LevellingCount = random.Next(43, int.MaxValue),
                                                                                                    "LevellingCount");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.ProbabilityCollisionSecondaryStructure = random.NextDouble(),
                                                                                                    "ProbabilityCollisionSecondaryStructure");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.FlowVelocityStructureClosable.Mean = cp.FlowVelocityStructureClosable.Mean + offset,
                                                                                                    "FlowVelocityStructureClosableMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.StabilityLinearLoadModel.Mean = cp.StabilityLinearLoadModel.Mean + offset,
                                                                                                    "StabilityLinearLoadModelMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.StabilityLinearLoadModel.CoefficientOfVariation = cp.StabilityLinearLoadModel.CoefficientOfVariation + offset,
                                                                                                    "StabilityLinearLoadModelCoefficientOfVariation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.StabilityQuadraticLoadModel.Mean = cp.StabilityQuadraticLoadModel.Mean + offset,
                                                                                                    "StabilityQuadraticLoadModelMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.StabilityQuadraticLoadModel.CoefficientOfVariation = cp.StabilityQuadraticLoadModel.CoefficientOfVariation + offset,
                                                                                                    "StabilityQuadraticLoadModelCoefficientOfVariation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.AreaFlowApertures.Mean = cp.AreaFlowApertures.Mean + offset,
                                                                                                    "AreaFlowAperturesMean");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.AreaFlowApertures.StandardDeviation = cp.AreaFlowApertures.StandardDeviation + offset,
                                                                                                    "AreaFlowAperturesStandardDeviation");
                yield return new ChangePropertyData<StabilityPointStructure.ConstructionProperties>(cp => cp.InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                                                                                                    "InflowModelType");
            }

            private static StabilityPointStructure.ConstructionProperties CreateConstructionProperties()
            {
                return new StabilityPointStructure.ConstructionProperties
                {
                    Name = "Name",
                    Id = "id",
                    Location = new Point2D(1.234, 2.3456),
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
                };
            }
        }

        private class DerivedStabilityPointStructure : StabilityPointStructure
        {
            public DerivedStabilityPointStructure(ConstructionProperties constructionProperties) : base(constructionProperties) {}
        }
    }
}