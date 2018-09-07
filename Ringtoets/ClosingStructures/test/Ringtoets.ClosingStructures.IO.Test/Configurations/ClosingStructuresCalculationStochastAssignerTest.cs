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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.IO.Configurations;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.ClosingStructures.IO.Test.Configurations
{
    [TestFixture]
    public class ClosingStructuresCalculationStochastAssignerTest
    {
        [Test]
        public void Constructor_WithParameters_ReturnsNewInstance()
        {
            // Setup
            var configuration = new ClosingStructuresCalculationConfiguration("name");
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            // Call
            var assigner = new ClosingStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            // Assert
            Assert.IsInstanceOf<StructuresCalculationStochastAssigner<
                ClosingStructuresCalculationConfiguration,
                ClosingStructuresInput, ClosingStructure>>(assigner);
        }

        [Test]
        [TestCaseSource(
            typeof(ClosingStructuresCalculationStochastAssignerTest),
            nameof(GetSetStochastParametersActions),
            new object[]
            {
                "Assign_WithoutStructureValueSetFor{0}_ReturnsFalse"
            })]
        public void Assign_WithoutStructureParametersDefinedForStructureDependentStochast_ReturnsFalse(
            Action<ClosingStructuresCalculationConfiguration> modify)
        {
            // Setup
            var configuration = new ClosingStructuresCalculationConfiguration("name");
            modify(configuration);

            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var assigner = new ClosingStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            // Call
            bool valid = assigner.Assign();

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        [TestCaseSource(
            typeof(ClosingStructuresCalculationStochastAssignerTest),
            nameof(GetSetStochastParametersActions),
            new object[]
            {
                "Assign_WithStructureValueSetFor{0}_ReturnsTrue"
            })]
        public void Assign_WithStructureParametersDefinedForStructureDependentStochast_ReturnsTrue(
            Action<ClosingStructuresCalculationConfiguration> modify)
        {
            // Setup
            var configuration = new ClosingStructuresCalculationConfiguration("name")
            {
                StructureId = "some structure"
            };
            modify(configuration);

            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var assigner = new ClosingStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            // Call
            bool valid = assigner.Assign();

            // Assert
            Assert.IsTrue(valid);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Assign_WithSpreadForModelFactorSuperCriticalFlow_LogsErrorReturnFalse(bool withStandardDeviation)
        {
            // Setup
            var configuration = new ClosingStructuresCalculationConfiguration("name")
            {
                StructureId = "some structure",
                ModelFactorSuperCriticalFlow = new StochastConfiguration
                {
                    Mean = 8.1
                }
            };

            if (withStandardDeviation)
            {
                configuration.ModelFactorSuperCriticalFlow.StandardDeviation = 0.8;
            }
            else
            {
                configuration.ModelFactorSuperCriticalFlow.VariationCoefficient = 0.8;
            }

            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var assigner = new ClosingStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            var valid = true;

            // Call
            Action test = () => valid = assigner.Assign();

            // Assert
            const string expectedMessage = "Er kan geen spreiding voor stochast 'modelfactor overloopdebiet volkomen overlaat' opgegeven worden. Berekening 'name' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(test, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Assign_WithSpreadForDrainCoefficient_LogsErrorReturnFalse(bool withStandardDeviation)
        {
            // Setup
            var configuration = new ClosingStructuresCalculationConfiguration("name")
            {
                StructureId = "some structure",
                DrainCoefficient = new StochastConfiguration
                {
                    Mean = 8.1
                }
            };
            if (withStandardDeviation)
            {
                configuration.DrainCoefficient.StandardDeviation = 0.8;
            }
            else
            {
                configuration.DrainCoefficient.VariationCoefficient = 0.8;
            }

            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var assigner = new ClosingStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            var valid = true;

            // Call
            Action test = () => valid = assigner.Assign();

            // Assert
            const string expectedMessage = "Er kan geen spreiding voor stochast 'afvoercoefficient' opgegeven worden. Berekening 'name' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(test, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
        }

        [Test]
        public void Assign_WithAllStochastsSet_SetExpectedValuesOnInput()
        {
            // Setup
            var configuration = new ClosingStructuresCalculationConfiguration("name")
            {
                StructureId = "some structure",
                LevelCrestStructureNotClosing = new StochastConfiguration
                {
                    Mean = 1.1,
                    StandardDeviation = 1.5
                },
                AreaFlowApertures = new StochastConfiguration
                {
                    Mean = 2.1,
                    StandardDeviation = 2.5
                },
                InsideWaterLevel = new StochastConfiguration
                {
                    Mean = 3.1,
                    StandardDeviation = 3.5
                },
                ThresholdHeightOpenWeir = new StochastConfiguration
                {
                    Mean = 4.1,
                    StandardDeviation = 4.5
                },
                AllowedLevelIncreaseStorage = new StochastConfiguration
                {
                    Mean = 5.1,
                    StandardDeviation = 5.5
                },
                FlowWidthAtBottomProtection = new StochastConfiguration
                {
                    Mean = 6.1,
                    StandardDeviation = 6.5
                },
                WidthFlowApertures = new StochastConfiguration
                {
                    Mean = 7.1,
                    StandardDeviation = 7.5
                },
                DrainCoefficient = new StochastConfiguration
                {
                    Mean = 8.1
                },
                ModelFactorSuperCriticalFlow = new StochastConfiguration
                {
                    Mean = 9.1
                },
                CriticalOvertoppingDischarge = new StochastConfiguration
                {
                    Mean = 10.1,
                    VariationCoefficient = 0.1
                },
                StorageStructureArea = new StochastConfiguration
                {
                    Mean = 11.1,
                    VariationCoefficient = 0.11
                },
                StormDuration = new StochastConfiguration
                {
                    Mean = 12.1
                }
            };

            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var assigner = new ClosingStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            // Call
            bool valid = assigner.Assign();

            // Assert
            Assert.IsTrue(valid);
        }

        private static IEnumerable<TestCaseData> GetSetStochastParametersActions(string testNameFormat)
        {
            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.LevelCrestStructureNotClosing = s,
                nameof(ClosingStructuresCalculationConfiguration.LevelCrestStructureNotClosing),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.AreaFlowApertures = s,
                nameof(ClosingStructuresCalculationConfiguration.AreaFlowApertures),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.InsideWaterLevel = s,
                nameof(ClosingStructuresCalculationConfiguration.InsideWaterLevel),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.ThresholdHeightOpenWeir = s,
                nameof(ClosingStructuresCalculationConfiguration.ThresholdHeightOpenWeir),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.AllowedLevelIncreaseStorage = s,
                nameof(ClosingStructuresCalculationConfiguration.AllowedLevelIncreaseStorage),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.FlowWidthAtBottomProtection = s,
                nameof(ClosingStructuresCalculationConfiguration.FlowWidthAtBottomProtection),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.WidthFlowApertures = s,
                nameof(ClosingStructuresCalculationConfiguration.WidthFlowApertures),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.CriticalOvertoppingDischarge = s,
                nameof(ClosingStructuresCalculationConfiguration.CriticalOvertoppingDischarge),
                testNameFormat,
                false))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.StorageStructureArea = s,
                nameof(ClosingStructuresCalculationConfiguration.StorageStructureArea),
                testNameFormat,
                false))
            {
                yield return caseData;
            }
        }

        private static IEnumerable<TestCaseData> StochastConfigurationCases(
            Action<ClosingStructuresCalculationConfiguration, StochastConfiguration> modifyStochastAction,
            string stochastName,
            string testNameFormat,
            bool standardDeviation)
        {
            var random = new Random(21);

            yield return new TestCaseData(
                    new Action<ClosingStructuresCalculationConfiguration>(c => modifyStochastAction(c, new StochastConfiguration
                    {
                        Mean = random.NextDouble()
                    })))
                .SetName(string.Format(testNameFormat, $"{stochastName}Mean"));

            if (standardDeviation)
            {
                yield return new TestCaseData(
                        new Action<ClosingStructuresCalculationConfiguration>(c => modifyStochastAction(c, new StochastConfiguration
                        {
                            StandardDeviation = random.NextDouble()
                        })))
                    .SetName(string.Format(testNameFormat, $"{stochastName}StandardDeviation"));
            }
            else
            {
                yield return new TestCaseData(
                        new Action<ClosingStructuresCalculationConfiguration>(c => modifyStochastAction(c, new StochastConfiguration
                        {
                            VariationCoefficient = random.NextDouble()
                        })))
                    .SetName(string.Format(testNameFormat, $"{stochastName}VariationCoefficient"));
            }
        }
    }
}