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

using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveHeightLocationContextPropertiesTest
    {
        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            const long id = 1234;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "<some name>";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Call
            var properties = new WaveHeightLocationContextProperties
            {
                Data = new WaveHeightLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation)
            };

            // Assert
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            Point2D coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, properties.Location);
            Assert.IsNaN(properties.WaveHeight);
            Assert.AreEqual(string.Empty, properties.Convergence);
        }

        [Test]
        public void GetProperties_ValidWaveHeight_ReturnsExpectedValues()
        {
            // Setup
            RoundedDouble waveHeight = (RoundedDouble) 0.123456;
            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateWaveHeightCalculated(waveHeight);

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Call
            var properties = new WaveHeightLocationContextProperties
            {
                Data = new WaveHeightLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation)
            };

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocation.Id, properties.Id);
            Assert.AreEqual(hydraulicBoundaryLocation.Name, properties.Name);
            Assert.AreEqual(hydraulicBoundaryLocation.Location, properties.Location);
            Assert.AreEqual(waveHeight, properties.WaveHeight, hydraulicBoundaryLocation.WaveHeight.GetAccuracy());
            Assert.AreEqual(string.Empty, properties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "", 0.0, 0.0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Call
            var properties = new WaveHeightLocationContextProperties
            {
                Data = new WaveHeightLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation)
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            const string expectedCategory = "Algemeen";
            const string expectedIdDisplayName = "ID";
            const string expectedNameDisplayName = "Naam";
            const string expectedLocationDisplayName = "Coördinaten [m]";
            const string expectedWaveHeightDisplayName = "Hs [m]";
            const string expectedConvergenceDisplayName = "Convergentie";
            const string expectedIdDescription = "ID van de hydraulische randvoorwaardenlocatie in de database.";
            const string expectedNameDescription = "Naam van de hydraulische randvoorwaardenlocatie.";
            const string expectedLocationDescription = "Coördinaten van de hydraulische randvoorwaardenlocatie.";
            const string expectedWaveHeightDescription = "Berekende golfhoogte.";
            const string expectedConvergenceDisplayDescription = "Is convergentie bereikt in de golfhoogte berekening?";

            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor idProperty = dynamicProperties.Find("Id", false);
            PropertyDescriptor nameProperty = dynamicProperties.Find("Name", false);
            PropertyDescriptor locationProperty = dynamicProperties.Find("Location", false);
            PropertyDescriptor waveHeightProperty = dynamicProperties.Find("WaveHeight", false);
            PropertyDescriptor convergenceProperty = dynamicProperties.Find("Convergence", false);

            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            Assert.IsNotNull(idProperty);
            Assert.IsTrue(idProperty.IsReadOnly);
            Assert.IsTrue(idProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, idProperty.Category);
            Assert.AreEqual(expectedIdDisplayName, idProperty.DisplayName);
            Assert.AreEqual(expectedIdDescription, idProperty.Description);
            Assert.AreEqual(1, idProperty.Attributes.OfType<PropertyOrderAttribute>().First().Order);

            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.IsTrue(nameProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, nameProperty.Category);
            Assert.AreEqual(expectedNameDisplayName, nameProperty.DisplayName);
            Assert.AreEqual(expectedNameDescription, nameProperty.Description);
            Assert.AreEqual(2, nameProperty.Attributes.OfType<PropertyOrderAttribute>().First().Order);

            Assert.IsNotNull(locationProperty);
            Assert.IsTrue(locationProperty.IsReadOnly);
            Assert.IsTrue(locationProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, locationProperty.Category);
            Assert.AreEqual(expectedLocationDisplayName, locationProperty.DisplayName);
            Assert.AreEqual(expectedLocationDescription, locationProperty.Description);
            Assert.AreEqual(3, locationProperty.Attributes.OfType<PropertyOrderAttribute>().First().Order);

            Assert.IsNotNull(waveHeightProperty);
            Assert.IsTrue(waveHeightProperty.IsReadOnly);
            Assert.IsTrue(waveHeightProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, waveHeightProperty.Category);
            Assert.AreEqual(expectedWaveHeightDisplayName, waveHeightProperty.DisplayName);
            Assert.AreEqual(expectedWaveHeightDescription, waveHeightProperty.Description);
            Assert.AreEqual(4, waveHeightProperty.Attributes.OfType<PropertyOrderAttribute>().First().Order);

            Assert.IsNotNull(convergenceProperty);
            Assert.IsTrue(convergenceProperty.IsReadOnly);
            Assert.IsTrue(convergenceProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, convergenceProperty.Category);
            Assert.AreEqual(expectedConvergenceDisplayName, convergenceProperty.DisplayName);
            Assert.AreEqual(expectedConvergenceDisplayDescription, convergenceProperty.Description);
            Assert.AreEqual(5, convergenceProperty.Attributes.OfType<PropertyOrderAttribute>().First().Order);
        }
    }
}