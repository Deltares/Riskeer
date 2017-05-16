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
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class ForeshoreProfilePermutationHelperTest
    {
        [Test]
        public void DifferentForeshoreProfileWithSameIdNameAndX0_ReturnsExpectedTestCaseData()
        {
            // Setup
            const string targetName = "C";
            const string testResultDescription = "D";

            // Call
            List<TestCaseData> testCaseDatas = ForeshoreProfilePermutationHelper.DifferentForeshoreProfileWithSameIdNameAndX0(
                targetName,
                testResultDescription).ToList();

            // Assert
            Assert.AreEqual(5, testCaseDatas.Count);
            AssertTestNames(testCaseDatas, targetName, testResultDescription);
            AssertProperties(testCaseDatas, true);
        }

        [Test]
        public void DifferentForeshoreProfileWithSameIdNameOrientationAndX0_ReturnsExpectedTestCaseData()
        {
            // Setup
            const string targetName = "C";
            const string testResultDescription = "D";

            // Call
            List<TestCaseData> testCaseDatas = ForeshoreProfilePermutationHelper.DifferentForeshoreProfileWithSameIdNameOrientationAndX0(
                targetName,
                testResultDescription).ToList();

            // Assert
            Assert.AreEqual(4, testCaseDatas.Count);
            AssertTestNames(testCaseDatas, targetName, testResultDescription);
            AssertProperties(testCaseDatas, false);
        }

        private static void AssertTestNames(ICollection<TestCaseData> testCaseDatas,
                                            string targetName,
                                            string testResultDescription)
        {
            IEnumerable<string> testNames = testCaseDatas
                .Select(tcd => tcd.TestName)
                .ToList();
            Assert.AreEqual(testCaseDatas.Count, testNames.Distinct().Count());
            Assert.IsTrue(testNames.All(tn => tn.StartsWith($"{targetName}_")));
            Assert.IsTrue(testNames.All(tn => tn.EndsWith($"_{testResultDescription}")));
        }

        private static void AssertProperties(ICollection<TestCaseData> testCaseDatas, bool orientationUnique)
        {
            var differentProfiles = new List<ForeshoreProfile>();
            var referenceProfile = new TestForeshoreProfile();

            IEnumerable<ForeshoreProfile> profiles = testCaseDatas.Select(tcd => tcd.Arguments[0])
                                                                  .OfType<ForeshoreProfile>()
                                                                  .ToList();

            Assert.AreEqual(profiles.Count(), testCaseDatas.Count);

            Assert.IsTrue(profiles.All(p => p.Id == referenceProfile.Id));
            Assert.IsTrue(profiles.All(p => p.Name == referenceProfile.Name));
            Assert.IsTrue(profiles.All(p => p.X0.Equals(referenceProfile.X0)));

            if (orientationUnique)
            {
                differentProfiles.Add(profiles.Single(p => !p.Orientation.Equals(referenceProfile.Orientation)));
            }
            else
            {
                Assert.IsTrue(profiles.All(s => s.Orientation.Equals(referenceProfile.Orientation)));
            }

            differentProfiles.Add(profiles.Single(p => !p.Geometry.SequenceEqual(referenceProfile.Geometry)));

            var defaultBreakWater = new BreakWater(BreakWaterType.Dam, 0.0);
            differentProfiles.Add(profiles.Single(p => p.BreakWater != null
                                                       && p.BreakWater.Type.Equals(defaultBreakWater.Type)
                                                       && !p.BreakWater.Height.Equals(defaultBreakWater.Height)));
            differentProfiles.Add(profiles.Single(p => p.BreakWater != null
                                                       && p.BreakWater.Type.Equals(BreakWaterType.Caisson)
                                                       && p.BreakWater.Height.Equals(defaultBreakWater.Height)));

            differentProfiles.Add(profiles.Single(p => p.BreakWater != null
                                                       && p.BreakWater.Type.Equals(BreakWaterType.Wall)
                                                       && p.BreakWater.Height.Equals(defaultBreakWater.Height)));

            Assert.AreEqual(differentProfiles.Distinct().Count(), testCaseDatas.Count);
        }
    }
}