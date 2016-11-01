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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties();

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        [TestCase(CalculationConvergence.CalculatedConverged, "Ja")]
        [TestCase(CalculationConvergence.CalculatedNotConverged, "Nee")]
        [TestCase(CalculationConvergence.NotCalculated, "")]
        public void GetProperties_ValidData_ReturnsExpectedValues(CalculationConvergence convergenceReached, string expectedConvergenceValue)
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var designWaterLevel = (RoundedDouble) 1234;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y)
            {
                DesignWaterLevel = designWaterLevel,
                DesignWaterLevelCalculationConvergence = convergenceReached
            };
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties
            {
                Data = context
            };

            // Assert
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            Point2D coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, properties.Location);
            Assert.AreEqual(designWaterLevel, properties.DesignWaterLevel, properties.DesignWaterLevel.GetAccuracy());
            Assert.AreEqual(expectedConvergenceValue, properties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties
            {
                Data = context
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor idProperty = dynamicProperties.Find("Id", false);
            PropertyDescriptor nameProperty = dynamicProperties.Find("Name", false);
            PropertyDescriptor locationProperty = dynamicProperties.Find("Location", false);
            PropertyDescriptor waterLevelProperty = dynamicProperties.Find("DesignWaterLevel", false);
            PropertyDescriptor convergenceProperty = dynamicProperties.Find("Convergence", false);

            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            const string expectedCategory = "Algemeen";
            const string expectedIdDisplayName = "ID";
            const string expectedIdDescription = "ID van de hydraulische randvoorwaardenlocatie in de database.";
            const string expectedNameDisplayName = "Naam";
            const string expectedNameDescription = "Naam van de hydraulische randvoorwaardenlocatie.";
            const string expectedLocationDisplayName = "Coördinaten [m]";
            const string expectedLocationDescription = "Coördinaten van de hydraulische randvoorwaardenlocatie.";
            const string expectedWaterLevelDisplayName = "Waterstand bij doorsnede-eis [m+NAP]";
            const string expectedWaterLevelDescription = "Berekende waterstand bij doorsnede-eis.";
            const string expectedConvergenceDisplayName = "Convergentie";
            const string expectedConvergenceDescription = "Is convergentie bereikt in de berekening van de waterstand bij doorsnede-eis?";

            Assert.IsNotNull(idProperty);
            Assert.IsTrue(idProperty.IsReadOnly);
            Assert.IsTrue(idProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, idProperty.Category);
            Assert.AreEqual(expectedIdDisplayName, idProperty.DisplayName);
            Assert.AreEqual(expectedIdDescription, idProperty.Description);

            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.IsTrue(nameProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, nameProperty.Category);
            Assert.AreEqual(expectedNameDisplayName, nameProperty.DisplayName);
            Assert.AreEqual(expectedNameDescription, nameProperty.Description);

            Assert.IsNotNull(locationProperty);
            Assert.IsTrue(locationProperty.IsReadOnly);
            Assert.IsTrue(locationProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, locationProperty.Category);
            Assert.AreEqual(expectedLocationDisplayName, locationProperty.DisplayName);
            Assert.AreEqual(expectedLocationDescription, locationProperty.Description);

            Assert.IsNotNull(waterLevelProperty);
            Assert.IsTrue(waterLevelProperty.IsReadOnly);
            Assert.IsTrue(waterLevelProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, waterLevelProperty.Category);
            Assert.AreEqual(expectedWaterLevelDisplayName, waterLevelProperty.DisplayName);
            Assert.AreEqual(expectedWaterLevelDescription, waterLevelProperty.Description);

            Assert.IsNotNull(convergenceProperty);
            Assert.IsTrue(convergenceProperty.IsReadOnly);
            Assert.IsTrue(convergenceProperty.IsBrowsable);
            Assert.AreEqual(expectedCategory, convergenceProperty.Category);
            Assert.AreEqual(expectedConvergenceDisplayName, convergenceProperty.DisplayName);
            Assert.AreEqual(expectedConvergenceDescription, convergenceProperty.Description);
        }
    }
}