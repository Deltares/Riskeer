﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class DikeHeightOutputCreateExtensionsTest
    {
        [Test]
        public void Create_OutputNull_ThrowArgumentNullException()
        {
            // Setup
            DikeHeightOutput output = null;

            // Call
            TestDelegate test = () => output.Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Create_WithValidParameters_ReturnsGrassCoverErosionInwardsDikeHeightOutputEntityWithOutputSet()
        {
            // Setup
            var random = new Random(21);
            var output = new DikeHeightOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), random.NextEnumValue<CalculationConvergence>(), null);

            // Call
            GrassCoverErosionInwardsDikeHeightOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(output.DikeHeight, entity.DikeHeight, output.DikeHeight.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
            AssertGeneralResult(output.GeneralResult, entity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        [Test]
        public void Create_WithNaNParameters_ReturnsGrassCoverErosionInwardsDikeHeightOutputEntityWithOutputNull()
        {
            // Setup
            var random = new Random(21);
            var output = new DikeHeightOutput(double.NaN, double.NaN, double.NaN,
                                              double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>(), null);

            // Call
            GrassCoverErosionInwardsDikeHeightOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.DikeHeight);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
            AssertGeneralResult(output.GeneralResult, entity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        [Test]
        public void Create_WithGeneralResult_ReturnsGrassCoverErosionInwardsDikeHeightOutputEntityWithGeneralResultEntity()
        {
            // Setup
            var random = new Random(21);
            var output = new DikeHeightOutput(double.NaN, double.NaN, double.NaN,
                                              double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>(),
                                              new TestGeneralResultFaultTreeIllustrationPoint());

            // Call
            GrassCoverErosionInwardsDikeHeightOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.DikeHeight);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte)output.CalculationConvergence, entity.CalculationConvergence);
            AssertGeneralResult(output.GeneralResult, entity.GeneralResultFaultTreeIllustrationPointEntity);
        }

        private static void AssertGeneralResult(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult,
                                                GeneralResultFaultTreeIllustrationPointEntity entity)
        {
            if (generalResult == null)
            {
                Assert.IsNull(entity);
                return;
            }

            Assert.IsNotNull(entity);
            WindDirection governingWindDirection = generalResult.GoverningWindDirection;
            TestHelper.AssertAreEqualButNotSame(governingWindDirection.Name, entity.GoverningWindDirectionName);
            Assert.AreEqual(governingWindDirection.Angle, entity.GoverningWindDirectionAngle,
                            governingWindDirection.Angle.GetAccuracy());

            Assert.AreEqual(generalResult.Stochasts.Count(), entity.StochastEntities.Count);
            Assert.AreEqual(generalResult.TopLevelIllustrationPoints.Count(),
                            entity.TopLevelFaultTreeIllustrationPointEntities.Count);
        }
    }
}