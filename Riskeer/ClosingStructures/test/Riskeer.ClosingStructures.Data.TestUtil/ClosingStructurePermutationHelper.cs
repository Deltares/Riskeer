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

namespace Riskeer.ClosingStructures.Data.TestUtil
{
    /// <summary>
    /// Helper containing a source of modified <see cref="ClosingStructure"/> entities that can
    /// be used in tests as a TestCaseSource.
    /// </summary>
    public static class ClosingStructurePermutationHelper
    {
        /// <summary>
        /// Returns a collection of modified <see cref="ClosingStructure"/> entities.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(
        ///     typeof(ClosingStructurePermutationHelper),
        ///     nameof(ClosingStructurePermutationHelper.DifferentClosingStructures),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentClosingStructures(string targetName, string testResultDescription)
        {
            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(new TestClosingStructure("Different id"))
                    .SetName($"{targetName}_DifferentId_{testResultDescription}")
            };

            testCaseData.AddRange(DifferentClosingStructuresWithSameId(targetName, testResultDescription));

            return testCaseData;
        }

        /// <summary>
        /// Returns a collection of modified <see cref="ClosingStructure"/> entities, which all differ
        /// except for their id.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(
        ///     typeof(ClosingStructurePermutationHelper),
        ///     nameof(ClosingStructurePermutationHelper.DifferentClosingStructuresWithSameId),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentClosingStructuresWithSameId(string targetName, string testResultDescription)
        {
            string referenceStructureId = new TestClosingStructure().Id;

            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(new TestClosingStructure(referenceStructureId, "Different name"))
                    .SetName($"{targetName}_DifferentName_{testResultDescription}"),
                new TestCaseData(new TestClosingStructure(new Point2D(1, 1), referenceStructureId))
                    .SetName($"{targetName}_DifferentLocation_{testResultDescription}")
            };

            testCaseData.AddRange(DifferentClosingStructuresWithSameIdNameAndLocation(targetName, testResultDescription));

