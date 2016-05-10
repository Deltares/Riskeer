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
        public void Constructor_NullGeneralInput_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => new GrassCoverErosionInwardsInput(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();

            // Call
            var input = new GrassCoverErosionInwardsInput(generalInput);

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            CollectionAssert.IsEmpty(input.DikeGeometry);
            Assert.IsNull(input.HydraulicBoundaryLocation);

            Assert.AreEqual(generalInput.FbFactor, input.FbFactor);
            Assert.AreEqual(generalInput.FnFactor, input.FnFactor);
            Assert.AreEqual(generalInput.FshallowModelFactor, input.FshallowModelFactor);
            Assert.AreEqual(generalInput.FrunupModelFactor, input.FrunupModelFactor);
            Assert.AreEqual(generalInput.CriticalOvertoppingModelFactor, input.CriticalOvertoppingModelFactor);
            Assert.AreEqual(generalInput.OvertoppingModelFactor, input.OvertoppingModelFactor);
        }

        [Test]
        public void Properties_ExpectedValues()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var input = new GrassCoverErosionInwardsInput(generalInput);
            var orientation = new RoundedDouble(2, 1.18);
            var logNormal = new LognormalDistribution(2);
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
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var input = new GrassCoverErosionInwardsInput(generalInput);

            // Call
            TestDelegate test = () => input.SetDikeGeometry(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void SetForeshoreGeometry_NullProfileSections_ThrowsArgumentNullException()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var input = new GrassCoverErosionInwardsInput(generalInput);

            // Call
            TestDelegate test = () => input.SetForeshoreGeometry(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void SetDikeGeometry_ValidGeometry_ReturnsExpectedValues()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var input = new GrassCoverErosionInwardsInput(generalInput);
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

        [Test]
        public void SetForeshoreGeometry_ValidGeometry_ReturnsExpectedValues()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var input = new GrassCoverErosionInwardsInput(generalInput);
            var sections = new[]
            {
                new ProfileSection(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4)),
                new ProfileSection(new Point2D(3.3, 4.4), new Point2D(5.5, 6.6))
            };

            // Call
            input.SetForeshoreGeometry(sections);

            // Assert
            Assert.AreEqual(sections, input.ForeshoreGeometry);
        }
    }
}