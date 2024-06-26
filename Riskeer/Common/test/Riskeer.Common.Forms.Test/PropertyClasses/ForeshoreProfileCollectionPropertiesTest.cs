﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ForeshoreProfileCollectionPropertiesTest
    {
        [Test]
        public void Constructor_WithoutCollection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ForeshoreProfileCollectionProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collection", paramName);
        }

        [Test]
        public void Constructor_WithData_ReturnExpectedValues()
        {
            // Setup
            const string someFilePath = "location/to/a/file";
            var collection = new ForeshoreProfileCollection();
            collection.AddRange(Enumerable.Empty<ForeshoreProfile>(), someFilePath);

            // Call
            var properties = new ForeshoreProfileCollectionProperties(collection);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ForeshoreProfileCollection>>(properties);
            Assert.AreSame(collection, properties.Data);
            Assert.AreEqual(someFilePath, properties.SourcePath);
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var collection = new ForeshoreProfileCollection();

            // Call
            var properties = new ForeshoreProfileCollectionProperties(collection);

            // Assert
            PropertyDescriptorCollection dynamicProperties =
                PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor sourcePathProperty = dynamicProperties[0];
            Assert.IsNotNull(sourcePathProperty);
            Assert.IsTrue(sourcePathProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", sourcePathProperty.Category);
            Assert.AreEqual("Bronlocatie", sourcePathProperty.DisplayName);
            Assert.AreEqual(
                "De locatie van het bestand waaruit de voorlandprofielen zijn geïmporteerd.",
                sourcePathProperty.Description);
        }
    }
}