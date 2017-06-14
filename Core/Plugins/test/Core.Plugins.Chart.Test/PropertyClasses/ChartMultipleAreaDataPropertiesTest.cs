﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Core.Plugins.Chart.PropertyClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Chart.Test.PropertyClasses
{
    [TestFixture]
    public class ChartMultipleAreaDataPropertiesTest
    {
        private const int fillColorPropertyIndex = 3;
        private const int strokeColorPropertyIndex = 4;
        private const int strokeThicknessPropertyIndex = 5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ChartMultipleAreaDataProperties();

            // Assert
            Assert.IsInstanceOf<ChartDataProperties<ChartMultipleAreaData>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Vlakken", properties.Type);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var chartAreaData = new ChartMultipleAreaData("Test");

            // Call
            var properties = new ChartMultipleAreaDataProperties
            {
                Data = chartAreaData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string styleCategory = "Stijl";

            PropertyDescriptor fillColorProperty = dynamicProperties[fillColorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(fillColorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fillColorProperty,
                                                                            styleCategory,
                                                                            "Kleur",
                                                                            "De kleur van de vlakken waarmee deze gegevensreeks wordt weergegeven.");

            PropertyDescriptor strokeColorProperty = dynamicProperties[strokeColorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(strokeColorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeColorProperty,
                                                                            styleCategory,
                                                                            "Lijnkleur",
                                                                            "De kleur van de lijn van de vlakken waarmee deze gegevensreeks wordt weergegeven.");

            PropertyDescriptor strokeThicknessProperty = dynamicProperties[strokeThicknessPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(strokeThicknessProperty,
                                                                            styleCategory,
                                                                            "Lijndikte",
                                                                            "De dikte van de lijn van de vlakken waarmee deze gegevensreeks wordt weergegeven.");
        }

        [Test]
        public void Data_SetNewChartAreaDataInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            Color fillColor = Color.Aqua;
            Color strokeColor = Color.Bisque;
            const int strokeThickness = 4;

            var chartAreaData = new ChartMultipleAreaData("Test", new ChartAreaStyle
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = strokeThickness
            });
            var properties = new ChartMultipleAreaDataProperties();

            // Call
            properties.Data = chartAreaData;

            // Assert
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
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mocks.ReplayAll();

            var chartAreaData = new ChartMultipleAreaData("Test", new ChartAreaStyle
            {
                FillColor = Color.AliceBlue,
                StrokeColor = Color.Blue,
                StrokeThickness = 3
            });

            chartAreaData.Attach(observerMock);

            var properties = new ChartMultipleAreaDataProperties
            {
                Data = chartAreaData
            };

            Color newFillColor = Color.Blue;
            Color newStrokeColor = Color.Red;
            const int newStrokeThickness = 6;

            // Call
            properties.FillColor = newFillColor;
            properties.StrokeColor = newStrokeColor;
            properties.StrokeThickness = newStrokeThickness;

            // Assert
            Assert.AreEqual(newFillColor, chartAreaData.Style.FillColor);
            Assert.AreEqual(newStrokeColor, chartAreaData.Style.StrokeColor);
            Assert.AreEqual(newStrokeThickness, chartAreaData.Style.StrokeThickness);
            mocks.VerifyAll();
        }
    }
}