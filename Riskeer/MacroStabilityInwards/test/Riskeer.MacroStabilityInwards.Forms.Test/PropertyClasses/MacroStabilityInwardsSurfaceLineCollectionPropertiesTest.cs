﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsSurfaceLineCollectionPropertiesTest
    {
        [Test]
        public void Constructor_WithoutCollection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSurfaceLineCollectionProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collection", paramName);
        }

        [Test]
        public void Constructor_WithData_ReturnExpectedValues()
        {
            // Setup
            const string someFilePath = "location/to/a/file";
            var collection = new MacroStabilityInwardsSurfaceLineCollection();
            collection.AddRange(Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(), someFilePath);

            // Call
            var properties = new MacroStabilityInwardsSurfaceLineCollectionProperties(collection);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsSurfaceLineCollection>>(properties);
            Assert.AreSame(collection, properties.Data);
            Assert.AreEqual(someFilePath, properties.SourcePath);
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var collection = new MacroStabilityInwardsSurfaceLineCollection();

            // Call
            var properties = new MacroStabilityInwardsSurfaceLineCollectionProperties(collection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor surfaceLineCollectionProperty = dynamicProperties[0];
            Assert.IsNotNull(surfaceLineCollectionProperty);
            Assert.IsTrue(surfaceLineCollectionProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", surfaceLineCollectionProperty.Category);
            Assert.AreEqual("Bronlocatie", surfaceLineCollectionProperty.DisplayName);
            Assert.AreEqual(
                "De locatie van het bestand waaruit de profielschematisaties zijn geïmporteerd.",
                surfaceLineCollectionProperty.Description);
        }
    }
}