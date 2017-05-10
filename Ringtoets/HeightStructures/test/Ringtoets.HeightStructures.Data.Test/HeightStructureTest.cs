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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructureTest
    {
        private static IEnumerable<TestCaseData> StructureCombinations
        {
            get
            {
                return GetInequalStructures.Concat(GetEqualStructures);
            }
        }

        private static IEnumerable<TestCaseData> GetEqualStructures
        {
            get
            {
                HeightStructure structure = CreateFullyDefinedStructure();
                yield return new TestCaseData(structure, structure, true)
                    .SetName("SameStructure");
                yield return new TestCaseData(structure, CreateFullyDefinedStructure(), true)
                    .SetName("EqualStructure");
            }
        }

        private static IEnumerable<TestCaseData> GetInequalStructures
        {
            get
            {
                HeightStructure structure = CreateFullyDefinedStructure();

                HeightStructure.ConstructionProperties differentId = CreateFullyConfiguredConstructionProperties();
                differentId.Id = "differentId";
                yield return new TestCaseData(structure, new HeightStructure(differentId), false)
                    .SetName(nameof(differentId));

                HeightStructure.ConstructionProperties differentName = CreateFullyConfiguredConstructionProperties();
                differentName.Name = "differentName";
                yield return new TestCaseData(structure, new HeightStructure(differentName), false)
                    .SetName(nameof(differentName));

                HeightStructure.ConstructionProperties differentLocation = CreateFullyConfiguredConstructionProperties();
                differentLocation.Location = new Point2D(9, 9);
                yield return new TestCaseData(structure, new HeightStructure(differentLocation), false)
                    .SetName(nameof(differentLocation));

                HeightStructure.ConstructionProperties differentOrientation = CreateFullyConfiguredConstructionProperties();
                differentOrientation.StructureNormalOrientation = (RoundedDouble) 90;
                yield return new TestCaseData(structure, new HeightStructure(differentOrientation), false)
                    .SetName(nameof(differentOrientation));

                HeightStructure.ConstructionProperties differentAllowedLevelIncreaseStorageMean = CreateFullyConfiguredConstructionProperties();
                differentAllowedLevelIncreaseStorageMean.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new HeightStructure(differentAllowedLevelIncreaseStorageMean), false)
                    .SetName(nameof(differentAllowedLevelIncreaseStorageMean));

                HeightStructure.ConstructionProperties differentAllowedLevelIncreaseStorageStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentAllowedLevelIncreaseStorageStandardDeviation.AllowedLevelIncreaseStorage.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new HeightStructure(differentAllowedLevelIncreaseStorageStandardDeviation), false)
                    .SetName(nameof(differentAllowedLevelIncreaseStorageStandardDeviation));

                HeightStructure.ConstructionProperties differentCriticalOvertoppingDischargeMean = CreateFullyConfiguredConstructionProperties();
                differentCriticalOvertoppingDischargeMean.CriticalOvertoppingDischarge.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new HeightStructure(differentCriticalOvertoppingDischargeMean), false)
                    .SetName(nameof(differentCriticalOvertoppingDischargeMean));

                HeightStructure.ConstructionProperties differentCriticalOvertoppingDischargeCoefficientOfVariation = CreateFullyConfiguredConstructionProperties();
                differentCriticalOvertoppingDischargeCoefficientOfVariation.CriticalOvertoppingDischarge.CoefficientOfVariation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new HeightStructure(differentCriticalOvertoppingDischargeCoefficientOfVariation), false)
                    .SetName(nameof(differentCriticalOvertoppingDischargeCoefficientOfVariation));

                HeightStructure.ConstructionProperties differentFailureProbabilityStructureWithErosion = CreateFullyConfiguredConstructionProperties();
                differentFailureProbabilityStructureWithErosion.FailureProbabilityStructureWithErosion = 987;
                yield return new TestCaseData(structure, new HeightStructure(differentFailureProbabilityStructureWithErosion), false)
                    .SetName(nameof(differentFailureProbabilityStructureWithErosion));

                HeightStructure.ConstructionProperties differentFlowWidthAtBottomProtectionMean = CreateFullyConfiguredConstructionProperties();
                differentFlowWidthAtBottomProtectionMean.FlowWidthAtBottomProtection.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new HeightStructure(differentFlowWidthAtBottomProtectionMean), false)
                    .SetName(nameof(differentFlowWidthAtBottomProtectionMean));

                HeightStructure.ConstructionProperties differentFlowWidthAtBottomProtectionStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentFlowWidthAtBottomProtectionStandardDeviation.FlowWidthAtBottomProtection.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new HeightStructure(differentFlowWidthAtBottomProtectionStandardDeviation), false)
                    .SetName(nameof(differentFlowWidthAtBottomProtectionStandardDeviation));

                HeightStructure.ConstructionProperties differentLevelCrestStructureMean = CreateFullyConfiguredConstructionProperties();
                differentLevelCrestStructureMean.LevelCrestStructure.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new HeightStructure(differentLevelCrestStructureMean), false)
                    .SetName(nameof(differentLevelCrestStructureMean));

                HeightStructure.ConstructionProperties differentLevelCrestStructureStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentLevelCrestStructureStandardDeviation.LevelCrestStructure.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new HeightStructure(differentLevelCrestStructureStandardDeviation), false)
                    .SetName(nameof(differentLevelCrestStructureStandardDeviation));

                HeightStructure.ConstructionProperties differentStorageStructureAreaMean = CreateFullyConfiguredConstructionProperties();
                differentStorageStructureAreaMean.StorageStructureArea.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new HeightStructure(differentStorageStructureAreaMean), false)
                    .SetName(nameof(differentStorageStructureAreaMean));

                HeightStructure.ConstructionProperties differentStorageStructureAreaCoefficientOfVariation = CreateFullyConfiguredConstructionProperties();
                differentStorageStructureAreaCoefficientOfVariation.StorageStructureArea.CoefficientOfVariation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new HeightStructure(differentStorageStructureAreaCoefficientOfVariation), false)
                    .SetName(nameof(differentStorageStructureAreaCoefficientOfVariation));

                HeightStructure.ConstructionProperties differentWidthFlowAperturesMean = CreateFullyConfiguredConstructionProperties();
                differentWidthFlowAperturesMean.WidthFlowApertures.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new HeightStructure(differentWidthFlowAperturesMean), false)
                    .SetName(nameof(differentWidthFlowAperturesMean));

                HeightStructure.ConstructionProperties differentWidthFlowAperturesStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentWidthFlowAperturesStandardDeviation.WidthFlowApertures.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new HeightStructure(differentWidthFlowAperturesStandardDeviation), false)
                    .SetName(nameof(differentWidthFlowAperturesStandardDeviation));
            }
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);

            // Call
            var heightStructure = new HeightStructure(
                new HeightStructure.ConstructionProperties
                {
                    Name = "aName",
                    Id = "anId",
                    Location = location,
                    StructureNormalOrientation = (RoundedDouble) 0.12345,
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) 234.567,
                        StandardDeviation = (RoundedDouble) 0.23456
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 345.678,
                        StandardDeviation = (RoundedDouble) 0.34567
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 456.789,
                        CoefficientOfVariation = (RoundedDouble) 0.45678
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 567.890,
                        StandardDeviation = (RoundedDouble) 0.56789
                    },
                    FailureProbabilityStructureWithErosion = 0.67890,
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 112.223,
                        CoefficientOfVariation = (RoundedDouble) 0.11222
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 225.336,
                        StandardDeviation = (RoundedDouble) 0.22533
                    }
                });

            // Assert
            Assert.IsInstanceOf<StructureBase>(heightStructure);
            Assert.AreEqual("aName", heightStructure.Name);
            Assert.AreEqual("anId", heightStructure.Id);
            Assert.IsInstanceOf<Point2D>(heightStructure.Location);
            Assert.AreEqual(location.X, heightStructure.Location.X);
            Assert.AreEqual(location.Y, heightStructure.Location.Y);

            Assert.AreEqual(2, heightStructure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.12, heightStructure.StructureNormalOrientation.Value);

            NormalDistribution levelCrestStructure = heightStructure.LevelCrestStructure;
            Assert.AreEqual(2, levelCrestStructure.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(234.57, levelCrestStructure.Mean.Value);
            Assert.AreEqual(2, levelCrestStructure.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, levelCrestStructure.StandardDeviation.Value);

            LogNormalDistribution flowWidthAtBottomProtection = heightStructure.FlowWidthAtBottomProtection;
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(345.68, flowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.35, flowWidthAtBottomProtection.StandardDeviation.Value);

            VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge = heightStructure.CriticalOvertoppingDischarge;
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(456.79, criticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(2, criticalOvertoppingDischarge.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.46, criticalOvertoppingDischarge.CoefficientOfVariation.Value);

            NormalDistribution widthFlowApertures = heightStructure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, widthFlowApertures.Mean.Value);
            Assert.AreEqual(2, widthFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, widthFlowApertures.StandardDeviation.Value);

            Assert.AreEqual(0.67890, heightStructure.FailureProbabilityStructureWithErosion);

            VariationCoefficientLogNormalDistribution storageStructureArea = heightStructure.StorageStructureArea;
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(112.22, storageStructureArea.Mean.Value);
            Assert.AreEqual(2, storageStructureArea.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.11, storageStructureArea.CoefficientOfVariation.Value);

            LogNormalDistribution allowedLevelIncreaseStorage = heightStructure.AllowedLevelIncreaseStorage;
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(225.34, allowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, allowedLevelIncreaseStorage.StandardDeviation.Value);
        }

        [Test]
        public void Constructor_DefaultConstructorProperties_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);

            // Call
            var heightStructure = new HeightStructure(
                new HeightStructure.ConstructionProperties
                {
                    Name = "aName",
                    Id = "anId",
                    Location = location
                });

            // Assert
            Assert.IsInstanceOf<StructureBase>(heightStructure);
            Assert.AreEqual("aName", heightStructure.Name);
            Assert.AreEqual("anId", heightStructure.Id);
            Assert.IsInstanceOf<Point2D>(heightStructure.Location);
            Assert.AreEqual(location.X, heightStructure.Location.X);
            Assert.AreEqual(location.Y, heightStructure.Location.Y);

            Assert.AreEqual(2, heightStructure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.IsNaN(heightStructure.StructureNormalOrientation);

            NormalDistribution levelCrestStructure = heightStructure.LevelCrestStructure;
            Assert.AreEqual(2, levelCrestStructure.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, levelCrestStructure.Mean.Value);
            Assert.AreEqual(2, levelCrestStructure.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.05, levelCrestStructure.StandardDeviation.Value);

            LogNormalDistribution flowWidthAtBottomProtection = heightStructure.FlowWidthAtBottomProtection;
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, flowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.05, flowWidthAtBottomProtection.StandardDeviation.Value);

            VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge = heightStructure.CriticalOvertoppingDischarge;
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, criticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(2, criticalOvertoppingDischarge.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.15, criticalOvertoppingDischarge.CoefficientOfVariation.Value);

            NormalDistribution widthFlowApertures = heightStructure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, widthFlowApertures.Mean.Value);
            Assert.AreEqual(2, widthFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.2, widthFlowApertures.StandardDeviation.Value);

            Assert.AreEqual(1, heightStructure.FailureProbabilityStructureWithErosion);

            VariationCoefficientLogNormalDistribution storageStructureArea = heightStructure.StorageStructureArea;
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, storageStructureArea.Mean.Value);
            Assert.AreEqual(2, storageStructureArea.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, storageStructureArea.CoefficientOfVariation.Value);

            LogNormalDistribution allowedLevelIncreaseStorage = heightStructure.AllowedLevelIncreaseStorage;
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, allowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, allowedLevelIncreaseStorage.StandardDeviation.Value);
        }

        [Test]
        public void CopyProperties_FromStructureNull_ThrowsArgumentNullException()
        {
            // Setup
            var structure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = new Point2D(0, 0)
            });

            // Call
            TestDelegate call = () => structure.CopyProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("fromStructure", paramName);
        }

        [Test]
        public void CopyProperties_FromStructure_UpdatesProperties()
        {
            // Setup
            var random = new Random(123);
            var structure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = new Point2D(0, 0)
            });

            var otherStructure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = "otherName",
                Id = "otherId",
                Location = new Point2D(1, 1),
                StructureNormalOrientation = (RoundedDouble) random.NextDouble(),
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) random.Next(1, 10),
                    Shift = (RoundedDouble) random.NextDouble(),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) random.Next(1, 10),
                    CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                },
                FailureProbabilityStructureWithErosion = random.NextDouble(),
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) random.Next(1, 10),
                    Shift = (RoundedDouble) random.NextDouble(),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) random.Next(1, 10),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) random.Next(1, 10),
                    CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) random.Next(1, 10),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                }
            });

            // Call
            structure.CopyProperties(otherStructure);

            // Assert
            Assert.AreNotEqual(otherStructure.Id, structure.Id);
            Assert.AreEqual(otherStructure.Name, structure.Name);
            Assert.AreEqual(otherStructure.Location, structure.Location);
            Assert.AreEqual(otherStructure.StructureNormalOrientation, structure.StructureNormalOrientation);

            Assert.AreEqual(otherStructure.AllowedLevelIncreaseStorage.Mean, structure.AllowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(otherStructure.AllowedLevelIncreaseStorage.StandardDeviation, structure.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.AreEqual(otherStructure.AllowedLevelIncreaseStorage.Shift, structure.AllowedLevelIncreaseStorage.Shift);

            Assert.AreEqual(otherStructure.CriticalOvertoppingDischarge.Mean, structure.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(otherStructure.CriticalOvertoppingDischarge.CoefficientOfVariation, structure.CriticalOvertoppingDischarge.CoefficientOfVariation);

            Assert.AreEqual(otherStructure.FailureProbabilityStructureWithErosion, structure.FailureProbabilityStructureWithErosion);

            Assert.AreEqual(otherStructure.FlowWidthAtBottomProtection.Mean, structure.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(otherStructure.FlowWidthAtBottomProtection.StandardDeviation, structure.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.AreEqual(otherStructure.FlowWidthAtBottomProtection.Shift, structure.FlowWidthAtBottomProtection.Shift);

            Assert.AreEqual(otherStructure.LevelCrestStructure.Mean, structure.LevelCrestStructure.Mean);
            Assert.AreEqual(otherStructure.LevelCrestStructure.StandardDeviation, structure.LevelCrestStructure.StandardDeviation);

            Assert.AreEqual(otherStructure.StorageStructureArea.Mean, structure.StorageStructureArea.Mean);
            Assert.AreEqual(otherStructure.StorageStructureArea.CoefficientOfVariation, structure.StorageStructureArea.CoefficientOfVariation);

            Assert.AreEqual(otherStructure.WidthFlowApertures.Mean, structure.WidthFlowApertures.Mean);
            Assert.AreEqual(otherStructure.WidthFlowApertures.StandardDeviation, structure.WidthFlowApertures.StandardDeviation);
        }

        [Test]
        [TestCase(null)]
        [TestCase("string")]
        public void Equals_ToDifferentTypeOrNull_ReturnsFalse(object other)
        {
            // Setup
            HeightStructure structure = CreateFullyDefinedStructure();

            // Call
            bool isEqual = structure.Equals(other);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_TransitivePropertyAllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            HeightStructure structureX = CreateFullyDefinedStructure();
            HeightStructure structureY = CreateFullyDefinedStructure();
            HeightStructure structureZ = CreateFullyDefinedStructure();

            // Call
            bool isXEqualToY = structureX.Equals(structureY);
            bool isYEqualToZ = structureY.Equals(structureZ);
            bool isXEqualToZ = structureX.Equals(structureZ);

            // Assert
            Assert.IsTrue(isXEqualToY);
            Assert.IsTrue(isYEqualToZ);
            Assert.IsTrue(isXEqualToZ);
        }

        [Test]
        [TestCaseSource(nameof(StructureCombinations))]
        public void Equal_DifferentProperty_RetunsIsEqual(HeightStructure structure,
                                                          HeightStructure otherStructure,
                                                          bool expectedToBeEqual)
        {
            // Call
            bool isStructureEqualToOther = structure.Equals(otherStructure);
            bool isOtherEqualToStructure = otherStructure.Equals(structure);

            // Assert
            Assert.AreEqual(expectedToBeEqual, isStructureEqualToOther);
            Assert.AreEqual(expectedToBeEqual, isOtherEqualToStructure);
        }

        [Test]
        public void GetHashCode_EqualStructures_ReturnsSameHashCode()
        {
            // Setup
            HeightStructure structureOne = CreateFullyDefinedStructure();
            HeightStructure structureTwo = CreateFullyDefinedStructure();

            // Call
            int hashCodeOne = structureOne.GetHashCode();
            int hashCodeTwo = structureTwo.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        private static HeightStructure CreateFullyDefinedStructure()
        {
            return new HeightStructure(CreateFullyConfiguredConstructionProperties());
        }

        private static HeightStructure.ConstructionProperties CreateFullyConfiguredConstructionProperties()
        {
            const string id = "structure id";
            const string name = "Structure name";
            return new HeightStructure.ConstructionProperties
            {
                Id = id,
                Name = name,
                Location = new Point2D(1, 1),
                StructureNormalOrientation = (RoundedDouble) 25,
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) 1,
                    StandardDeviation = (RoundedDouble) 0.51
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) 2,
                    StandardDeviation = (RoundedDouble) 0.52
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) 3,
                    CoefficientOfVariation = (RoundedDouble) 0.53
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) 4,
                    StandardDeviation = (RoundedDouble) 0.54
                },
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) 5,
                    CoefficientOfVariation = (RoundedDouble) 0.55
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) 6,
                    StandardDeviation = (RoundedDouble) 0.56
                },
                FailureProbabilityStructureWithErosion = 9
            };
        }
    }
}