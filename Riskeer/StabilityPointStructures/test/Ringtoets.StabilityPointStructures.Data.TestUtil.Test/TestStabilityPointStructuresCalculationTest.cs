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

using NUnit.Framework;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class TestStabilityPointStructuresCalculationTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Setup
            var referenceStructure = new TestStabilityPointStructure();

            // Call 
            var calculation = new TestStabilityPointStructuresCalculation();

            // Assert
            Assert.IsInstanceOf<StructuresCalculation<StabilityPointStructuresInput>>(calculation);
            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsNull(calculation.Comments.Body);
            Assert.IsFalse(calculation.HasOutput);

            Assert.IsNotNull(calculation.InputParameters.Structure);
            Assert.IsNotNull(calculation.InputParameters.HydraulicBoundaryLocation);

            DistributionAssert.AreEqual(referenceStructure.StorageStructureArea, calculation.InputParameters.StorageStructureArea);
            DistributionAssert.AreEqual(referenceStructure.AllowedLevelIncreaseStorage, calculation.InputParameters.AllowedLevelIncreaseStorage);
            Assert.AreEqual(123.456, calculation.InputParameters.StructureNormalOrientation, calculation.InputParameters.StructureNormalOrientation.GetAccuracy());
            DistributionAssert.AreEqual(referenceStructure.WidthFlowApertures, calculation.InputParameters.WidthFlowApertures);
            DistributionAssert.AreEqual(referenceStructure.InsideWaterLevel, calculation.InputParameters.InsideWaterLevel);
            DistributionAssert.AreEqual(referenceStructure.ThresholdHeightOpenWeir, calculation.InputParameters.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(referenceStructure.AreaFlowApertures, calculation.InputParameters.AreaFlowApertures);
            DistributionAssert.AreEqual(referenceStructure.CriticalOvertoppingDischarge, calculation.InputParameters.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(referenceStructure.FlowWidthAtBottomProtection, calculation.InputParameters.FlowWidthAtBottomProtection);
            Assert.AreEqual(referenceStructure.InflowModelType, calculation.InputParameters.InflowModelType);
            DistributionAssert.AreEqual(referenceStructure.ConstructiveStrengthLinearLoadModel, calculation.InputParameters.ConstructiveStrengthLinearLoadModel);
            DistributionAssert.AreEqual(referenceStructure.ConstructiveStrengthQuadraticLoadModel, calculation.InputParameters.ConstructiveStrengthQuadraticLoadModel);
            DistributionAssert.AreEqual(referenceStructure.BankWidth, calculation.InputParameters.BankWidth);
            DistributionAssert.AreEqual(referenceStructure.InsideWaterLevelFailureConstruction, calculation.InputParameters.InsideWaterLevelFailureConstruction);
            Assert.AreEqual(referenceStructure.EvaluationLevel, calculation.InputParameters.EvaluationLevel);
            DistributionAssert.AreEqual(referenceStructure.LevelCrestStructure, calculation.InputParameters.LevelCrestStructure);
            Assert.AreEqual(referenceStructure.VerticalDistance, calculation.InputParameters.VerticalDistance);
            Assert.AreEqual(referenceStructure.FailureProbabilityRepairClosure, calculation.InputParameters.FailureProbabilityRepairClosure);
            DistributionAssert.AreEqual(referenceStructure.FailureCollisionEnergy, calculation.InputParameters.FailureCollisionEnergy);
            DistributionAssert.AreEqual(referenceStructure.ShipMass, calculation.InputParameters.ShipMass);
            DistributionAssert.AreEqual(referenceStructure.ShipVelocity, calculation.InputParameters.ShipVelocity);
            Assert.AreEqual(referenceStructure.LevellingCount, calculation.InputParameters.LevellingCount);
            Assert.AreEqual(referenceStructure.ProbabilityCollisionSecondaryStructure, calculation.InputParameters.ProbabilityCollisionSecondaryStructure);
            DistributionAssert.AreEqual(referenceStructure.FlowVelocityStructureClosable, calculation.InputParameters.FlowVelocityStructureClosable);
            DistributionAssert.AreEqual(referenceStructure.StabilityLinearLoadModel, calculation.InputParameters.StabilityLinearLoadModel);
            DistributionAssert.AreEqual(referenceStructure.StabilityQuadraticLoadModel, calculation.InputParameters.StabilityQuadraticLoadModel);
            Assert.AreEqual(referenceStructure.InflowModelType, calculation.InputParameters.InflowModelType);
            Assert.AreEqual(1.00, calculation.InputParameters.FactorStormDurationOpenStructure, calculation.InputParameters.FactorStormDurationOpenStructure.GetAccuracy());
            Assert.AreEqual(1.0, calculation.InputParameters.FailureProbabilityStructureWithErosion);
        }
    }
}