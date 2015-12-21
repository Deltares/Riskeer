using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Charting.Series;
using Core.Common.Utils.PropertyBag;
using Core.Plugins.Charting.Property;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Property.Charting
{
    [TestFixture]
    public class PolygonChartSeriesPropertiesTest
    {

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PolygonChartSeriesProperties();

            // Assert
            Assert.IsInstanceOf<ChartSeriesProperties<IPolygonChartSeries>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var random = new Random(21);
            var closed = false;
            var color = Color.AliceBlue;
            var transparency = random.Next(1, 10);
            var useHatch = false;
            var hatchStyle = HatchStyle.Cross;
            var hatchColor = Color.AliceBlue;
            var lineColor = Color.AliceBlue;
            var lineWidth = random.Next(1, 10);
            var lineVisible = false;
            var chartSeries = mocks.StrictMock<IPolygonChartSeries>();

            chartSeries.Expect(a => a.AutoClose).Return(closed);
            chartSeries.Expect(a => a.Color).Return(color);
            chartSeries.Expect(a => a.Transparency).Return(transparency);
            chartSeries.Expect(a => a.UseHatch).Return(useHatch);
            chartSeries.Expect(a => a.HatchStyle).Return(hatchStyle);
            chartSeries.Expect(a => a.HatchColor).Return(hatchColor);
            chartSeries.Expect(a => a.LineColor).Return(lineColor);
            chartSeries.Expect(a => a.LineWidth).Return(lineWidth);
            chartSeries.Expect(a => a.LineVisible).Return(lineVisible);

            mocks.ReplayAll();

            var properties = new PolygonChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(closed, properties.Closed);
            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(transparency, properties.Transparency);
            Assert.AreEqual(useHatch, properties.UseHatch);
            Assert.AreEqual(hatchStyle, properties.HatchStyle);
            Assert.AreEqual(hatchColor, properties.HatchColor);
            Assert.AreEqual(lineColor, properties.LineColor);
            Assert.AreEqual(lineWidth, properties.LineWidth);
            Assert.AreEqual(lineVisible, properties.LineVisible);

            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_CallsSetters()
        {
            // Setup
            var mocks = new MockRepository();
            var random = new Random(21);
            var closed = false;
            var color = Color.AliceBlue;
            var transparency = random.Next(1, 10);
            var useHatch = false;
            var hatchStyle = HatchStyle.Cross;
            var hatchColor = Color.AliceBlue;
            var lineColor = Color.AliceBlue;
            var lineWidth = random.Next(1, 10);
            var lineVisible = false;
            var chartSeries = mocks.StrictMock<IPolygonChartSeries>();

            chartSeries.Expect(a => a.AutoClose).SetPropertyWithArgument(closed);
            chartSeries.Expect(a => a.Color).SetPropertyWithArgument(color);
            chartSeries.Expect(a => a.Transparency).SetPropertyWithArgument(transparency);
            chartSeries.Expect(a => a.UseHatch).SetPropertyWithArgument(useHatch);
            chartSeries.Expect(a => a.HatchStyle).SetPropertyWithArgument(hatchStyle);
            chartSeries.Expect(a => a.HatchColor).SetPropertyWithArgument(hatchColor);
            chartSeries.Expect(a => a.LineColor).SetPropertyWithArgument(lineColor);
            chartSeries.Expect(a => a.LineWidth).SetPropertyWithArgument(lineWidth);
            chartSeries.Expect(a => a.LineVisible).SetPropertyWithArgument(lineVisible);

            mocks.ReplayAll();

            // Call
            new PolygonChartSeriesProperties
            {
                Data = chartSeries,
                Closed = closed,
                Color = color,
                Transparency = transparency,
                UseHatch = useHatch,
                HatchStyle = hatchStyle,
                HatchColor = hatchColor,
                LineColor = lineColor,
                LineWidth = lineWidth,
                LineVisible = lineVisible
            };

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_Always_ReturnsTwelveProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<IPolygonChartSeries>();

            mocks.ReplayAll();

            var bag = new DynamicPropertyBag(new PolygonChartSeriesProperties
            {
                Data = data
            });

            // Call
            var properties = bag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(12, properties.Count);
        }
    }
}