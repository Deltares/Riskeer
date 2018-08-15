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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.ClosingStructures.Data.Test
{
    [TestFixture]
    public class ClosingStructureTest
    {
        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);

            // Call
            var structure = new ClosingStructure(
                new ClosingStructure.ConstructionProperties
                {
                    Name = "aName",
                    Id = "anId",
                    Location = location,
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 123.456,
                        CoefficientOfVariation = (RoundedDouble) 0.123
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 234.567,
                        StandardDeviation = (RoundedDouble) 0.234
                    },
                    StructureNormalOrientation = (RoundedDouble) 345.678,
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 456.789,
                        StandardDeviation = (RoundedDouble) 0.456
                    },
                    LevelCrestStructureNotClosing =
                    {
                        Mean = (RoundedDouble) 567.890,
                        StandardDeviation = (RoundedDouble) 0.567
                    },
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) 678.901,
                        StandardDeviation = (RoundedDouble) 0.678
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) 789.012,
                        StandardDeviation = (RoundedDouble) 0.789
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) 890.123,
                        StandardDeviation = (RoundedDouble) 0.890
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 901.234,
                        CoefficientOfVariation = (RoundedDouble) 0.901
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 111.222,
                        StandardDeviation = (RoundedDouble) 0.111
                    },
                    ProbabilityOpenStructureBeforeFlooding = 321.987,
                    FailureProbabilityOpenStructure = 654.321,
                    IdenticalApertures = 42,
                    FailureProbabilityReparation = 987.654,
                    InflowModelType = ClosingStructureInflowModelType.LowSill
                });

            // Assert
            Assert.IsInstanceOf<StructureBase>(structure);
            Assert.AreEqual("aName", structure.Name);
            Assert.AreEqual("anId", structure.Id);
            Assert.AreEqual(location.X, structure.Location.X);
            Assert.AreEqual(location.Y, structure.Location.Y);

            Assert.AreEqual(345.68, structure.StructureNormalOrientation, structure.StructureNormalOrientation.GetAccuracy());

            VariationCoefficientLogNormalDistribution storageStructureArea = structure.StorageStructureArea;
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(123.46, storageStructureArea.Mean, storageStructureArea.Mean.GetAccuracy());
            Assert.AreEqual(2, storageStructureArea.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.12, storageStructureArea.CoefficientOfVariation, storageStructureArea.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution allowedLevelIncreaseStorage = structure.AllowedLevelIncreaseStorage;
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(234.57, allowedLevelIncreaseStorage.Mean, allowedLevelIncreaseStorage.Mean.GetAccuracy());
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, allowedLevelIncreaseStorage.StandardDeviation, allowedLevelIncreaseStorage.StandardDeviation.GetAccuracy());

            NormalDistribution widthFlowApertures = structure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(456.79, widthFlowApertures.Mean, widthFlowApertures.Mean.GetAccuracy());
            Assert.AreEqual(2, widthFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.46, widthFlowApertures.StandardDeviation, widthFlowApertures.StandardDeviation.GetAccuracy());

            NormalDistribution levelCrestStructureNotClosing = structure.LevelCrestStructureNotClosing;
            Assert.AreEqual(2, levelCrestStructureNotClosing.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, levelCrestStructureNotClosing.Mean, levelCrestStructureNotClosing.Mean.GetAccuracy());
            Assert.AreEqual(2, levelCrestStructureNotClosing.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, levelCrestStructureNotClosing.StandardDeviation, levelCrestStructureNotClosing.StandardDeviation.GetAccuracy());

            NormalDistribution insideWaterLevel = structure.InsideWaterLevel;
            Assert.AreEqual(2, insideWaterLevel.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(678.90, insideWaterLevel.Mean, insideWaterLevel.Mean.GetAccuracy());
            Assert.AreEqual(2, insideWaterLevel.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.68, insideWaterLevel.StandardDeviation, insideWaterLevel.StandardDeviation.GetAccuracy());

            NormalDistribution thresholdHeightOpenWeir = structure.ThresholdHeightOpenWeir;
            Assert.AreEqual(2, thresholdHeightOpenWeir.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(789.01, thresholdHeightOpenWeir.Mean, thresholdHeightOpenWeir.Mean.GetAccuracy());
            Assert.AreEqual(2, thresholdHeightOpenWeir.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.79, thresholdHeightOpenWeir.StandardDeviation, thresholdHeightOpenWeir.StandardDeviation.GetAccuracy());

            LogNormalDistribution areaFlowApertures = structure.AreaFlowApertures;
            Assert.AreEqual(2, areaFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(890.12, areaFlowApertures.Mean, areaFlowApertures.Mean.GetAccuracy());
            Assert.AreEqual(2, areaFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.89, areaFlowApertures.StandardDeviation, areaFlowApertures.StandardDeviation.GetAccuracy());

            VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge = structure.CriticalOvertoppingDischarge;
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(901.23, criticalOvertoppingDischarge.Mean, criticalOvertoppingDischarge.Mean.GetAccuracy());
            Assert.AreEqual(2, criticalOvertoppingDischarge.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.90, criticalOvertoppingDischarge.CoefficientOfVariation, criticalOvertoppingDischarge.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution flowWidthAtBottomProtection = structure.FlowWidthAtBottomProtection;
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(111.22, flowWidthAtBottomProtection.Mean, flowWidthAtBottomProtection.Mean.GetAccuracy());
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.11, flowWidthAtBottomProtection.StandardDeviation, flowWidthAtBottomProtection.StandardDeviation.GetAccuracy());

            Assert.AreEqual(321.987, structure.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(654.321, structure.FailureProbabilityOpenStructure);
            Assert.AreEqual(42, structure.IdenticalApertures);
            Assert.AreEqual(987.654, structure.FailureProbabilityReparation);

            Assert.AreEqual(ClosingStructureInflowModelType.LowSill, structure.InflowModelType);
        }

        [Test]
        public void Constructor_DefaultConstructionProperties_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);

            // Call
            var structure = new ClosingStructure(
                new ClosingStructure.ConstructionProperties
                {
                    Name = "aName",
                    Id = "anId",
                    Location = location
                });

            // Assert
            Assert.IsInstanceOf<StructureBase>(structure);
            Assert.AreEqual("aName", structure.Name);
            Assert.AreEqual("anId", structure.Id);
            Assert.AreEqual(location.X, structure.Location.X);
            Assert.AreEqual(location.Y, structure.Location.Y);

            Assert.AreEqual(2, structure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.IsNaN(structure.StructureNormalOrientation);

            VariationCoefficientLogNormalDistribution storageStructureArea = structure.StorageStructureArea;
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(storageStructureArea.Mean);
            Assert.AreEqual(2, storageStructureArea.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, storageStructureArea.CoefficientOfVariation, storageStructureArea.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution allowedLevelIncreaseStorage = structure.AllowedLevelIncreaseStorage;
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(allowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, allowedLevelIncreaseStorage.StandardDeviation, allowedLevelIncreaseStorage.StandardDeviation.GetAccuracy());

            NormalDistribution widthFlowApertures = structure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(widthFlowApertures.Mean);
            Assert.AreEqual(2, widthFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.2, widthFlowApertures.StandardDeviation, widthFlowApertures.StandardDeviation.GetAccuracy());

            NormalDistribution levelCrestStructureNotClosing = structure.LevelCrestStructureNotClosing;
            Assert.AreEqual(2, levelCrestStructureNotClosing.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(levelCrestStructureNotClosing.Mean);
            Assert.AreEqual(2, levelCrestStructureNotClosing.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.05, levelCrestStructureNotClosing.StandardDeviation, levelCrestStructureNotClosing.StandardDeviation.GetAccuracy());

            NormalDistribution insideWaterLevel = structure.InsideWaterLevel;
            Assert.AreEqual(2, insideWaterLevel.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(insideWaterLevel.Mean);
            Assert.AreEqual(2, insideWaterLevel.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, insideWaterLevel.StandardDeviation, insideWaterLevel.StandardDeviation.GetAccuracy());

            NormalDistribution thresholdHeightOpenWeir = structure.ThresholdHeightOpenWeir;
            Assert.AreEqual(2, thresholdHeightOpenWeir.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(thresholdHeightOpenWeir.Mean);
            Assert.AreEqual(2, thresholdHeightOpenWeir.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, thresholdHeightOpenWeir.StandardDeviation, thresholdHeightOpenWeir.StandardDeviation.GetAccuracy());

            LogNormalDistribution areaFlowApertures = structure.AreaFlowApertures;
            Assert.AreEqual(2, areaFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(areaFlowApertures.Mean);
            Assert.AreEqual(2, areaFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.01, areaFlowApertures.StandardDeviation, areaFlowApertures.StandardDeviation.GetAccuracy());

            VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge = structure.CriticalOvertoppingDischarge;
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(criticalOvertoppingDischarge.Mean);
            Assert.AreEqual(2, criticalOvertoppingDischarge.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.15, criticalOvertoppingDischarge.CoefficientOfVariation, criticalOvertoppingDischarge.CoefficientOfVariation.GetAccuracy());

            LogNormalDistribution flowWidthAtBottomProtection = structure.FlowWidthAtBottomProtection;
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(flowWidthAtBottomProtection.Mean);
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.05, flowWidthAtBottomProtection.StandardDeviation, flowWidthAtBottomProtection.StandardDeviation.GetAccuracy());

            Assert.AreEqual(1, structure.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(1, structure.FailureProbabilityOpenStructure);
            Assert.AreEqual(1, structure.IdenticalApertures);
            Assert.AreEqual(1, structure.FailureProbabilityReparation);

            Assert.AreEqual(ClosingStructureInflowModelType.VerticalWall, structure.InflowModelType);
        }

        [Test]
        public void CopyProperties_FromStructureNull_ThrowsArgumentNullException()
        {
            // Setup
            var structure = new ClosingStructure(new ClosingStructure.ConstructionProperties
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
            var structure = new ClosingStructure(new ClosingStructure.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = new Point2D(0, 0)
            });

            var otherStructure = new ClosingStructure(new ClosingStructure.ConstructionProperties
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
                AreaFlowApertures =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                InsideWaterLevel =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                LevelCrestStructureNotClosing =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                StorageStructureArea =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                WidthFlowApertures =
                {
                    Mean = random.NextRoundedDouble(),
                    StandardDeviation = random.NextRoundedDouble()
                },
                FailureProbabilityOpenStructure = random.NextDouble(),
                FailureProbabilityReparation = random.NextDouble(),
                InflowModelType = ClosingStructureInflowModelType.FloodedCulvert,
                IdenticalApertures = random.Next(),
                ProbabilityOpenStructureBeforeFlooding = random.NextDouble()
            });

            // Call
            structure.CopyProperties(otherStructure);

            // Assert
            Assert.AreNotEqual(otherStructure.Id, structure.Id);
            Assert.AreEqual(otherStructure.Name, structure.Name);
            TestHelper.AssertAreEqualButNotSame(otherStructure.Location, structure.Location);
            Assert.AreEqual(otherStructure.StructureNormalOrientation, structure.StructureNormalOrientation);
            TestHelper.AssertAreEqualButNotSame(otherStructure.AllowedLevelIncreaseStorage, structure.AllowedLevelIncreaseStorage);
            TestHelper.AssertAreEqualButNotSame(otherStructure.AreaFlowApertures, structure.AreaFlowApertures);
            TestHelper.AssertAreEqualButNotSame(otherStructure.CriticalOvertoppingDischarge, structure.CriticalOvertoppingDischarge);
            Assert.AreEqual(otherStructure.FailureProbabilityOpenStructure, structure.FailureProbabilityOpenStructure);
            Assert.AreEqual(otherStructure.FailureProbabilityReparation, structure.FailureProbabilityReparation);
            Assert.AreEqual(otherStructure.IdenticalApertures, structure.IdenticalApertures);
            Assert.AreEqual(otherStructure.ProbabilityOpenStructureBeforeFlooding, structure.ProbabilityOpenStructureBeforeFlooding);
            TestHelper.AssertAreEqualButNotSame(otherStructure.FlowWidthAtBottomProtection, structure.FlowWidthAtBottomProtection);
            TestHelper.AssertAreEqualButNotSame(otherStructure.InsideWaterLevel, structure.InsideWaterLevel);
            TestHelper.AssertAreEqualButNotSame(otherStructure.LevelCrestStructureNotClosing, structure.LevelCrestStructureNotClosing);
            TestHelper.AssertAreEqualButNotSame(otherStructure.StorageStructureArea, structure.StorageStructureArea);
            TestHelper.AssertAreEqualButNotSame(otherStructure.ThresholdHeightOpenWeir, structure.ThresholdHeightOpenWeir);
            TestHelper.AssertAreEqualButNotSame(otherStructure.WidthFlowApertures, structure.WidthFlowApertures);
        }

        [TestFixture]
        private class ClosingStructureEqualsTest : EqualsTestFixture<ClosingStructure, DerivedClosingStructures>
        {
            protected override ClosingStructure CreateObject()
            {
                return new ClosingStructure(CreateConstructionProperties());
            }

            protected override DerivedClosingStructures CreateDerivedObject()
            {
                return new DerivedClosingStructures(CreateConstructionProperties());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                foreach (ChangePropertyData<ClosingStructure.ConstructionProperties> changeSingleDataProperty in ChangeSingleDataProperties())
                {
                    ClosingStructure.ConstructionProperties differentConstructionProperties = CreateConstructionProperties();
                    changeSingleDataProperty.ActionToChangeProperty(differentConstructionProperties);

                    yield return new TestCaseData(new ClosingStructure(differentConstructionProperties))
                        .SetName(changeSingleDataProperty.PropertyName);
                }
            }

            private static IEnumerable<ChangePropertyData<ClosingStructure.ConstructionProperties>> ChangeSingleDataProperties()
            {
                var random = new Random(21);
                RoundedDouble offset = random.NextRoundedDouble();

                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.Name = "Different Name",
                                                                                             "Name");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.Id = "Different Id",
                                                                                             "Id");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.Location = new Point2D(random.NextDouble(), random.NextDouble()),
                                                                                             "Location");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.StorageStructureArea.Mean = cp.StorageStructureArea.Mean + offset,
                                                                                             "StorageStructureAreaMean");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.StorageStructureArea.CoefficientOfVariation = cp.StorageStructureArea.CoefficientOfVariation + offset,
                                                                                             "StorageStructureAreaCoefficientOfVariation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.AllowedLevelIncreaseStorage.Mean = cp.AllowedLevelIncreaseStorage.Mean + offset,
                                                                                             "AllowedLevelIncreaseStorageMean");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.AllowedLevelIncreaseStorage.StandardDeviation = cp.AllowedLevelIncreaseStorage.StandardDeviation + offset,
                                                                                             "AllowedLevelIncreaseStorageStandardDeviation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.StructureNormalOrientation = random.NextRoundedDouble(),
                                                                                             "NormalOrientation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.WidthFlowApertures.Mean = cp.WidthFlowApertures.Mean + offset,
                                                                                             "WidthFlowAperturesMean");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.WidthFlowApertures.StandardDeviation = cp.WidthFlowApertures.StandardDeviation + offset,
                                                                                             "WidthFlowAperturesStandardDeviation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.LevelCrestStructureNotClosing.Mean = cp.LevelCrestStructureNotClosing.Mean + offset,
                                                                                             "LevelCrestStructureNotClosingMean");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.LevelCrestStructureNotClosing.StandardDeviation = cp.LevelCrestStructureNotClosing.StandardDeviation + offset,
                                                                                             "LevelCrestStructureNotClosingStandardDeviation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.InsideWaterLevel.Mean = cp.InsideWaterLevel.Mean + offset,
                                                                                             "InsideWaterLevelMean");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.InsideWaterLevel.StandardDeviation = cp.InsideWaterLevel.StandardDeviation + offset,
                                                                                             "InsideWaterLevelStandardDeviation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.ThresholdHeightOpenWeir.Mean = cp.ThresholdHeightOpenWeir.Mean + offset,
                                                                                             "ThresholdHeightOpenWeirMean");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.ThresholdHeightOpenWeir.StandardDeviation = cp.ThresholdHeightOpenWeir.StandardDeviation + offset,
                                                                                             "ThresholdHeightOpenWeirStandardDeviation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.AreaFlowApertures.Mean = cp.AreaFlowApertures.Mean + offset,
                                                                                             "AreaFlowAperturesMean");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.AreaFlowApertures.StandardDeviation = cp.AreaFlowApertures.StandardDeviation + offset,
                                                                                             "AreaFlowAperturesStandardDeviation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.CriticalOvertoppingDischarge.Mean = cp.CriticalOvertoppingDischarge.Mean + offset,
                                                                                             "CriticalOvertoppingDischargeMean");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.CriticalOvertoppingDischarge.CoefficientOfVariation = cp.CriticalOvertoppingDischarge.CoefficientOfVariation + offset,
                                                                                             "CriticalOvertoppingDischargeCoefficientOfVariation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.FlowWidthAtBottomProtection.Mean = cp.FlowWidthAtBottomProtection.Mean + offset,
                                                                                             "FlowWidthAtBottomProtectionMean");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.FlowWidthAtBottomProtection.StandardDeviation = cp.FlowWidthAtBottomProtection.StandardDeviation + offset,
                                                                                             "FlowWidthAtBottomProtectionStandardDeviation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.ProbabilityOpenStructureBeforeFlooding = random.NextDouble(),
                                                                                             "ProbabilityOpenStructureBeforeFlooding");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.FailureProbabilityOpenStructure = random.NextDouble(),
                                                                                             "FailureProbabilityOpenStructure");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.IdenticalApertures = random.Next(5, int.MaxValue),
                                                                                             "IdenticalApertures");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.FailureProbabilityReparation = random.NextDouble(),
                                                                                             "FailureProbabilityReparation");
                yield return new ChangePropertyData<ClosingStructure.ConstructionProperties>(cp => cp.InflowModelType = ClosingStructureInflowModelType.FloodedCulvert,
                                                                                             "InflowModelType");
            }

            private static ClosingStructure.ConstructionProperties CreateConstructionProperties()
            {
                return new ClosingStructure.ConstructionProperties
                {
                    Name = "name",
                    Id = "id",
                    Location = new Point2D(12345.56789, 9876.54321),
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 20000,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    StructureNormalOrientation = (RoundedDouble) 10.0,
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 21,
                        StandardDeviation = (RoundedDouble) 0.05
                    },
                    LevelCrestStructureNotClosing =
                    {
                        Mean = (RoundedDouble) 4.95,
                        StandardDeviation = (RoundedDouble) 0.05
                    },
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) 0.5,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) 4.95,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) 31.5,
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 1.0,
                        CoefficientOfVariation = (RoundedDouble) 0.15
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 25.0,
                        StandardDeviation = (RoundedDouble) 0.05
                    },
                    ProbabilityOpenStructureBeforeFlooding = 1.0,
                    FailureProbabilityOpenStructure = 0.1,
                    IdenticalApertures = 4,
                    FailureProbabilityReparation = 1.0,
                    InflowModelType = ClosingStructureInflowModelType.VerticalWall
                };
            }
        }

        private class DerivedClosingStructures : ClosingStructure
        {
            public DerivedClosingStructures(ConstructionProperties properties) : base(properties) {}
        }
    }
}