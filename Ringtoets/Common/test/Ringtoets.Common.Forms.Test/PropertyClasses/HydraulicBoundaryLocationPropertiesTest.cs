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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationPropertiesTest
    {
        private const int idPropertyIndex = 0;
        private const int namePropertyIndex = 1;
        private const int coordinatesPropertyIndex = 2;
        private const int categoryBoundariesPropertyIndex = 3;

        [Test]
        public void Constructor_LocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationProperties(null, 
                                                                              Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationPerCategoryBoundaryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationProperties(new TestHydraulicBoundaryLocation(), 
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
            var properties = new HydraulicBoundaryLocationProperties(location,
                                                                     Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryLocation>>(properties);
            Assert.AreSame(location, properties.Data);
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationProperties, ExpandableObjectConverter>();
            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationProperties, ExpandableArrayConverter>(
                nameof(HydraulicBoundaryLocationProperties.CategoryBoundaries));
        }

        [Test]
        public void Constructor_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new HydraulicBoundaryLocationProperties(location,
                                                                     Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor idProperty = dynamicProperties[idPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            generalCategory,
                                                                            "ID",
                                                                            "ID van de hydraulische randvoorwaardenlocatie in de database.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "Naam van de hydraulische randvoorwaardenlocatie.",
                                                                            true);

            PropertyDescriptor coordinatesProperty = dynamicProperties[coordinatesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coordinatesProperty,
                                                                            generalCategory,
                                                                            "Coördinaten [m]",
                                                                            "Coördinaten van de hydraulische randvoorwaardenlocatie.",
                                                                            true);

            PropertyDescriptor categoryBoundariesProperty = dynamicProperties[categoryBoundariesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(categoryBoundariesProperty,
                                                                            generalCategory,
                                                                            "Categoriegrenzen",
                                                                            "De berekeningen per categoriegrens voor deze locatie.",
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
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput()
            };

            var calculations = new[]
            {
                new Tuple<string, HydraulicBoundaryLocationCalculation>("A", hydraulicBoundaryLocationCalculation)
            };

            // Call
            var properties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryLocation, 
                                                                     calculations);

            // Assert
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            var coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, properties.Location);

            DesignWaterLevelCalculationOutputProperties categoryBoundaryCalculation = properties.CategoryBoundaries.Single();
            Assert.AreEqual("A", categoryBoundaryCalculation.CategoryBoundaryName);
            Assert.AreEqual(hydraulicBoundaryLocationCalculation.Output.Result, categoryBoundaryCalculation.Result);
            Assert.AreEqual(hydraulicBoundaryLocationCalculation.Output.CalculationConvergence, categoryBoundaryCalculation.Convergence);
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
            var properties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryLocation, 
                                                                     Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            string expectedString = $"{name} {new Point2D(x, y)}";
            Assert.AreEqual(expectedString, properties.ToString());
        }
    }
}