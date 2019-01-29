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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.IO.Configurations;

namespace Riskeer.StabilityPointStructures.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityPointStructuresCalculationStochastAssignerTest
    {
        [Test]
        public void Constructor_WithParameters_ReturnsNewInstance()
        {
            // Setup
            var configuration = new StabilityPointStructuresCalculationConfiguration("name");
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            // Call
            var assigner = new StabilityPointStructuresCalculationStochastAssigner(
                configuration, calculation);

            // Assert
            Assert.IsInstanceOf<StructuresCalculationStochastAssigner<
                StabilityPointStructuresCalculationConfiguration,
                StabilityPointStructuresInput, StabilityPointStructure>>(assigner);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Assign_WithSpreadForDrainCoefficient_LogsErrorReturnFalse(bool withStandardDeviation)
        {
            // Setup
            var configuration = new StabilityPointStructuresCalculationConfiguration("name")
            {
                StructureId = "some structure"
            };
            configuration.DrainCoefficient = new StochastConfiguration();
            configuration.DrainCoefficient.Mean = 8.1;
            if (withStandardDeviation)
            {
                configuration.DrainCoefficient.StandardDeviation = 0.8;
            }
            else
            {
                configuration.DrainCoefficient.VariationCoefficient = 0.8;
            }

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            var assigner = new StabilityPointStructuresCalculationStochastAssigner(
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
        [TestCase(true)]
        [TestCase(false)]
        public void Assign_WithSpreadForFlowVelocityStructureClosable_LogsErrorReturnFalse(bool withStandardDeviation)
        {
            // Setup
            var configuration = new StabilityPointStructuresCalculationConfiguration("name")
            {
                StructureId = "some structure"
            };
            configuration.FlowVelocityStructureClosable = new StochastConfiguration();
            configuration.FlowVelocityStructureClosable.Mean = 8.1;
            if (withStandardDeviation)
            {
                configuration.FlowVelocityStructureClosable.StandardDeviation = 0.8;
            }
            else
            {
                configuration.FlowVelocityStructureClosable.VariationCoefficient = 0.8;
            }

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            var assigner = new StabilityPointStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            var valid = true;

            // Call
            Action test = () => valid = assigner.Assign();

            // Assert
            const string expectedMessage = "Er kan geen spreiding voor stochast 'kritiekestroomsnelheid' opgegeven worden. Berekening 'name' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(test, Tuple.Create(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(valid);
        }

        [Test]
        public void Assign_WithAllStochastsSet_SetExpectedValuesOnInput()
        {
            // Setup
            var configuration = new StabilityPointStructuresCalculationConfiguration("name")
            {
                AllowedLevelIncreaseStorage = new StochastConfiguration
                {
                    Mean = 1,
                    StandardDeviation = 0.1
                },
                AreaFlowApertures = new StochastConfiguration
                {
                    Mean = 2,
                    StandardDeviation = 0.2
                },
                BankWidth = new StochastConfiguration
                {
                    Mean = 3,
                    StandardDeviation = 0.3
                },
                FlowWidthAtBottomProtection = new StochastConfiguration
                {
                    Mean = 4,
                    StandardDeviation = 0.4
                },
                InsideWaterLevel = new StochastConfiguration
                {
                    Mean = 5,
                    StandardDeviation = 0.5
                },
                InsideWaterLevelFailureConstruction = new StochastConfiguration
                {
                    Mean = 6,
                    StandardDeviation = 0.6
                },
                LevelCrestStructure = new StochastConfiguration
                {
                    Mean = 7,
                    StandardDeviation = 0.7
                },
                WidthFlowApertures = new StochastConfiguration
                {
                    Mean = 8,
                    StandardDeviation = 0.8
                },
                ThresholdHeightOpenWeir = new StochastConfiguration
                {
                    Mean = 9,
                    StandardDeviation = 0.9
                },
                DrainCoefficient = new StochastConfiguration
                {
                    Mean = 10
                },
                FlowVelocityStructureClosable = new StochastConfiguration
                {
                    Mean = 1
                },
                CriticalOvertoppingDischarge = new StochastConfiguration
                {
                    Mean = 2,
                    VariationCoefficient = 0.2
                },
                ConstructiveStrengthLinearLoadModel = new StochastConfiguration
                {
                    Mean = 3,
                    VariationCoefficient = 0.3
                },
                ConstructiveStrengthQuadraticLoadModel = new StochastConfiguration
                {
                    Mean = 4,
                    VariationCoefficient = 0.4
                },
                FailureCollisionEnergy = new StochastConfiguration
                {
                    Mean = 5,
                    VariationCoefficient = 0.5
                },
                ShipMass = new StochastConfiguration
                {
                    Mean = 6,
                    VariationCoefficient = 0.6
                },
                ShipVelocity = new StochastConfiguration
                {
                    Mean = 7,
                    VariationCoefficient = 0.7
                },
                StabilityLinearLoadModel = new StochastConfiguration
                {
                    Mean = 8,
                    VariationCoefficient = 0.8
                },
                StabilityQuadraticLoadModel = new StochastConfiguration
                {
                    Mean = 9,
                    VariationCoefficient = 0.9
                },
                StorageStructureArea = new StochastConfiguration
                {
                    Mean = 10,
                    VariationCoefficient = 0.10
                },
                StormDuration = new StochastConfiguration
                {
                    Mean = 11
                },
                StructureId = "some structure"
            };

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            var assigner = new StabilityPointStructuresCalculationStochastAssigner(
                configuration,
                calculation);

            // Call
            bool valid = assigner.Assign();

            // Assert
            Assert.IsTrue(valid);
        }

        [Test]
        [TestCaseSource(
            typeof(StabilityPointStructuresCalculationStochastAssignerTest),
            nameof(GetSetStochastParametersActions),
            new object[]
            {
                "Assign_{0}WithStructure_ReturnsTrue"
            })]
        public void Assign_WithStructureParametersDefinedForStructureDependentStochast_ReturnsTrue(
            Action<StabilityPointStructuresCalculationConfiguration> updateConfiguration)
        {
            // Setup
            var configuration = new StabilityPointStructuresCalculationConfiguration("name")
            {
                StructureId = "some structure"
            };
            updateConfiguration(configuration);

            var assigner = new StabilityPointStructuresCalculationStochastAssigner(
                configuration,
                new StructuresCalculation<StabilityPointStructuresInput>());

            // Call
            bool valid = assigner.Assign();

            // Assert
            Assert.IsTrue(valid);
        }

        [Test]
        [TestCaseSource(
            typeof(StabilityPointStructuresCalculationStochastAssignerTest),
            nameof(GetSetStochastParametersActions),
            new object[]
            {
                "Assign_{0}SetWithoutStructure_ReturnsFalse"
            })]
        public void Assign_WithoutStructureParametersDefinedForStructureDependentStochast_ReturnsFalse(
            Action<StabilityPointStructuresCalculationConfiguration> updateConfiguration)
        {
            // Setup
            var configuration = new StabilityPointStructuresCalculationConfiguration("name");
            updateConfiguration(configuration);

            var assigner = new StabilityPointStructuresCalculationStochastAssigner(
                configuration,
                new StructuresCalculation<StabilityPointStructuresInput>());

            // Call
            bool valid = assigner.Assign();

            // Assert
            Assert.IsFalse(valid);
        }

        private static IEnumerable<TestCaseData> GetSetStochastParametersActions(string testNameFormat)
        {
            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.AllowedLevelIncreaseStorage = s,
                nameof(StabilityPointStructuresCalculationConfiguration.AllowedLevelIncreaseStorage),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.AreaFlowApertures = s,
                nameof(StabilityPointStructuresCalculationConfiguration.AreaFlowApertures),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.BankWidth = s,
                nameof(StabilityPointStructuresCalculationConfiguration.BankWidth),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.FlowWidthAtBottomProtection = s,
                nameof(StabilityPointStructuresCalculationConfiguration.FlowWidthAtBottomProtection),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.InsideWaterLevel = s,
                nameof(StabilityPointStructuresCalculationConfiguration.InsideWaterLevel),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.InsideWaterLevelFailureConstruction = s,
                nameof(StabilityPointStructuresCalculationConfiguration.InsideWaterLevelFailureConstruction),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.LevelCrestStructure = s,
                nameof(StabilityPointStructuresCalculationConfiguration.LevelCrestStructure),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.WidthFlowApertures = s,
                nameof(StabilityPointStructuresCalculationConfiguration.WidthFlowApertures),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.ThresholdHeightOpenWeir = s,
                nameof(StabilityPointStructuresCalculationConfiguration.ThresholdHeightOpenWeir),
                testNameFormat,
                true))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.CriticalOvertoppingDischarge = s,
                nameof(StabilityPointStructuresCalculationConfiguration.CriticalOvertoppingDischarge),
                testNameFormat,
                false))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.ConstructiveStrengthLinearLoadModel = s,
                nameof(StabilityPointStructuresCalculationConfiguration.ConstructiveStrengthLinearLoadModel),
                testNameFormat,
                false))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.ConstructiveStrengthQuadraticLoadModel = s,
                nameof(StabilityPointStructuresCalculationConfiguration.ConstructiveStrengthQuadraticLoadModel),
                testNameFormat,
                false))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.FailureCollisionEnergy = s,
                nameof(StabilityPointStructuresCalculationConfiguration.FailureCollisionEnergy),
                testNameFormat,
                false))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.FlowVelocityStructureClosable = s,
                nameof(StabilityPointStructuresCalculationConfiguration.FlowVelocityStructureClosable),
                testNameFormat,
                null))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.ShipMass = s,
                nameof(StabilityPointStructuresCalculationConfiguration.ShipMass),
                testNameFormat,
                false))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.ShipVelocity = s,
                nameof(StabilityPointStructuresCalculationConfiguration.ShipVelocity),
                testNameFormat,
                false))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.StabilityLinearLoadModel = s,
                nameof(StabilityPointStructuresCalculationConfiguration.StabilityLinearLoadModel),
                testNameFormat,
                false))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.StabilityQuadraticLoadModel = s,
                nameof(StabilityPointStructuresCalculationConfiguration.StabilityQuadraticLoadModel),
                testNameFormat,
                false))
            {
                yield return caseData;
            }

            foreach (TestCaseData caseData in StochastConfigurationCases(
                (c, s) => c.StorageStructureArea = s,
                nameof(StabilityPointStructuresCalculationConfiguration.StorageStructureArea),
                testNameFormat,
                false))
            {
                yield return caseData;
            }
        }

        private static IEnumerable<TestCaseData> StochastConfigurationCases(
            Action<StabilityPointStructuresCalculationConfiguration, StochastConfiguration> modifyStochastAction,
            string stochastName,
            string testNameFormat,
            bool? standardDeviation)
        {
            var random = new Random(21);

            yield return new TestCaseData(
                    new Action<StabilityPointStructuresCalculationConfiguration>(c => modifyStochastAction(c, new StochastConfiguration
                    {
                        Mean = random.NextDouble()
                    })))
                .SetName(string.Format(testNameFormat, $"{stochastName}Mean"));

            if (standardDeviation == true)
            {
                yield return new TestCaseData(
                        new Action<StabilityPointStructuresCalculationConfiguration>(c => modifyStochastAction(c, new StochastConfiguration
                        {
                            StandardDeviation = random.NextDouble()
                        })))
                    .SetName(string.Format(testNameFormat, $"{stochastName}StandardDeviation"));
            }
            else if (standardDeviation == false)
            {
                yield return new TestCaseData(
                        new Action<StabilityPointStructuresCalculationConfiguration>(c => modifyStochastAction(c, new StochastConfiguration
                        {
                            VariationCoefficient = random.NextDouble()
                        })))
                    .SetName(string.Format(testNameFormat, $"{stochastName}VariationCoefficient"));
            }
        }
    }
}