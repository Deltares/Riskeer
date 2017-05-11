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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.ClosingStructures.Data.Test
{
    [TestFixture]
    public class ClosingStructureTest
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
                ClosingStructure structure = CreateFullyDefinedStructure();
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
                ClosingStructure structure = CreateFullyDefinedStructure();

                ClosingStructure.ConstructionProperties differentId = CreateFullyConfiguredConstructionProperties();
                differentId.Id = "differentId";
                yield return new TestCaseData(structure, new ClosingStructure(differentId), false)
                    .SetName(nameof(differentId));

                ClosingStructure.ConstructionProperties differentName = CreateFullyConfiguredConstructionProperties();
                differentName.Name = "differentName";
                yield return new TestCaseData(structure, new ClosingStructure(differentName), false)
                    .SetName(nameof(differentName));

                ClosingStructure.ConstructionProperties differentLocation = CreateFullyConfiguredConstructionProperties();
                differentLocation.Location = new Point2D(9, 9);
                yield return new TestCaseData(structure, new ClosingStructure(differentLocation), false)
                    .SetName(nameof(differentLocation));

                ClosingStructure.ConstructionProperties differentOrientation = CreateFullyConfiguredConstructionProperties();
                differentOrientation.StructureNormalOrientation = (RoundedDouble) 90;
                yield return new TestCaseData(structure, new ClosingStructure(differentOrientation), false)
                    .SetName(nameof(differentOrientation));

                ClosingStructure.ConstructionProperties differentAllowedLevelIncreaseStorageMean = CreateFullyConfiguredConstructionProperties();
                differentAllowedLevelIncreaseStorageMean.AreaFlowApertures.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentAllowedLevelIncreaseStorageMean), false)
                    .SetName(nameof(differentAllowedLevelIncreaseStorageMean));

                ClosingStructure.ConstructionProperties differentAllowedLevelIncreaseStorageStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentAllowedLevelIncreaseStorageStandardDeviation.AllowedLevelIncreaseStorage.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentAllowedLevelIncreaseStorageStandardDeviation), false)
                    .SetName(nameof(differentAllowedLevelIncreaseStorageStandardDeviation));

                ClosingStructure.ConstructionProperties differentAreaFlowAperturesMean = CreateFullyConfiguredConstructionProperties();
                differentAreaFlowAperturesMean.AreaFlowApertures.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentAreaFlowAperturesMean), false)
                    .SetName(nameof(differentAreaFlowAperturesMean));

                ClosingStructure.ConstructionProperties differentAreaFlowAperturesStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentAreaFlowAperturesStandardDeviation.AreaFlowApertures.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentAreaFlowAperturesStandardDeviation), false)
                    .SetName(nameof(differentAreaFlowAperturesStandardDeviation));

                ClosingStructure.ConstructionProperties differentCriticalOvertoppingDischargeMean = CreateFullyConfiguredConstructionProperties();
                differentCriticalOvertoppingDischargeMean.CriticalOvertoppingDischarge.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentCriticalOvertoppingDischargeMean), false)
                    .SetName(nameof(differentCriticalOvertoppingDischargeMean));

                ClosingStructure.ConstructionProperties differentCriticalOvertoppingDischargeCoefficientOfVariation = CreateFullyConfiguredConstructionProperties();
                differentCriticalOvertoppingDischargeCoefficientOfVariation.CriticalOvertoppingDischarge.CoefficientOfVariation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentCriticalOvertoppingDischargeCoefficientOfVariation), false)
                    .SetName(nameof(differentCriticalOvertoppingDischargeCoefficientOfVariation));

                ClosingStructure.ConstructionProperties differentFailureProbabilityOpenStructure = CreateFullyConfiguredConstructionProperties();
                differentFailureProbabilityOpenStructure.FailureProbabilityOpenStructure = 987;
                yield return new TestCaseData(structure, new ClosingStructure(differentFailureProbabilityOpenStructure), false)
                    .SetName(nameof(differentFailureProbabilityOpenStructure));

                ClosingStructure.ConstructionProperties differentFailureProbabilityReparation = CreateFullyConfiguredConstructionProperties();
                differentFailureProbabilityReparation.FailureProbabilityReparation = 987;
                yield return new TestCaseData(structure, new ClosingStructure(differentFailureProbabilityReparation), false)
                    .SetName(nameof(differentFailureProbabilityReparation));

                ClosingStructure.ConstructionProperties differentFlowWidthAtBottomProtectionMean = CreateFullyConfiguredConstructionProperties();
                differentFlowWidthAtBottomProtectionMean.FlowWidthAtBottomProtection.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentFlowWidthAtBottomProtectionMean), false)
                    .SetName(nameof(differentFlowWidthAtBottomProtectionMean));

                ClosingStructure.ConstructionProperties differentFlowWidthAtBottomProtectionStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentFlowWidthAtBottomProtectionStandardDeviation.FlowWidthAtBottomProtection.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentFlowWidthAtBottomProtectionStandardDeviation), false)
                    .SetName(nameof(differentFlowWidthAtBottomProtectionStandardDeviation));

                ClosingStructure.ConstructionProperties differentIdenticalApertures = CreateFullyConfiguredConstructionProperties();
                differentIdenticalApertures.IdenticalApertures = 987;
                yield return new TestCaseData(structure, new ClosingStructure(differentIdenticalApertures), false)
                    .SetName(nameof(differentIdenticalApertures));

                ClosingStructure.ConstructionProperties differentInflowModelType = CreateFullyConfiguredConstructionProperties();
                differentInflowModelType.InflowModelType = ClosingStructureInflowModelType.LowSill;
                yield return new TestCaseData(structure, new ClosingStructure(differentInflowModelType), false)
                    .SetName(nameof(differentInflowModelType));

                ClosingStructure.ConstructionProperties differentInsideWaterLevelMean = CreateFullyConfiguredConstructionProperties();
                differentInsideWaterLevelMean.InsideWaterLevel.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentInsideWaterLevelMean), false)
                    .SetName(nameof(differentInsideWaterLevelMean));

                ClosingStructure.ConstructionProperties differentInsideWaterLevelStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentInsideWaterLevelStandardDeviation.InsideWaterLevel.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentInsideWaterLevelStandardDeviation), false)
                    .SetName(nameof(differentInsideWaterLevelStandardDeviation));

                ClosingStructure.ConstructionProperties differentLevelCrestStructureNotClosingMean = CreateFullyConfiguredConstructionProperties();
                differentLevelCrestStructureNotClosingMean.LevelCrestStructureNotClosing.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentLevelCrestStructureNotClosingMean), false)
                    .SetName(nameof(differentLevelCrestStructureNotClosingMean));

                ClosingStructure.ConstructionProperties differentLevelCrestStructureNotClosingStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentLevelCrestStructureNotClosingStandardDeviation.LevelCrestStructureNotClosing.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentLevelCrestStructureNotClosingStandardDeviation), false)
                    .SetName(nameof(differentLevelCrestStructureNotClosingStandardDeviation));

                ClosingStructure.ConstructionProperties differentProbabilityOrFrequencyOpenStructureBeforeFlooding = CreateFullyConfiguredConstructionProperties();
                differentProbabilityOrFrequencyOpenStructureBeforeFlooding.ProbabilityOrFrequencyOpenStructureBeforeFlooding = 987;
                yield return new TestCaseData(structure, new ClosingStructure(differentProbabilityOrFrequencyOpenStructureBeforeFlooding), false)
                    .SetName(nameof(differentProbabilityOrFrequencyOpenStructureBeforeFlooding));

                ClosingStructure.ConstructionProperties differentStorageStructureAreaMean = CreateFullyConfiguredConstructionProperties();
                differentStorageStructureAreaMean.StorageStructureArea.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentStorageStructureAreaMean), false)
                    .SetName(nameof(differentStorageStructureAreaMean));

                ClosingStructure.ConstructionProperties differentStorageStructureAreaCoefficientOfVariation = CreateFullyConfiguredConstructionProperties();
                differentStorageStructureAreaCoefficientOfVariation.StorageStructureArea.CoefficientOfVariation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentStorageStructureAreaCoefficientOfVariation), false)
                    .SetName(nameof(differentStorageStructureAreaCoefficientOfVariation));

                ClosingStructure.ConstructionProperties differentThresholdHeightOpenWeirMean = CreateFullyConfiguredConstructionProperties();
                differentThresholdHeightOpenWeirMean.ThresholdHeightOpenWeir.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentThresholdHeightOpenWeirMean), false)
                    .SetName(nameof(differentThresholdHeightOpenWeirMean));

                ClosingStructure.ConstructionProperties differentThresholdHeightOpenWeirStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentThresholdHeightOpenWeirStandardDeviation.ThresholdHeightOpenWeir.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentThresholdHeightOpenWeirStandardDeviation), false)
                    .SetName(nameof(differentThresholdHeightOpenWeirStandardDeviation));

                ClosingStructure.ConstructionProperties differentWidthFlowAperturesMean = CreateFullyConfiguredConstructionProperties();
                differentWidthFlowAperturesMean.WidthFlowApertures.Mean = (RoundedDouble) 10.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentWidthFlowAperturesMean), false)
                    .SetName(nameof(differentWidthFlowAperturesMean));

                ClosingStructure.ConstructionProperties differentWidthFlowAperturesStandardDeviation = CreateFullyConfiguredConstructionProperties();
                differentWidthFlowAperturesStandardDeviation.WidthFlowApertures.StandardDeviation = (RoundedDouble) 0.1;
                yield return new TestCaseData(structure, new ClosingStructure(differentWidthFlowAperturesStandardDeviation), false)
                    .SetName(nameof(differentWidthFlowAperturesStandardDeviation));
            }
        }

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
                    ProbabilityOrFrequencyOpenStructureBeforeFlooding = 321.987,
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

            Assert.AreEqual(321.987, structure.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
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

            Assert.AreEqual(1, structure.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
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
                StructureNormalOrientation = (RoundedDouble)random.Next(),
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) (random.NextDouble() + 1.0),
                    Shift = (RoundedDouble) random.NextDouble(),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                AreaFlowApertures =
                {
                    Mean = (RoundedDouble) (random.NextDouble() + 1.0),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) (random.NextDouble() + 1.0),
                    CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) (random.NextDouble() + 1.0),
                    Shift = (RoundedDouble) random.NextDouble(),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                InsideWaterLevel =
                {
                    Mean = (RoundedDouble) (random.NextDouble() + 1.0),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                LevelCrestStructureNotClosing =
                {
                    Mean = (RoundedDouble) (random.NextDouble() + 1.0),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) (random.NextDouble() + 1.0),
                    CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = (RoundedDouble) (random.NextDouble() + 1.0),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) (random.NextDouble() + 1.0),
                    StandardDeviation = (RoundedDouble) random.NextDouble()
                },
                FailureProbabilityOpenStructure = random.NextDouble(),
                FailureProbabilityReparation = random.NextDouble(),
                InflowModelType = ClosingStructureInflowModelType.FloodedCulvert,
                IdenticalApertures = random.Next(),
                ProbabilityOrFrequencyOpenStructureBeforeFlooding = random.NextDouble()
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

            Assert.AreEqual(otherStructure.AreaFlowApertures.Mean, structure.AreaFlowApertures.Mean);
            Assert.AreEqual(otherStructure.AreaFlowApertures.StandardDeviation, structure.AreaFlowApertures.StandardDeviation);

            Assert.AreEqual(otherStructure.CriticalOvertoppingDischarge.Mean, structure.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(otherStructure.CriticalOvertoppingDischarge.CoefficientOfVariation, structure.CriticalOvertoppingDischarge.CoefficientOfVariation);

            Assert.AreEqual(otherStructure.FailureProbabilityOpenStructure, structure.FailureProbabilityOpenStructure);
            Assert.AreEqual(otherStructure.FailureProbabilityReparation, structure.FailureProbabilityReparation);
            Assert.AreEqual(otherStructure.IdenticalApertures, structure.IdenticalApertures);
            Assert.AreEqual(otherStructure.ProbabilityOrFrequencyOpenStructureBeforeFlooding, structure.ProbabilityOrFrequencyOpenStructureBeforeFlooding);

            Assert.AreEqual(otherStructure.FlowWidthAtBottomProtection.Mean, structure.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(otherStructure.FlowWidthAtBottomProtection.StandardDeviation, structure.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.AreEqual(otherStructure.FlowWidthAtBottomProtection.Shift, structure.FlowWidthAtBottomProtection.Shift);

            Assert.AreEqual(otherStructure.InsideWaterLevel.Mean, structure.InsideWaterLevel.Mean);
            Assert.AreEqual(otherStructure.InsideWaterLevel.StandardDeviation, structure.InsideWaterLevel.StandardDeviation);

            Assert.AreEqual(otherStructure.LevelCrestStructureNotClosing.Mean, structure.LevelCrestStructureNotClosing.Mean);
            Assert.AreEqual(otherStructure.LevelCrestStructureNotClosing.StandardDeviation, structure.LevelCrestStructureNotClosing.StandardDeviation);

            Assert.AreEqual(otherStructure.StorageStructureArea.Mean, structure.StorageStructureArea.Mean);
            Assert.AreEqual(otherStructure.StorageStructureArea.CoefficientOfVariation, structure.StorageStructureArea.CoefficientOfVariation);

            Assert.AreEqual(otherStructure.ThresholdHeightOpenWeir.Mean, structure.ThresholdHeightOpenWeir.Mean);
            Assert.AreEqual(otherStructure.ThresholdHeightOpenWeir.StandardDeviation, structure.ThresholdHeightOpenWeir.StandardDeviation);

            Assert.AreEqual(otherStructure.WidthFlowApertures.Mean, structure.WidthFlowApertures.Mean);
            Assert.AreEqual(otherStructure.WidthFlowApertures.StandardDeviation, structure.WidthFlowApertures.StandardDeviation);
        }

        [Test]
        [TestCase(null)]
        [TestCase("string")]
        public void Equals_ToDifferentTypeOrNull_ReturnsFalse(object other)
        {
            // Setup
            ClosingStructure structure = CreateFullyDefinedStructure();

            // Call
            bool isEqual = structure.Equals(other);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_TransitivePropertyAllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            ClosingStructure structureX = CreateFullyDefinedStructure();
            ClosingStructure structureY = CreateFullyDefinedStructure();
            ClosingStructure structureZ = CreateFullyDefinedStructure();

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
        public void Equal_DifferentProperty_RetunsIsEqual(ClosingStructure structure,
                                                          ClosingStructure otherStructure,
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
            ClosingStructure structureOne = CreateFullyDefinedStructure();
            ClosingStructure structureTwo = CreateFullyDefinedStructure();

            // Call
            int hashCodeOne = structureOne.GetHashCode();
            int hashCodeTwo = structureTwo.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        private static ClosingStructure CreateFullyDefinedStructure()
        {
            return new ClosingStructure(CreateFullyConfiguredConstructionProperties());
        }

        private static ClosingStructure.ConstructionProperties CreateFullyConfiguredConstructionProperties()
        {
            const string id = "structure id";
            const string name = "Structure name";
            return new ClosingStructure.ConstructionProperties
            {
                Id = id,
                Name = name,
                Location = new Point2D(1, 1),
                StructureNormalOrientation = (RoundedDouble) 25,
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) 1,
                    StandardDeviation = (RoundedDouble) 0.51
                },
                AreaFlowApertures =
                {
                    Mean = (RoundedDouble) 2,
                    StandardDeviation = (RoundedDouble) 0.52
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) 3,
                    CoefficientOfVariation = (RoundedDouble) 0.53
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) 4,
                    StandardDeviation = (RoundedDouble) 0.54
                },
                InsideWaterLevel =
                {
                    Mean = (RoundedDouble) 5,
                    StandardDeviation = (RoundedDouble) 0.55
                },
                LevelCrestStructureNotClosing =
                {
                    Mean = (RoundedDouble) 6,
                    StandardDeviation = (RoundedDouble) 0.56
                },
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) 7,
                    CoefficientOfVariation = (RoundedDouble) 0.57
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = (RoundedDouble) 8,
                    StandardDeviation = (RoundedDouble) 0.58
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) 9,
                    StandardDeviation = (RoundedDouble) 0.59
                },
                FailureProbabilityOpenStructure = 9,
                FailureProbabilityReparation = 10,
                InflowModelType = ClosingStructureInflowModelType.FloodedCulvert,
                IdenticalApertures = 11,
                ProbabilityOrFrequencyOpenStructureBeforeFlooding = 12
            };
        }
    }
}