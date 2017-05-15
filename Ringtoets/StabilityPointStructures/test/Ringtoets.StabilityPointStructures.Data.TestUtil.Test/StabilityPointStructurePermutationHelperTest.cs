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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ringtoets.StabilityPointStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class StabilityPointStructurePermutationHelperTest
    {
        [Test]
        public void DifferentStabilityPointStructures_ReturnsExpectedTestCaseData()
        {
            // Setup
            var referenceStructure = new TestStabilityPointStructure();
            var differentStructures = new List<StabilityPointStructure>();

            // Call
            List<TestCaseData> testCaseDatas = StabilityPointStructurePermutationHelper.DifferentStabilityPointStructures("A", "B").ToList();

            // Assert
            Assert.AreEqual(28, testCaseDatas.Count);
            IEnumerable<string> testNames = testCaseDatas
                .Select(tcd => tcd.TestName)
                .ToList();
            Assert.AreEqual(28, testNames.Distinct().Count());
            Assert.IsTrue(testNames.All(tn => tn.StartsWith("A_")));
            Assert.IsTrue(testNames.All(tn => tn.EndsWith("_B")));
            IEnumerable<StabilityPointStructure> structures = testCaseDatas
                .Select(tcd => tcd.Arguments[0])
                .OfType<StabilityPointStructure>()
                .ToList();
            Assert.AreEqual(28, structures.Count());
            differentStructures.Add(structures.Single(s => !s.Id.Equals(referenceStructure.Id)));
            differentStructures.Add(structures.Single(s => !s.Name.Equals(referenceStructure.Name)));
            differentStructures.Add(structures.Single(s => !s.Location.Equals(referenceStructure.Location)));
            differentStructures.Add(structures.Single(s => !s.StorageStructureArea.Equals(referenceStructure.StorageStructureArea)));
            differentStructures.Add(structures.Single(s => !s.AllowedLevelIncreaseStorage.Equals(referenceStructure.AllowedLevelIncreaseStorage)));
            differentStructures.Add(structures.Single(s => !s.WidthFlowApertures.Equals(referenceStructure.WidthFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.InsideWaterLevel.Equals(referenceStructure.InsideWaterLevel)));
            differentStructures.Add(structures.Single(s => !s.ThresholdHeightOpenWeir.Equals(referenceStructure.ThresholdHeightOpenWeir)));
            differentStructures.Add(structures.Single(s => !s.CriticalOvertoppingDischarge.Equals(referenceStructure.CriticalOvertoppingDischarge)));
            differentStructures.Add(structures.Single(s => !s.FlowWidthAtBottomProtection.Equals(referenceStructure.FlowWidthAtBottomProtection)));
            differentStructures.Add(structures.Single(s => !s.ConstructiveStrengthLinearLoadModel.Equals(referenceStructure.ConstructiveStrengthLinearLoadModel)));
            differentStructures.Add(structures.Single(s => !s.ConstructiveStrengthQuadraticLoadModel.Equals(referenceStructure.ConstructiveStrengthQuadraticLoadModel)));
            differentStructures.Add(structures.Single(s => !s.BankWidth.Equals(referenceStructure.BankWidth)));
            differentStructures.Add(structures.Single(s => !s.InsideWaterLevelFailureConstruction.Equals(referenceStructure.InsideWaterLevelFailureConstruction)));
            differentStructures.Add(structures.Single(s => !s.EvaluationLevel.Equals(referenceStructure.EvaluationLevel)));
            differentStructures.Add(structures.Single(s => !s.LevelCrestStructure.Equals(referenceStructure.LevelCrestStructure)));
            differentStructures.Add(structures.Single(s => !s.VerticalDistance.Equals(referenceStructure.VerticalDistance)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityRepairClosure.Equals(referenceStructure.FailureProbabilityRepairClosure)));
            differentStructures.Add(structures.Single(s => !s.FailureCollisionEnergy.Equals(referenceStructure.FailureCollisionEnergy)));
            differentStructures.Add(structures.Single(s => !s.ShipMass.Equals(referenceStructure.ShipMass)));
            differentStructures.Add(structures.Single(s => !s.ShipVelocity.Equals(referenceStructure.ShipVelocity)));
            differentStructures.Add(structures.Single(s => !s.LevellingCount.Equals(referenceStructure.LevellingCount)));
            differentStructures.Add(structures.Single(s => !s.ProbabilityCollisionSecondaryStructure.Equals(referenceStructure.ProbabilityCollisionSecondaryStructure)));
            differentStructures.Add(structures.Single(s => !s.FlowVelocityStructureClosable.Equals(referenceStructure.FlowVelocityStructureClosable)));
            differentStructures.Add(structures.Single(s => !s.StabilityLinearLoadModel.Equals(referenceStructure.StabilityLinearLoadModel)));
            differentStructures.Add(structures.Single(s => !s.StabilityQuadraticLoadModel.Equals(referenceStructure.StabilityQuadraticLoadModel)));
            differentStructures.Add(structures.Single(s => !s.AreaFlowApertures.Equals(referenceStructure.AreaFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.InflowModelType.Equals(referenceStructure.InflowModelType)));
            Assert.AreEqual(28, differentStructures.Distinct().Count());
        }

        [Test]
        public void DifferentStabilityPointStructuresWithSameId_ReturnsExpectedTestCaseData()
        {
            // Setup
            var referenceStructure = new TestStabilityPointStructure();
            var differentStructures = new List<StabilityPointStructure>();

            // Call
            List<TestCaseData> testCaseDatas = StabilityPointStructurePermutationHelper.DifferentStabilityPointStructuresWithSameId("A", "B").ToList();

            // Assert
            Assert.AreEqual(27, testCaseDatas.Count);
            IEnumerable<string> testNames = testCaseDatas
                .Select(tcd => tcd.TestName)
                .ToList();
            Assert.AreEqual(27, testNames.Distinct().Count());
            Assert.IsTrue(testNames.All(tn => tn.StartsWith("A_")));
            Assert.IsTrue(testNames.All(tn => tn.EndsWith("_B")));
            IEnumerable<StabilityPointStructure> structures = testCaseDatas
                .Select(tcd => tcd.Arguments[0])
                .OfType<StabilityPointStructure>()
                .ToList();
            Assert.AreEqual(27, structures.Count());
            Assert.IsTrue(structures.All(s => s.Id == referenceStructure.Id));
            differentStructures.Add(structures.Single(s => !s.Name.Equals(referenceStructure.Name)));
            differentStructures.Add(structures.Single(s => !s.Location.Equals(referenceStructure.Location)));
            differentStructures.Add(structures.Single(s => !s.StorageStructureArea.Equals(referenceStructure.StorageStructureArea)));
            differentStructures.Add(structures.Single(s => !s.AllowedLevelIncreaseStorage.Equals(referenceStructure.AllowedLevelIncreaseStorage)));
            differentStructures.Add(structures.Single(s => !s.WidthFlowApertures.Equals(referenceStructure.WidthFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.InsideWaterLevel.Equals(referenceStructure.InsideWaterLevel)));
            differentStructures.Add(structures.Single(s => !s.ThresholdHeightOpenWeir.Equals(referenceStructure.ThresholdHeightOpenWeir)));
            differentStructures.Add(structures.Single(s => !s.CriticalOvertoppingDischarge.Equals(referenceStructure.CriticalOvertoppingDischarge)));
            differentStructures.Add(structures.Single(s => !s.FlowWidthAtBottomProtection.Equals(referenceStructure.FlowWidthAtBottomProtection)));
            differentStructures.Add(structures.Single(s => !s.ConstructiveStrengthLinearLoadModel.Equals(referenceStructure.ConstructiveStrengthLinearLoadModel)));
            differentStructures.Add(structures.Single(s => !s.ConstructiveStrengthQuadraticLoadModel.Equals(referenceStructure.ConstructiveStrengthQuadraticLoadModel)));
            differentStructures.Add(structures.Single(s => !s.BankWidth.Equals(referenceStructure.BankWidth)));
            differentStructures.Add(structures.Single(s => !s.InsideWaterLevelFailureConstruction.Equals(referenceStructure.InsideWaterLevelFailureConstruction)));
            differentStructures.Add(structures.Single(s => !s.EvaluationLevel.Equals(referenceStructure.EvaluationLevel)));
            differentStructures.Add(structures.Single(s => !s.LevelCrestStructure.Equals(referenceStructure.LevelCrestStructure)));
            differentStructures.Add(structures.Single(s => !s.VerticalDistance.Equals(referenceStructure.VerticalDistance)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityRepairClosure.Equals(referenceStructure.FailureProbabilityRepairClosure)));
            differentStructures.Add(structures.Single(s => !s.FailureCollisionEnergy.Equals(referenceStructure.FailureCollisionEnergy)));
            differentStructures.Add(structures.Single(s => !s.ShipMass.Equals(referenceStructure.ShipMass)));
            differentStructures.Add(structures.Single(s => !s.ShipVelocity.Equals(referenceStructure.ShipVelocity)));
            differentStructures.Add(structures.Single(s => !s.LevellingCount.Equals(referenceStructure.LevellingCount)));
            differentStructures.Add(structures.Single(s => !s.ProbabilityCollisionSecondaryStructure.Equals(referenceStructure.ProbabilityCollisionSecondaryStructure)));
            differentStructures.Add(structures.Single(s => !s.FlowVelocityStructureClosable.Equals(referenceStructure.FlowVelocityStructureClosable)));
            differentStructures.Add(structures.Single(s => !s.StabilityLinearLoadModel.Equals(referenceStructure.StabilityLinearLoadModel)));
            differentStructures.Add(structures.Single(s => !s.StabilityQuadraticLoadModel.Equals(referenceStructure.StabilityQuadraticLoadModel)));
            differentStructures.Add(structures.Single(s => !s.AreaFlowApertures.Equals(referenceStructure.AreaFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.InflowModelType.Equals(referenceStructure.InflowModelType)));
            Assert.AreEqual(27, differentStructures.Distinct().Count());
        }

        [Test]
        public void DifferentStabilityPointStructuresWithSameIdNameAndLocation_ReturnsExpectedTestCaseData()
        {
            // Setup
            var referenceStructure = new TestStabilityPointStructure();
            var differentStructures = new List<StabilityPointStructure>();

            // Call
            List<TestCaseData> testCaseDatas = StabilityPointStructurePermutationHelper.DifferentStabilityPointStructuresWithSameIdNameAndLocation("C", "D").ToList();

            // Assert
            Assert.AreEqual(25, testCaseDatas.Count);
            IEnumerable<string> testNames = testCaseDatas
                .Select(tcd => tcd.TestName)
                .ToList();
            Assert.AreEqual(25, testNames.Distinct().Count());
            Assert.IsTrue(testNames.All(tn => tn.StartsWith("C_")));
            Assert.IsTrue(testNames.All(tn => tn.EndsWith("_D")));
            IEnumerable<StabilityPointStructure> structures = testCaseDatas
                .Select(tcd => tcd.Arguments[0])
                .OfType<StabilityPointStructure>()
                .ToList();
            Assert.AreEqual(25, structures.Count());
            Assert.IsTrue(structures.All(s => s.Id == referenceStructure.Id));
            Assert.IsTrue(structures.All(s => s.Name == referenceStructure.Name));
            Assert.IsTrue(structures.All(s => s.Location.Equals(referenceStructure.Location)));
            differentStructures.Add(structures.Single(s => !s.StorageStructureArea.Equals(referenceStructure.StorageStructureArea)));
            differentStructures.Add(structures.Single(s => !s.AllowedLevelIncreaseStorage.Equals(referenceStructure.AllowedLevelIncreaseStorage)));
            differentStructures.Add(structures.Single(s => !s.WidthFlowApertures.Equals(referenceStructure.WidthFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.InsideWaterLevel.Equals(referenceStructure.InsideWaterLevel)));
            differentStructures.Add(structures.Single(s => !s.ThresholdHeightOpenWeir.Equals(referenceStructure.ThresholdHeightOpenWeir)));
            differentStructures.Add(structures.Single(s => !s.CriticalOvertoppingDischarge.Equals(referenceStructure.CriticalOvertoppingDischarge)));
            differentStructures.Add(structures.Single(s => !s.FlowWidthAtBottomProtection.Equals(referenceStructure.FlowWidthAtBottomProtection)));
            differentStructures.Add(structures.Single(s => !s.ConstructiveStrengthLinearLoadModel.Equals(referenceStructure.ConstructiveStrengthLinearLoadModel)));
            differentStructures.Add(structures.Single(s => !s.ConstructiveStrengthQuadraticLoadModel.Equals(referenceStructure.ConstructiveStrengthQuadraticLoadModel)));
            differentStructures.Add(structures.Single(s => !s.BankWidth.Equals(referenceStructure.BankWidth)));
            differentStructures.Add(structures.Single(s => !s.InsideWaterLevelFailureConstruction.Equals(referenceStructure.InsideWaterLevelFailureConstruction)));
            differentStructures.Add(structures.Single(s => !s.EvaluationLevel.Equals(referenceStructure.EvaluationLevel)));
            differentStructures.Add(structures.Single(s => !s.LevelCrestStructure.Equals(referenceStructure.LevelCrestStructure)));
            differentStructures.Add(structures.Single(s => !s.VerticalDistance.Equals(referenceStructure.VerticalDistance)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityRepairClosure.Equals(referenceStructure.FailureProbabilityRepairClosure)));
            differentStructures.Add(structures.Single(s => !s.FailureCollisionEnergy.Equals(referenceStructure.FailureCollisionEnergy)));
            differentStructures.Add(structures.Single(s => !s.ShipMass.Equals(referenceStructure.ShipMass)));
            differentStructures.Add(structures.Single(s => !s.ShipVelocity.Equals(referenceStructure.ShipVelocity)));
            differentStructures.Add(structures.Single(s => !s.LevellingCount.Equals(referenceStructure.LevellingCount)));
            differentStructures.Add(structures.Single(s => !s.ProbabilityCollisionSecondaryStructure.Equals(referenceStructure.ProbabilityCollisionSecondaryStructure)));
            differentStructures.Add(structures.Single(s => !s.FlowVelocityStructureClosable.Equals(referenceStructure.FlowVelocityStructureClosable)));
            differentStructures.Add(structures.Single(s => !s.StabilityLinearLoadModel.Equals(referenceStructure.StabilityLinearLoadModel)));
            differentStructures.Add(structures.Single(s => !s.StabilityQuadraticLoadModel.Equals(referenceStructure.StabilityQuadraticLoadModel)));
            differentStructures.Add(structures.Single(s => !s.AreaFlowApertures.Equals(referenceStructure.AreaFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.InflowModelType.Equals(referenceStructure.InflowModelType)));
            Assert.AreEqual(25, differentStructures.Distinct().Count());
        }
    }
}