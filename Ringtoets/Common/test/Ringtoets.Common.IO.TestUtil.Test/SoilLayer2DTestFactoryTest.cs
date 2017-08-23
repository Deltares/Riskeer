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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.TestUtil.Test
{
    [TestFixture]
    public class SoilLayer2DTestFactoryTest
    {
        [Test]
        public void CreateSoilLayer2D_InnerLoopNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SoilLayer2DTestFactory.CreateSoilLayer2D(
                null,
                Enumerable.Empty<Segment2D>());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("innerLoops", parameter);
        }

        [Test]
        public void CreateSoilLayer2D_OuterLoopNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SoilLayer2DTestFactory.CreateSoilLayer2D(
                Enumerable.Empty<Segment2D[]>(),
                null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("outerLoop", parameter);
        }

        [Test]
        public void CreateSoilLayer2D_InvalidInnerLoop_ThrowsArgumentException()
        {
            // Setup
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);

            // Call
            TestDelegate test = () => SoilLayer2DTestFactory.CreateSoilLayer2D(
                new[]
                {
                    new[]
                    {
                        new Segment2D(pointA, pointB),
                        new Segment2D(pointB, pointC)
                    }
                },
                Enumerable.Empty<Segment2D>());

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void CreateSoilLayer2D_InvalidOuterLoop_ThrowsArgumentException()
        {
            // Setup
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);

            // Call
            TestDelegate test = () => SoilLayer2DTestFactory.CreateSoilLayer2D(
                Enumerable.Empty<Segment2D[]>(),
                new[]
                {
                    new Segment2D(pointA, pointB),
                    new Segment2D(pointB, pointC)
                }
            );

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void CreateSoilLayer2D_ValidArguments_ExpectedProperties()
        {
            // Setup
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);
            var pointD = new Point2D(2.0, 1.0);

            // Call
            SoilLayer2D soilLayer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                new[]
                {
                    new[]
                    {
                        new Segment2D(pointC, pointD),
                        new Segment2D(pointD, pointC)
                    }
                },
                new List<Segment2D>
                {
                    new Segment2D(pointA, pointB),
                    new Segment2D(pointB, pointA)
                });

            // Assert
            Assert.AreEqual(new Segment2D(pointA, pointB), soilLayer.OuterLoop.ElementAt(0));
            Assert.AreEqual(new Segment2D(pointB, pointA), soilLayer.OuterLoop.ElementAt(1));

            Assert.AreEqual(1, soilLayer.InnerLoops.Count());
            Assert.AreEqual(new Segment2D(pointC, pointD), soilLayer.InnerLoops.ElementAt(0)[0]);
            Assert.AreEqual(new Segment2D(pointD, pointC), soilLayer.InnerLoops.ElementAt(0)[1]);
        }

        [Test]
        public void CreateSoilLayer2D_Parameterless_ExpectedProperties()
        {
            // Setup
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);
            var pointD = new Point2D(2.0, 1.0);

            // Call
            SoilLayer2D soilLayer = SoilLayer2DTestFactory.CreateSoilLayer2D();

            // Assert
            Assert.AreEqual(new Segment2D(pointA, pointB), soilLayer.OuterLoop.ElementAt(0));
            Assert.AreEqual(new Segment2D(pointB, pointA), soilLayer.OuterLoop.ElementAt(1));

            Assert.AreEqual(1, soilLayer.InnerLoops.Count());
            Assert.AreEqual(new Segment2D(pointC, pointD), soilLayer.InnerLoops.ElementAt(0)[0]);
            Assert.AreEqual(new Segment2D(pointD, pointC), soilLayer.InnerLoops.ElementAt(0)[1]);
        }
    }
}