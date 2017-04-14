// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.IO.Configurations;

namespace Ringtoets.HeightStructures.IO.Test.Configurations
{
    [TestFixture]
    public class HeightStructuresCalculationStochastAssignerTest
    {
        private static IEnumerable<TestCaseData> GetSetStochastParametersActions(string testNameFormat)
        {
            foreach (TestCaseData caseData in ThreeParameterCase((c, s) => c.LevelCrestStructure = s, "LevelCrest", testNameFormat))
            {
                yield return caseData;
            }
            foreach (TestCaseData caseData in ThreeParameterCase((c, s) => c.AllowedLevelIncreaseStorage = s, "AllowedLevelIncreaseStorage", testNameFormat))
            {
                yield return caseData;
            }
            foreach (TestCaseData caseData in ThreeParameterCase((c, s) => c.FlowWidthAtBottomProtection = s, "FlowWidthAtBottomProtection", testNameFormat))
            {
                yield return caseData;
            }
            foreach (TestCaseData caseData in ThreeParameterCase((c, s) => c.WidthFlowApertures = s, "WidthFlowApertures", testNameFormat))
            {
                yield return caseData;
            }
            foreach (TestCaseData caseData in ThreeParameterCase((c, s) => c.CriticalOvertoppingDischarge = s, "CriticalOvertoppingDischarge", testNameFormat))
            {
                yield return caseData;
            }
            foreach (TestCaseData caseData in ThreeParameterCase((c, s) => c.StorageStructureArea = s, "StorageStructureArea", testNameFormat))
            {
                yield return caseData;
            }
        }

        private static IEnumerable<TestCaseData> ThreeParameterCase(
            Action<HeightStructuresCalculationConfiguration, StochastConfiguration> setStochastAction,
            string stochastName,
            string testNameFormat)
        {
            var random = new Random(21);

            yield return new TestCaseData(
                    new Action<HeightStructuresCalculationConfiguration>(c => setStochastAction(c, new StochastConfiguration
                    {
                        Mean = random.NextDouble()
                    })))
                .SetName(string.Format(testNameFormat, $"{stochastName}Mean"));

            yield return new TestCaseData(
                    new Action<HeightStructuresCalculationConfiguration>(c => setStochastAction(c, new StochastConfiguration
                    {
                        StandardDeviation = random.NextDouble()
                    })))
                .SetName(string.Format(testNameFormat, $"{stochastName}StandardDeviation"));

            yield return new TestCaseData(
                    new Action<HeightStructuresCalculationConfiguration>(c => setStochastAction(c, new StochastConfiguration
                    {
                        VariationCoefficient = random.NextDouble()
                    })))
                .SetName(string.Format(testNameFormat, $"{stochastName}VariationCoefficient"));
        }

        [Test]
        public void Constructor_WithParameters_ReturnsNewInstance()
        {
            // Setup
            var configuration = new HeightStructuresCalculationConfiguration("name");
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            var assigner = new HeightStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

            // Assert
            Assert.NotNull(assigner);
        }

        [Test]
        [TestCaseSource(
            typeof(HeightStructuresCalculationStochastAssignerTest),
            nameof(GetSetStochastParametersActions),
            new object[]
            {
                "AreStochastsValid_WithoutStructureValueSetFor{0}_ReturnsFalse"
            })]
        public void AreStochastsValid_WithoutStructureParametersDefinedForStructureDependentStochats_ReturnsFalse(
            Action<HeightStructuresCalculationConfiguration> modify)
        {
            // Setup
            var configuration = new HeightStructuresCalculationConfiguration("name");
            modify(configuration);

            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var assigner = new HeightStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

            // Call
            var valid = assigner.AreStochastsValid();

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        [TestCaseSource(
            typeof(HeightStructuresCalculationStochastAssignerTest),
            nameof(GetSetStochastParametersActions),
            new object[]
            {
                "AreStochastsValid_WithStructureValueSetFor{0}_ReturnsTrue"
            })]
        public void AreStochastsValid_WithStructureParametersDefinedForStructureDependentStochats_ReturnsTrue(
            Action<HeightStructuresCalculationConfiguration> modify)
        {
            // Setup
            var configuration = new HeightStructuresCalculationConfiguration("name")
            {
                StructureName = "some structure"
            };
            modify(configuration);

            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var assigner = new HeightStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

            // Call
            var valid = assigner.AreStochastsValid();

            // Assert
            Assert.IsTrue(valid);
        }

        [Test]
        public void SetStochasts_WithAllStochastsSet_SetExpectedValuesOnInput()
        {
            // Setup
            var levelCrestStructure = new StochastConfiguration
            {
                Mean = 1.1,
                StandardDeviation = 1.5
            };

            var configuration = new HeightStructuresCalculationConfiguration("name")
            {
                StructureName = "some structure",
                LevelCrestStructure = levelCrestStructure,
                AllowedLevelIncreaseStorage = new StochastConfiguration
                {
                    Mean = 2.1,
                    StandardDeviation = 2.5
                },
                FlowWidthAtBottomProtection = new StochastConfiguration
                {
                    Mean = 3.1,
                    StandardDeviation = 3.5
                },
                WidthFlowApertures = new StochastConfiguration
                {
                    Mean = 4.1,
                    StandardDeviation = 4.5
                },
                ModelFactorSuperCriticalFlow = new StochastConfiguration
                {
                    Mean = 5.1,
                    StandardDeviation = 5.5
                },
                CriticalOvertoppingDischarge = new StochastConfiguration
                {
                    Mean = 6.1,
                    VariationCoefficient = 0.6
                },
                StorageStructureArea = new StochastConfiguration
                {
                    Mean = 7.1,
                    VariationCoefficient = 0.7
                },
                StormDuration = new StochastConfiguration
                {
                    Mean = 8.1,
                    VariationCoefficient = 0.8
                }
            };

            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var assigner = new HeightStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

            // Call
            var valid = assigner.SetAllStochasts();

            // Assert
            Assert.IsTrue(valid);
        }

        bool StandardDeviationStochastSetter(HeightStructuresCalculationStochastAssigner.StandardDeviationDefinition definition)
        {
            return true;
        }

        bool VariationCoefficientStochastSetter(HeightStructuresCalculationStochastAssigner.VariationCoefficientDefinition definition)
        {
            return true;
        }
    }
}