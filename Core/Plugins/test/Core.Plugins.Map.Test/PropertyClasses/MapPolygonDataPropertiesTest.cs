// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Core.Plugins.Map.PropertyClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class MapPolygonDataPropertiesTest
    {
        private const int fillColorPropertyIndex = 5;
        private const int strokeColorPropertyIndex = 6;
        private const int strokeThicknessPropertyIndex = 7;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new MapPolygonDataProperties();

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapDataProperties<MapPolygonData>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Polygonen", properties.Type);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapPolygonData = new MapPolygonData("Test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                ShowLabels = true
            };

            // Call
            var properties = new MapPolygonDataProperties
            {
                Data = mapPolygonData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);
            const string styleCategory = "Stijl";

            PropertyDescriptor colorProperty = dynamicProperties[fillColorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(colorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de vlakken waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor widthProperty = dynamicProperties[strokeColorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(colorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(widthProperty,
                                                                            styleCategory,
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de vlakken waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor styleProperty = dynamicProperties[strokeThicknessPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(styleProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de vlakken waarmee deze kaartlaag wordt weergegeven.");
        }

        [Test]
        public void Data_SetNewMapPolygonDataInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            Color fillColor = Color.Aqua;
            Color strokeColor = Color.Bisque;
            const int strokeThickness = 4;

            var mapPolygonData = new MapPolygonData("Test", new PolygonStyle
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = strokeThickness
            });
            var properties = new MapPolygonDataProperties();

            // Call
            properties.Data = mapPolygonData;

            // Assert
            Assert.AreEqual(mapPolygonData.ShowLabels, properties.ShowLabels);
            Assert.IsEmpty(properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPolygonData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(fillColor, properties.FillColor);
            Assert.AreEqual(strokeColor, properties.StrokeColor);
            Assert.AreEqual(strokeThickness, properties.StrokeThickness);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 3;
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            var mapPolygonData = new MapPolygonData("Test", new PolygonStyle
            {
                FillColor = Color.AliceBlue,
                StrokeColor = Color.Blue,
                StrokeThickness = 3
            });

            mapPolygonData.Attach(observer);

            var properties = new MapPolygonDataProperties
            {
                Data = mapPolygonData
            };

            Color newFillColor = Color.Blue;
            Color newStrokeColor = Color.Red;
            const int newStrokeThickness = 6;

            // Call
            properties.FillColor = newFillColor;
            properties.StrokeColor = newStrokeColor;
            properties.StrokeThickness = newStrokeThickness;

            // Assert
            Assert.AreEqual(newFillColor, mapPolygonData.Style.FillColor);
            Assert.AreEqual(newStrokeColor, mapPolygonData.Style.StrokeColor);
            Assert.AreEqual(newStrokeThickness, mapPolygonData.Style.StrokeThickness);
            mocks.VerifyAll();
        }
    }
}