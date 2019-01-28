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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Helper containing a source of modified <see cref="ForeshoreProfile"/> entities that can
    /// be used in tests as a TestCaseSource.
    /// </summary>
    public static class ForeshoreProfilePermutationHelper
    {
        /// <summary>
        /// Returns a collection of modified <see cref="ForeshoreProfile"/> entities, which all differ
        /// except for their id, name, and X0.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(
        ///     typeof(ForeshoreProfilePermutationHelper),
        ///     nameof(ForeshoreProfilePermutationHelper.DifferentForeshoreProfilesWithSameIdNameAndX0),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentForeshoreProfilesWithSameIdNameAndX0(string targetName, string testResultDescription)
        {
            var random = new Random(532);

            var referenceProfile = new TestForeshoreProfile();
            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(
                        new ForeshoreProfile(
                            referenceProfile.WorldReferencePoint,
                            referenceProfile.Geometry,
                            referenceProfile.BreakWater,
                            new ForeshoreProfile.ConstructionProperties
                            {
                                Name = referenceProfile.Name,
                                Id = referenceProfile.Id,
                                X0 = referenceProfile.X0,
                                Orientation = random.NextDouble()
                            }))
                    .SetName($"{targetName}_differentOrientationProfileConstructionProperties_{testResultDescription}")
            };

            testCaseData.AddRange(DifferentForeshoreProfilesWithSameIdNameOrientationAndX0(targetName, testResultDescription));

            return testCaseData;
        }

        /// <summary>
        /// Returns a collection of modified <see cref="ForeshoreProfile"/> entities, which all differ
        /// except for their id, name, orientation, and X0.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(
        ///     typeof(ForeshoreProfilePermutationHelper),
        ///     nameof(ForeshoreProfilePermutationHelper.DifferentForeshoreProfilesWithSameIdNameOrientationAndX0),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentForeshoreProfilesWithSameIdNameOrientationAndX0(string targetName, string testResultDescription)
        {
            var referenceProfile = new TestForeshoreProfile();

            var defaultBreakWater = new BreakWater(BreakWaterType.Dam, 0.0);

            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(new TestForeshoreProfile(referenceProfile.Id, new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }))
                    .SetName($"{targetName}_DifferentGeometry_{testResultDescription}"),
                new TestCaseData(new TestForeshoreProfile(new BreakWater(defaultBreakWater.Type, 1 + defaultBreakWater.Height)))
                    .SetName($"{targetName}_DifferentBreakWaterHeight_{testResultDescription}"),
                new TestCaseData(new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, defaultBreakWater.Height)))
                    .SetName($"{targetName}_DifferentBreakWaterTypeCaisson_{testResultDescription}"),
                new TestCaseData(new TestForeshoreProfile(new BreakWater(BreakWaterType.Wall, defaultBreakWater.Height)))
                    .SetName($"{targetName}_DifferentBreakWaterTypeWall_{testResultDescription}")
            };

            return testCaseData;
        }
    }
}