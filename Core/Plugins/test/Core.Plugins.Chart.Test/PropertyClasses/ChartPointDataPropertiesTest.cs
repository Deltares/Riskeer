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
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Core.Plugins.Chart.PropertyClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Chart.Test.PropertyClasses
{
    [TestFixture]
    public class ChartPointDataPropertiesTest
    {
        private const int colorPropertyIndex = 3;
        private const int strokeColorPropertyIndex = 4;
        private const int strokeThicknessPropertyIndex = 5;
        private const int sizePropertyIndex = 6;
        private const int symbolPropertyIndex = 7;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ChartPointDataProperties();

            // Assert
            Assert.IsInstanceOf<ChartDataProperties<ChartPointData>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Punten", properties.Type);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var chartPointData = new ChartPointData("Test");

            // Call
            var properties = new ChartPointDataProperties
            {
                Data = chartPointData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(colorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            "Stijl",
                                                                            "Kleur",
                                                                            "De kleur van de symbolen waarmee deze gegevensreeks wordt weergegeven.");

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(colorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            "Stijl",
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de symbolen waarmee deze gegevensreeks wordt weergegeven.");

            PropertyDescriptor strokeThicknessProperty = dynamicProperties[strokeThicknessPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeThicknessProperty,
                                                                            "Stijl",
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de symbolen waarmee deze gegevensreeks wordt weergegeven.");

            PropertyDescriptor sizeProperty = dynamicProperties[sizePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sizeProperty,
                                                                            "Stijl",
                                                                            "Grootte",
                                                                            "De grootte van de symbolen waarmee deze gegevensreeks wordt weergegeven.");

            PropertyDescriptor symbolProperty = dynamicProperties[symbolPropertyIndex];
            Assert.IsInstanceOf<EnumTypeConverter>(symbolProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(symbolProperty,
                                                                            "Stijl",
                                                                            "Symbool",
                                                                            "Het symbool waarmee deze gegevensreeks wordt weergegeven.");
        }

        [Test]
        public void Data_SetNewChartPointDataInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            Color color = Color.Aqua;
            Color strokeColor = Color.Crimson;
            const int size = 4;
            const int strokeThickness = 2;
            const ChartPointSymbol symbol = ChartPointSymbol.Circle;

            var chartPointData = new ChartPointData("Test", new ChartPointStyle(color, size, strokeColor, strokeThickness, symbol));
            var properties = new ChartPointDataProperties();

            // Call
            properties.Data = chartPointData;

            // Assert
            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(strokeColor, properties.StrokeColor);
            Assert.AreEqual(strokeThickness, properties.StrokeThickness);
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

            var chartPointData = new ChartPointData("Test", new ChartPointStyle(Color.AliceBlue, 3, Color.Fuchsia, 1, ChartPointSymbol.Circle));

            chartPointData.Attach(observerMock);

            var properties = new ChartPointDataProperties
            {
                Data = chartPointData
            };

            Color newColor = Color.Blue;
            Color newStrokeColor = Color.Aquamarine;
            const int newSize = 6;
            const ChartPointSymbol newSymbol = ChartPointSymbol.Diamond;
            const int newStrokeThickness = 4;

            // Call
            properties.Color = newColor;
            properties.Size = newSize;
            properties.Symbol = newSymbol;
            properties.StrokeColor = newStrokeColor;
            properties.StrokeThickness = newStrokeThickness;

            // Assert
            Assert.AreEqual(newColor, chartPointData.Style.Color);
            Assert.AreEqual(newSize, chartPointData.Style.Size);
            Assert.AreEqual(newSymbol, chartPointData.Style.Symbol);
            Assert.AreEqual(newStrokeColor, chartPointData.Style.StrokeColor);
            Assert.AreEqual(newStrokeThickness, chartPointData.Style.StrokeThickness);
            mocks.VerifyAll();
        }
    }
}