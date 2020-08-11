// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.Piping;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil.Hydraulics;

namespace Riskeer.Storage.Core.Test.Read.Piping
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
            void Call() => entity.Read(null, new GeneralPipingInput());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        [TestCase(true, false, 0.98, "haha", "hihi", 0.0, 3.4, 5.8, 123)]
        [TestCase(false, true, 0.0, null, null, double.NaN, double.NaN, double.NaN, 321)]
        public void Read_ValidEntity_ReturnPipingCalculationScenario(bool isRelevant, bool useAssessmentLevelManualInput, double contribution,
                                                                     string name, string comments, double entryPoint, double exitPoint,
                                                                     double assessmentLevel, int seed)
        {
            // Setup
            var random = new Random(seed);

            var entity = new PipingCalculationEntity
            {
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
                AssessmentLevel = assessmentLevel.ToNaNAsNull(),
                UseAssessmentLevelManualInput = Convert.ToByte(useAssessmentLevelManualInput)
            };

            var collector = new ReadConversionCollector();
            var generalInputParameters = new GeneralPipingInput();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, generalInputParameters);

            // Assert
            Assert.AreEqual(isRelevant, calculation.IsRelevant);
            Assert.AreEqual(contribution, calculation.Contribution, 1e-6);
            Assert.AreEqual(name, calculation.Name);
            Assert.AreEqual(comments, calculation.Comments.Body);

            Assert.AreEqual(generalInputParameters.BeddingAngle, calculation.InputParameters.BeddingAngle);
            Assert.AreEqual(generalInputParameters.CriticalHeaveGradient, calculation.InputParameters.CriticalHeaveGradient);
            Assert.AreEqual(generalInputParameters.Gravity, calculation.InputParameters.Gravity);
            Assert.AreEqual(generalInputParameters.MeanDiameter70, calculation.InputParameters.MeanDiameter70);
            Assert.AreEqual(generalInputParameters.SandParticlesVolumicWeight.Value, calculation.InputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(generalInputParameters.SellmeijerModelFactor, calculation.InputParameters.SellmeijerModelFactor);
            Assert.AreEqual(generalInputParameters.SellmeijerReductionFactor, calculation.InputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(generalInputParameters.UpliftModelFactor, calculation.InputParameters.UpliftModelFactor);
            Assert.AreEqual(generalInputParameters.WaterKinematicViscosity, calculation.InputParameters.WaterKinematicViscosity);
            Assert.AreEqual(generalInputParameters.WaterVolumetricWeight.Value, calculation.InputParameters.WaterVolumetricWeight);
            Assert.AreEqual(generalInputParameters.WhitesDragCoefficient, calculation.InputParameters.WhitesDragCoefficient);

            AssertRoundedDouble(entryPoint, calculation.InputParameters.EntryPointL);
            AssertRoundedDouble(exitPoint, calculation.InputParameters.ExitPointL);
            AssertRoundedDouble(entity.PhreaticLevelExitMean, calculation.InputParameters.PhreaticLevelExit.Mean);
            AssertRoundedDouble(entity.PhreaticLevelExitStandardDeviation, calculation.InputParameters.PhreaticLevelExit.StandardDeviation);
            AssertRoundedDouble(entity.DampingFactorExitMean, calculation.InputParameters.DampingFactorExit.Mean);
            AssertRoundedDouble(entity.DampingFactorExitStandardDeviation, calculation.InputParameters.DampingFactorExit.StandardDeviation);

            Assert.AreEqual(useAssessmentLevelManualInput, calculation.InputParameters.UseAssessmentLevelManualInput);
            Assert.AreEqual(entity.AssessmentLevel.ToNullAsNaN(), calculation.InputParameters.AssessmentLevel.Value);

            Assert.IsNull(calculation.InputParameters.SurfaceLine);
            Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            Assert.IsNull(calculation.InputParameters.StochasticSoilProfile);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Read_EntityWithSurfaceLineInCollector_CalculationHasAlreadyReadSurfaceLine()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);
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
                DampingFactorExitMean = 1
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
                Name = "surface line",
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(points)
            };

            var entity = new PipingCalculationEntity
            {
                SurfaceLineEntity = surfaceLineEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.IsTrue(collector.ContainsPipingSurfaceLine(surfaceLineEntity));
            CollectionAssert.AreEqual(points, calculation.InputParameters.SurfaceLine.Points);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationInCollector_CalculationHasAlreadyReadHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var entity = new PipingCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
                AssessmentLevel = 5.81,
                UseAssessmentLevelManualInput = Convert.ToByte(false)
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
            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            var entity = new PipingCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1,
                UseAssessmentLevelManualInput = Convert.ToByte(false)
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
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();
            var stochasticSoilModelEntity = new StochasticSoilModelEntity();

            var stochasticSoilProfile = new PipingStochasticSoilProfile(1, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            var stochasticSoilProfileEntity = new PipingStochasticSoilProfileEntity
            {
                StochasticSoilModelEntity = stochasticSoilModelEntity
            };

            var entity = new PipingCalculationEntity
            {
                PipingStochasticSoilProfileEntity = stochasticSoilProfileEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1
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
            var stochasticSoilProfileEntity = new PipingStochasticSoilProfileEntity
            {
                PipingSoilProfileEntity = new PipingSoilProfileEntity
                {
                    Name = "SoilProfile",
                    PipingSoilLayerEntities =
                    {
                        new PipingSoilLayerEntity()
                    }
                }
            };

            var random = new Random(21);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            };
            var stochasticSoilModelEntity = new StochasticSoilModelEntity
            {
                Name = "StochasticSoilModel",
                StochasticSoilModelSegmentPointXml = new Point2DCollectionXmlSerializer().ToXml(geometry),
                PipingStochasticSoilProfileEntities =
                {
                    stochasticSoilProfileEntity
                }
            };
            stochasticSoilProfileEntity.StochasticSoilModelEntity = stochasticSoilModelEntity;

            var entity = new PipingCalculationEntity
            {
                PipingStochasticSoilProfileEntity = stochasticSoilProfileEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1
            };

            var collector = new ReadConversionCollector();

            // Call
            entity.Read(collector, new GeneralPipingInput());

            // Assert
            Assert.IsTrue(collector.Contains(stochasticSoilProfileEntity));
            Assert.IsTrue(collector.ContainsPipingStochasticSoilModel(stochasticSoilModelEntity));
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
                PipingCalculationOutputEntities =
                {
                    new PipingCalculationOutputEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingCalculationScenario calculation = entity.Read(collector, new GeneralPipingInput());

            // Assert
            PipingOutput output = calculation.Output;
            Assert.IsNotNull(output);

            Assert.IsNaN(output.HeaveFactorOfSafety);
            Assert.IsNaN(output.SellmeijerFactorOfSafety);
            Assert.IsNaN(output.UpliftFactorOfSafety);
            Assert.IsNaN(output.UpliftEffectiveStress);
            Assert.IsNaN(output.HeaveGradient);
            Assert.IsNaN(output.SellmeijerCreepCoefficient);
            Assert.IsNaN(output.SellmeijerCriticalFall);
            Assert.IsNaN(output.SellmeijerReducedFall);
        }

        private static void AssertRoundedDouble(double? expectedValue, RoundedDouble actualValue)
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
            double difference = upperLimit - lowerLimit;
            return lowerLimit + random.NextDouble(0, difference);
        }
    }
}