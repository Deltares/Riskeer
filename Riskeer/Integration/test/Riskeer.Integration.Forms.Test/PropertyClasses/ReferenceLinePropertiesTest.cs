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
using System.ComponentModel;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
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
            TestHelper.AssertTypeConverter<ReferenceLineProperties, ExpandableArrayConverter>(
                nameof(ReferenceLineProperties.Geometry));

            Assert.AreSame(referenceLine, properties.Data);
        }

        [Test]
        public void GetProperties_ReferenceLineWithGeometry_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            });

            // Call
            var properties = new ReferenceLineProperties(referenceLine);

            // Assert
            Assert.AreEqual(2, properties.Length.NumberOfDecimalPlaces);
            Assert.AreEqual(referenceLine.Length, properties.Length, properties.Length.GetAccuracy());
            CollectionAssert.AreEqual(referenceLine.Points, properties.Geometry);
        }

        [Test]
        public void Constructor_ReferenceLineWithGeometry_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new ReferenceLineProperties(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor lengthProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthProperty,
                                                                            generalCategoryName,
                                                                            "Lengte* [m]",
                                                                            "Totale lengte van het traject in meters (afgerond).",
                                                                            true);

            PropertyDescriptor geometryProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(geometryProperty,
                                                                            generalCategoryName,
                                                                            "Coördinaten",
                                                                            "Lijst van alle coördinaten (X-coördinaat, Y-coördinaat) " +
                                                                            "die samen de referentielijn vormen.",
                                                                            true);
        }

        [Test]
        public void Constructor_ReferenceLineWithoutGeometry_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new ReferenceLineProperties(new ReferenceLine());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(0, dynamicProperties.Count);
        }
    }
}