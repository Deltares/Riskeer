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
using System.Drawing;
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
        public void DefaultConstructor_NotInstantiatedOuterLoopAndEmptyInnerLoops()
        {
            // Call
            var layer = new SoilLayer2D();

            // Assert
            Assert.IsInstanceOf<SoilLayerBase>(layer);
            Assert.IsFalse(layer.IsAquifer);
            Assert.IsEmpty(layer.MaterialName);
            Assert.AreEqual(Color.Empty, layer.Color);
            Assert.IsNull(layer.OuterLoop);
            CollectionAssert.IsEmpty(layer.InnerLoops);

            Assert.IsNull(layer.BelowPhreaticLevelDistribution);
            Assert.IsNaN(layer.BelowPhreaticLevelShift);
            Assert.IsNaN(layer.BelowPhreaticLevelMean);
            Assert.IsNaN(layer.BelowPhreaticLevelDeviation);

            Assert.IsNull(layer.DiameterD70Distribution);
            Assert.IsNaN(layer.DiameterD70Shift);
            Assert.IsNaN(layer.DiameterD70Mean);
            Assert.IsNaN(layer.DiameterD70CoefficientOfVariation);

            Assert.IsNull(layer.PermeabilityDistribution);
            Assert.IsNaN(layer.PermeabilityShift);
            Assert.IsNaN(layer.PermeabilityMean);
            Assert.IsNaN(layer.PermeabilityCoefficientOfVariation);
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
        [TestCase(1e-6)]
        [TestCase(1)]
        public void OuterLoop_TwoDisconnectedSegment_ThrowsArgumentException(double diff)
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(0.0, diff);

            // Call
            TestDelegate test = () => layer.OuterLoop = new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC)
            };

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void OuterLoop_ThreeDisconnectedSegment_ThrowsArgumentException(double diff)
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);
            var pointD = new Point2D(0.0, diff);

            // Call
            TestDelegate test = () => layer.OuterLoop = new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC),
                new Segment2D(pointC, pointD)
            };

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public void OuterLoop_TwoConnectedSegment_SetsNewLoop()
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
            Assert.AreEqual(new Segment2D(pointA, pointB), layer.OuterLoop.ElementAt(0));
            Assert.AreEqual(new Segment2D(pointB, pointA), layer.OuterLoop.ElementAt(1));
        }

        [Test]
        public void OuterLoop_ThreeConnectedSegment_SetsNewLoop()
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);

            // Call
            layer.OuterLoop = new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC),
                new Segment2D(pointC, pointA)
            };

            // Assert
            Assert.NotNull(layer.OuterLoop);
            Assert.AreEqual(new Segment2D(pointA, pointB), layer.OuterLoop.ElementAt(0));
            Assert.AreEqual(new Segment2D(pointB, pointC), layer.OuterLoop.ElementAt(1));
            Assert.AreEqual(new Segment2D(pointC, pointA), layer.OuterLoop.ElementAt(2));
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void AddInnerLoop_TwoDisconnectedSegment_ThrowsArgumentException(double diff)
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(0.0, diff);

            // Call
            TestDelegate test = () => layer.AddInnerLoop(new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC)
            });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void AddInnerLoop_ThreeDisconnectedSegment_ThrowsArgumentException(double diff)
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);
            var pointD = new Point2D(0.0, diff);

            // Call
            TestDelegate test = () => layer.AddInnerLoop(new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC),
                new Segment2D(pointC, pointD)
            });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public void AddInnerLoop_TwoConnectedSegment_SetsNewLoop()
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
            Assert.AreEqual(new Segment2D(pointA, pointB), layer.InnerLoops.ElementAt(0)[0]);
            Assert.AreEqual(new Segment2D(pointB, pointA), layer.InnerLoops.ElementAt(0)[1]);
        }

        [Test]
        public void AddInnerLoop_ThreeConnectedSegment_SetsNewLoop()
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);

            // Call
            layer.AddInnerLoop(new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC),
                new Segment2D(pointC, pointA)
            });

            // Assert
            Assert.AreEqual(1, layer.InnerLoops.Count());
            Assert.AreEqual(new Segment2D(pointA, pointB), layer.InnerLoops.ElementAt(0)[0]);
            Assert.AreEqual(new Segment2D(pointB, pointC), layer.InnerLoops.ElementAt(0)[1]);
            Assert.AreEqual(new Segment2D(pointC, pointA), layer.InnerLoops.ElementAt(0)[2]);
        }
    }
}