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

using System;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.HydraRing.Data.Test
{
    [TestFixture]
    public class HydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NullName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocation(0L, null, 0.0, 0.0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_ValidParameters_PropertiesAsExpected()
        {
            // Setup
            long id = 1234L;
            string name = "<some name>";
            double x = 567.0;
            double y = 890.0;

            // Call
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);

            // Assert
            Assert.IsInstanceOf<Observable>(hydraulicBoundaryLocation);
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(hydraulicBoundaryLocation);
            Assert.AreEqual(id, hydraulicBoundaryLocation.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocation.Name);
            Point2D location = hydraulicBoundaryLocation.Location;
            Assert.IsInstanceOf<Point2D>(location);
            Assert.AreEqual(x, location.X);
            Assert.AreEqual(y, location.Y);
        }

        [Test]
        public void Properties_ValidParameters_PropertiesAsExpected()
        {
            // Setup
            double designWaterLevel = 741.0;
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "", 0, 0);

            // Call
            hydraulicBoundaryLocation.DesignWaterLevel = designWaterLevel;

            // Assert
            Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocationProperties = new HydraulicBoundaryDatabaseProperties();

            var dynamicPropertyBag = new DynamicPropertyBag(hydraulicBoundaryLocationProperties);
            const string expectedFilePathDisplayName = "Hydraulische randvoorwaarden database";
            const string expectedFilePathDescription = "Locatie van het hydraulische randvoorwaarden database bestand.";
            const string expectedFilePathCategory = "Algemeen";

            const string expectedLocationsDisplayName = "Locaties";
            const string expectedLocationsDescription = "Locaties uit de hydraulische randvoorwaarden database.";
            const string expectedLocationsCategory = "Algemeen";

            // Call
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(hydraulicBoundaryLocationProperties, true);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor filePathProperty = dynamicProperties.Find("FilePath", false);
            PropertyDescriptor locationsProperty = dynamicProperties.Find("Locations", false);

            // Assert
            Assert.IsInstanceOf<TypeConverter>(classTypeConverter);

            Assert.IsNotNull(filePathProperty);
            Assert.IsTrue(filePathProperty.IsReadOnly);
            Assert.IsTrue(filePathProperty.IsBrowsable);
            Assert.AreEqual(expectedFilePathDisplayName, filePathProperty.DisplayName);
            Assert.AreEqual(expectedFilePathDescription, filePathProperty.Description);
            Assert.AreEqual(expectedFilePathCategory, filePathProperty.Category);

            Assert.IsNotNull(locationsProperty);
            Assert.IsInstanceOf<ExpandableArrayConverter>(locationsProperty.Converter);
            Assert.IsTrue(locationsProperty.IsReadOnly);
            Assert.IsTrue(locationsProperty.IsBrowsable);
            Assert.AreEqual(expectedLocationsDisplayName, locationsProperty.DisplayName);
            Assert.AreEqual(expectedLocationsDescription, locationsProperty.Description);
            Assert.AreEqual(expectedLocationsCategory, filePathProperty.Category);
        }

        [Test]
        public void ToString_Always_ReturnsName()
        {
            // Setup
            var testName = "testName";
            var hydraulicBoundaryLocationProperties = new HydraulicBoundaryLocation(0, testName, 0, 0);

            // Call
            var result = hydraulicBoundaryLocationProperties.ToString();

            // Assert
            Assert.AreEqual(testName, result);
        }
    }
}