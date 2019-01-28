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

using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.Waternet.Output
{
    [TestFixture]
    public class WaternetLineResultTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WaternetLineResult(null, new Point2D[0], new WaternetPhreaticLineResult("test", new Point2D[0]));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WaternetLineResult("test", null, new WaternetPhreaticLineResult("test", new Point2D[0]));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_PhreaticLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WaternetLineResult("test", new Point2D[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("phreaticLine", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "Line";
            var geometry = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            };
            var phreaticLine = new WaternetPhreaticLineResult("test", new Point2D[0]);

            // Call
            var waternetLine = new WaternetLineResult(name, geometry, phreaticLine);

            // Assert
            Assert.AreEqual(name, waternetLine.Name);
            Assert.AreSame(geometry, waternetLine.Geometry);
            Assert.AreSame(phreaticLine, waternetLine.PhreaticLine);
        }
    }
}