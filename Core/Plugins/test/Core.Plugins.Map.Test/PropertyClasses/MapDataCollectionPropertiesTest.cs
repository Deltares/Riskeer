// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
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
using Core.Common.Util;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using Core.Plugins.Map.PropertyClasses;
using NUnit.Framework;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class MapDataCollectionPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int visibilityPropertyIndex = 1;

        [Test]
        public void Constructor_MapDataCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapDataCollectionProperties(null, Enumerable.Empty<MapDataCollection>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mapDataCollection", exception.ParamName);
        }

        [Test]
        public void Constructor_ParentsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapDataCollectionProperties(new MapDataCollection("Test"), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parents", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("Test");
            mapDataCollection.Add(new TestFeatureBasedMapData());

            // Call
            var properties = new MapDataCollectionProperties(mapDataCollection, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MapDataCollection>>(properties);
            Assert.AreSame(mapDataCollection, properties.Data);

            Assert.AreEqual(mapDataCollection.Name, properties.Name);
            Assert.AreEqual(mapDataCollection.GetVisibility(), properties.Visibility);

            TestHelper.AssertTypeConverter<MapDataCollectionProperties, EnumTypeConverter>(
                nameof(MapDataCollectionProperties.Visibility));
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("Test");

            // Call
            var properties = new MapDataCollectionProperties(mapDataCollection, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string mapDataCollectionCategory = "Kaartlagenmap";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            mapDataCollectionCategory,
                                                                            "Naam",
                                                                            "De naam van deze kaartlagenmap.",
                                                                            true);

            PropertyDescriptor visibilityProperty = dynamicProperties[visibilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(visibilityProperty,
                                                                            mapDataCollectionCategory,
                                                                            "Weergeven",
                                                                            "Geeft aan of deze kaartlagenmap wordt weergegeven.",
                                                                            true);
        }
    }
}