using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Charting.Series;
using Core.Common.Utils.PropertyBag;
using Core.Plugins.Charting.Property;
using NUnit.Framework;

namespace Core.Plugins.Charting.Test.Property
{
    [TestFixture]
    public class BarSeriesPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new BarSeriesProperties();

            // Assert
            Assert.IsInstanceOf<ChartSeriesProperties<BarSeries>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Color_WithData_SameAsData()
        {
            // Setup
            var someColor = Color.AliceBlue;
            var otherColor = Color.AntiqueWhite;

            var chartSeries = new BarSeries
            {
                Color = someColor
            };
            var properties = new BarSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someColor, properties.Color);

            // Call
            properties.Color = otherColor;

            // Assert
            Assert.AreEqual(otherColor, chartSeries.Color);
        }

        [Test]
        public void LineColor_WithData_SameAsData()
        {
            // Setup
            var someLineColor = Color.AliceBlue;
            var otherLineColor = Color.AntiqueWhite;

            var chartSeries = new BarSeries
            {
                LineColor = someLineColor
            };
            var properties = new BarSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someLineColor, properties.LineColor);

            // Call
            properties.LineColor = otherLineColor;

            // Assert
            Assert.AreEqual(otherLineColor, chartSeries.LineColor);
        }

        [Test]
        public void Width_WithData_SameAsData()
        {
            // Setup
            var someWidth = 2;
            var otherWidth = 3;

            var chartSeries = new BarSeries
            {
                LineWidth = someWidth
            };
            var properties = new BarSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someWidth, properties.Width);

            // Call
            properties.Width = otherWidth;

            // Assert
            Assert.AreEqual(otherWidth, chartSeries.LineWidth);
        }

        [Test]
        public void DashStyle_WithData_SameAsData()
        {
            // Setup
            var someDashStyle = DashStyle.Dash;
            var otherDashStyle = DashStyle.Dot;

            var chartSeries = new BarSeries
            {
                DashStyle = someDashStyle
            };
            var properties = new BarSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someDashStyle, properties.DashStyle);

            // Call
            properties.DashStyle = otherDashStyle;

            // Assert
            Assert.AreEqual(otherDashStyle, chartSeries.DashStyle);
        }

        [Test]
        public void LineVisible_WithData_SameAsData()
        {
            // Setup
            var someLineVisible = false;
            var otherLineVisible = true;

            var chartSeries = new BarSeries
            {
                LineVisible = someLineVisible
            };
            var properties = new BarSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someLineVisible, properties.LineVisible);

            // Call
            properties.LineVisible = otherLineVisible;

            // Assert
            Assert.AreEqual(otherLineVisible, chartSeries.LineVisible);
        }

        [Test]
        public void GetProperties_Always_ReturnsEightProperties()
        {
            // Setup
            var data = new BarSeries();
            
            var bag = new DynamicPropertyBag(new BarSeriesProperties
            {
                Data = data
            });

            // Call
            var properties = bag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(8, properties.Count);
        }
    }
}