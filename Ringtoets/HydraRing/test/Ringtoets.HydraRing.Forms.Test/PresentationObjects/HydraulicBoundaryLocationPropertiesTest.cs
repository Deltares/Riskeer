﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.Forms.PresentationObjects;

namespace Ringtoets.HydraRing.Forms.Test.PresentationObjects
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
                0.0,
                ""
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
            string designWaterLevel = "<some level>";

            var mockRepository = new MockRepository();
            object[] hydraulicBoundaryLocationArguments =
            {
                id,
                name,
                x,
                y,
                designWaterLevel
            };
            var hydraulicBoundaryLocationMock = mockRepository.StrictMock<HydraulicBoundaryLocation>(hydraulicBoundaryLocationArguments);
            mockRepository.ReplayAll();

            // Call
            HydraulicBoundaryLocationProperties hydraulicBoundaryLocationProperties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryLocationMock);

            // Assert
            Assert.AreEqual(id, hydraulicBoundaryLocationProperties.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocationProperties.Name);
            Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocationProperties.DesignWaterLevel);
            Assert.AreEqual(coordinates, hydraulicBoundaryLocationProperties.Location);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(null)]
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
                y,
                ""
            };
            var hydraulicBoundaryLocationMock = mockRepository.StrictMock<HydraulicBoundaryLocation>(hydraulicBoundaryLocationArguments);
            mockRepository.ReplayAll();

            var expectedString = string.Format("{0} ({1})", name, new Point2D(x, y));

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
            var hydraulicBoundaryLocationMock = mockRepository.StrictMock<HydraulicBoundaryLocation>(0, "", 0.0, 0.0, "");
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationProperties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryLocationMock);

            var dynamicPropertyBag = new DynamicPropertyBag(hydraulicBoundaryLocationProperties);
            const string expectedIdDisplayName = "Id";
            const string expectedNameDisplayName = "Naam";
            const string expectedLocationDisplayName = "Coördinaten";
            const string expectedDesignWaterLevelDisplayName = "Toetspeil";

            // Call
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(hydraulicBoundaryLocationProperties, true);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor idProperty = dynamicProperties[0];
            PropertyDescriptor nameProperty = dynamicProperties[1];
            PropertyDescriptor locationProperty = dynamicProperties[2];
            PropertyDescriptor designWaterLevelProperty = dynamicProperties[3];

            // Assert
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            Assert.IsNotNull(idProperty);
            Assert.IsTrue(idProperty.IsReadOnly);
            Assert.IsTrue(idProperty.IsBrowsable);
            Assert.AreEqual(expectedIdDisplayName, idProperty.DisplayName);

            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.IsTrue(nameProperty.IsBrowsable);
            Assert.AreEqual(expectedNameDisplayName, nameProperty.DisplayName);

            Assert.IsNotNull(locationProperty);
            Assert.IsTrue(locationProperty.IsReadOnly);
            Assert.IsTrue(locationProperty.IsBrowsable);
            Assert.AreEqual(expectedLocationDisplayName, locationProperty.DisplayName);

            Assert.IsNotNull(designWaterLevelProperty);
            Assert.IsTrue(designWaterLevelProperty.IsReadOnly);
            Assert.IsTrue(designWaterLevelProperty.IsBrowsable);
            Assert.AreEqual(expectedDesignWaterLevelDisplayName, designWaterLevelProperty.DisplayName);

            mockRepository.VerifyAll();
        }
    }
}