            return testCaseData;
        }

        /// <summary>
        /// Returns a collection of modified <see cref="ClosingStructure"/> entities, which all differ
        /// except for their id, name and location.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(
        ///     typeof(ClosingStructurePermutationHelper),
        ///     nameof(ClosingStructurePermutationHelper.DifferentClosingStructuresWithSameIdNameAndLocation),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentClosingStructuresWithSameIdNameAndLocation(string targetName, string testResultDescription)
        {
            var random = new Random(532);

            yield return new TestCaseData(new TestClosingStructure
            {
                AllowedLevelIncreaseStorage =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAllowedLevelIncreaseStorageMean_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                AllowedLevelIncreaseStorage =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAllowedLevelIncreaseStorageStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                AreaFlowApertures =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAreaFlowAperturesMean_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                AreaFlowApertures =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAreaFlowAperturesStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                CriticalOvertoppingDischarge =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentCriticalOvertoppingDischargeMean_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                CriticalOvertoppingDischarge =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentCriticalOvertoppingDischargeCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                FlowWidthAtBottomProtection =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFlowWidthAtBottomProtectionMean_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                FlowWidthAtBottomProtection =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFlowWidthAtBottomProtectionStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                InsideWaterLevel =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentInsideWaterLevelMean_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                InsideWaterLevel =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentInsideWaterLevelStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                LevelCrestStructureNotClosing =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentLevelCrestStructureNotClosingMean_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                LevelCrestStructureNotClosing =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentLevelCrestStructureNotClosingStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                StorageStructureArea =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStorageStructureAreaMean_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                StorageStructureArea =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStorageStructureAreaCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                ThresholdHeightOpenWeir =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentThresholdHeightOpenWeirMean_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                ThresholdHeightOpenWeir =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentThresholdHeightOpenWeirStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                WidthFlowApertures =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentWidthFlowAperturesMean_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                WidthFlowApertures =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentWidthFlowAperturesStandardDeviation_{testResultDescription}");

            ClosingStructure.ConstructionProperties differentFailureProbabilityReparationConstructionProperties =
                CreateTestClosingStructureConstructionProperties();
            differentFailureProbabilityReparationConstructionProperties.FailureProbabilityReparation = random.NextDouble();
            yield return new TestCaseData(new ClosingStructure(differentFailureProbabilityReparationConstructionProperties))
                .SetName($"{targetName}_DifferentFailureProbabilityReparation_{testResultDescription}");

            ClosingStructure.ConstructionProperties differentFailureProbabilityOpenStructureConstructionProperties =
                CreateTestClosingStructureConstructionProperties();
            differentFailureProbabilityOpenStructureConstructionProperties.FailureProbabilityOpenStructure = random.NextDouble();
            yield return new TestCaseData(new ClosingStructure(differentFailureProbabilityOpenStructureConstructionProperties))
                .SetName($"{targetName}_DifferentFailureProbabilityOpenStructure_{testResultDescription}");

            ClosingStructure.ConstructionProperties differentIdenticalAperturesProperties =
                CreateTestClosingStructureConstructionProperties();
            differentIdenticalAperturesProperties.IdenticalApertures = random.Next();
            yield return new TestCaseData(new ClosingStructure(differentIdenticalAperturesProperties))
                .SetName($"{targetName}_DifferentIdenticalApertures_{testResultDescription}");

            ClosingStructure.ConstructionProperties differentInflowModelTypeConstructionProperties =
                CreateTestClosingStructureConstructionProperties();
            differentInflowModelTypeConstructionProperties.InflowModelType = ClosingStructureInflowModelType.LowSill;
            yield return new TestCaseData(new ClosingStructure(differentInflowModelTypeConstructionProperties))
                .SetName($"{targetName}_DifferentInflowModelType_{testResultDescription}");

            ClosingStructure.ConstructionProperties differentProbabilityOpenStructureBeforeFloodingConstructionProperties =
                CreateTestClosingStructureConstructionProperties();
            differentProbabilityOpenStructureBeforeFloodingConstructionProperties.ProbabilityOpenStructureBeforeFlooding = random.NextDouble();
            yield return new TestCaseData(new ClosingStructure(differentProbabilityOpenStructureBeforeFloodingConstructionProperties))
                .SetName($"{targetName}_DifferentProbabilityOpenStructureBeforeFlooding_{testResultDescription}");

            ClosingStructure.ConstructionProperties differentStructureNormalOrientationConstructionProperties =
                CreateTestClosingStructureConstructionProperties();
            differentStructureNormalOrientationConstructionProperties.StructureNormalOrientation = random.NextRoundedDouble();
            yield return new TestCaseData(new ClosingStructure(differentStructureNormalOrientationConstructionProperties))
                .SetName($"{targetName}_DifferentStructureNormalOrientation_{testResultDescription}");
        }

        private static ClosingStructure.ConstructionProperties CreateTestClosingStructureConstructionProperties()
        {
            var referenceStructure = new TestClosingStructure();

            return new ClosingStructure.ConstructionProperties
            {
                Name = referenceStructure.Name,
                Id = referenceStructure.Id,
                Location = referenceStructure.Location,
                AllowedLevelIncreaseStorage =
                {
                    Mean = referenceStructure.AllowedLevelIncreaseStorage.Mean,
                    StandardDeviation = referenceStructure.AllowedLevelIncreaseStorage.StandardDeviation
                },
                AreaFlowApertures =
                {
                    Mean = referenceStructure.AreaFlowApertures.Mean,
                    StandardDeviation = referenceStructure.AreaFlowApertures.StandardDeviation
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
                InsideWaterLevel =
                {
                    Mean = referenceStructure.InsideWaterLevel.Mean,
                    StandardDeviation = referenceStructure.InsideWaterLevel.StandardDeviation
                },
                LevelCrestStructureNotClosing =
                {
                    Mean = referenceStructure.LevelCrestStructureNotClosing.Mean,
                    StandardDeviation = referenceStructure.LevelCrestStructureNotClosing.StandardDeviation
                },
                StorageStructureArea =
                {
                    Mean = referenceStructure.StorageStructureArea.Mean,
                    CoefficientOfVariation = referenceStructure.StorageStructureArea.CoefficientOfVariation
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = referenceStructure.ThresholdHeightOpenWeir.Mean,
                    StandardDeviation = referenceStructure.ThresholdHeightOpenWeir.StandardDeviation
                },
                WidthFlowApertures =
                {
                    Mean = referenceStructure.WidthFlowApertures.Mean,
                    StandardDeviation = referenceStructure.WidthFlowApertures.StandardDeviation
                },
                FailureProbabilityReparation = referenceStructure.FailureProbabilityReparation,
                FailureProbabilityOpenStructure = referenceStructure.FailureProbabilityOpenStructure,
                IdenticalApertures = referenceStructure.IdenticalApertures,
                InflowModelType = referenceStructure.InflowModelType,
                ProbabilityOpenStructureBeforeFlooding = referenceStructure.ProbabilityOpenStructureBeforeFlooding,
                StructureNormalOrientation = referenceStructure.StructureNormalOrientation
            };
        }
    }
}