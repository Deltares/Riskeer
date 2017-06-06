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
    public class MapLineDataPropertiesTest
    {
        private const int colorPropertyIndex = 5;
        private const int widthPropertyIndex = 6;
        private const int stylePropertyIndex = 7;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new MapLineDataProperties();

            // Assert
            Assert.IsInstanceOf<FeatureBasedMapDataProperties<MapLineData>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Lijnen", properties.Type);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapLineData = new MapLineData("Test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                },
                ShowLabels = true
            };

            // Call
            var properties = new MapLineDataProperties
            {
                Data = mapLineData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            PropertyDescriptor colorProperty = dynamicProperties[colorPropertyIndex];
            Assert.IsInstanceOf<ColorTypeConverter>(colorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(colorProperty,
                                                                            "Stijl",
                                                                            "Kleur",
                                                                            "De kleur van de lijnen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor widthProperty = dynamicProperties[widthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(widthProperty,
                                                                            "Stijl",
                                                                            "Lijndikte",
                                                                            "De dikte van de lijnen waarmee deze kaartlaag wordt weergegeven.");

            PropertyDescriptor styleProperty = dynamicProperties[stylePropertyIndex];
            Assert.IsInstanceOf<DashStyleConverter>(styleProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(styleProperty,
                                                                            "Stijl",
                                                                            "Lijnstijl",
                                                                            "De stijl van de lijnen waarmee deze kaartlaag wordt weergegeven.");
        }

        [Test]
        public void Data_SetNewMapLineDataInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            Color color = Color.Aqua;
            const int width = 4;
            const DashStyle dashStyle = DashStyle.DashDot;

            var mapLineData = new MapLineData("Test", new LineStyle(color, width, dashStyle));
            var properties = new MapLineDataProperties();

            // Call
            properties.Data = mapLineData;

            // Assert
            Assert.AreEqual(mapLineData.ShowLabels, properties.ShowLabels);
            Assert.AreEqual(string.Empty, properties.SelectedMetaDataAttribute.MetaDataAttribute);
            Assert.AreEqual(mapLineData.MetaData, properties.GetAvailableMetaDataAttributes());

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

            var mapLineData = new MapLineData("Test", new LineStyle(Color.AliceBlue, 3, DashStyle.Solid));

            mapLineData.Attach(observerMock);

            var properties = new MapLineDataProperties
            {
                Data = mapLineData
            };

            Color newColor = Color.Blue;
            const int newWidth = 6;
            const DashStyle newDashStyle = DashStyle.DashDot;

            // Call
            properties.Color = newColor;
            properties.Width = newWidth;
            properties.DashStyle = newDashStyle;

            // Assert
            Assert.AreEqual(newColor, mapLineData.Style.Color);
            Assert.AreEqual(newWidth, mapLineData.Style.Width);
            Assert.AreEqual(newDashStyle, mapLineData.Style.DashStyle);
            mocks.VerifyAll();
        }
    }
}