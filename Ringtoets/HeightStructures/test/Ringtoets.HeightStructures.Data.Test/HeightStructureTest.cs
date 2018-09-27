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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructureTest
    {
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
                StructureNormalOrientation = random.NextRoundedDouble(),
                AllowedLevelIncreaseStorage =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                FailureProbabilityStructureWithErosion = random.NextDouble(),
                FlowWidthAtBottomProtection =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                LevelCrestStructure =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                StorageStructureArea =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                WidthFlowApertures =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                }
            });

            // Call
            structure.CopyProperties(otherStructure);

            // Assert
            Assert.AreNotEqual(otherStructure.Id, structure.Id);
            Assert.AreEqual(otherStructure.Name, structure.Name);
            TestHelper.AssertAreEqualButNotSame(otherStructure.Location, structure.Location);
            Assert.AreEqual(otherStructure.StructureNormalOrientation, structure.StructureNormalOrientation);
            TestHelper.AssertAreEqualButNotSame(otherStructure.AllowedLevelIncreaseStorage, structure.AllowedLevelIncreaseStorage);
            TestHelper.AssertAreEqualButNotSame(otherStructure.CriticalOvertoppingDischarge, structure.CriticalOvertoppingDischarge);
            Assert.AreEqual(otherStructure.FailureProbabilityStructureWithErosion, structure.FailureProbabilityStructureWithErosion);
            TestHelper.AssertAreEqualButNotSame(otherStructure.FlowWidthAtBottomProtection, structure.FlowWidthAtBottomProtection);
            TestHelper.AssertAreEqualButNotSame(otherStructure.LevelCrestStructure, structure.LevelCrestStructure);
            TestHelper.AssertAreEqualButNotSame(otherStructure.StorageStructureArea, structure.StorageStructureArea);
            TestHelper.AssertAreEqualButNotSame(otherStructure.WidthFlowApertures, structure.WidthFlowApertures);
        }

        [TestFixture]
        private class HeightStructureEqualsTest : EqualsTestFixture<HeightStructure, DerivedHeightStructure>
        {
            protected override HeightStructure CreateObject()
            {
                return new HeightStructure(CreateConstructionProperties());
            }

            protected override DerivedHeightStructure CreateDerivedObject()
            {
                return new DerivedHeightStructure(CreateConstructionProperties());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                foreach (ChangePropertyData<HeightStructure.ConstructionProperties> changeSingleDataProperty in ChangeSingleDataProperties())
                {
                    HeightStructure.ConstructionProperties differentConstructionProperties = CreateConstructionProperties();
                    changeSingleDataProperty.ActionToChangeProperty(differentConstructionProperties);

                    yield return new TestCaseData(new HeightStructure(differentConstructionProperties))
                        .SetName(changeSingleDataProperty.PropertyName);
                }
            }

            private static IEnumerable<ChangePropertyData<HeightStructure.ConstructionProperties>> ChangeSingleDataProperties()
            {
                var random = new Random(21);
                RoundedDouble offset = random.NextRoundedDouble();

                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.Name = "Different Name",
                                                                                            "Name");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.Id = "Different Id",
                                                                                            "Id");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.Location = new Point2D(random.NextDouble(), random.NextDouble()),
                                                                                            "Location");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.StructureNormalOrientation = random.NextRoundedDouble(),
                                                                                            "NormalOrientation");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.LevelCrestStructure.Mean = cp.LevelCrestStructure.Mean + offset,
                                                                                            "LevelCrestStructureMean");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.LevelCrestStructure.StandardDeviation = cp.LevelCrestStructure.StandardDeviation + offset,
                                                                                            "LevelCrestStructureStandardDeviation");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.FlowWidthAtBottomProtection.Mean = cp.FlowWidthAtBottomProtection.Mean + offset,
                                                                                            "FlowWidthAtBottomProtectionMean");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.FlowWidthAtBottomProtection.StandardDeviation = cp.FlowWidthAtBottomProtection.StandardDeviation + offset,
                                                                                            "FlowWidthAtBottomProtectionStandardDeviation");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.CriticalOvertoppingDischarge.Mean = cp.CriticalOvertoppingDischarge.Mean + offset,
                                                                                            "CriticalOvertoppingDischargeMean");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.CriticalOvertoppingDischarge.CoefficientOfVariation = cp.CriticalOvertoppingDischarge.CoefficientOfVariation + offset,
                                                                                            "CriticalOvertoppingDischargeCoefficientofVariation");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.WidthFlowApertures.Mean = cp.WidthFlowApertures.Mean + offset,
                                                                                            "WidthFlowAperturesMean");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.WidthFlowApertures.StandardDeviation = cp.WidthFlowApertures.StandardDeviation + offset,
                                                                                            "WidthFlowAperturesStandardDeviation");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.FailureProbabilityStructureWithErosion = random.NextDouble(),
                                                                                            "FailureProbabilityStructureWithErosion");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.StorageStructureArea.Mean = cp.StorageStructureArea.Mean + offset,
                                                                                            "StorageStructureAreaMean");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.StorageStructureArea.CoefficientOfVariation = cp.StorageStructureArea.CoefficientOfVariation + offset,
                                                                                            "StorageStructureAreaCoefficientOfVariation");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.AllowedLevelIncreaseStorage.Mean = cp.AllowedLevelIncreaseStorage.Mean + offset,
                                                                                            "AllowedLevelIncreaseStorageMean");
                yield return new ChangePropertyData<HeightStructure.ConstructionProperties>(cp => cp.AllowedLevelIncreaseStorage.StandardDeviation = cp.AllowedLevelIncreaseStorage.StandardDeviation + offset,
                                                                                            "AllowedLevelIncreaseStorageStandardDeviation");
            }

            private static HeightStructure.ConstructionProperties CreateConstructionProperties()
            {
                return new HeightStructure.ConstructionProperties
                {
                    Name = "Name",
                    Id = "Id",
                    Location = new Point2D(0.0, 0.0),
                    StructureNormalOrientation = (RoundedDouble) 0.12345,
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) 234.567,
                        StandardDeviation = (RoundedDouble) 0.23456
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 345.68,
                        StandardDeviation = (RoundedDouble) 0.35
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 456.79,
                        CoefficientOfVariation = (RoundedDouble) 0.46
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 567.89,
                        StandardDeviation = (RoundedDouble) 0.57
                    },
                    FailureProbabilityStructureWithErosion = 0.67890,
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 112.22,
                        CoefficientOfVariation = (RoundedDouble) 0.11
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 225.34,
                        StandardDeviation = (RoundedDouble) 0.23
                    }
                };
            }
        }

        private class DerivedHeightStructure : HeightStructure
        {
            public DerivedHeightStructure(ConstructionProperties constructionProperties) : base(constructionProperties) {}
        }
    }
}