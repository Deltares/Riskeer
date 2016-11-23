// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Plugins.Map.PropertyClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class FeatureBasedMapDataPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int typePropertyIndex = 1;
        private const int isVisiblePropertyIndex = 2;
        private const int showLabelsPropertyIndex = 3;
        private const int selectedMetaDataAttributePropertyIndex = 4;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new TestFeatureBasedMapDataProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MapPointData>>(properties);
            Assert.IsInstanceOf<IHasMetaData>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewMapPointDataInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var mapPointData = new MapPointData("Test");
            var properties = new TestFeatureBasedMapDataProperties();

            // Call
            properties.Data = mapPointData;

            // Assert
            Assert.AreEqual(mapPointData.Name, properties.Name);
            Assert.AreEqual("Test feature based map data", properties.Type);
            Assert.AreEqual(mapPointData.IsVisible, properties.IsVisible);
            Assert.AreEqual(mapPointData.ShowLabels, properties.ShowLabels);
        }

        [Test]
        [Combinatorial]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues([Values(true, false)] bool hasMetaData,
                                                                              [Values(true, false)] bool showLabels)
        {
            // Setup
            var feature = new MapFeature(Enumerable.Empty<MapGeometry>());
            if (hasMetaData)
            {
                feature.MetaData["key"] = "value";
            }

            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    feature
                },
                ShowLabels = showLabels
            };

            // Call
            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(showLabels ? 5 : 4, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string labelCategory = "Label";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van de kaartlaag.",
                                                                            true);

            PropertyDescriptor typeProperty = dynamicProperties[typePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(typeProperty,
                                                                            generalCategory,
                                                                            "Type",
                                                                            "Type van de data dat wordt getoond op de kaartlaag.",
                                                                            true);

            PropertyDescriptor isVisibleProperty = dynamicProperties[isVisiblePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isVisibleProperty,
                                                                            generalCategory,
                                                                            "Zichtbaar",
                                                                            "Geeft aan of deze kaartlaag moet worden getoond.");

            PropertyDescriptor showlabelsProperty = dynamicProperties[showLabelsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(showlabelsProperty,
                                                                            labelCategory,
                                                                            "Weergeven",
                                                                            "Geeft aan of op deze kaartlaag labels moeten worden weergegeven.",
                                                                            !hasMetaData);

            if (showLabels)
            {
                PropertyDescriptor selectedMetaDataAttributeProperty = dynamicProperties[selectedMetaDataAttributePropertyIndex];
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(selectedMetaDataAttributeProperty,
                                                                                labelCategory,
                                                                                "Op basis van",
                                                                                "Toont de eigenschap op basis waarvan labels op de geselecteerde kaartlaag worden weergegeven.");
            }
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 3;
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            var mapPointData = new MapPointData("Test")
            {
                ShowLabels = true
            };

            mapPointData.Attach(observerMock);

            var properties = new TestFeatureBasedMapDataProperties
            {
                Data = mapPointData
            };

            // Call
            properties.IsVisible = false;
            properties.ShowLabels = false;
            properties.SelectedMetaDataAttribute = "ID";

            // Assert
            Assert.AreEqual(properties.IsVisible, mapPointData.IsVisible);
            Assert.AreEqual(properties.ShowLabels, mapPointData.ShowLabels);
            Assert.AreEqual(properties.SelectedMetaDataAttribute, mapPointData.SelectedMetaDataAttribute);
            mocks.VerifyAll();
        }

        private class TestFeatureBasedMapDataProperties : FeatureBasedMapDataProperties<MapPointData>
        {
            public override string Type
            {
                get
                {
                    return "Test feature based map data";
                }
            }
        }
    }
}