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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.SoilProfile;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayer2DLoopTest
    {
        [Test]
        public void Constructor_SegmentsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SoilLayer2DLoop(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("segments", paramName);
        }

        [Test]
        public void Constructor_ConnectedSegments_SetsSegments()
        {
            // Setup
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);

            var arrayWithConnectedSegments = new[]
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointA)
            };

            // Call
            var loop = new SoilLayer2DLoop(arrayWithConnectedSegments);

            // Assert
            Assert.AreSame(arrayWithConnectedSegments, loop.Segments);
        }

        [Test]
        public void Constructor_DisconnectedSegment_ThrowsArgumentException()
        {
            // Setup
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(0.0, 1.0e-20);

            var arrayWithDisconnectedSegment = new[]
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC)
            };

            // Call
            TestDelegate test = () => { new SoilLayer2DLoop(arrayWithDisconnectedSegment); };

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public void Constructor_InversedSegment_ThrowsArgumentException()
        {
            // Setup
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(2.0, 0.0);
            var pointD = new Point2D(0.0, 0.0);

            var arrayWithInversedSegment = new[]
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC),
                new Segment2D(pointD, pointC),
                new Segment2D(pointD, pointA)
            };

            // Call
            TestDelegate test = () => { new SoilLayer2DLoop(arrayWithInversedSegment); };

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [TestFixture]
        private class SoilLayer2DLoopEqualsTest : EqualsTestFixture<SoilLayer2DLoop, DerivedSoilLayer2DLoop>
        {
            protected override SoilLayer2DLoop CreateObject()
            {
                return CreateValidLoop(1.0);
            }

            protected override DerivedSoilLayer2DLoop CreateDerivedObject()
            {
                return new DerivedSoilLayer2DLoop(CreateValidLoop(1.0));
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(CreateValidLoop(2.0))
                    .SetName("Segment");
            }

            private static SoilLayer2DLoop CreateValidLoop(double x)
            {
                var pointA = new Point2D(0.0, 0.0);
                var pointB = new Point2D(x, 0.0);

                var segments = new[]
                {
                    new Segment2D(pointA, pointB),
                    new Segment2D(pointB, pointA)
                };

                return new SoilLayer2DLoop(segments);
            }
        }

        private class DerivedSoilLayer2DLoop : SoilLayer2DLoop
        {
            public DerivedSoilLayer2DLoop(SoilLayer2DLoop loop) : base(loop.Segments) {}
        }
    }
}