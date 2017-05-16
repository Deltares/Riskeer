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
        public void DifferentHeightStructures_ReturnsExpectedTestCaseData()
        {
            // Setup
            const string targetName = "A";
            const string testResultDescription = "B";

            // Call

            List<TestCaseData> testCaseDatas = HeightStructurePermutationHelper.DifferentHeightStructures(targetName, testResultDescription).ToList();

            // Assert
            Assert.AreEqual(17, testCaseDatas.Count);
            AssertTestNames(testCaseDatas, targetName, testResultDescription);
            AssertParameters(testCaseDatas, true, true, true);
        }

        [Test]
        public void DifferentHeightStructuresWithSameId_ReturnsExpectedTestCaseData()
        {
            // Setup
            const string targetName = "C";
            const string testResultDescription = "D";

            // Call
            List<TestCaseData> testCaseDatas = HeightStructurePermutationHelper.DifferentHeightStructuresWithSameId(targetName, testResultDescription).ToList();

            // Assert
            Assert.AreEqual(16, testCaseDatas.Count);
            AssertTestNames(testCaseDatas, targetName, testResultDescription);
            AssertParameters(testCaseDatas, false, true, true);
        }

        [Test]
        public void DifferentHeightStructuresWithSameIdNameAndLocation_ReturnsExpectedTestCaseData()
        {
            // Setup
            const string targetName = "E";
            const string testResultDescription = "F";

            // Call
            List<TestCaseData> testCaseDatas = HeightStructurePermutationHelper.DifferentHeightStructuresWithSameIdNameAndLocation(targetName, testResultDescription).ToList();

            // Assert
            Assert.AreEqual(14, testCaseDatas.Count);
            AssertTestNames(testCaseDatas, targetName, testResultDescription);
            AssertParameters(testCaseDatas, false, false, false);
        }

        private static void AssertTestNames(ICollection<TestCaseData> testCaseDatas, string targetName, string testResultDescription)
        {
            IEnumerable<string> testNames = testCaseDatas
                .Select(tcd => tcd.TestName)
                .ToList();
            Assert.AreEqual(testCaseDatas.Count, testNames.Distinct().Count());
            Assert.IsTrue(testNames.All(tn => tn.StartsWith($"{targetName}_")));
            Assert.IsTrue(testNames.All(tn => tn.EndsWith($"_{testResultDescription}")));
        }

        private static void AssertParameters(ICollection<TestCaseData> testCaseDatas, bool idUnique, bool nameUnique, bool locationUnique)
        {
            var differentStructures = new List<HeightStructure>();
            var referenceStructure = new TestHeightStructure();

            IEnumerable<HeightStructure> structures = testCaseDatas
                .Select(tcd => tcd.Arguments[0])
                .OfType<HeightStructure>()
                .ToList();

            Assert.AreEqual(testCaseDatas.Count, structures.Count());

            if (idUnique)
            {
                differentStructures.Add(structures.Single(s => !s.Id.Equals(referenceStructure.Id)));
            }
            else
            {
                Assert.IsTrue(structures.All(s => s.Id == referenceStructure.Id));
            }

            if (nameUnique)
            {
                differentStructures.Add(structures.Single(s => !s.Name.Equals(referenceStructure.Name)));
            }
            else
            {
                Assert.IsTrue(structures.All(s => s.Name == referenceStructure.Name));
            }

            if (locationUnique)
            {
                differentStructures.Add(structures.Single(s => !s.Location.Equals(referenceStructure.Location)));
            }
            else
            {
                Assert.IsTrue(structures.All(s => s.Location.Equals(referenceStructure.Location)));
            }

            differentStructures.Add(structures.Single(s => !s.AllowedLevelIncreaseStorage.Mean.Equals(referenceStructure.AllowedLevelIncreaseStorage.Mean)));
            differentStructures.Add(structures.Single(s => !s.AllowedLevelIncreaseStorage.StandardDeviation.Equals(referenceStructure.AllowedLevelIncreaseStorage.StandardDeviation)));
            differentStructures.Add(structures.Single(s => !s.CriticalOvertoppingDischarge.Mean.Equals(referenceStructure.CriticalOvertoppingDischarge.Mean)));
            differentStructures.Add(structures.Single(s => !s.CriticalOvertoppingDischarge.CoefficientOfVariation.Equals(referenceStructure.CriticalOvertoppingDischarge.CoefficientOfVariation)));
            differentStructures.Add(structures.Single(s => !s.FlowWidthAtBottomProtection.Mean.Equals(referenceStructure.FlowWidthAtBottomProtection.Mean)));
            differentStructures.Add(structures.Single(s => !s.FlowWidthAtBottomProtection.StandardDeviation.Equals(referenceStructure.FlowWidthAtBottomProtection.StandardDeviation)));
            differentStructures.Add(structures.Single(s => !s.LevelCrestStructure.Mean.Equals(referenceStructure.LevelCrestStructure.Mean)));
            differentStructures.Add(structures.Single(s => !s.LevelCrestStructure.StandardDeviation.Equals(referenceStructure.LevelCrestStructure.StandardDeviation)));
            differentStructures.Add(structures.Single(s => !s.StorageStructureArea.Mean.Equals(referenceStructure.StorageStructureArea.Mean)));
            differentStructures.Add(structures.Single(s => !s.StorageStructureArea.CoefficientOfVariation.Equals(referenceStructure.StorageStructureArea.CoefficientOfVariation)));
            differentStructures.Add(structures.Single(s => !s.WidthFlowApertures.Mean.Equals(referenceStructure.WidthFlowApertures.Mean)));
            differentStructures.Add(structures.Single(s => !s.WidthFlowApertures.StandardDeviation.Equals(referenceStructure.WidthFlowApertures.StandardDeviation)));
            differentStructures.Add(structures.Single(s => !s.FailureProbabilityStructureWithErosion.Equals(referenceStructure.FailureProbabilityStructureWithErosion)));
            differentStructures.Add(structures.Single(s => !s.StructureNormalOrientation.Equals(referenceStructure.StructureNormalOrientation)));
            Assert.AreEqual(testCaseDatas.Count, differentStructures.Distinct().Count());
        }
    }
}