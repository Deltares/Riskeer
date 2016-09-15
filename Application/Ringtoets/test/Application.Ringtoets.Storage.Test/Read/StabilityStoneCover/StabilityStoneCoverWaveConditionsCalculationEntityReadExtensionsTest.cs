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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.Piping;
using Application.Ringtoets.Storage.Read.StabilityStoneCover;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;

namespace Application.Ringtoets.Storage.Test.Read.StabilityStoneCover
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase("haha", "hihi", true, BreakWaterType.Dam, 0.98, true, 0.0, 3.4, 2.4, 16.8, 2.8, WaveConditionsInputStepSize.Two)]
        [TestCase(null, null, false, BreakWaterType.Wall, 0.0, false, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.One)]
        public void Read_ValidEntity_ReturnStabilityStoneCoverWaveConditionsCalculation(string name, string comments, bool useBreakWater, BreakWaterType breakWaterType, 
            double breakWaterHeight, bool useForeshore, double orientation, double upperBoundaryRevetment, double lowerBoundaryRevetment, double upperBoundaryWaterLevels,
            double lowerBoundaryWaterLevels, WaveConditionsInputStepSize stepSize)
        {
            // Setup
            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity
            {
                Name = name,
                Comments = comments,
                UseBreakWater = Convert.ToByte(useBreakWater),
                BreakWaterType = (byte) breakWaterType,
                BreakWaterHeight = breakWaterHeight,
                UseForeshore = Convert.ToByte(useForeshore),
                Orientation = orientation,
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = (byte)stepSize
            };

            var collector = new ReadConversionCollector();

            // Call
            StabilityStoneCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, calculation.Name);
            Assert.AreEqual(comments, calculation.Comments);
            Assert.AreEqual(useBreakWater, calculation.InputParameters.UseBreakWater);
            Assert.AreEqual(breakWaterType, calculation.InputParameters.BreakWater.Type);
            AssertRoundedDouble(breakWaterHeight, calculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(useForeshore, calculation.InputParameters.UseForeshore);
            AssertRoundedDouble(orientation, calculation.InputParameters.Orientation);
            AssertRoundedDouble(upperBoundaryRevetment, calculation.InputParameters.UpperBoundaryRevetment);
            AssertRoundedDouble(lowerBoundaryRevetment, calculation.InputParameters.LowerBoundaryRevetment);
            AssertRoundedDouble(upperBoundaryWaterLevels, calculation.InputParameters.UpperBoundaryWaterLevels);
            AssertRoundedDouble(lowerBoundaryWaterLevels, calculation.InputParameters.LowerBoundaryWaterLevels);
            Assert.AreEqual(stepSize, calculation.InputParameters.StepSize);

            Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Read_EntityWithSurfaceLineInCollector_CalculationHasAlreadyReadSurfaceLine()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var surfaceLineEntity = new SurfaceLineEntity();
            var entity = new PipingCalculationEntity
            {
                SurfaceLineEntity = surfaceLineEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
            };

            var collector = new ReadConversionCollector();
            collector.Read(surfaceLineEntity, surfaceLine);

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.AreSame(surfaceLine, calculation.InputParameters.SurfaceLine);
            Assert.AreEqual(1, calculation.InputParameters.EntryPointL, 1e-6);
            Assert.AreEqual(2, calculation.InputParameters.ExitPointL, 1e-6);
        }

        [Test]
        public void Read_EntityWithSurfaceLineNotYetInCollector_CalculationWithCreatedSurfaceLineAndRegisteredNewEntities()
        {
            // Setup
            var points = new[]
            {
                new Point3D(1, 3, 4),
                new Point3D(7, 10, 11)
            };

            var surfaceLineEntity = new SurfaceLineEntity
            {
                PointsXml = new Point3DXmlSerializer().ToXml(points)
            };

            var entity = new PipingCalculationEntity
            {
                SurfaceLineEntity = surfaceLineEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.IsTrue(collector.Contains(surfaceLineEntity));
            CollectionAssert.AreEqual(points, calculation.InputParameters.SurfaceLine.Points);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationInCollector_CalculationHasAlreadyReadHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1.1, 2.2);
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var entity = new PipingCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationNotYetInCollector_CalculationWithCreatedHydraulicBoundaryLocationAndRegisteredNewEntities()
        {
            // Setup
            var hydraulicLocationEntity = new HydraulicLocationEntity
            {
                Name = "A"
            };

            var entity = new PipingCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
            };

            var collector = new ReadConversionCollector();

            // Call
            entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.IsTrue(collector.Contains(hydraulicLocationEntity));
        }

        [Test]
        public void Read_EntityWithStochasticSoilModelEntityInCollector_CalculationHasAlreadyReadStochasticSoilModel()
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel(1, "A", "B");
            var stochasticSoilModelEntity = new StochasticSoilModelEntity();

            var stochasticSoilProfile = new StochasticSoilProfile(1, SoilProfileType.SoilProfile1D, 1);
            var stochasticSoilProfileEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilModelEntity = stochasticSoilModelEntity
            };

            var entity = new PipingCalculationEntity
            {
                StochasticSoilProfileEntity = stochasticSoilProfileEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
            };

            var collector = new ReadConversionCollector();
            collector.Read(stochasticSoilProfileEntity, stochasticSoilProfile);
            collector.Read(stochasticSoilModelEntity, stochasticSoilModel);

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.AreSame(stochasticSoilProfile, calculation.InputParameters.StochasticSoilProfile);
            Assert.AreSame(stochasticSoilModel, calculation.InputParameters.StochasticSoilModel);
        }

        [Test]
        public void Read_EntityWithStochasticSoilProfileEntityNotYetInCollector_CalculationWithCreatedStochasticSoilProfileAndRegisteredNewEntities()
        {
            // Setup
            var stochasticSoilProfileEntity = new StochasticSoilProfileEntity
            {
                SoilProfileEntity = new SoilProfileEntity
                {
                    SoilLayerEntities =
                    {
                        new SoilLayerEntity()
                    }
                }
            };

            var stochasticSoilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelSegmentPointXml = new Point2DXmlSerializer().ToXml(new Point2D[0]),
                StochasticSoilProfileEntities =
                {
                    stochasticSoilProfileEntity
                }
            };
            stochasticSoilProfileEntity.StochasticSoilModelEntity = stochasticSoilModelEntity;

            var entity = new PipingCalculationEntity
            {
                StochasticSoilProfileEntity = stochasticSoilProfileEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
            };

            var collector = new ReadConversionCollector();

            // Call
            entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.IsTrue(collector.Contains(stochasticSoilProfileEntity));
            Assert.IsTrue(collector.Contains(stochasticSoilModelEntity));
        }

        [Test]
        public void Read_EntityWithPipingCalculationOutputEntity_CalculationWithPipingOutput()
        {
            // Setup
            var entity = new PipingCalculationEntity
            {
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
                PipingCalculationOutputEntity = new PipingCalculationOutputEntity()
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.IsNotNull(calculation.Output);
        }

        [Test]
        public void Read_EntityWithPipingSemiProbabilisticOutputEntity_CalculationWithPipingSemiProbabilisticOutput()
        {
            // Setup
            var entity = new PipingCalculationEntity
            {
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
                PipingSemiProbabilisticOutputEntity = new PipingSemiProbabilisticOutputEntity()
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.IsNotNull(calculation.SemiProbabilisticOutput);
        }

        private void AssertRoundedDouble(double? expectedValue, RoundedDouble actualValue)
        {
            Assert.IsTrue(expectedValue.HasValue);
            Assert.AreEqual(expectedValue.Value, actualValue, actualValue.GetAccuracy());
        }

        private static void AssertRoundedDouble(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }

        private static double? GetRandomNullableDoubleInRange(Random random, double lowerLimit, double upperLimit)
        {
            var difference = upperLimit - lowerLimit;
            return lowerLimit + random.NextDouble()*difference;
        }
    }
}