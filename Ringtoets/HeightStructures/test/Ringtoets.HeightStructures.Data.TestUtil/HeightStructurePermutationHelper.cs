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

using System;
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.HeightStructures.Data.TestUtil
{
    /// <summary>
    /// Helper containing a source of modified <see cref="HeightStructure"/> entities that can
    /// be used in tests as a TestCaseSource.
    /// </summary>
    public static class HeightStructurePermutationHelper
    {
        /// <summary>
        /// Returns a collection of modified <see cref="HeightStructure"/> entities, which all differ
        /// except for their id.
        /// </summary>
        /// <example>[TestCaseSource(typeof(HeightStructurePermutationHelper), nameof(HeightStructurePermutationHelper.DifferentHeightStructureWithSameId))]</example>
        public static IEnumerable<TestCaseData> DifferentHeightStructuresWithSameId()
        {
            var referenceStructure = new TestHeightStructure();

            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(new TestHeightStructure(referenceStructure.Id, "Different Name"))
                    .SetName("Different Name"),
                new TestCaseData(new TestHeightStructure(new Point2D(1, 1), referenceStructure.Id))
                    .SetName("Different Location")
            };

            testCaseData.AddRange(DifferentHeightStructuresWithSameIdNameAndLocation());

            return testCaseData;
        }

        /// <summary>
        /// Returns a collection of modified <see cref="HeightStructure"/> entities, which all
        /// differ except for their id, name and location.
        /// </summary>
        /// <example>[TestCaseSource(typeof(HeightStructurePermutationHelper), nameof(HeightStructurePermutationHelper.DifferentHeightStructureWithSameIdLocationAndName))]</example>
        public static IEnumerable<TestCaseData> DifferentHeightStructuresWithSameIdNameAndLocation()
        {
            var random = new Random(532);
            var referenceStructure = new TestHeightStructure();

            yield return new TestCaseData(new TestHeightStructure
            {
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) random.Next(),
                    Shift = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName("Different AllowedLevelIncreaseStorage");
            yield return new TestCaseData(new TestHeightStructure
            {
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) random.Next(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName("Different CriticalOvertoppingDischarge");
            yield return new TestCaseData(new TestHeightStructure
            {
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) random.Next(),
                    Shift = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName("Different FlowWidthAtBottomProtection");
            yield return new TestCaseData(new TestHeightStructure
            {
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) random.Next(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName("Different LevelCrestStructure");
            yield return new TestCaseData(new TestHeightStructure
            {
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) random.Next(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName("Different StorageStructureArea");
            yield return new TestCaseData(new TestHeightStructure
            {
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) random.Next(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName("Different WidthFlowApertures");

            yield return new TestCaseData(new HeightStructure(new HeightStructure.ConstructionProperties
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
                FailureProbabilityStructureWithErosion = random.NextRoundedDouble(),
                StructureNormalOrientation = referenceStructure.StructureNormalOrientation
            })).SetName("Different FailureProbabilityStructureWithErosion");
            yield return new TestCaseData(new HeightStructure(new HeightStructure.ConstructionProperties
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
                StructureNormalOrientation = random.NextRoundedDouble()
            })).SetName("Different StructureNormalOrientation");
        }
    }
}