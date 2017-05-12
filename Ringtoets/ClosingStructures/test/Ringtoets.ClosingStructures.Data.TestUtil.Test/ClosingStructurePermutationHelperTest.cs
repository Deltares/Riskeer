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

namespace Ringtoets.ClosingStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class ClosingStructurePermutationHelperTest
    {
        [Test]
        public void DifferentClosingStructureWithSameId_ReturnsExpectedTestCaseData()
        {
            // Setup
            var referenceStructure = new TestClosingStructure();
            var differentStructures = new List<ClosingStructure>();

            // Call
            List<TestCaseData> testCaseDatas = ClosingStructurePermutationHelper.DifferentClosingStructuresWithSameId("A", "B").ToList();

            // Assert
            Assert.AreEqual(16, testCaseDatas.Count);
            IEnumerable<string> testNames = testCaseDatas
                .Select(tcd => tcd.TestName)
                .ToList();
            Assert.AreEqual(16, testNames.Distinct().Count());
            Assert.IsTrue(testNames.All(tn => tn.StartsWith("A_")));
            Assert.IsTrue(testNames.All(tn => tn.EndsWith("_B")));
            IEnumerable<ClosingStructure> structures = testCaseDatas
                .Select(tcd => tcd.Arguments[0])
                .OfType<ClosingStructure>()
                .ToList();
            Assert.AreEqual(16, structures.Count());
            Assert.IsTrue(structures.All(s => s.Id == structures.First().Id));
            differentStructures.Add(structures.Single(s => !s.Name.Equals(referenceStructure.Name)));
            differentStructures.Add(structures.Single(s => !s.Location.Equals(referenceStructure.Location)));
            differentStructures.Add(structures.Single(s => !s.AreaFlowApertures.Equals(referenceStructure.AreaFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.CriticalOvertoppingDischarge.Equals(referenceStructure.CriticalOvertoppingDischarge)));
            differentStructures.Add(structures.Single(s => !s.FlowWidthAtBottomProtection.Equals(referenceStructure.FlowWidthAtBottomProtection)));
            differentStructures.Add(structures.Single(s => !s.InsideWaterLevel.Equals(referenceStructure.InsideWaterLevel)));
            differentStructures.Add(structures.Single(s => !s.LevelCrestStructureNotClosing.Equals(referenceStructure.LevelCrestStructureNotClosing)));
            differentStructures.Add(structures.Single(s => !s.StorageStructureArea.Equals(referenceStructure.StorageStructureArea)));
            differentStructures.Add(structures.Single(s => !s.ThresholdHeightOpenWeir.Equals(referenceStructure.ThresholdHeightOpenWeir)));
            differentStructures.Add(structures.Single(s => !s.WidthFlowApertures.Equals(referenceStructure.WidthFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityReparation.Equals(referenceStructure.FailureProbabilityReparation)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityOpenStructure.Equals(referenceStructure.FailureProbabilityOpenStructure)));
            differentStructures.Add(structures.Single(s => !s.IdenticalApertures.Equals(referenceStructure.IdenticalApertures)));
            differentStructures.Add(structures.Single(s => !s.InflowModelType.Equals(referenceStructure.InflowModelType)));
            differentStructures.Add(structures.Single(s => !s.ProbabilityOrFrequencyOpenStructureBeforeFlooding.Equals(referenceStructure.ProbabilityOrFrequencyOpenStructureBeforeFlooding)));
            differentStructures.Add(structures.Single(s => !s.StructureNormalOrientation.Equals(referenceStructure.StructureNormalOrientation)));
            Assert.AreEqual(16, differentStructures.Distinct().Count());
        }

        [Test]
        public void DifferentClosingStructuresWithSameIdNameAndLocation_ReturnsExpectedTestCaseData()
        {
            // Setup
            var referenceStructure = new TestClosingStructure();
            var differentStructures = new List<ClosingStructure>();

            // Call
            List<TestCaseData> testCaseDatas = ClosingStructurePermutationHelper.DifferentClosingStructuresWithSameIdNameAndLocation("C", "D").ToList();

            // Assert
            Assert.AreEqual(14, testCaseDatas.Count);
            IEnumerable<string> testNames = testCaseDatas
                .Select(tcd => tcd.TestName)
                .ToList();
            Assert.AreEqual(14, testNames.Distinct().Count());
            Assert.IsTrue(testNames.All(tn => tn.StartsWith("C_")));
            Assert.IsTrue(testNames.All(tn => tn.EndsWith("_D")));
            IEnumerable<ClosingStructure> structures = testCaseDatas
                .Select(tcd => tcd.Arguments[0])
                .OfType<ClosingStructure>()
                .ToList();
            Assert.AreEqual(14, structures.Count());
            Assert.IsTrue(structures.All(s => s.Id == referenceStructure.Id));
            Assert.IsTrue(structures.All(s => s.Name == referenceStructure.Name));
            Assert.IsTrue(structures.All(s => s.Location.Equals(referenceStructure.Location)));
            differentStructures.Add(structures.Single(s => !s.AreaFlowApertures.Equals(referenceStructure.AreaFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.CriticalOvertoppingDischarge.Equals(referenceStructure.CriticalOvertoppingDischarge)));
            differentStructures.Add(structures.Single(s => !s.FlowWidthAtBottomProtection.Equals(referenceStructure.FlowWidthAtBottomProtection)));
            differentStructures.Add(structures.Single(s => !s.InsideWaterLevel.Equals(referenceStructure.InsideWaterLevel)));
            differentStructures.Add(structures.Single(s => !s.LevelCrestStructureNotClosing.Equals(referenceStructure.LevelCrestStructureNotClosing)));
            differentStructures.Add(structures.Single(s => !s.StorageStructureArea.Equals(referenceStructure.StorageStructureArea)));
            differentStructures.Add(structures.Single(s => !s.ThresholdHeightOpenWeir.Equals(referenceStructure.ThresholdHeightOpenWeir)));
            differentStructures.Add(structures.Single(s => !s.WidthFlowApertures.Equals(referenceStructure.WidthFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityReparation.Equals(referenceStructure.FailureProbabilityReparation)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityOpenStructure.Equals(referenceStructure.FailureProbabilityOpenStructure)));
            differentStructures.Add(structures.Single(s => !s.IdenticalApertures.Equals(referenceStructure.IdenticalApertures)));
            differentStructures.Add(structures.Single(s => !s.InflowModelType.Equals(referenceStructure.InflowModelType)));
            differentStructures.Add(structures.Single(s => !s.ProbabilityOrFrequencyOpenStructureBeforeFlooding.Equals(referenceStructure.ProbabilityOrFrequencyOpenStructureBeforeFlooding)));
            differentStructures.Add(structures.Single(s => !s.StructureNormalOrientation.Equals(referenceStructure.StructureNormalOrientation)));
            Assert.AreEqual(14, differentStructures.Distinct().Count());
        }
    }
}