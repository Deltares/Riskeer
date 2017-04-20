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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class HydraulicBoundaryLocationOutputCreateExtensionsTest
    {
        [Test]
        public void CreateHydraulicLocationOutputEntity_WithValidParameters_ReturnsHydraulicLocationEntityWithOutputSet()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicBoundaryLocationOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), random.NextEnumValue<CalculationConvergence>());

            var outputType = random.NextEnumValue<HydraulicLocationOutputType>();

            // Call
            var entity = output.Create<HydraulicLocationOutputEntity>(outputType);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((byte) outputType, entity.HydraulicLocationOutputType);
            Assert.AreEqual(output.Result, entity.Result, output.Result.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }

        [Test]
        public void CreateHydraulicLocationOutputEntity_WithNaNParameters_ReturnsHydraulicLocationEntityWithOutputNaN()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN, double.NaN,
                                                             double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>());

            var outputType = random.NextEnumValue<HydraulicLocationOutputType>();

            // Call
            var entity = output.Create<HydraulicLocationOutputEntity>(outputType);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((byte) outputType, entity.HydraulicLocationOutputType);
            Assert.IsNull(entity.Result);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }

        [Test]
        public void CreateGrassCoverErosionOutwardsHydraulicLocationOutputEntity_WithValidParameters_ReturnsHydraulicLocationEntityWithOutputSet()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicBoundaryLocationOutput(
                random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextDouble(), random.NextEnumValue<CalculationConvergence>());

            var outputType = random.NextEnumValue<HydraulicLocationOutputType>();

            // Call
            var entity = output.Create<GrassCoverErosionOutwardsHydraulicLocationOutputEntity>(outputType);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((byte) outputType, entity.HydraulicLocationOutputType);
            Assert.AreEqual(output.Result, entity.Result, output.Result.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }

        [Test]
        public void CreateGrassCoverErosionOutwardsHydraulicLocationOutputEntity_WithNaNParameters_ReturnsHydraulicLocationEntityWithOutputNaN()
        {
            // Setup
            var random = new Random(21);
            var output = new HydraulicBoundaryLocationOutput(double.NaN, double.NaN, double.NaN,
                                                             double.NaN, double.NaN, random.NextEnumValue<CalculationConvergence>());

            var outputType = random.NextEnumValue<HydraulicLocationOutputType>();

            // Call
            var entity = output.Create<GrassCoverErosionOutwardsHydraulicLocationOutputEntity>(
                outputType);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((byte) outputType, entity.HydraulicLocationOutputType);
            Assert.IsNull(entity.Result);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }
    }
}