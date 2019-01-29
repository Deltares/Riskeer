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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationBasePropertiesTest
    {
        private const int idPropertyIndex = 0;
        private const int namePropertyIndex = 1;
        private const int coordinatesPropertyIndex = 2;

        [Test]
        public void Constructor_LocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraulicBoundaryLocationBaseProperties(null,
                                                                                      Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationPerCategoryBoundaryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraulicBoundaryLocationBaseProperties(new TestHydraulicBoundaryLocation(),
                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationPerCategoryBoundary", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new TestHydraulicBoundaryLocationBaseProperties(location,
                                                                             Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryLocation>>(properties);
            Assert.AreSame(location, properties.Data);
            TestHelper.AssertTypeConverter<TestHydraulicBoundaryLocationBaseProperties, ExpandableObjectConverter>();
        }

        [Test]
        public void Constructor_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new TestHydraulicBoundaryLocationBaseProperties(location,
                                                                             Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor idProperty = dynamicProperties[idPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            generalCategory,
                                                                            "ID",
                                                                            "ID van de hydraulische belastingenlocatie in de database.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "Naam van de hydraulische belastingenlocatie.",
                                                                            true);

            PropertyDescriptor coordinatesProperty = dynamicProperties[coordinatesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coordinatesProperty,
                                                                            generalCategory,
                                                                            "Coördinaten [m]",
                                                                            "Coördinaten van de hydraulische belastingenlocatie.",
                                                                            true);
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            const long id = 1234L;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "<some name>";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);

            var calculations = new[]
            {
                new Tuple<string, HydraulicBoundaryLocationCalculation>("A", new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation))
            };

            // Call
            var properties = new TestHydraulicBoundaryLocationBaseProperties(hydraulicBoundaryLocation,
                                                                             calculations);

            // Assert
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            var coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, properties.Location);
        }

        [Test]
        public void ToString_Always_ReturnsNameAndLocation()
        {
            // Setup
            const string name = "test";
            const long id = 1234L;
            const double x = 567.0;
            const double y = 890.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);

            // Call
            var properties = new TestHydraulicBoundaryLocationBaseProperties(hydraulicBoundaryLocation,
                                                                             Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            string expectedString = $"{name} {new Point2D(x, y)}";
            Assert.AreEqual(expectedString, properties.ToString());
        }

        private class TestHydraulicBoundaryLocationBaseProperties : HydraulicBoundaryLocationBaseProperties
        {
            public TestHydraulicBoundaryLocationBaseProperties(HydraulicBoundaryLocation location, IEnumerable<Tuple<string, HydraulicBoundaryLocationCalculation>> calculationPerCategoryBoundary)
                : base(location, calculationPerCategoryBoundary) {}
        }
    }
}