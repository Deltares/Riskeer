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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.HeightStructures.Data.TestUtil
{
    /// <summary>
    /// Helper containing a source of modified <see cref="HeightStructure"/> entities that can
    /// be used in tests as a TestCaseSource.
    /// </summary>
    public static class HeightStructurePermutationHelper
    {
        /// <summary>
        /// Returns a collection of modified <see cref="HeightStructure"/> entities.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(
        ///     typeof(HeightStructurePermutationHelper),
        ///     nameof(HeightStructurePermutationHelper.DifferentHeightStructures),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentHeightStructures(string targetName, string testResultDescription)
        {
            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(new TestHeightStructure("Different id"))
                    .SetName($"{targetName}_DifferentId_{testResultDescription}")
            };

            testCaseData.AddRange(DifferentHeightStructuresWithSameId(targetName, testResultDescription));

            return testCaseData;
        }

        /// <summary>
        /// Returns a collection of modified <see cref="HeightStructure"/> entities, which all differ
        /// except for their id.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(
        ///     typeof(HeightStructurePermutationHelper),
        ///     nameof(HeightStructurePermutationHelper.DifferentHeightStructuresWithSameId),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentHeightStructuresWithSameId(string targetName, string testResultDescription)
        {
            string referenceStructureId = new TestHeightStructure().Id;

            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(new TestHeightStructure(referenceStructureId, "Different name"))
                    .SetName($"{targetName}_DifferentName_{testResultDescription}"),
                new TestCaseData(new TestHeightStructure(new Point2D(1, 1), referenceStructureId))
                    .SetName($"{targetName}_DifferentLocation_{testResultDescription}")
            };

            testCaseData.AddRange(DifferentHeightStructuresWithSameIdNameAndLocation(targetName, testResultDescription));

            return testCaseData;
        }

        /// <summary>
        /// Returns a collection of modified <see cref="HeightStructure"/> entities, which all differ
        /// except for their id, name and location.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(
        ///     typeof(HeightStructurePermutationHelper),
        ///     nameof(HeightStructurePermutationHelper.DifferentHeightStructuresWithSameIdNameAndLocation),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentHeightStructuresWithSameIdNameAndLocation(string targetName, string testResultDescription)
        {
            var random = new Random(532);

            yield return new TestCaseData(new TestHeightStructure
            {
                AllowedLevelIncreaseStorage =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAllowedLevelIncreaseStorageMean_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                AllowedLevelIncreaseStorage =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAllowedLevelIncreaseStorageStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                CriticalOvertoppingDischarge =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentCriticalOvertoppingDischargeMean_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                CriticalOvertoppingDischarge =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentCriticalOvertoppingDischargeCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                FlowWidthAtBottomProtection =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFlowWidthAtBottomProtectionMean_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                FlowWidthAtBottomProtection =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFlowWidthAtBottomProtectionStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                LevelCrestStructure =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentLevelCrestStructureMean_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                LevelCrestStructure =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentLevelCrestStructureStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                StorageStructureArea =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStorageStructureAreaMean_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                StorageStructureArea =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStorageStructureAreaCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                WidthFlowApertures =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentWidthFlowAperturesMean_{testResultDescription}");

            yield return new TestCaseData(new TestHeightStructure
            {
                WidthFlowApertures =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentWidthFlowAperturesStandardDeviation_{testResultDescription}");

            HeightStructure.ConstructionProperties differentFailureProbabilityStructureWithErosionConstructionProperties = CreateTestHeightStructureConstructionProperties();
            differentFailureProbabilityStructureWithErosionConstructionProperties.FailureProbabilityStructureWithErosion = random.NextDouble();
            yield return new TestCaseData(new HeightStructure(differentFailureProbabilityStructureWithErosionConstructionProperties))
                .SetName($"{targetName}_DifferentFailureProbabilityStructureWithErosion_{testResultDescription}");

            HeightStructure.ConstructionProperties differentStructureNormalOrientationConstructionProperties = CreateTestHeightStructureConstructionProperties();
            differentStructureNormalOrientationConstructionProperties.StructureNormalOrientation = random.NextRoundedDouble();
            yield return new TestCaseData(new HeightStructure(differentStructureNormalOrientationConstructionProperties))
                .SetName($"{targetName}_DifferentStructureNormalOrientation_{testResultDescription}");
        }

        private static HeightStructure.ConstructionProperties CreateTestHeightStructureConstructionProperties()
        {
            var referenceStructure = new TestHeightStructure();

            return new HeightStructure.ConstructionProperties
            {
                Name = referenceStructure.Name,
                Id = referenceStructure.Id,
                Location = referenceStructure.Location,
                AllowedLevelIncreaseStorage =
                {
                    Mean = referenceStructure.AllowedLevelIncreaseStorage.Mean,
                    StandardDeviation = referenceStructure.AllowedLevelIncreaseStorage.StandardDeviation
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = referenceStructure.CriticalOvertoppingDischarge.Mean,
                    CoefficientOfVariation = referenceStructure.CriticalOvertoppingDischarge.CoefficientOfVariation
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = referenceStructure.FlowWidthAtBottomProtection.Mean,
                    StandardDeviation = referenceStructure.FlowWidthAtBottomProtection.StandardDeviation
                },
                LevelCrestStructure =
                {
                    Mean = referenceStructure.LevelCrestStructure.Mean,
                    StandardDeviation = referenceStructure.LevelCrestStructure.StandardDeviation
                },
                StorageStructureArea =
                {
                    Mean = referenceStructure.StorageStructureArea.Mean,
                    CoefficientOfVariation = referenceStructure.StorageStructureArea.CoefficientOfVariation
                },
                WidthFlowApertures =
                {
                    Mean = referenceStructure.WidthFlowApertures.Mean,
                    StandardDeviation = referenceStructure.WidthFlowApertures.StandardDeviation
                },
                FailureProbabilityStructureWithErosion = referenceStructure.FailureProbabilityStructureWithErosion,
                StructureNormalOrientation = referenceStructure.StructureNormalOrientation
            };
        }
    }
}