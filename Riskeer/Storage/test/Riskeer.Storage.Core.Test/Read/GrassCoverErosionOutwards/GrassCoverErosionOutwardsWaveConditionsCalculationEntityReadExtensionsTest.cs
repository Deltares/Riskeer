﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Revetment.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.GrassCoverErosionOutwards;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil.Hydraulics;

namespace Riskeer.Storage.Core.Test.Read.GrassCoverErosionOutwards
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((GrassCoverErosionOutwardsWaveConditionsCalculationEntity) null).Read(new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_ValidEntity_ReturnCalculation()
        {
            // Setup
            const string name = "Calculation Name";
            const string comments = "Calculation Comment";

            var random = new Random(21);
            double orientation = random.NextDouble();
            bool useBreakWater = random.NextBoolean();
            var breakWaterType = random.NextEnumValue<BreakWaterType>();
            double breakWaterHeight = random.NextDouble();
            bool useForeshore = random.NextBoolean();
            const double lowerBoundaryRevetment = 3.58;
            const double upperBoundaryRevetment = 6.10;
            const double lowerBoundaryWaterLevels = 3.40;
            const double upperBoundaryWaterLevels = 5.88;
            var stepSize = random.NextEnumValue<WaveConditionsInputStepSize>();
            var categoryType = random.NextEnumValue<FailureMechanismCategoryType>();
            var calculationType = random.NextEnumValue<GrassCoverErosionOutwardsWaveConditionsCalculationType>();

            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
            {
                Name = name,
                Comments = comments,
                UseBreakWater = Convert.ToByte(useBreakWater),
                BreakWaterType = Convert.ToByte(breakWaterType),
                BreakWaterHeight = breakWaterHeight,
                UseForeshore = Convert.ToByte(useForeshore),
                Orientation = orientation,
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = Convert.ToByte(stepSize),
                CategoryType = Convert.ToByte(categoryType),
                CalculationType = Convert.ToByte(calculationType)
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, calculation.Name);
            Assert.AreEqual(comments, calculation.Comments.Body);

            GrassCoverErosionOutwardsWaveConditionsInput calculationInput = calculation.InputParameters;
            Assert.AreEqual(useBreakWater, calculationInput.UseBreakWater);
            Assert.AreEqual(breakWaterType, calculationInput.BreakWater.Type);
            AssertRoundedDouble(breakWaterHeight, calculationInput.BreakWater.Height);
            Assert.AreEqual(useForeshore, calculationInput.UseForeshore);
            AssertRoundedDouble(orientation, calculationInput.Orientation);
            AssertRoundedDouble(upperBoundaryRevetment, calculationInput.UpperBoundaryRevetment);
            AssertRoundedDouble(lowerBoundaryRevetment, calculationInput.LowerBoundaryRevetment);
            AssertRoundedDouble(upperBoundaryWaterLevels, calculationInput.UpperBoundaryWaterLevels);
            AssertRoundedDouble(lowerBoundaryWaterLevels, calculationInput.LowerBoundaryWaterLevels);
            Assert.AreEqual(stepSize, calculationInput.StepSize);
            Assert.AreEqual(categoryType, calculationInput.CategoryType);
            Assert.AreEqual(calculationType, calculationInput.CalculationType);

            Assert.IsNull(calculationInput.HydraulicBoundaryLocation);
            Assert.IsNull(calculationInput.ForeshoreProfile);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnCalculationWithNaNValues()
        {
            // Setup
            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity();
            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNull(calculation.Name);
            Assert.IsNull(calculation.Comments.Body);

            FailureMechanismCategoryWaveConditionsInput calculationInput = calculation.InputParameters;
            Assert.IsNaN(calculationInput.BreakWater.Height);
            Assert.IsNaN(calculationInput.Orientation);
            Assert.IsNaN(calculationInput.UpperBoundaryRevetment);
            Assert.IsNaN(calculationInput.LowerBoundaryRevetment);
            Assert.IsNaN(calculationInput.UpperBoundaryWaterLevels);
            Assert.IsNaN(calculationInput.LowerBoundaryWaterLevels);

            Assert.IsNull(calculationInput.HydraulicBoundaryLocation);
            Assert.IsNull(calculationInput.ForeshoreProfile);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Read_EntityWithForeshoreProfileInCollector_CalculationHasAlreadyReadForeshoreProfile()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var foreshoreProfileEntity = new ForeshoreProfileEntity
            {
                GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
            };
            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
            {
                ForeshoreProfileEntity = foreshoreProfileEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(foreshoreProfileEntity, foreshoreProfile);

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(foreshoreProfile, calculation.InputParameters.ForeshoreProfile);
        }

        [Test]
        public void Read_EntityWithForeshoreProfileNotYetInCollector_CalculationWithCreatedForeshoreProfileAndRegisteredNewEntities()
        {
            // Setup
            const string id = "profile";
            var foreshoreProfileEntity = new ForeshoreProfileEntity
            {
                Id = id,
                GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
            };

            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
            {
                ForeshoreProfileEntity = foreshoreProfileEntity
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(foreshoreProfileEntity));
            CollectionAssert.AreEqual(id, calculation.InputParameters.ForeshoreProfile.Id);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationInCollector_CalculationHasAlreadyReadHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithCalculationOutputEntityWithWaveRunUpAndWaveImpact_CalculationWithOutput()
        {
            // Setup
            const double outputALevel = 0;
            const double outputBLevel = 1.1;
            const double outputCLevel = 2.2;
            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
            {
                GrassCoverErosionOutwardsWaveConditionsOutputEntities =
                {
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputBLevel,
                        Order = 1,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveRunUp)
                    },
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputALevel,
                        Order = 0,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveRunUp)
                    },
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputCLevel,
                        Order = 2,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveImpact)
                    },
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputBLevel,
                        Order = 4,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.TailorMadeWaveImpact)
                    },
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputALevel,
                        Order = 3,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.TailorMadeWaveImpact)
                    },
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputCLevel,
                        Order = 5,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.TailorMadeWaveImpact)
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.Output);
            double accuracy = calculation.Output.WaveImpactOutput.First().WaterLevel.GetAccuracy();

            Assert.AreEqual(2, calculation.Output.WaveRunUpOutput.Count());
            Assert.AreEqual(outputALevel, calculation.Output.WaveRunUpOutput.ElementAt(0).WaterLevel, accuracy);
            Assert.AreEqual(outputBLevel, calculation.Output.WaveRunUpOutput.ElementAt(1).WaterLevel, accuracy);

            Assert.AreEqual(1, calculation.Output.WaveImpactOutput.Count());
            Assert.AreEqual(outputCLevel, calculation.Output.WaveImpactOutput.ElementAt(0).WaterLevel, accuracy);

            Assert.AreEqual(3, calculation.Output.TailorMadeWaveImpactOutput.Count());
            Assert.AreEqual(outputALevel, calculation.Output.TailorMadeWaveImpactOutput.ElementAt(0).WaterLevel, accuracy);
            Assert.AreEqual(outputBLevel, calculation.Output.TailorMadeWaveImpactOutput.ElementAt(1).WaterLevel, accuracy);
            Assert.AreEqual(outputCLevel, calculation.Output.TailorMadeWaveImpactOutput.ElementAt(2).WaterLevel, accuracy);
        }

        [Test]
        public void Read_EntityWithCalculationOutputEntityWithWaveRunUp_CalculationWithOutput()
        {
            // Setup
            const double outputALevel = 0;
            const double outputBLevel = 1.1;
            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
            {
                GrassCoverErosionOutwardsWaveConditionsOutputEntities =
                {
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputBLevel,
                        Order = 1,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveRunUp)
                    },
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputALevel,
                        Order = 0,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveRunUp)
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.Output);
            double accuracy = calculation.Output.WaveRunUpOutput.First().WaterLevel.GetAccuracy();

            Assert.AreEqual(2, calculation.Output.WaveRunUpOutput.Count());
            Assert.AreEqual(outputALevel, calculation.Output.WaveRunUpOutput.ElementAt(0).WaterLevel, accuracy);
            Assert.AreEqual(outputBLevel, calculation.Output.WaveRunUpOutput.ElementAt(1).WaterLevel, accuracy);

            Assert.IsNull(calculation.Output.WaveImpactOutput);
        }

        [Test]
        public void Read_EntityWithCalculationOutputEntityWithWavImpact_CalculationWithOutput()
        {
            // Setup
            const double outputALevel = 0;
            const double outputBLevel = 1.1;
            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
            {
                GrassCoverErosionOutwardsWaveConditionsOutputEntities =
                {
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputBLevel,
                        Order = 1,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveImpact)
                    },
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputALevel,
                        Order = 0,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveImpact)
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.Output);
            double accuracy = calculation.Output.WaveImpactOutput.First().WaterLevel.GetAccuracy();

            Assert.AreEqual(2, calculation.Output.WaveImpactOutput.Count());
            Assert.AreEqual(outputALevel, calculation.Output.WaveImpactOutput.ElementAt(0).WaterLevel, accuracy);
            Assert.AreEqual(outputBLevel, calculation.Output.WaveImpactOutput.ElementAt(1).WaterLevel, accuracy);

            Assert.IsNull(calculation.Output.WaveRunUpOutput);
        }

        [Test]
        public void Read_EntityWithCalculationOutputEntityWithTailorMadeWavImpact_CalculationWithOutput()
        {
            // Setup
            const double outputALevel = 0;
            const double outputBLevel = 1.1;
            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
            {
                GrassCoverErosionOutwardsWaveConditionsOutputEntities =
                {
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputBLevel,
                        Order = 1,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.TailorMadeWaveImpact)
                    },
                    new GrassCoverErosionOutwardsWaveConditionsOutputEntity
                    {
                        WaterLevel = outputALevel,
                        Order = 0,
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        OutputType = Convert.ToByte(GrassCoverErosionOutwardsWaveConditionsOutputType.TailorMadeWaveImpact)
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.Output);
            double accuracy = calculation.Output.TailorMadeWaveImpactOutput.First().WaterLevel.GetAccuracy();

            Assert.AreEqual(2, calculation.Output.TailorMadeWaveImpactOutput.Count());
            Assert.AreEqual(outputALevel, calculation.Output.TailorMadeWaveImpactOutput.ElementAt(0).WaterLevel, accuracy);
            Assert.AreEqual(outputBLevel, calculation.Output.TailorMadeWaveImpactOutput.ElementAt(1).WaterLevel, accuracy);

            Assert.IsNull(calculation.Output.WaveRunUpOutput);
        }

        private static void AssertRoundedDouble(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }
    }
}