﻿using System;
using System.Windows.Forms.VisualStyles;
using Core.Common.Base.Geometry;
using Core.Common.Gui;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.PropertyClasses;

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
            var random = new Random(21);
            double expectedEntryPointL = random.NextDouble();
            double expectedExitPointL = random.NextDouble();
            var point1 = new Point3D
            {
                X = 1.1, Y = 2.2, Z = 3.3
            };
            var point2 = new Point3D
            {
                X = 2.1, Y = 2.2, Z = 3.3
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = expectedName
            };
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2
            });
            surfaceLine.EntryPointL = expectedEntryPointL;
            surfaceLine.ExitPointL = expectedExitPointL;
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
            Assert.AreEqual(expectedEntryPointL, properties.EntryPointL);
            Assert.AreEqual(expectedExitPointL, properties.ExitPointL);
            Assert.AreEqual(point1, properties.DitchDikeSide);
            Assert.AreEqual(point1, properties.BottomDitchDikeSide);
            Assert.AreEqual(point2, properties.BottomDitchPolderSide);
            Assert.AreEqual(point2, properties.DitchPolderSide);
        }
    }
}