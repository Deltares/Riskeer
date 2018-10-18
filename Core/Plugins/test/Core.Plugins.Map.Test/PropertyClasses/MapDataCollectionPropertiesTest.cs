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

using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Plugins.Map.PropertyClasses;
using NUnit.Framework;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class MapDataCollectionPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int isVisblePropertyIndex = 1;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("Test");
            mapDataCollection.Add(new MapPointData("Test 2"));

            // Call
            var properties = new MapDataCollectionProperties(mapDataCollection, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MapDataCollection>>(properties);
            Assert.AreSame(mapDataCollection, properties.Data);

            Assert.AreEqual(mapDataCollection.Name, properties.Name);
            Assert.AreEqual(mapDataCollection.IsVisible, properties.IsVisible);
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

            PropertyDescriptor isVisibleProperty = dynamicProperties[isVisblePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isVisibleProperty,
                                                                            mapDataCollectionCategory,
                                                                            "Weergeven",
                                                                            "Geeft aan of deze kaartlagenmap wordt weergegeven.");
        }
    }
}