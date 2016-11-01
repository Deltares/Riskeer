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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ForeshoreGeometryPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var properties = new ForeshoreGeometryProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ForeshoreProfile>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void Data_SetNewForeshoreProfileInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var properties = new ForeshoreGeometryProperties();

            // Call
            properties.Data = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0], null,
                                                   new ForeshoreProfile.ConstructionProperties());

            // Assert
            CollectionAssert.IsEmpty(properties.Coordinates);
        }

        [Test]
        public void Data_SetForeshoreProfileInstanceWithData_ReturnCorrectPropertyValues()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        new[]
                                                        {
                                                            new Point2D(0, 0),
                                                            new Point2D(1, 1)
                                                        }, null, new ForeshoreProfile.ConstructionProperties());

            var properties = new ForeshoreGeometryProperties();

            // Call
            properties.Data = foreshoreProfile;

            // Assert
            var expectedCoordinates = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            };
            CollectionAssert.AreEqual(expectedCoordinates, properties.Coordinates);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0], null,
                                                        new ForeshoreProfile.ConstructionProperties());

            // Call
            var properties = new ForeshoreGeometryProperties
            {
                Data = foreshoreProfile
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor coordinatesProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coordinatesProperty,
                                                                            "Misc",
                                                                            "Coördinaten [m]",
                                                                            "Lijst met punten in lokale coördinaten.",
                                                                            true);
        }
    }
}