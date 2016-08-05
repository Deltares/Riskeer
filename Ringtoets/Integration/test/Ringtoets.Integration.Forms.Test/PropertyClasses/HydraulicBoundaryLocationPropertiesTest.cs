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
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationPropertiesTest
    {
        [Test]
        public void Constructor_DefaultArgumentValues_DoesNotThrowException()
        {
            // Setup
            var mockRepository = new MockRepository();
            object[] hydraulicBoundaryLocationArguments =
            {
                0,
                "",
                0.0,
                0.0
            };
            var hydraulicBoundaryLocationMock = mockRepository.StrictMock<HydraulicBoundaryLocation>(hydraulicBoundaryLocationArguments);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new HydraulicBoundaryLocationProperties(hydraulicBoundaryLocationMock);

            // Assert
            Assert.DoesNotThrow(test);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            long id = 1234L;
            double x = 567.0;
            double y = 890.0;
            Point2D coordinates = new Point2D(x, y);
            string name = "<some name>";

            var mockRepository = new MockRepository();
            var hydraulicBoundaryLocationMock = mockRepository.StrictMock<HydraulicBoundaryLocation>(id, name, x, y);
            mockRepository.ReplayAll();

            // Call
            HydraulicBoundaryLocationProperties hydraulicBoundaryLocationProperties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryLocationMock);

            // Assert
            Assert.AreEqual(id, hydraulicBoundaryLocationProperties.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocationProperties.Name);
            Assert.AreEqual(coordinates, hydraulicBoundaryLocationProperties.Location);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            long id = 1234L;
            double x = 567.0;
            double y = 890.0;
            var mockRepository = new MockRepository();
            object[] hydraulicBoundaryLocationArguments =
            {
                id,
                name,
                x,
                y
            };
            var hydraulicBoundaryLocationMock = mockRepository.StrictMock<HydraulicBoundaryLocation>(hydraulicBoundaryLocationArguments);
            mockRepository.ReplayAll();

            var expectedString = string.Format("{0} {1}", name, new Point2D(x, y));

            // Call
            HydraulicBoundaryLocationProperties hydraulicBoundaryLocationProperties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryLocationMock);

            // Assert
            Assert.AreEqual(expectedString, hydraulicBoundaryLocationProperties.ToString());
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var hydraulicBoundaryLocationMock = mockRepository.StrictMock<HydraulicBoundaryLocation>(0, "", 0.0, 0.0);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationProperties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryLocationMock);

            var dynamicPropertyBag = new DynamicPropertyBag(hydraulicBoundaryLocationProperties);
            const string expectedIdDisplayName = "ID";
            const string expectedNameDisplayName = "Naam";
            const string expectedLocationDisplayName = "Coördinaten [m]";
            const string expectedIdDescription = "Id van de hydraulische randvoorwaardenlocatie in de database.";
            const string expectedNameDescription = "Naam van de hydraulische randvoorwaardenlocatie.";
            const string expectedLocationDescription = "Coördinaten van de hydraulische randvoorwaardenlocatie.";

            // Call
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(hydraulicBoundaryLocationProperties, true);

            // Assert
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor idProperty = dynamicProperties.Find("Id", false);
            PropertyDescriptor nameProperty = dynamicProperties.Find("Name", false);
            PropertyDescriptor locationProperty = dynamicProperties.Find("Location", false);
           
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            Assert.IsNotNull(idProperty);
            Assert.IsTrue(idProperty.IsReadOnly);
            Assert.IsTrue(idProperty.IsBrowsable);
            Assert.AreEqual(expectedIdDisplayName, idProperty.DisplayName);
            Assert.AreEqual(expectedIdDescription, idProperty.Description);
            Assert.AreEqual(1, idProperty.Attributes.OfType<PropertyOrderAttribute>().First().Order);

            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.IsTrue(nameProperty.IsBrowsable);
            Assert.AreEqual(expectedNameDisplayName, nameProperty.DisplayName);
            Assert.AreEqual(expectedNameDescription, nameProperty.Description);
            Assert.AreEqual(2, nameProperty.Attributes.OfType<PropertyOrderAttribute>().First().Order);

            Assert.IsNotNull(locationProperty);
            Assert.IsTrue(locationProperty.IsReadOnly);
            Assert.IsTrue(locationProperty.IsBrowsable);
            Assert.AreEqual(expectedLocationDisplayName, locationProperty.DisplayName);
            Assert.AreEqual(expectedLocationDescription, locationProperty.Description);
            Assert.AreEqual(3, locationProperty.Attributes.OfType<PropertyOrderAttribute>().First().Order);

            mockRepository.VerifyAll();
        }
    }
}