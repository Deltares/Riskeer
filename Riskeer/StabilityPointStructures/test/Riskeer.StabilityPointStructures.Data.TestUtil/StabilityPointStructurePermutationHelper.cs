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

namespace Riskeer.StabilityPointStructures.Data.TestUtil
{
    /// <summary>
    /// Helper containing a source of modified <see cref="StabilityPointStructure"/> entities that can
    /// be used in tests as a TestCaseSource.
    /// </summary>
    public static class StabilityPointStructurePermutationHelper
    {
        /// <summary>
        /// Returns a collection of modified <see cref="StabilityPointStructure"/> entities.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(typeof(StabilityPointStructurePermutationHelper),
        ///     nameof(StabilityPointStructurePermutationHelper.DifferentStabilityPointStructures),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentStabilityPointStructures(string targetName, string testResultDescription)
        {
            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(new TestStabilityPointStructure("Different id"))
                    .SetName($"{targetName}_DifferentId_{testResultDescription}")
            };

            testCaseData.AddRange(DifferentStabilityPointStructuresWithSameId(targetName, testResultDescription));

            return testCaseData;
        }

        /// <summary>
        /// Returns a collection of modified <see cref="StabilityPointStructure"/> entities, which all differ
        /// except for their id.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(typeof(StabilityPointStructurePermutationHelper),
        ///     nameof(StabilityPointStructurePermutationHelper.DifferentStabilityPointStructuresWithSameId),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentStabilityPointStructuresWithSameId(string targetName, string testResultDescription)
        {
            string referenceStructureId = new TestStabilityPointStructure().Id;

            var testCaseData = new List<TestCaseData>
            {
                new TestCaseData(new TestStabilityPointStructure(referenceStructureId, "Different name"))
                    .SetName($"{targetName}_DifferentName_{testResultDescription}"),
                new TestCaseData(new TestStabilityPointStructure(new Point2D(1, 1), referenceStructureId))
                    .SetName($"{targetName}_DifferentLocation_{testResultDescription}")
            };

            testCaseData.AddRange(DifferentStabilityPointStructuresWithSameIdNameAndLocation(targetName, testResultDescription));

            return testCaseData;
        }

