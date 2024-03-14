﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.WaveImpactAsphaltCover;
using Riskeer.Storage.Core.DbContext;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Storage.Core.Test.Create.WaveImpactAsphaltCover
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationCreateExtensionsTest
    {
        [Test]
        public void Create_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((WaveImpactAsphaltCoverWaveConditionsCalculation) null).Create(new PersistenceRegistry(), 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            // Call
            TestDelegate call = () => calculation.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual(paramName, "registry");
        }

        [Test]
        public void Create_CalculationWithPropertiesSet_ReturnCalculationEntity()
        {
            // Setup
            var random = new Random(21);
            int order = random.Next();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    Orientation = random.NextRoundedDouble(0, 360),
                    UseBreakWater = random.NextBoolean(),
                    UseForeshore = random.NextBoolean(),
                    WaterLevelType = random.NextEnumValue<WaveConditionsInputWaterLevelType>(),
                    UpperBoundaryRevetment = (RoundedDouble) 6.10,
                    LowerBoundaryRevetment = (RoundedDouble) 3.58,
                    UpperBoundaryWaterLevels = (RoundedDouble) 5.88,
                    LowerBoundaryWaterLevels = (RoundedDouble) 3.40,
                    StepSize = (RoundedDouble) 0.25
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, order);

            // Assert
            WaveConditionsInput input = calculation.InputParameters;
            Assert.AreEqual(input.Orientation, entity.Orientation, input.Orientation.GetAccuracy());
            Assert.AreEqual(Convert.ToByte(input.UseBreakWater), entity.UseBreakWater);
            Assert.AreEqual(Convert.ToByte(input.UseForeshore), entity.UseForeshore);
            Assert.AreEqual(input.UpperBoundaryRevetment, entity.UpperBoundaryRevetment, input.UpperBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(input.LowerBoundaryRevetment, entity.LowerBoundaryRevetment, input.LowerBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(input.UpperBoundaryWaterLevels, entity.UpperBoundaryWaterLevels, input.UpperBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(input.LowerBoundaryWaterLevels, entity.LowerBoundaryWaterLevels, input.LowerBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(Convert.ToByte(input.StepSize), entity.StepSize, input.StepSize.GetAccuracy());
            Assert.AreEqual(Convert.ToByte(input.WaterLevelType), entity.WaterLevelType);

            Assert.AreEqual(order, entity.Order);
            Assert.IsNull(entity.CalculationGroupEntity);
            CollectionAssert.IsEmpty(entity.WaveImpactAsphaltCoverWaveConditionsOutputEntities);
            Assert.IsNull(entity.ForeshoreProfileEntity);
            Assert.IsNull(entity.HydraulicLocationEntity);
            Assert.IsNull(entity.HydraulicLocationCalculationForTargetProbabilityCollectionEntity);
        }

        [Test]
        public void Create_CalculationWithNaNProperties_ReturnCalculationEntity()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    Orientation = RoundedDouble.NaN,
                    UpperBoundaryRevetment = RoundedDouble.NaN,
                    LowerBoundaryRevetment = RoundedDouble.NaN,
                    UpperBoundaryWaterLevels = RoundedDouble.NaN,
                    LowerBoundaryWaterLevels = RoundedDouble.NaN,
                    BreakWater =
                    {
                        Height = RoundedDouble.NaN
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 1234);

            // Assert
            Assert.IsNull(entity.Orientation);
            Assert.IsNull(entity.UpperBoundaryRevetment);
            Assert.IsNull(entity.LowerBoundaryRevetment);
            Assert.IsNull(entity.UpperBoundaryWaterLevels);
            Assert.IsNull(entity.LowerBoundaryWaterLevels);
            Assert.IsNull(entity.BreakWaterHeight);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "A";
            const string comments = "B";
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = name,
                Comments =
                {
                    Body = comments
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(calculation.Name, entity.Name);
            TestHelper.AssertAreEqualButNotSame(calculation.Comments.Body, entity.Comments);
        }

        [Test]
        public void Create_HasHydraulicLocationEntity_EntityHasHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2.3, 4.5);

            var registry = new PersistenceRegistry();
            HydraulicLocationEntity hydraulicLocationEntity = hydraulicBoundaryLocation.Create(registry, 0);

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_HasHydraulicLocationCalculationsForTargetProbability_EntityHasHydraulicLocationCalculationForTargetProbabilityCollectionEntity()
        {
            // Setup
            var random = new Random(21);
            var hydraulicCalculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1));
            var hydraulicCalculationsEntity = new HydraulicLocationCalculationForTargetProbabilityCollectionEntity();

            var registry = new PersistenceRegistry();
            registry.Register(hydraulicCalculationsEntity, hydraulicCalculations);

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    CalculationsTargetProbability = hydraulicCalculations
                }
            };

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(hydraulicCalculationsEntity, entity.HydraulicLocationCalculationForTargetProbabilityCollectionEntity);
        }

        [Test]
        public void Create_HasForeshoreProfile_EntityHasForeshoreProfileEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity.ForeshoreProfileEntity);
        }

        [Test]
        public void Create_HasCalculationOutputs_EntityHasOrderedCalculationOutputEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(new[]
                {
                    new TestWaveConditionsOutput(),
                    new TestWaveConditionsOutput()
                })
            };

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreEqual(2, entity.WaveImpactAsphaltCoverWaveConditionsOutputEntities.Count);
            Assert.AreEqual(new[]
            {
                0,
                1
            }, entity.WaveImpactAsphaltCoverWaveConditionsOutputEntities.Select(oe => oe.Order));
        }
    }
}