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
using System.Drawing.Drawing2D;
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Core.Plugins.Chart.PropertyClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Chart.Test.PropertyClasses
{
    [TestFixture]
    public class ChartMultipleLineDataPropertiesTest
    {
        private const int colorPropertyIndex = 3;
        private const int widthPropertyIndex = 4;
        private const int stylePropertyIndex = 5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ChartMultipleLineDataProperties();

            // Assert
            Assert.IsInstanceOf<ChartDataProperties<ChartMultipleLineData>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Lijnen", properties.Type);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var chartLineData = new ChartMultipleLineData("Test");

            // Call
            var properties = new ChartMultipleLineDataProperties
            {
                Data = chartLineData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string styleCategory = "Stijl";

            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(colorProperty.Converter);            
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de lijnen waarmee deze gegevensreeks wordt weergegeven.");

            PropertyDescriptor widthProperty = dynamicProperties[widthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(widthProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijnen waarmee deze gegevensreeks wordt weergegeven.");

            PropertyDescriptor styleProperty = dynamicProperties[stylePropertyIndex];
            Assert.IsInstanceOf<DashStyleConverter>(styleProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(styleProperty,
                                                                            styleCategory,
                                                                            "Lijnstijl",
                                                                            "De stijl van de lijnen waarmee deze gegevensreeks wordt weergegeven.");
        }

        [Test]
        public void Data_SetNewChartLineDataInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            Color color = Color.Aqua;
            const int width = 4;
            const DashStyle dashStyle = DashStyle.DashDot;

            var chartLineData = new ChartMultipleLineData("Test", new ChartLineStyle
            {
                Color = color,
                Width = width,
                DashStyle = dashStyle
            });
            var properties = new ChartMultipleLineDataProperties();

            // Call
            properties.Data = chartLineData;

            // Assert
            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(width, properties.Width);
            Assert.AreEqual(dashStyle, properties.DashStyle);
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

            var chartLineData = new ChartMultipleLineData("Test", new ChartLineStyle
            {
                Color = Color.AliceBlue,
                Width = 3,
                DashStyle = DashStyle.Solid
            });

            chartLineData.Attach(observerMock);

            var properties = new ChartMultipleLineDataProperties
            {
                Data = chartLineData
            };

            Color newColor = Color.Blue;
            const int newWidth = 6;
            const DashStyle newDashStyle = DashStyle.DashDot;

            // Call
            properties.Color = newColor;
            properties.Width = newWidth;
            properties.DashStyle = newDashStyle;

            // Assert
            Assert.AreEqual(newColor, chartLineData.Style.Color);
            Assert.AreEqual(newWidth, chartLineData.Style.Width);
            Assert.AreEqual(newDashStyle, chartLineData.Style.DashStyle);
            mocks.VerifyAll();
        }
    }
}