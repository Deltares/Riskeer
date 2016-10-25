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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.HeightStructures;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Create.HeightStructures
{
    [TestFixture]
    public class HeightStructuresCalculationCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            TestDelegate call = () => calculation.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        [TestCase("I have no comments", null, 0, 827364)]
        [TestCase("I have a comment", "I am comment", 98, 231)]
        public void Create_ValidCalculation_ReturnEntity(string name, string comments, int order, int randomSeed)
        {
            // Setup
            var random = new Random(randomSeed);

            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = name,
                Comments = comments,
                InputParameters =
                {
                    StructureNormalOrientation = (RoundedDouble) GetRandomDoubleFromRange(random, 0, 360),
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = (RoundedDouble) GetRandomDoubleFromRange(random, -9999.9999, 9999.9999)
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        CoefficientOfVariation = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        StandardDeviation = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        CoefficientOfVariation = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    FailureProbabilityStructureWithErosion = random.NextDouble(),
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) GetRandomDoubleFromRange(random, -9999.9999, 9999.9999),
                        CoefficientOfVariation = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    StormDuration =
                    {
                        Mean = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999),
                        CoefficientOfVariation = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    DeviationWaveDirection = (RoundedDouble) random.NextDouble(),
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) GetRandomDoubleFromRange(random, -9999.9999, 9999.9999),
                        StandardDeviation = (RoundedDouble) GetRandomDoubleFromRange(random, 1e-6, 9999.9999)
                    },
                    UseBreakWater = true,
                    UseForeshore = false,
                    BreakWater =
                    {
                        Height = (RoundedDouble) random.NextDouble(),
                        Type = BreakWaterType.Dam
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            HeightStructuresCalculationEntity entity = calculation.Create(registry, order);

            // Assert
            Assert.AreEqual(0, entity.HeightStructuresCalculationEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(comments, entity.Comments);
            Assert.AreEqual(order, entity.Order);

            HeightStructuresInput input = calculation.InputParameters;
            Assert.AreEqual(input.StructureNormalOrientation.Value, entity.StructureNormalOrientation);
            Assert.AreEqual(input.ModelFactorSuperCriticalFlow.Mean.Value, entity.ModelFactorSuperCriticalFlowMean);
            Assert.AreEqual(input.AllowedLevelIncreaseStorage.Mean.Value, entity.AllowedLevelIncreaseStorageMean);
            Assert.AreEqual(input.AllowedLevelIncreaseStorage.StandardDeviation.Value, entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.AreEqual(input.StorageStructureArea.Mean.Value, entity.StorageStructureAreaMean);
            Assert.AreEqual(input.StorageStructureArea.CoefficientOfVariation.Value, entity.StorageStructureAreaCoefficientOfVariation);
            Assert.AreEqual(input.FlowWidthAtBottomProtection.Mean.Value, entity.FlowWidthAtBottomProtectionMean);
            Assert.AreEqual(input.FlowWidthAtBottomProtection.StandardDeviation.Value, entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.AreEqual(input.CriticalOvertoppingDischarge.Mean.Value, entity.CriticalOvertoppingDischargeMean);
            Assert.AreEqual(input.CriticalOvertoppingDischarge.CoefficientOfVariation.Value, entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.AreEqual(input.FailureProbabilityStructureWithErosion, entity.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(input.WidthFlowApertures.Mean.Value, entity.WidthFlowAperturesMean);
            Assert.AreEqual(input.WidthFlowApertures.CoefficientOfVariation.Value, entity.WidthFlowAperturesCoefficientOfVariation);
            Assert.AreEqual(input.StormDuration.Mean.Value, entity.StormDurationMean);

            Assert.AreEqual(input.LevelCrestStructure.Mean.Value, entity.LevelCrestStructureMean);
            Assert.AreEqual(input.LevelCrestStructure.StandardDeviation.Value, entity.LevelCrestStructureStandardDeviation);
            Assert.AreEqual(input.DeviationWaveDirection.Value, entity.DeviationWaveDirection);

            Assert.IsNull(entity.CalculationGroupEntity);
            Assert.IsNull(entity.ForeshoreProfileEntityId);
            Assert.IsNull(entity.HeightStructureEntityId);
            Assert.IsNull(entity.HydraulicLocationEntityId);

            Assert.AreEqual(input.BreakWater.Height.Value, entity.BreakWaterHeight);
            Assert.AreEqual((short) input.BreakWater.Type, entity.BreakWaterType);
            Assert.AreEqual(Convert.ToByte(input.UseBreakWater), entity.UseBreakWater);
            Assert.AreEqual(Convert.ToByte(input.UseForeshore), entity.UseForeshore);
        }

        [Test]
        public void Create_NaNParameters_EntityWithNullFields()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    StructureNormalOrientation = (RoundedDouble) double.NaN,
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = (RoundedDouble) double.NaN
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) double.NaN,
                        StandardDeviation = (RoundedDouble) double.NaN
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) double.NaN,
                        CoefficientOfVariation = (RoundedDouble) double.NaN
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) double.NaN,
                        StandardDeviation = (RoundedDouble) double.NaN
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) double.NaN,
                        CoefficientOfVariation = (RoundedDouble) double.NaN
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) double.NaN,
                        CoefficientOfVariation = (RoundedDouble) double.NaN
                    },
                    StormDuration =
                    {
                        Mean = (RoundedDouble) double.NaN,
                        CoefficientOfVariation = (RoundedDouble) double.NaN
                    },
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) double.NaN,
                        StandardDeviation = (RoundedDouble) double.NaN
                    },
                    DeviationWaveDirection = (RoundedDouble) double.NaN,
                    BreakWater =
                    {
                        Height = (RoundedDouble) double.NaN
                    }
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            HeightStructuresCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNull(entity.StructureNormalOrientation);
            Assert.IsNull(entity.ModelFactorSuperCriticalFlowMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.IsNull(entity.StorageStructureAreaMean);
            Assert.IsNull(entity.StorageStructureAreaCoefficientOfVariation);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionMean);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.IsNull(entity.CriticalOvertoppingDischargeMean);
            Assert.IsNull(entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.IsNull(entity.WidthFlowAperturesMean);
            Assert.IsNull(entity.WidthFlowAperturesCoefficientOfVariation);
            Assert.IsNull(entity.StormDurationMean);
            Assert.IsNull(entity.DeviationWaveDirection);
            Assert.IsNull(entity.LevelCrestStructureMean);
            Assert.IsNull(entity.LevelCrestStructureStandardDeviation);

            Assert.IsNull(entity.BreakWaterHeight);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "A";
            const string comment = "B";
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = name,
                Comments = comment
            };

            var registry = new PersistenceRegistry();

            // Call
            HeightStructuresCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreNotSame(name, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(name, entity.Name);

            Assert.AreNotSame(comment, entity.Comments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(comment, entity.Comments);
        }

        [Test]
        public void Create_CalculationWithAlreadySavedHydraulicBoundaryLocation_ReturnEntityWithHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1, 1);
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            HeightStructuresCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_CalculationWithAlreadySavedHeightStructure_ReturnEntityWithHeightStructureEntity()
        {
            // Setup
            var heightStructure = new TestHeightStructure();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = heightStructure
                }
            };

            var heightStructureEntity = new HeightStructureEntity();
            var registry = new PersistenceRegistry();
            registry.Register(heightStructureEntity, heightStructure);

            // Call
            HeightStructuresCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(heightStructureEntity, entity.HeightStructureEntity);
        }

        [Test]
        public void Create_CalculationWithAlreadySavedForeshoreProfile_ReturnEntityWithForeshoreProfileEntity()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };

            var foreshoreProfileEntity = new ForeshoreProfileEntity();
            var registry = new PersistenceRegistry();
            registry.Register(foreshoreProfileEntity, foreshoreProfile);

            // Call
            HeightStructuresCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(foreshoreProfileEntity, entity.ForeshoreProfileEntity);
        }

        private static double GetRandomDoubleFromRange(Random random, double lowerLimit, double upperLimit)
        {
            double difference = upperLimit - lowerLimit;
            return lowerLimit + random.NextDouble()*difference;
        }
    }
}