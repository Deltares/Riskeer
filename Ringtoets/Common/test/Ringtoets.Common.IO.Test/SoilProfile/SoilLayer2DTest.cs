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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayer2DTest
    {
        [Test]
        public void DefaultConstructor_ExpectedPropertiesSet()
        {
            // Call
            var layer = new SoilLayer2D();

            // Assert
            Assert.IsInstanceOf<SoilLayerBase>(layer);
        }

        [Test]
        public void OuterLoop_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var layer = new SoilLayer2D();

            // Call
            TestDelegate test = () => layer.OuterLoop = null;

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void OuterLoop_DisconnectedSegment_ThrowsArgumentException()
        {
            // Setup
            var layer = new SoilLayer2D();

            // Call
            TestDelegate test = () => layer.OuterLoop = CreateLoopWithDisconnectedSegment();

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public void OuterLoop_InversedSegment_ThrowsArgumentException()
        {
            // Setup
            var layer = new SoilLayer2D();

            // Call
            TestDelegate test = () => layer.OuterLoop = CreateLoopWithInversedSegment();

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public void OuterLoop_ConnectedSegments_SetsNewLoop()
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);

            // Call
            layer.OuterLoop = new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointA)
            };

            // Assert
            Assert.NotNull(layer.OuterLoop);
            CollectionAssert.AreEqual(new []
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointA)
            }, layer.OuterLoop); 
        }

        [Test]
        public void AddInnerLoop_DisconnectedSegment_ThrowsArgumentException()
        {
            // Setup
            var layer = new SoilLayer2D();

            // Call
            TestDelegate test = () => layer.AddInnerLoop(CreateLoopWithDisconnectedSegment());

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public void AddInnerLoop_InversedSegment_ThrowsArgumentException()
        {
            // Setup
            var layer = new SoilLayer2D();

            // Call
            TestDelegate test = () => layer.AddInnerLoop(CreateLoopWithInversedSegment());

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public void AddInnerLoop_ConnectedSegments_SetsNewLoop()
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);

            // Call
            layer.AddInnerLoop(new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointA)
            });

            // Assert
            Assert.AreEqual(1, layer.InnerLoops.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointA)
            }, layer.InnerLoops.ElementAt(0));
        }

        private static IEnumerable<Segment2D> CreateLoopWithDisconnectedSegment()
        {
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(0.0, 1.0e-20);

            return new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC)
            };
        }

        private static IEnumerable<Segment2D> CreateLoopWithInversedSegment()
        {
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(2.0, 0.0);
            var pointD = new Point2D(0.0, 0.0);

            return new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC),
                new Segment2D(pointD, pointC),
                new Segment2D(pointD, pointA)
            };
        }
    }
}