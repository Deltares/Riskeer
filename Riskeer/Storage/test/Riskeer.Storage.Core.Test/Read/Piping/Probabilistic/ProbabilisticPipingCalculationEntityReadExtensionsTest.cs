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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.Piping.Probabilistic;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil.Hydraulics;

namespace Riskeer.Storage.Core.Test.Read.Piping.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new ProbabilisticPipingCalculationEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        [TestCase(true, 0.98, "haha", "hihi", 0.0, 3.4, 123, true, true)]
        [TestCase(false, 0.0, null, null, double.NaN, double.NaN, 321, false, false)]
        public void Read_ValidEntity_ReturnProbabilisticPipingCalculationScenario(
            bool isRelevant, double contribution,
            string name, string comments, double entryPoint, double exitPoint,
            int seed, bool calculateProfileIllustrationPoints, bool calculateSectionIllustrationPoints)
        {
            // Setup
            var random = new Random(seed);

            var entity = new ProbabilisticPipingCalculationEntity
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
                ShouldProfileSpecificIllustrationPointsBeCalculated = Convert.ToByte(calculateProfileIllustrationPoints),
                ShouldSectionSpecificIllustrationPointsBeCalculated = Convert.ToByte(calculateSectionIllustrationPoints)
            };

            var collector = new ReadConversionCollector();

            // Call
            ProbabilisticPipingCalculationScenario calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, calculation.IsRelevant);
            Assert.AreEqual(contribution, calculation.Contribution, 1e-6);
            Assert.AreEqual(name, calculation.Name);
            Assert.AreEqual(comments, calculation.Comments.Body);

            RoundedDoubleTestHelper.AssertRoundedDouble(entryPoint, calculation.InputParameters.EntryPointL);
            RoundedDoubleTestHelper.AssertRoundedDouble(exitPoint, calculation.InputParameters.ExitPointL);
            RoundedDoubleTestHelper.AssertRoundedDouble(entity.PhreaticLevelExitMean, calculation.InputParameters.PhreaticLevelExit.Mean);
            RoundedDoubleTestHelper.AssertRoundedDouble(entity.PhreaticLevelExitStandardDeviation, calculation.InputParameters.PhreaticLevelExit.StandardDeviation);
            RoundedDoubleTestHelper.AssertRoundedDouble(entity.DampingFactorExitMean, calculation.InputParameters.DampingFactorExit.Mean);
            RoundedDoubleTestHelper.AssertRoundedDouble(entity.DampingFactorExitStandardDeviation, calculation.InputParameters.DampingFactorExit.StandardDeviation);

            Assert.AreEqual(calculateProfileIllustrationPoints, calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated);
            Assert.AreEqual(calculateSectionIllustrationPoints, calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated);

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
            var entity = new ProbabilisticPipingCalculationEntity
            {
                SurfaceLineEntity = surfaceLineEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1
            };

            var collector = new ReadConversionCollector();
            collector.Read(surfaceLineEntity, surfaceLine);

            // Call
            ProbabilisticPipingCalculationScenario calculation = entity.Read(collector);

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

            var entity = new ProbabilisticPipingCalculationEntity
            {
                SurfaceLineEntity = surfaceLineEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1
            };

            var collector = new ReadConversionCollector();

            // Call
            ProbabilisticPipingCalculationScenario calculation = entity.Read(collector);

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
            var entity = new ProbabilisticPipingCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            ProbabilisticPipingCalculationScenario calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationNotYetInCollector_CalculationWithCreatedHydraulicBoundaryLocationAndRegisteredNewEntities()
        {
            // Setup
            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            var entity = new ProbabilisticPipingCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1
            };

            var collector = new ReadConversionCollector();

            // Call
            entity.Read(collector);

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

            var entity = new ProbabilisticPipingCalculationEntity
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
            ProbabilisticPipingCalculationScenario calculation = entity.Read(collector);

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

            var entity = new ProbabilisticPipingCalculationEntity
            {
                PipingStochasticSoilProfileEntity = stochasticSoilProfileEntity,
                EntryPointL = 1,
                ExitPointL = 2,
                DampingFactorExitMean = 1
            };

            var collector = new ReadConversionCollector();

            // Call
            entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(stochasticSoilProfileEntity));
            Assert.IsTrue(collector.ContainsPipingStochasticSoilModel(stochasticSoilModelEntity));
        }

        [Test]
        public void Read_EntityWithProbabilisticPipingCalculationOutputEntity_CalculationWithProbabilisticPipingOutput()
        {
            // Setup
            var entity = new ProbabilisticPipingCalculationEntity
            {
                ProbabilisticPipingCalculationOutputEntities =
                {
                    new ProbabilisticPipingCalculationOutputEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            ProbabilisticPipingCalculationScenario calculation = entity.Read(collector);

            // Assert
            ProbabilisticPipingOutput output = calculation.Output;
            Assert.IsNotNull(output);

            Assert.IsNaN(output.ProfileSpecificOutput.Reliability);
            Assert.IsNull(output.ProfileSpecificOutput.GeneralResult);
            Assert.IsNaN(output.SectionSpecificOutput.Reliability);
            Assert.IsNull(output.SectionSpecificOutput.GeneralResult);
        }

        private static double? GetRandomNullableDoubleInRange(Random random, double lowerLimit, double upperLimit)
        {
            double difference = upperLimit - lowerLimit;
            return lowerLimit + random.NextDouble(0, difference);
        }
    }
}