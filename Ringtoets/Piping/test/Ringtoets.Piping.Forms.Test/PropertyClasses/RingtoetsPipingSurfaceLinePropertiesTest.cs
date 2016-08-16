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

using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLinePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new RingtoetsPipingSurfaceLineProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<RingtoetsPipingSurfaceLine>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string expectedName = "<some nice name>";
            var point1 = new Point3D(1.1, 2.2, 3.3);
            var point2 = new Point3D(2.1, 2.2, 3.3);

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = expectedName
            };
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2
            });
            surfaceLine.SetDikeToeAtRiverAt(point1);
            surfaceLine.SetDikeToeAtPolderAt(point2);
            surfaceLine.SetDitchDikeSideAt(point1);
            surfaceLine.SetBottomDitchDikeSideAt(point1);
            surfaceLine.SetBottomDitchPolderSideAt(point2);
            surfaceLine.SetDitchPolderSideAt(point2);

            var properties = new RingtoetsPipingSurfaceLineProperties
            {
                Data = surfaceLine
            };

            // Call & Assert
            Assert.AreEqual(expectedName, properties.Name);
            CollectionAssert.AreEqual(surfaceLine.Points, properties.Points);
            Assert.AreEqual(point1, properties.DikeToeAtRiver);
            Assert.AreEqual(point2, properties.DikeToeAtPolder);
            Assert.AreEqual(point1, properties.DitchDikeSide);
            Assert.AreEqual(point1, properties.BottomDitchDikeSide);
            Assert.AreEqual(point2, properties.BottomDitchPolderSide);
            Assert.AreEqual(point2, properties.DitchPolderSide);
        }
    }
}