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

namespace Ringtoets.HeightStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class HeightStructurePermutationHelperTest
    {
        [Test]
        public void DifferentHeightStructureWithSameId_ReturnsExpectedTestCaseData()
        {
            // Setup
            var referenceStructure = new TestHeightStructure();
            var differentStructures = new List<HeightStructure>();

            // Call
            List<TestCaseData> testCaseDatas = HeightStructurePermutationHelper.DifferentHeightStructuresWithSameId().ToList();

            // Assert
            Assert.AreEqual(10, testCaseDatas.Count);
            IEnumerable<HeightStructure> structures = testCaseDatas
                .Select(tcd => tcd.Arguments[0])
                .OfType<HeightStructure>()
                .ToList();
            Assert.AreEqual(10, structures.Count());
            Assert.IsTrue(structures.All(s => s.Id == structures.First().Id));
            differentStructures.Add(structures.Single(s => !s.Name.Equals(referenceStructure.Name)));
            differentStructures.Add(structures.Single(s => !s.Location.Equals(referenceStructure.Location)));
            differentStructures.Add(structures.Single(s => !s.AllowedLevelIncreaseStorage.Equals(referenceStructure.AllowedLevelIncreaseStorage)));
            differentStructures.Add(structures.Single(s => !s.CriticalOvertoppingDischarge.Equals(referenceStructure.CriticalOvertoppingDischarge)));
            differentStructures.Add(structures.Single(s => !s.FlowWidthAtBottomProtection.Equals(referenceStructure.FlowWidthAtBottomProtection)));
            differentStructures.Add(structures.Single(s => !s.LevelCrestStructure.Equals(referenceStructure.LevelCrestStructure)));
            differentStructures.Add(structures.Single(s => !s.StorageStructureArea.Equals(referenceStructure.StorageStructureArea)));
            differentStructures.Add(structures.Single(s => !s.WidthFlowApertures.Equals(referenceStructure.WidthFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityStructureWithErosion.Equals(referenceStructure.FailureProbabilityStructureWithErosion)));
            differentStructures.Add(structures.Single(s => !s.StructureNormalOrientation.Equals(referenceStructure.StructureNormalOrientation)));
            Assert.AreEqual(10, differentStructures.Count);
        }

        [Test]
        public void DifferentHeightStructuresWithSameIdNameAndLocation_ReturnsExpectedTestCaseData()
        {
            // Setup
            var referenceStructure = new TestHeightStructure();
            var differentStructures = new List<HeightStructure>();

            // Call
            List<TestCaseData> testCaseDatas = HeightStructurePermutationHelper.DifferentHeightStructuresWithSameIdNameAndLocation().ToList();

            // Assert
            Assert.AreEqual(8, testCaseDatas.Count);
            IEnumerable<HeightStructure> structures = testCaseDatas
                .Select(tcd => tcd.Arguments[0])
                .OfType<HeightStructure>()
                .ToList();
            Assert.AreEqual(8, structures.Count());
            Assert.IsTrue(structures.All(s => s.Id == referenceStructure.Id));
            Assert.IsTrue(structures.All(s => s.Name == referenceStructure.Name));
            Assert.IsTrue(structures.All(s => s.Location.Equals(referenceStructure.Location)));
            differentStructures.Add(structures.Single(s => !s.AllowedLevelIncreaseStorage.Equals(referenceStructure.AllowedLevelIncreaseStorage)));
            differentStructures.Add(structures.Single(s => !s.CriticalOvertoppingDischarge.Equals(referenceStructure.CriticalOvertoppingDischarge)));
            differentStructures.Add(structures.Single(s => !s.FlowWidthAtBottomProtection.Equals(referenceStructure.FlowWidthAtBottomProtection)));
            differentStructures.Add(structures.Single(s => !s.LevelCrestStructure.Equals(referenceStructure.LevelCrestStructure)));
            differentStructures.Add(structures.Single(s => !s.StorageStructureArea.Equals(referenceStructure.StorageStructureArea)));
            differentStructures.Add(structures.Single(s => !s.WidthFlowApertures.Equals(referenceStructure.WidthFlowApertures)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityStructureWithErosion.Equals(referenceStructure.FailureProbabilityStructureWithErosion)));
            differentStructures.Add(structures.Single(s => !s.StructureNormalOrientation.Equals(referenceStructure.StructureNormalOrientation)));
            Assert.AreEqual(8, differentStructures.Count);
        }
    }
}