        /// <summary>
        /// Returns a collection of modified <see cref="StabilityPointStructure"/> entities, which all differ
        /// except for their id, name and location.
        /// </summary>
        /// <param name="targetName">The name of the target to test while using the test case source.</param>
        /// <param name="testResultDescription">A description of the result of the test while using the test case source.</param>
        /// <returns>The collection of test case data.</returns>
        /// <example>
        /// <code>
        /// [TestCaseSource(
        ///     typeof(StabilityPointStructurePermutationHelper),
        ///     nameof(StabilityPointStructurePermutationHelper.DifferentStabilityPointStructuresWithSameIdNameAndLocation),
        ///     new object[]
        ///     {
        ///         "TargetMethodName",
        ///         "TestResult"
        ///     })]
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> DifferentStabilityPointStructuresWithSameIdNameAndLocation(string targetName, string testResultDescription)
        {
            var random = new Random(532);

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                StorageStructureArea =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStorageStructureAreaMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                StorageStructureArea =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStorageStructureAreaCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                AllowedLevelIncreaseStorage =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAllowedLevelIncreaseStorageMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                AllowedLevelIncreaseStorage =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAllowedLevelIncreaseStorageStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                WidthFlowApertures =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentWidthFlowAperturesMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                WidthFlowApertures =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentWidthFlowAperturesStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                InsideWaterLevel =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentInsideWaterLevelMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                InsideWaterLevel =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentInsideWaterLevelStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ThresholdHeightOpenWeir =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentThresholdHeightOpenWeirMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ThresholdHeightOpenWeir =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentThresholdHeightOpenWeirStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                CriticalOvertoppingDischarge =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentCriticalOvertoppingDischargeMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                CriticalOvertoppingDischarge =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentCriticalOvertoppingDischargeCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                FlowWidthAtBottomProtection =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFlowWidthAtBottomProtectionMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                FlowWidthAtBottomProtection =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFlowWidthAtBottomProtectionStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ConstructiveStrengthLinearLoadModel =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentConstructiveStrengthLinearLoadModelMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ConstructiveStrengthLinearLoadModel =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentConstructiveStrengthLinearLoadModelCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ConstructiveStrengthQuadraticLoadModel =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentConstructiveStrengthQuadraticLoadModelMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ConstructiveStrengthQuadraticLoadModel =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentConstructiveStrengthQuadraticLoadModelCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                BankWidth =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentBankWidthMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                BankWidth =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentBankWidthStandardDeviation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                InsideWaterLevelFailureConstruction =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentInsideWaterLevelFailureConstructionMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                InsideWaterLevelFailureConstruction =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentInsideWaterLevelFailureConstructionStandardDeviation_{testResultDescription}");

            StabilityPointStructure.ConstructionProperties differentEvaluationLevelConstructionProperties =
                CreateTestStabilityPointStructureConstructionProperties();
            differentEvaluationLevelConstructionProperties.EvaluationLevel = random.NextDouble();
            yield return new TestCaseData(new StabilityPointStructure(differentEvaluationLevelConstructionProperties))
                .SetName($"{targetName}_DifferentEvaluationLevel_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                LevelCrestStructure =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentLevelCrestStructureMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                LevelCrestStructure =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentLevelCrestStructureStandardDeviation_{testResultDescription}");

            StabilityPointStructure.ConstructionProperties differentVerticalDistanceConstructionProperties =
                CreateTestStabilityPointStructureConstructionProperties();
            differentVerticalDistanceConstructionProperties.VerticalDistance = random.NextDouble();
            yield return new TestCaseData(new StabilityPointStructure(differentVerticalDistanceConstructionProperties))
                .SetName($"{targetName}_DifferentVerticalDistance_{testResultDescription}");

            StabilityPointStructure.ConstructionProperties differentFailureProbabilityRepairClosureConstructionProperties =
                CreateTestStabilityPointStructureConstructionProperties();
            differentFailureProbabilityRepairClosureConstructionProperties.FailureProbabilityRepairClosure = random.NextDouble();
            yield return new TestCaseData(new StabilityPointStructure(differentFailureProbabilityRepairClosureConstructionProperties))
                .SetName($"{targetName}_DifferentFailureProbabilityRepairClosure_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                FailureCollisionEnergy =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFailureCollisionEnergyMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                FailureCollisionEnergy =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFailureCollisionEnergyCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ShipMass =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentShipMassMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ShipMass =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentShipMassCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ShipVelocity =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentShipVelocityMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                ShipVelocity =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentShipVelocityCoefficientOfVariation_{testResultDescription}");

            StabilityPointStructure.ConstructionProperties differentLevellingCountConstructionProperties =
                CreateTestStabilityPointStructureConstructionProperties();
            differentLevellingCountConstructionProperties.LevellingCount = random.Next();
            yield return new TestCaseData(new StabilityPointStructure(differentLevellingCountConstructionProperties))
                .SetName($"{targetName}_DifferentLevellingCount_{testResultDescription}");

            StabilityPointStructure.ConstructionProperties differentProbabilityCollisionSecondaryStructureConstructionProperties =
                CreateTestStabilityPointStructureConstructionProperties();
            differentProbabilityCollisionSecondaryStructureConstructionProperties.ProbabilityCollisionSecondaryStructure = random.NextDouble();
            yield return new TestCaseData(new StabilityPointStructure(differentProbabilityCollisionSecondaryStructureConstructionProperties))
                .SetName($"{targetName}_DifferentProbabilityCollisionSecondaryStructure_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                FlowVelocityStructureClosable =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentFlowVelocityStructureClosableMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                StabilityLinearLoadModel =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStabilityLinearLoadModelMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                StabilityLinearLoadModel =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStabilityLinearLoadModelCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                StabilityQuadraticLoadModel =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStabilityQuadraticLoadModelMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                StabilityQuadraticLoadModel =
                {
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentStabilityQuadraticLoadModelCoefficientOfVariation_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                AreaFlowApertures =
                {
                    Mean = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAreaFlowAperturesMean_{testResultDescription}");

            yield return new TestCaseData(new TestStabilityPointStructure
            {
                AreaFlowApertures =
                {
                    StandardDeviation = random.NextRoundedDouble()
                }
            }).SetName($"{targetName}_DifferentAreaFlowAperturesStandardDeviation_{testResultDescription}");

            StabilityPointStructure.ConstructionProperties differentInflowModelTypeConstructionProperties =
                CreateTestStabilityPointStructureConstructionProperties();
            differentInflowModelTypeConstructionProperties.InflowModelType = StabilityPointStructureInflowModelType.LowSill;
            yield return new TestCaseData(new StabilityPointStructure(differentInflowModelTypeConstructionProperties))
                .SetName($"{targetName}_DifferentInflowModelType_{testResultDescription}");
        }

        private static StabilityPointStructure.ConstructionProperties CreateTestStabilityPointStructureConstructionProperties()
        {
            var referenceStructure = new TestStabilityPointStructure();

            return new StabilityPointStructure.ConstructionProperties
            {
                Name = referenceStructure.Name,
                Id = referenceStructure.Id,
                Location = referenceStructure.Location,
                StorageStructureArea =
                {
                    Mean = referenceStructure.StorageStructureArea.Mean,
                    CoefficientOfVariation = referenceStructure.StorageStructureArea.CoefficientOfVariation
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = referenceStructure.AllowedLevelIncreaseStorage.Mean,
                    StandardDeviation = referenceStructure.AllowedLevelIncreaseStorage.StandardDeviation
                },
                WidthFlowApertures =
                {
                    Mean = referenceStructure.WidthFlowApertures.Mean,
                    StandardDeviation = referenceStructure.WidthFlowApertures.StandardDeviation
                },
                InsideWaterLevel =
                {
                    Mean = referenceStructure.InsideWaterLevel.Mean,
                    StandardDeviation = referenceStructure.InsideWaterLevel.StandardDeviation
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = referenceStructure.ThresholdHeightOpenWeir.Mean,
                    StandardDeviation = referenceStructure.ThresholdHeightOpenWeir.StandardDeviation
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
                ConstructiveStrengthLinearLoadModel =
                {
                    Mean = referenceStructure.ConstructiveStrengthLinearLoadModel.Mean,
                    CoefficientOfVariation = referenceStructure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation
                },
                ConstructiveStrengthQuadraticLoadModel =
                {
                    Mean = referenceStructure.ConstructiveStrengthQuadraticLoadModel.Mean,
                    CoefficientOfVariation = referenceStructure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation
                },
                BankWidth =
                {
                    Mean = referenceStructure.BankWidth.Mean,
                    StandardDeviation = referenceStructure.BankWidth.StandardDeviation
                },
                InsideWaterLevelFailureConstruction =
                {
                    Mean = referenceStructure.InsideWaterLevelFailureConstruction.Mean,
                    StandardDeviation = referenceStructure.InsideWaterLevelFailureConstruction.StandardDeviation
                },
                EvaluationLevel = referenceStructure.EvaluationLevel,
                LevelCrestStructure =
                {
                    Mean = referenceStructure.LevelCrestStructure.Mean,
                    StandardDeviation = referenceStructure.LevelCrestStructure.StandardDeviation
                },
                VerticalDistance = referenceStructure.VerticalDistance,
                FailureProbabilityRepairClosure = referenceStructure.FailureProbabilityRepairClosure,
                FailureCollisionEnergy =
                {
                    Mean = referenceStructure.FailureCollisionEnergy.Mean,
                    CoefficientOfVariation = referenceStructure.FailureCollisionEnergy.CoefficientOfVariation
                },
                ShipMass =
                {
                    Mean = referenceStructure.ShipMass.Mean,
                    CoefficientOfVariation = referenceStructure.ShipMass.CoefficientOfVariation
                },
                ShipVelocity =
                {
                    Mean = referenceStructure.ShipVelocity.Mean,
                    CoefficientOfVariation = referenceStructure.ShipVelocity.CoefficientOfVariation
                },
                LevellingCount = referenceStructure.LevellingCount,
                ProbabilityCollisionSecondaryStructure = referenceStructure.ProbabilityCollisionSecondaryStructure,
                FlowVelocityStructureClosable =
                {
                    Mean = referenceStructure.FlowVelocityStructureClosable.Mean,
                    CoefficientOfVariation = referenceStructure.FlowVelocityStructureClosable.CoefficientOfVariation
                },
                StabilityLinearLoadModel =
                {
                    Mean = referenceStructure.StabilityLinearLoadModel.Mean,
                    CoefficientOfVariation = referenceStructure.StabilityLinearLoadModel.CoefficientOfVariation
                },
                StabilityQuadraticLoadModel =
                {
                    Mean = referenceStructure.StabilityQuadraticLoadModel.Mean,
                    CoefficientOfVariation = referenceStructure.StabilityQuadraticLoadModel.CoefficientOfVariation
                },
                AreaFlowApertures =
                {
                    Mean = referenceStructure.AreaFlowApertures.Mean,
                    StandardDeviation = referenceStructure.AreaFlowApertures.StandardDeviation
                },
                InflowModelType = referenceStructure.InflowModelType
            };
        }
    }
}