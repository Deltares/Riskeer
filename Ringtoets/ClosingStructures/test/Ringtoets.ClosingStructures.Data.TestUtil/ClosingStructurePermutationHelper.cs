﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.ClosingStructures.Data.TestUtil
{
    /// <summary>
    /// Helper containing a source of modified <see cref="ClosingStructure"/> entities that can
    /// be used in tests as a TestCaseSource.
    /// </summary>
    public static class ClosingStructurePermutationHelper
    {
        /// <summary>
        /// Returns a collection of modified <see cref="ClosingStructure"/> entities, which all differ
        /// except for their id.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// [TestCaseSource(typeof(ClosingStructurePermutationHelper),
        ///                 nameof(ClosingStructurePermutationHelper.DifferentClosingStructuresWithSameId),
        ///                 new object[]
        ///                 {
        ///                     "TargetMethodName",
        ///                     "TestResult"
        ///                 })]
        /// </example>
        public static IEnumerable<TestCaseData> DifferentClosingStructuresWithSameId(string targetName, string testResultDescription)
        {
            var referenceStructure = new TestClosingStructure();

            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(new TestClosingStructure(referenceStructure.Id, "Different name"))
                    .SetName($"{targetName}_DifferentName_{testResultDescription}"),
                new TestCaseData(new TestClosingStructure(new Point2D(1, 1), referenceStructure.Id))
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
        /// [TestCaseSource(typeof(ClosingStructurePermutationHelper),
        ///                 nameof(ClosingStructurePermutationHelper.DifferentClosingStructuresWithSameIdNameAndLocation),
        ///                 new object[]
        ///                 {
        ///                     "TargetMethodName",
        ///                     "TestResult"
        ///                 })]
        /// </example>
        public static IEnumerable<TestCaseData> DifferentClosingStructuresWithSameIdNameAndLocation(string targetName, string testResultDescription)
        {
            var random = new Random(532);

            yield return new TestCaseData(new TestClosingStructure
            {
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) random.Next(),
                    Shift = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_AllowedLevelIncreaseStorage_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                AreaFlowApertures =
                {
                    Mean = (RoundedDouble) random.Next(),
                    Shift = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAreaFlowApertures_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) random.Next(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentCriticalOvertoppingDischarge_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) random.Next(),
                    Shift = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFlowWidthAtBottomProtection_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                InsideWaterLevel =
                {
                    Mean = (RoundedDouble) random.Next(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentInsideWaterLevel_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                LevelCrestStructureNotClosing =
                {
                    Mean = (RoundedDouble) random.Next(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentLevelCrestStructureNotClosing_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) random.Next(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStorageStructureArea_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                ThresholdHeightOpenWeir =
                {
                    Mean = (RoundedDouble) random.Next(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentThresholdHeightOpenWeir_{testResultDescription}");

            yield return new TestCaseData(new TestClosingStructure
            {
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) random.Next(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentWidthFlowApertures_{testResultDescription}");

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
            differentInflowModelTypeConstructionProperties.InflowModelType = random.NextEnumValue<ClosingStructureInflowModelType>();
            yield return new TestCaseData(new ClosingStructure(differentInflowModelTypeConstructionProperties))
                .SetName($"{targetName}_DifferentInflowModelType_{testResultDescription}");

            ClosingStructure.ConstructionProperties differentProbabilityOrFrequencyOpenStructureBeforeFloodingConstructionProperties =
                CreateTestClosingStructureConstructionProperties();
            differentProbabilityOrFrequencyOpenStructureBeforeFloodingConstructionProperties.ProbabilityOrFrequencyOpenStructureBeforeFlooding = random.NextDouble();
            yield return new TestCaseData(new ClosingStructure(differentProbabilityOrFrequencyOpenStructureBeforeFloodingConstructionProperties))
                .SetName($"{targetName}_DifferentProbabilityOrFrequencyOpenStructureBeforeFlooding_{testResultDescription}");

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
                    Shift = referenceStructure.AllowedLevelIncreaseStorage.Shift,
                    StandardDeviation = referenceStructure.AllowedLevelIncreaseStorage.StandardDeviation
                },
                AreaFlowApertures =
                {
                    Mean = referenceStructure.AreaFlowApertures.Mean,
                    Shift = referenceStructure.AreaFlowApertures.Shift,
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
                    Shift = referenceStructure.FlowWidthAtBottomProtection.Shift,
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
                ProbabilityOrFrequencyOpenStructureBeforeFlooding = referenceStructure.ProbabilityOrFrequencyOpenStructureBeforeFlooding,
                StructureNormalOrientation = referenceStructure.StructureNormalOrientation
            };
        }
    }
}