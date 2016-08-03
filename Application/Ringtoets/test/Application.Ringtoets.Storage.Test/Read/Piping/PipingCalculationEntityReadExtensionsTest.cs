﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.Piping;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

using NUnit.Framework;

using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Read.Piping
{
    [TestFixture]
    public class PipingCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new PipingCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null, new GeneralPipingInput());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase(3456789, true, 0.98, "haha", "hihi", 0.0, 3.4, 123)]
        [TestCase(234, false, 0.0, null, null, double.NaN, double.NaN, 321)]
        public void Read_ValidEntity_ReturnPipingCalculationScenario(long id, bool isRelevant, double contribution,
            string name, string comments, double entryPoint, double exitPoint, int seed)
        {
            // Setup
            var random = new Random(seed);

            var entity = new PipingCalculationEntity
            {
                PipingCalculationEntityId = id,
                RelevantForScenario = Convert.ToByte(isRelevant),
                ScenarioContribution = contribution.ToNaNAsNull(),
                Name = name,
                Comments = comments,
                EntryPointL = entryPoint.ToNaNAsNull(),
                ExitPointL = exitPoint.ToNaNAsNull(),
                PhreaticLevelExitMean = GetRandomNullableDoubleInRange(random, -9999.99, 9999.99),
                PhreaticLevelExitStandardDeviation = GetRandomNullableDoubleInRange(random, 0, 9999.99),
                DampingFactorExitMean = GetRandomNullableDoubleInRange(random, 1e-6, 9999.99),
                DampingFactorExitStandardDeviation = GetRandomNullableDoubleInRange(random, 0, 9999.99),
                SaturatedVolumicWeightOfCoverageLayerMean = GetRandomNullableDoubleInRange(random, 10.0, 9999.99),
                SaturatedVolumicWeightOfCoverageLayerStandardDeviation = GetRandomNullableDoubleInRange(random, 0, 9999.99),
                SaturatedVolumicWeightOfCoverageLayerShift = GetRandomNullableDoubleInRange(random, -9999.99, 10.0),
                Diameter70Mean = GetRandomNullableDoubleInRange(random, 1e-6, 9999.99),
                Diameter70StandardDeviation = GetRandomNullableDoubleInRange(random, 0.0, 9999.99),
                DarcyPermeabilityMean = GetRandomNullableDoubleInRange(random, 1e-6, 9999.99),
                DarcyPermeabilityStandardDeviation = GetRandomNullableDoubleInRange(random, 0.0, 9999.99)
            };

            var collector = new ReadConversionCollector();
            var generalInputParameters = new GeneralPipingInput();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, generalInputParameters);

            // Assert
            Assert.AreEqual(id, calculation.StorageId);
            Assert.AreEqual(isRelevant, calculation.IsRelevant);
            Assert.AreEqual(contribution, calculation.Contribution, 1e-6);
            Assert.AreEqual(name, calculation.Name);
            Assert.AreEqual(comments, calculation.Comments);

            Assert.AreEqual(generalInputParameters.BeddingAngle, calculation.InputParameters.BeddingAngle);
            Assert.AreEqual(generalInputParameters.CriticalHeaveGradient, calculation.InputParameters.CriticalHeaveGradient);
            Assert.AreEqual(generalInputParameters.Gravity, calculation.InputParameters.Gravity);
            Assert.AreEqual(generalInputParameters.MeanDiameter70, calculation.InputParameters.MeanDiameter70);
            Assert.AreEqual(generalInputParameters.SandParticlesVolumicWeight, calculation.InputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(generalInputParameters.SellmeijerModelFactor, calculation.InputParameters.SellmeijerModelFactor);
            Assert.AreEqual(generalInputParameters.SellmeijerReductionFactor, calculation.InputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(generalInputParameters.UpliftModelFactor, calculation.InputParameters.UpliftModelFactor);
            Assert.AreEqual(generalInputParameters.WaterKinematicViscosity, calculation.InputParameters.WaterKinematicViscosity);
            Assert.AreEqual(generalInputParameters.WaterVolumetricWeight, calculation.InputParameters.WaterVolumetricWeight);
            Assert.AreEqual(generalInputParameters.WhitesDragCoefficient, calculation.InputParameters.WhitesDragCoefficient);

            AssertRoundedDouble(entryPoint, calculation.InputParameters.EntryPointL);
            AssertRoundedDouble(exitPoint, calculation.InputParameters.ExitPointL);
            AssertRoundedDouble(entity.PhreaticLevelExitMean, calculation.InputParameters.PhreaticLevelExit.Mean);
            AssertRoundedDouble(entity.PhreaticLevelExitStandardDeviation, calculation.InputParameters.PhreaticLevelExit.StandardDeviation);
            AssertRoundedDouble(entity.DampingFactorExitMean, calculation.InputParameters.DampingFactorExit.Mean);
            AssertRoundedDouble(entity.DampingFactorExitStandardDeviation, calculation.InputParameters.DampingFactorExit.StandardDeviation);
            AssertRoundedDouble(entity.SaturatedVolumicWeightOfCoverageLayerMean, calculation.InputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean);
            AssertRoundedDouble(entity.SaturatedVolumicWeightOfCoverageLayerStandardDeviation, calculation.InputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation);
            AssertRoundedDouble(entity.SaturatedVolumicWeightOfCoverageLayerShift, calculation.InputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift);
            AssertRoundedDouble(entity.Diameter70Mean, calculation.InputParameters.Diameter70.Mean);
            AssertRoundedDouble(entity.Diameter70StandardDeviation, calculation.InputParameters.Diameter70.StandardDeviation);
            AssertRoundedDouble(entity.DarcyPermeabilityMean, calculation.InputParameters.DarcyPermeability.Mean);
            AssertRoundedDouble(entity.DarcyPermeabilityStandardDeviation, calculation.InputParameters.DarcyPermeability.StandardDeviation);

            Assert.IsNull(calculation.InputParameters.SurfaceLine);
            Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            Assert.IsNull(calculation.InputParameters.StochasticSoilProfile);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.SemiProbabilisticOutput);
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
                SaturatedVolumicWeightOfCoverageLayerMean = 1,
                Diameter70Mean = 1,
                DarcyPermeabilityMean = 1
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
                SurfaceLineEntityId = 123,
                PointsData = new Point3DBinaryConverter().ToBytes(points)
            };

            var entity = new PipingCalculationEntity
            {
                SurfaceLineEntity = surfaceLineEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
                SaturatedVolumicWeightOfCoverageLayerMean = 1,
                Diameter70Mean = 1,
                DarcyPermeabilityMean = 1
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.AreEqual(surfaceLineEntity.SurfaceLineEntityId, calculation.InputParameters.SurfaceLine.StorageId);
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
                SaturatedVolumicWeightOfCoverageLayerMean = 1,
                Diameter70Mean = 1,
                DarcyPermeabilityMean = 1
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
                HydraulicLocationEntityId = 123,
                Name = "A"
            };

            var entity = new PipingCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
                SaturatedVolumicWeightOfCoverageLayerMean = 1,
                Diameter70Mean = 1,
                DarcyPermeabilityMean = 1
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.AreEqual(hydraulicLocationEntity.HydraulicLocationEntityId, calculation.InputParameters.HydraulicBoundaryLocation.StorageId);
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
                SaturatedVolumicWeightOfCoverageLayerMean = 1,
                Diameter70Mean = 1,
                DarcyPermeabilityMean = 1
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
                StochasticSoilProfileEntityId = 546,
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
                StochasticSoilModelEntityId = 75,
                StochasticSoilModelSegmentPointData = new Point2DBinaryConverter().ToBytes(new Point2D[0]),
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
                SaturatedVolumicWeightOfCoverageLayerMean = 1,
                Diameter70Mean = 1,
                DarcyPermeabilityMean = 1
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.AreEqual(stochasticSoilProfileEntity.StochasticSoilProfileEntityId, calculation.InputParameters.StochasticSoilProfile.StorageId);
            Assert.AreEqual(stochasticSoilModelEntity.StochasticSoilModelEntityId, calculation.InputParameters.StochasticSoilModel.StorageId);
            Assert.IsTrue(collector.Contains(stochasticSoilProfileEntity));
            Assert.IsTrue(collector.Contains(stochasticSoilModelEntity));
        }

        [Test]
        public void Read_EntityWithPipingCalculationOutputEntity_CalculationWithPipingOutput()
        {
            // Setup
            const int outputId = 4578;
            var entity = new PipingCalculationEntity
            {
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
                SaturatedVolumicWeightOfCoverageLayerMean = 1,
                Diameter70Mean = 1,
                DarcyPermeabilityMean = 1,
                PipingCalculationOutputEntity = new PipingCalculationOutputEntity
                {
                    PipingCalculationOutputEntityId = outputId
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.AreEqual(outputId, calculation.Output.StorageId);
        }

        [Test]
        public void Read_EntityWithPipingSemiProbabilisticOutputEntity_CalculationWithPipingSemiProbabilisticOutput()
        {
            // Setup
            const int outputId = 675;
            var entity = new PipingCalculationEntity
            {
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
                SaturatedVolumicWeightOfCoverageLayerMean = 1,
                Diameter70Mean = 1,
                DarcyPermeabilityMean = 1,
                PipingSemiProbabilisticOutputEntity = new PipingSemiProbabilisticOutputEntity
                {
                    PipingSemiProbabilisticOutputEntityId = outputId
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.IsNotNull(calculation.SemiProbabilisticOutput);
            Assert.AreEqual(outputId, calculation.SemiProbabilisticOutput.StorageId);
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
            return lowerLimit + random.NextDouble() * difference;
        }
    }
}