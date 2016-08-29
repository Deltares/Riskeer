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
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new TestGrassCoverErosionOutwardsLocationProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<SectionSpecificWaterLevelHydraulicBoundaryLocationContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_DefaultArgumentValues_DoesNotThrowException()
        {
            // Setup
            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(
                new HydraulicBoundaryLocation(1, "name", 2.0, 3.0));
            var locations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            };

            var context = new SectionSpecificWaterLevelHydraulicBoundaryLocationContext(locations, grassCoverErosionOutwardsHydraulicBoundaryLocation);

            // Call
            TestDelegate test = () => new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context
            };

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(
                new HydraulicBoundaryLocation(id, name, x, y));
            var locations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            };
            var context = new SectionSpecificWaterLevelHydraulicBoundaryLocationContext(locations, grassCoverErosionOutwardsHydraulicBoundaryLocation);

            // Call
            TestGrassCoverErosionOutwardsLocationProperties hydraulicBoundaryLocationProperties = new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context
            };

            // Assert
            Assert.AreEqual(id, hydraulicBoundaryLocationProperties.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocationProperties.Name);
            Point2D coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, hydraulicBoundaryLocationProperties.Location);
        }

        [Test]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            const double x = 567.0;
            const double y = 890.0;
            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(
                new HydraulicBoundaryLocation(1, name, x, y));
            var locations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            };
            var context = new SectionSpecificWaterLevelHydraulicBoundaryLocationContext(locations, grassCoverErosionOutwardsHydraulicBoundaryLocation);

            // Call
            TestGrassCoverErosionOutwardsLocationProperties hydraulicBoundaryLocationProperties = new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context
            };

            // Assert
            var expectedString = string.Format("{0} {1}", name, new Point2D(x, y));
            Assert.AreEqual(expectedString, hydraulicBoundaryLocationProperties.ToString());
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(
                new HydraulicBoundaryLocation(id, name, x, y));
            var locations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            };
            var context = new SectionSpecificWaterLevelHydraulicBoundaryLocationContext(locations, grassCoverErosionOutwardsHydraulicBoundaryLocation);

            var properties = new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context
            };

            // Call
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor idProperty = dynamicProperties.Find("Id", false);
            PropertyDescriptor nameProperty = dynamicProperties.Find("Name", false);
            PropertyDescriptor locationProperty = dynamicProperties.Find("Location", false);

            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            const string expectedCategory = "Algemeen";
            const string expectedIdDisplayName = "ID";
            const string expectedNameDisplayName = "Naam";
            const string expectedLocationDisplayName = "Coördinaten [m]";
            const string expectedIdDescription = "ID van de hydraulische randvoorwaardenlocatie in de database.";
            const string expectedNameDescription = "Naam van de hydraulische randvoorwaardenlocatie.";
            const string expectedLocationDescription = "Coördinaten van de hydraulische randvoorwaardenlocatie.";

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
        }

        private class TestGrassCoverErosionOutwardsLocationProperties : GrassCoverErosionOutwardsHydraulicBoundaryLocationProperties {}
    }
}