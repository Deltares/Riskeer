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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new GrassCoverErosionInwardsInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            CollectionAssert.IsEmpty(input.DikeGeometry);
            Assert.IsNull(input.HydraulicBoundaryLocation);

            var criticalFlowRate = new LogNormalDistribution(4)
            {
                Mean = new RoundedDouble(4, 0.004),
                StandardDeviation = new RoundedDouble(4, 0.0006)
            };
            Assert.AreEqual(criticalFlowRate.Mean, input.CriticalFlowRate.Mean);
            Assert.AreEqual(criticalFlowRate.StandardDeviation, input.CriticalFlowRate.StandardDeviation);
        }

        [Test]
        public void Properties_ExpectedValues()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();
            var orientation = new RoundedDouble(2, 1.18);
            var logNormal = new LogNormalDistribution(2);
            const bool useForeshore = true;
            var breakWater = new BreakWater(BreakWaterType.Caisson, 2.2);
            const bool useBreakWater = true;
            RoundedDouble dikeHeight = new RoundedDouble(input.DikeHeight.NumberOfDecimalPlaces, 1.1);

            // Call
            input.Orientation = orientation;
            input.CriticalFlowRate = logNormal;
            input.DikeHeight = dikeHeight;
            input.UseForeshore = useForeshore;
            input.BreakWater.Type = breakWater.Type;
            input.BreakWater.Height = breakWater.Height;
            input.UseBreakWater = useBreakWater;

            // Assert
            Assert.AreEqual(dikeHeight, input.DikeHeight);
            Assert.AreEqual(orientation, input.Orientation);
            Assert.AreEqual(logNormal.Mean, input.CriticalFlowRate.Mean);
            Assert.AreEqual(logNormal.StandardDeviation, input.CriticalFlowRate.StandardDeviation);
            Assert.AreEqual(useForeshore, input.UseForeshore);
            Assert.AreEqual(breakWater.Type, input.BreakWater.Type);
            Assert.AreEqual(breakWater.Height, input.BreakWater.Height);
            Assert.AreEqual(useBreakWater, input.UseBreakWater);
            CollectionAssert.IsEmpty(input.DikeGeometry);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
        }

        [Test]
        public void SetDikeGeometry_NullRoughnessProfileSections_ThrowsArgumentNullException()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            // Call
            TestDelegate test = () => input.SetDikeGeometry(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void SetDikeGeometry_ValidGeometry_ReturnsExpectedValues()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();
            var sections = new[]
            {
                new RoughnessProfileSection(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4), 1.1),
                new RoughnessProfileSection(new Point2D(3.3, 4.4), new Point2D(5.5, 6.6), 2.2)
            };

            // Call
            input.SetDikeGeometry(sections);

            // Assert
            Assert.AreEqual(sections, input.DikeGeometry);
        }
    }
}