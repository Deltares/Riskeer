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
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class DikeHeightAssessmentOutputCreateExtensionsTest
    {
        [Test]
        public void CreateDikeHeight_OutputNull_ThrowArgumentNullException()
        {
            // Setup
            HydraulicLoadsOutput output = null;

            // Call
            TestDelegate test = () => output.CreateDikeHeight();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateDikeHeight_WithValidParameters_ReturnsGrassCoverErosionInwardsDikeHeightOutputEntityWithOutputSet()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicLoadsOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), random.NextEnumValue<CalculationConvergence>());

            // Call
            GrassCoverErosionInwardsDikeHeightOutputEntity entity = output.CreateDikeHeight();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(output.Result, entity.DikeHeight, output.Result.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }

        [Test]
        public void CreateDikeHeight_WithNaNParameters_ReturnsGrassCoverErosionInwardsDikeHeightOutputEntityWithOutputNull()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicLoadsOutput(double.NaN, double.NaN, double.NaN,
                                                  double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>());

            // Call
            GrassCoverErosionInwardsDikeHeightOutputEntity entity = output.CreateDikeHeight();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.DikeHeight);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }

        [Test]
        public void CreateOvertoppingRate_OutputNull_ThrowArgumentNullException()
        {
            // Setup
            HydraulicLoadsOutput output = null;

            // Call
            TestDelegate test = () => output.CreateOvertoppingRate();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateOvertoppingRate_WithValidParameters_ReturnsGrassCoverErosionInwardsOvertoppingRateOutputEntityWithOutputSet()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicLoadsOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), random.NextEnumValue<CalculationConvergence>());

            // Call
            GrassCoverErosionInwardsOvertoppingRateOutputEntity entity = output.CreateOvertoppingRate();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(output.Result, entity.OvertoppingRate, output.Result.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }

        [Test]
        public void CreateOvertoppingRate_WithNaNParameters_ReturnsGrassCoverErosionInwardsOvertoppingRateOutputEntityWithOutputNull()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicLoadsOutput(double.NaN, double.NaN, double.NaN,
                                                  double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>());

            // Call
            GrassCoverErosionInwardsOvertoppingRateOutputEntity entity = output.CreateOvertoppingRate();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.OvertoppingRate);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }
    }
}