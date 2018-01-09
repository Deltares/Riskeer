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
using System.ComponentModel;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ReferenceLinePropertiesTest
    {
        [Test]
        public void Constructor_ReferenceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ReferenceLineProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("referenceLine", paramName);
        }

        [Test]
        public void Constructor_WithReferenceLine_ExpectedValues()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            // Call
            var properties = new ReferenceLineProperties(referenceLine);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ReferenceLine>>(properties);
            Assert.AreSame(referenceLine, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var geometry = new List<Point2D>
            {
                new Point2D(0.1234, 0.1236),
                new Point2D(5.1234, 0.1236)
            };

            referenceLine.SetGeometry(geometry);
            var properties = new ReferenceLineProperties(referenceLine);

            // Call & Assert
            Assert.AreEqual(2, properties.Length.NumberOfDecimalPlaces);
            Assert.AreEqual(referenceLine.Length, properties.Length, properties.Length.GetAccuracy());

            Point2D[] expectedPoints =
            {
                new Point2D(0.123, 0.124),
                new Point2D(5.123, 0.124)
            };
            var index = 0;
            foreach (Point2D roundedPoint in properties.Geometry)
            {
                Assert.AreEqual(expectedPoints[index], roundedPoint);
                index++;
            }
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new ReferenceLineProperties(new ReferenceLine());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor lengthProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthProperty,
                                                                            generalCategoryName,
                                                                            "Lengte [m]",
                                                                            "Totale lengte van het traject in meters.",
                                                                            true);

            PropertyDescriptor geometryProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(geometryProperty,
                                                                            generalCategoryName,
                                                                            "Coördinaten",
                                                                            "Lijst met coördinaten van punten op de referentielijn.",
                                                                            true);
        }
    }
}