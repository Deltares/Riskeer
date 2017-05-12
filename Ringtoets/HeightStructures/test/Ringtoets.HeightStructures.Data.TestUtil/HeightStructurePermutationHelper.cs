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
    public static class HeightStructurePermutationHelper
    {
        public static IEnumerable<TestCaseData> DifferentHeightStructureWithSameId
        {
            get
            {
                const string defaultId = "id";

                var testCaseData = new List<TestCaseData>
                {
                    new TestCaseData(new TestHeightStructure(defaultId, "Different Name"))
                        .SetName("Different Name"),
                    new TestCaseData(new TestHeightStructure(new Point2D(1, 1), defaultId))
                        .SetName("Different Location")
                };

                testCaseData.AddRange(DifferentHeightStructureWithSameIdLocationAndName);

                return testCaseData;
            }
        }

        public static IEnumerable<TestCaseData> DifferentHeightStructureWithSameIdLocationAndName
        {
            get
            {
                var random = new Random(532);
                const string defaultId = "id";
                const string defaultName = "name";
                var defaultLocation = new Point2D(0, 0);

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
                    Name = defaultName,
                    Id = defaultId,
                    Location = defaultLocation,
                    FailureProbabilityStructureWithErosion = random.NextDouble()
                })).SetName("Different FailureProbabilityStructureWithErosion");
                yield return new TestCaseData(new HeightStructure(new HeightStructure.ConstructionProperties
                {
                    Name = defaultName,
                    Id = defaultId,
                    Location = defaultLocation,
                    StructureNormalOrientation = random.NextRoundedDouble()
                })).SetName("Different StructureNormalOrientation");
            }
        }

    }
}