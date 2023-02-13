﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.WaveImpactAsphaltCover;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil.Hydraulics;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Storage.Core.Test.Read.WaveImpactAsphaltCover
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((WaveImpactAsphaltCoverWaveConditionsCalculationEntity) null).Read(new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity();

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
            var waterLevelType = random.NextEnumValue<WaveConditionsInputWaterLevelType>();

            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
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
                WaterLevelType = Convert.ToByte(waterLevelType)
            };

            var collector = new ReadConversionCollector();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, calculation.Name);
            Assert.AreEqual(comments, calculation.Comments.Body);

            WaveConditionsInput calculationInput = calculation.InputParameters;
            Assert.AreEqual(useBreakWater, calculationInput.UseBreakWater);
            Assert.AreEqual(breakWaterType, calculationInput.BreakWater.Type);
            RoundedDoubleTestHelper.AssertRoundedDouble(breakWaterHeight, calculationInput.BreakWater.Height);
            Assert.AreEqual(useForeshore, calculationInput.UseForeshore);
            RoundedDoubleTestHelper.AssertRoundedDouble(orientation, calculationInput.Orientation);
            RoundedDoubleTestHelper.AssertRoundedDouble(upperBoundaryRevetment, calculationInput.UpperBoundaryRevetment);
            RoundedDoubleTestHelper.AssertRoundedDouble(lowerBoundaryRevetment, calculationInput.LowerBoundaryRevetment);
            RoundedDoubleTestHelper.AssertRoundedDouble(upperBoundaryWaterLevels, calculationInput.UpperBoundaryWaterLevels);
            RoundedDoubleTestHelper.AssertRoundedDouble(lowerBoundaryWaterLevels, calculationInput.LowerBoundaryWaterLevels);
            Assert.AreEqual(stepSize, calculationInput.StepSize);
            Assert.AreEqual(waterLevelType, calculationInput.WaterLevelType);

            Assert.IsNull(calculationInput.HydraulicBoundaryLocation);
            Assert.IsNull(calculationInput.ForeshoreProfile);
            Assert.IsNull(calculationInput.CalculationsTargetProbability);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnCalculationWithNaNValues()
        {
            // Setup
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity();
            var collector = new ReadConversionCollector();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNull(calculation.Name);
            Assert.IsNull(calculation.Comments.Body);

            WaveConditionsInput calculationInput = calculation.InputParameters;
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
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                ForeshoreProfileEntity = foreshoreProfileEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(foreshoreProfileEntity, foreshoreProfile);

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

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

            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                ForeshoreProfileEntity = foreshoreProfileEntity
            };

            var collector = new ReadConversionCollector();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(foreshoreProfileEntity));
            CollectionAssert.AreEqual(id, calculation.InputParameters.ForeshoreProfile.Id);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationInCollector_CalculationHasAlreadyReadHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1.1, 2.2, new HrdFile());
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationNotYetInCollector_CalculationWithCreatedHydraulicBoundaryLocationAndRegisteredNewEntities()
        {
            // Setup
            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity
            };

            var collector = new ReadConversionCollector();

            // Call
            entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(hydraulicLocationEntity));
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationCalculationsForTargetProbabilityInCollector_CalculationHasAlreadyReadHydraulicBoundaryLocationCalculationsForTargetProbability()
        {
            // Setup
            var hydraulicBoundaryLocationCalculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.05);
            HydraulicLocationCalculationForTargetProbabilityCollectionEntity calculationForTargetProbabilityCollectionEntity =
                HydraulicLocationCalculationForTargetProbabilityCollectionEntityTestFactory.CreateHydraulicLocationCalculationForTargetProbabilityCollectionEntity();
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                HydraulicLocationCalculationForTargetProbabilityCollectionEntity = calculationForTargetProbabilityCollectionEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(calculationForTargetProbabilityCollectionEntity, hydraulicBoundaryLocationCalculationsForTargetProbability);

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocationCalculationsForTargetProbability, calculation.InputParameters.CalculationsTargetProbability);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationCalculationsForTargetProbabilityNotYetInCollector_CalculationWithCreatedHydraulicBoundaryLocationCalculationsForTargetProbabilityAndRegisteredNewEntities()
        {
            // Setup
            HydraulicLocationCalculationForTargetProbabilityCollectionEntity calculationForTargetProbabilityCollectionEntity =
                HydraulicLocationCalculationForTargetProbabilityCollectionEntityTestFactory.CreateHydraulicLocationCalculationForTargetProbabilityCollectionEntity();
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                HydraulicLocationCalculationForTargetProbabilityCollectionEntity = calculationForTargetProbabilityCollectionEntity
            };

            var collector = new ReadConversionCollector();

            // Call
            entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(calculationForTargetProbabilityCollectionEntity));
        }

        [Test]
        public void Read_EntityWithCalculationOutputEntity_CalculationWithOutput()
        {
            // Setup
            const double outputALevel = 5.4;
            const double outputBLevel = 2.3;
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                WaveImpactAsphaltCoverWaveConditionsOutputEntities =
                {
                    new WaveImpactAsphaltCoverWaveConditionsOutputEntity
                    {
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        WaterLevel = outputBLevel,
                        Order = 1
                    },
                    new WaveImpactAsphaltCoverWaveConditionsOutputEntity
                    {
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        WaterLevel = outputALevel,
                        Order = 0
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.AreEqual(2, calculation.Output.Items.Count());
            double accuracy = calculation.Output.Items.ElementAt(0).WaterLevel.GetAccuracy();
            Assert.AreEqual(outputALevel, calculation.Output.Items.ElementAt(0).WaterLevel, accuracy);
            Assert.AreEqual(outputBLevel, calculation.Output.Items.ElementAt(1).WaterLevel, accuracy);
        }
    }
}