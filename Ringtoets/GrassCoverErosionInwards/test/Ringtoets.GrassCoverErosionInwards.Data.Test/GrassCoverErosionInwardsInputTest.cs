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
using System.Collections.Generic;
using System.Linq;
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
            // Setup & Call
            var input = new GrassCoverErosionInwardsInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            CollectionAssert.IsEmpty(input.DikeGeometry);
            Assert.IsNull(input.HydraulicBoundaryLocation);
        }

        [Test]
        public void Properties_ExpectedValues()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();
            var orientation = new RoundedDouble(2, 1.18);
            var logNormal = new LognormalDistribution(2);
            const bool foreshorePresent = true;
            var breakWater = new List<BreakWater>
            {
                new BreakWater(BreakWaterType.Caisson, 2.2)
            };
            const bool breakWaterPresent = true;
            const double dikeHeight = 1.1;

            // Call
            input.Orientation = orientation;
            input.CriticalFlowRate = logNormal;
            input.DikeHeight = dikeHeight;
            input.ForeshorePresent = foreshorePresent;
            input.BreakWater = breakWater;
            input.BreakWaterPresent = breakWaterPresent;

            // Assert
            Assert.AreEqual(dikeHeight, input.DikeHeight);
            Assert.AreEqual(orientation, input.Orientation);
            Assert.AreEqual(logNormal, input.CriticalFlowRate);
            Assert.AreEqual(foreshorePresent, input.ForeshorePresent);
            Assert.AreEqual(breakWater, input.BreakWater);
            Assert.AreEqual(breakWaterPresent, input.BreakWaterPresent);
        }

        [Test]
        public void SetGeometry_NullRoughnessProfileSections_ThrowsArgumentNullException()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            // Call
            TestDelegate test = () => input.SetGeometry(null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void SetGeometry_ValidRoughnessProfileSections_ReturnsExpectedValues(int foreshoreDikeGeometryPoints)
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();
            var foreshoreSection = new RoughnessProfileSection(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4), 1.1);
            var dikeSection = new RoughnessProfileSection(new Point2D(3.3, 4.4), new Point2D(5.5, 6.6), 2.2);

            // Call
            input.SetGeometry(new[]
            {
                foreshoreSection,
                dikeSection
            }, foreshoreDikeGeometryPoints);

            // Assert
            Assert.AreEqual(foreshoreSection, input.ForeshoreGeometry.Single());
            Assert.AreEqual(dikeSection, input.DikeGeometry.Single());
            Assert.AreEqual(foreshoreDikeGeometryPoints, input.ForeshoreDikeGeometryPoints);
        }
    }
}