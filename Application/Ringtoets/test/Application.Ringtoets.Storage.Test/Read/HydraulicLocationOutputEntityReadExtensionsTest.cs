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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class HydraulicLocationOutputEntityReadExtensionsTest
    {
        [Test]
        [Combinatorial]
        public void Read_ValidParameters_ReturnsHydraulicBoundaryLocationOutput(
            [Values(HydraulicLocationOutputType.DesignWaterLevel, HydraulicLocationOutputType.WaveHeight)] HydraulicLocationOutputType outputType,
            [Values(CalculationConvergence.CalculatedNotConverged, CalculationConvergence.CalculatedConverged,
                CalculationConvergence.NotCalculated)] CalculationConvergence convergence)
        {
            // Setup
            var random = new Random(22);
            double result = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var entity = new TestHydraulicLocationOutputEntity
            {
                HydraulicLocationOutputType = (byte) outputType,
                Result = result,
                TargetProbability = targetProbability,
                TargetReliability = targetReliability,
                CalculatedProbability = calculatedProbability,
                CalculatedReliability = calculatedReliability,
                CalculationConvergence = (byte) convergence
            };

            // Call
            HydraulicBoundaryLocationOutput output = entity.Read();

            // Assert
            Assert.AreEqual((RoundedDouble) result, output.Result, output.Result.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual((RoundedDouble) targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual((RoundedDouble) calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void Read_NaNParameters_ReturnsHydraulicBoundaryLocationOutputWithNaN(
        [Values(CalculationConvergence.CalculatedNotConverged, CalculationConvergence.CalculatedConverged,
            CalculationConvergence.NotCalculated)] CalculationConvergence convergence)
        {
            // Setup
            var entity = new TestHydraulicLocationOutputEntity
            {
                Result = double.NaN,
                TargetProbability = double.NaN,
                TargetReliability = double.NaN,
                CalculatedProbability = double.NaN,
                CalculatedReliability = double.NaN,
                CalculationConvergence = (byte) convergence
            };

            // Call
            HydraulicBoundaryLocationOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.Result);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(convergence, output.CalculationConvergence);
            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void Read_EntityWithIllustrationPoints_ReturnsHydraulicBoundarLocationOutputWithGeneralResult()
        {
            // Setup
            var random = new Random(21);

            var stochastEntities = new[]
            {
                new StochastEntity
                {
                    Name = "stochastEntityOne",
                    Duration = random.NextDouble(),
                    Alpha = random.NextDouble(),
                    Order = 0
                }
            };
            var topLevelIllustrationPointEntities = new[]
            {
                new TopLevelSubMechanismIllustrationPointEntity
                {
                    WindDirectionName = "WindDirectionTwo",
                    WindDirectionAngle = random.NextDouble(),
                    ClosingSituation = "ClosingSituationTwo",
                    SubMechanismIllustrationPointEntity = new SubMechanismIllustrationPointEntity
                    {
                        Beta = random.NextDouble(),
                        Name = "IllustrationPointTwo"
                    },
                    Order = 0
                }
            };

            var entity = new TestHydraulicLocationOutputEntity
            {
                Result = double.NaN,
                TargetProbability = double.NaN,
                TargetReliability = double.NaN,
                CalculatedProbability = double.NaN,
                CalculatedReliability = double.NaN,
                CalculationConvergence = (byte) random.NextEnumValue<CalculationConvergence>(),
                GeneralResultSubMechanismIllustrationPointEntity = new GeneralResultSubMechanismIllustrationPointEntity
                {
                    GoverningWindDirectionName = "SSE",
                    GoverningWindDirectionAngle = random.NextDouble(),
                    StochastEntities = stochastEntities,
                    TopLevelSubMechanismIllustrationPointEntities = topLevelIllustrationPointEntities
                }
            };

            // Call
            HydraulicBoundaryLocationOutput output = entity.Read();

            // Assert
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = output.GeneralResult;
            GeneralResultSubMechanismIllustrationPointEntity generalResultEntity =
                entity.GeneralResultSubMechanismIllustrationPointEntity;

            AssertWindDirection(generalResultEntity, generalResult.GoverningWindDirection);
            AssertStochasts(generalResultEntity.StochastEntities.ToArray(), generalResult.Stochasts.ToArray());
            AssertIllustrationPoints(generalResultEntity.TopLevelSubMechanismIllustrationPointEntities.ToArray(),
                                     generalResult.TopLevelIllustrationPoints.ToArray());
        }

        private static void AssertIllustrationPoints(
            IList<TopLevelSubMechanismIllustrationPointEntity> entities,
            IList<TopLevelSubMechanismIllustrationPoint> illustrationPoints)
        {
            Assert.AreEqual(entities.Count, illustrationPoints.Count);
            for (var i = 0; i < entities.Count; i++)
            {
                AssertTopLevelSubMechanismIllustrationPoint(entities[i], illustrationPoints[i]);
            }
        }

        private static void AssertTopLevelSubMechanismIllustrationPoint(
            TopLevelSubMechanismIllustrationPointEntity illustrationPointEntity,
            TopLevelSubMechanismIllustrationPoint readTopLevelSubMechanismIllustrationPoint)
        {
            Assert.AreEqual(illustrationPointEntity.ClosingSituation, readTopLevelSubMechanismIllustrationPoint.ClosingSituation);

            WindDirection actualWindDirection = readTopLevelSubMechanismIllustrationPoint.WindDirection;
            Assert.AreEqual(illustrationPointEntity.WindDirectionName, actualWindDirection.Name);
            Assert.AreEqual(illustrationPointEntity.WindDirectionAngle, actualWindDirection.Angle, actualWindDirection.Angle);

            Assert.IsNotNull(readTopLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint);
        }

        private static void AssertStochasts(IList<StochastEntity> entities, IList<Stochast> stochasts)
        {
            Assert.AreEqual(entities.Count, stochasts.Count);
            for (var i = 0; i < entities.Count; i++)
            {
                AssertStochast(entities[i], stochasts[i]);
            }
        }

        private static void AssertStochast(StochastEntity stochastEntity,
                                           Stochast readStochast)
        {
            Assert.AreEqual(stochastEntity.Name, readStochast.Name);
            Assert.AreEqual(stochastEntity.Alpha, readStochast.Alpha, readStochast.Alpha.GetAccuracy());
            Assert.AreEqual(stochastEntity.Duration, readStochast.Duration, readStochast.Duration.GetAccuracy());
        }

        private static void AssertWindDirection(IGeneralResultEntity entity, WindDirection windDirection)
        {
            Assert.AreEqual(entity.GoverningWindDirectionName, windDirection.Name);
            Assert.AreEqual(entity.GoverningWindDirectionAngle, windDirection.Angle,
                            windDirection.Angle.GetAccuracy());
        }

        private class TestHydraulicLocationOutputEntity : IHydraulicLocationOutputEntity
        {
            public double? Result { get; set; }
            public double? TargetProbability { get; set; }
            public double? TargetReliability { get; set; }
            public double? CalculatedProbability { get; set; }
            public double? CalculatedReliability { get; set; }
            public byte CalculationConvergence { get; set; }
            public byte HydraulicLocationOutputType { get; set; }
            public GeneralResultSubMechanismIllustrationPointEntity GeneralResultSubMechanismIllustrationPointEntity { get; set; }
        }
    }
}