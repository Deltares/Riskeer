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
using System.ComponentModel;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    public class RingPropertiesTest
    {
        [Test]
        public void Constructor_RingNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new RingProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("ring", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidRing_ExpectedValues()
        {
            // Setup
            var ring = new Ring(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            });

            // Call
            var properties = new RingProperties(ring);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<Ring>>(properties);
            TestHelper.AssertTypeConverter<RingProperties, ExpandableArrayConverter>(nameof(RingProperties.Geometry));
            Assert.AreSame(ring, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var ring = new Ring(new[]
            {
                new Point2D(20.210230, 26.00001),
                new Point2D(3.830, 1.040506)
            });

            // Call
            var properties = new RingProperties(ring);

            // Assert
            CollectionAssert.AreEqual(ring.Points, properties.Geometry);
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var ring = new Ring(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            });

            var properties = new RingProperties(ring);

            // Call
            string name = properties.ToString();

            // Assert
            Assert.IsEmpty(name);
        }

        [Test]
        public void Constructor_ValidData_PropertieshaveExpectedAttributeValues()
        {
            // Setup
            var ring = new Ring(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            });

            // Call
            var properties = new RingProperties(ring);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(1, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Geometrie",
                                                                            "",
                                                                            true);
        }
    }
}