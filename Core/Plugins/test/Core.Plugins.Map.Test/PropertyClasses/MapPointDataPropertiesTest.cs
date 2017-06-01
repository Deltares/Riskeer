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
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Core.Plugins.Map.Converters;
using Core.Plugins.Map.PropertyClasses;
using Core.Plugins.Map.UITypeEditors;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class MapPointDataPropertiesTest
    {
        private const int colorPropertyIndex = 5;
        private const int strokeColorPropertyIndex = 6;
        private const int strokeThicknessPropertyIndex = 7;
        private const int sizePropertyIndex = 8;
        private const int symbolPropertyIndex = 9;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new MapPointDataProperties();

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapDataProperties<MapPointData>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Punten", properties.Type);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapPointData = new MapPointData("Test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                ShowLabels = true
            };

            // Call
            var properties = new MapPointDataProperties
            {
                Data = mapPointData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(10, dynamicProperties.Count);
            
            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            Assert.IsInstanceOf<MapColorConverter>(colorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            "Stijl",
                                                                            "Kleur",
                                                                            "De kleur van de symbolen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            "Stijl",
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de symbolen waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);

            PropertyDescriptor strokeThicknessProperty = dynamicProperties[strokeThicknessPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeThicknessProperty,
                                                                            "Stijl",
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de symbolen waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);

            PropertyDescriptor sizeProperty = dynamicProperties[sizePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sizeProperty,
                                                                            "Stijl",
                                                                            "Grootte",
                                                                            "De grootte van de symbolen waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);

            PropertyDescriptor symbolProperty = dynamicProperties[symbolPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(symbolProperty,
                                                                            "Stijl",
                                                                            "Symbool",
                                                                            "Het symbool waarmee deze kaartlaag wordt weergegeven.",
                                                                            true);
        }

        [Test]
        public void Data_SetNewMapPointDataInstanceWithStyle_ReturnCorrectPropertyValues()
        {
            // Setup
            Color color = Color.Aqua;
            const int size = 4;
            const PointSymbol symbol = PointSymbol.Circle;

            var mapPointData = new MapPointData("Test")
            {
                Style = new PointStyle(color, size, symbol)
            };
            var properties = new MapPointDataProperties();

            // Call
            properties.Data = mapPointData;

            // Assert
            Assert.AreEqual(mapPointData.ShowLabels, properties.ShowLabels);
            Assert.AreEqual(string.Empty, properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPointData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(color.ToString(), properties.StrokeColor);
            Assert.AreEqual(size, properties.StrokeThickness);
            Assert.AreEqual(size, properties.Size);
            Assert.AreEqual(symbol.ToString(), properties.Symbol);
        }

        [Test]
        public void Data_SetNewMapPointDataInstanceWithoutStyle_ReturnCorrectPropertyValues()
        {
            // Setup
            var mapPointData = new MapPointData("Test");
            var properties = new MapPointDataProperties();

            // Call
            properties.Data = mapPointData;

            // Assert
            Assert.AreEqual(mapPointData.ShowLabels, properties.ShowLabels);
            Assert.AreEqual(string.Empty, properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPointData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(Color.Transparent, properties.Color);
            Assert.AreEqual(string.Empty, properties.StrokeColor);
            Assert.AreEqual(0, properties.StrokeThickness);
            Assert.AreEqual(0, properties.Size);
            Assert.AreEqual(string.Empty, properties.Symbol);
        }


        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 1;
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            var mapPointData = new MapPointData("Test")
            {
                Style = new PointStyle(Color.AliceBlue, 3, PointSymbol.Circle)
            };

            mapPointData.Attach(observerMock);

            var properties = new MapPointDataProperties
            {
                Data = mapPointData
            };

            Color newColor = Color.Blue;

            // Call
            properties.Color = newColor;

            // Assert
            Assert.AreEqual(newColor, mapPointData.Style.Color);
            mocks.VerifyAll();
        }
    }
}