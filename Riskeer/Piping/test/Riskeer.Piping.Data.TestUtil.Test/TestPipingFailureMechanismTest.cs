// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Primitives;

namespace Riskeer.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class TestPipingFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new TestPipingFailureMechanism();

            // Assert
            Assert.IsInstanceOf<PipingFailureMechanism>(failureMechanism);
            Assert.AreEqual(24, failureMechanism.Contribution);
        }

        [Test]
        public void GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels()
        {
            // Call
            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            // Assert
            Assert.AreEqual(24, failureMechanism.Contribution);

            PipingSurfaceLineCollection surfaceLines = failureMechanism.SurfaceLines;
            Assert.AreEqual("path/to/surfaceLines", surfaceLines.SourcePath);
            Assert.AreEqual(1, surfaceLines.Count);
            PipingSurfaceLine[] surfaceLineArray = surfaceLines.ToArray();
            PipingSurfaceLine surfaceLine = surfaceLineArray[0];
            Assert.AreEqual(new Point2D(0.0, 0.0), surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.IsNotEmpty(surfaceLine.LocalGeometry);

            Assert.AreEqual("path/to/stochasticSoilModels", failureMechanism.StochasticSoilModels.SourcePath);
            Assert.AreEqual(1, failureMechanism.StochasticSoilModels.Count);

            IEnumerable<FailureMechanismSection> sections = failureMechanism.Sections;
            FailureMechanismSection[] sectionArray = sections.ToArray();
            Assert.AreEqual(1, sectionArray.Length);

            FailureMechanismSection section = sectionArray[0];
            Assert.AreEqual("Section", section.Name);
            var expectedGeometry = new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            };
            CollectionAssert.AreEqual(expectedGeometry, section.Points);
        }
    }
}