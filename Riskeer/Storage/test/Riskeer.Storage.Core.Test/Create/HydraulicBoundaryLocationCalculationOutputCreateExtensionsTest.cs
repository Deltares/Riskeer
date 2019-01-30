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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationOutputCreateExtensionsTest
    {
        [Test]
        public void CreateHydraulicLocationOutputEntity_HydraulicBoundaryLocationCalculationOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((HydraulicBoundaryLocationCalculationOutput) null).CreateHydraulicLocationOutputEntity();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateHydraulicLocationOutputEntity_WithValidParameters_ReturnsHydraulicLocationEntityWithOutputSet()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicBoundaryLocationCalculationOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), random.NextEnumValue<CalculationConvergence>(), null);

            // Call
            HydraulicLocationOutputEntity entity = output.CreateHydraulicLocationOutputEntity();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(output.Result, entity.Result, output.Result.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);

            AssertGeneralResult(output.GeneralResult, entity.GeneralResultSubMechanismIllustrationPointEntity);
        }

        [Test]
        public void CreateHydraulicLocationOutputEntity_WithNaNParameters_ReturnsHydraulicLocationEntityWithOutputNaN()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicBoundaryLocationCalculationOutput(double.NaN, double.NaN, double.NaN,
                                                                        double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>(), null);

            // Call
            HydraulicLocationOutputEntity entity = output.CreateHydraulicLocationOutputEntity();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.Result);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);

            AssertGeneralResult(output.GeneralResult, entity.GeneralResultSubMechanismIllustrationPointEntity);
        }

        [Test]
        public void CreateHydraulicLocationOutputEntity_WithGeneralResult_ReturnsHydraulicLocationEntityWithGeneralResult()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicBoundaryLocationCalculationOutput(double.NaN, double.NaN, double.NaN,
                                                                        double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>(),
                                                                        GetGeneralResult());

            // Call
            HydraulicLocationOutputEntity entity = output.CreateHydraulicLocationOutputEntity();

            // Assert
            AssertGeneralResult(output.GeneralResult, entity.GeneralResultSubMechanismIllustrationPointEntity);
        }

        private static GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResult()
        {
            var random = new Random(55);
            var generalResult = new GeneralResult<TopLevelSubMechanismIllustrationPoint>(
                WindDirectionTestFactory.CreateTestWindDirection(),
                new[]
                {
                    new Stochast("stochastOne", random.NextDouble(), random.NextDouble())
                },
                new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                              "IllustrationPointTwo",
                                                              new TestSubMechanismIllustrationPoint())
                });

            return generalResult;
        }

        private static void AssertGeneralResult(GeneralResult<TopLevelSubMechanismIllustrationPoint> illustrationPoint,
                                                GeneralResultSubMechanismIllustrationPointEntity entity)
        {
            if (illustrationPoint == null)
            {
                Assert.IsNull(entity);
                return;
            }

            Assert.IsNotNull(entity);
            WindDirection governingWindDirection = illustrationPoint.GoverningWindDirection;
            TestHelper.AssertAreEqualButNotSame(governingWindDirection.Name, entity.GoverningWindDirectionName);
            Assert.AreEqual(governingWindDirection.Angle, entity.GoverningWindDirectionAngle,
                            governingWindDirection.Angle.GetAccuracy());

            Assert.AreEqual(illustrationPoint.Stochasts.Count(), entity.StochastEntities.Count);
            Assert.AreEqual(illustrationPoint.TopLevelIllustrationPoints.Count(),
                            entity.TopLevelSubMechanismIllustrationPointEntities.Count);
        }
    }
}