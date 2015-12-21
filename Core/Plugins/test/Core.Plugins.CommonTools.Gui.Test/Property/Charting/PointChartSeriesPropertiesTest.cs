using System;
using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Utils.PropertyBag;
using Core.Plugins.Charting.Property;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Property.Charting
{
    [TestFixture]
    public class PointChartSeriesPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PointChartSeriesProperties();

            // Assert
            Assert.IsInstanceOf<ChartSeriesProperties<IPointChartSeries>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var random = new Random(21);
            var color = Color.AliceBlue;
            var lineColor = Color.AliceBlue;
            var pointerLineVisible = false;
            var size = random.Next(1, 10);
            var style = PointerStyles.SmallDot;
            var chartSeries = mocks.StrictMock<IPointChartSeries>();

            chartSeries.Expect(a => a.Color).Return(color);
            chartSeries.Expect(a => a.LineColor).Return(lineColor);
            chartSeries.Expect(a => a.LineVisible).Return(pointerLineVisible);
            chartSeries.Expect(a => a.Size).Return(size);
            chartSeries.Expect(a => a.Style).Return(style);

            mocks.ReplayAll();

            var properties = new PointChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(lineColor, properties.LineColor);
            Assert.AreEqual(pointerLineVisible, properties.PointerLineVisible);
            Assert.AreEqual(size, properties.Size);
            Assert.AreEqual(style, properties.Style);

            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_CallsSetters()
        {
            // Setup
            var mocks = new MockRepository();
            var random = new Random(21);
            var color = Color.AliceBlue;
            var lineColor = Color.AliceBlue;
            var pointerLineVisible = false;
            var size = random.Next(1, 10);
            var style = PointerStyles.SmallDot;
            var chartSeries = mocks.StrictMock<IPointChartSeries>();

            chartSeries.Expect(a => a.Color).SetPropertyWithArgument(color);
            chartSeries.Expect(a => a.LineColor).SetPropertyWithArgument(lineColor);
            chartSeries.Expect(a => a.LineVisible).SetPropertyWithArgument(pointerLineVisible);
            chartSeries.Expect(a => a.Size).SetPropertyWithArgument(size);
            chartSeries.Expect(a => a.Style).SetPropertyWithArgument(style);

            mocks.ReplayAll();

            // Call
            new PointChartSeriesProperties
            {
                Data = chartSeries,
                Color = color,
                LineColor = lineColor,
                PointerLineVisible = pointerLineVisible,
                Size = size,
                Style = style
            };

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_Always_ReturnsEightProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<IPointChartSeries>();

            mocks.ReplayAll();

            var bag = new DynamicPropertyBag(new PointChartSeriesProperties
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