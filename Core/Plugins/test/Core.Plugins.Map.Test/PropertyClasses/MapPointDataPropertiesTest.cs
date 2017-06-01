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
using Core.Common.Utils;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Core.Plugins.Map.Converters;
using Core.Plugins.Map.PropertyClasses;
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
            Assert.IsInstanceOf<MapColorConverter>(colorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            "Stijl",
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de symbolen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor strokeThicknessProperty = dynamicProperties[strokeThicknessPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeThicknessProperty,
                                                                            "Stijl",
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de symbolen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor sizeProperty = dynamicProperties[sizePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sizeProperty,
                                                                            "Stijl",
                                                                            "Grootte",
                                                                            "De grootte van de symbolen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor symbolProperty = dynamicProperties[symbolPropertyIndex];
            Assert.IsInstanceOf<EnumTypeConverter>(symbolProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(symbolProperty,
                                                                            "Stijl",
                                                                            "Symbool",
                                                                            "Het symbool waarmee deze kaartlaag wordt weergegeven.");
        }

        [Test]
        public void Data_SetNewMapPointDataInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            Color color = Color.Aqua;
            const int size = 4;
            const PointSymbol symbol = PointSymbol.Circle;

            var mapPointData = new MapPointData("Test", new PointStyle(color, size, symbol));
            var properties = new MapPointDataProperties();

            // Call
            properties.Data = mapPointData;

            // Assert
            Assert.AreEqual(mapPointData.ShowLabels, properties.ShowLabels);
            Assert.AreEqual(string.Empty, properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapPointData.MetaData, properties.GetAvailableMetaDataAttributes());

            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(color, properties.StrokeColor);
            Assert.AreEqual(1, properties.StrokeThickness);
            Assert.AreEqual(size, properties.Size);
            Assert.AreEqual(symbol, properties.Symbol);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 5;
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            var mapPointData = new MapPointData("Test", new PointStyle(Color.AliceBlue, 3, PointSymbol.Circle));

            mapPointData.Attach(observerMock);

            var properties = new MapPointDataProperties
            {
                Data = mapPointData
            };

            Color newColor = Color.Blue;
            Color newStrokeColor = Color.Aquamarine;
            const int newSize = 6;
            const PointSymbol newSymbol = PointSymbol.Diamond;
            const int newStrokeThickness = 4;

            // Call
            properties.Color = newColor;
            properties.Size = newSize;
            properties.Symbol = newSymbol;
            properties.StrokeColor = newStrokeColor;
            properties.StrokeThickness = newStrokeThickness;

            // Assert
            Assert.AreEqual(newColor, mapPointData.Style.Color);
            Assert.AreEqual(newSize, mapPointData.Style.Size);
            Assert.AreEqual(newSymbol, mapPointData.Style.Symbol);
            Assert.AreEqual(newStrokeColor, mapPointData.Style.StrokeColor);
            Assert.AreEqual(newStrokeThickness, mapPointData.Style.StrokeThickness);
            mocks.VerifyAll();
        }
    }
}