using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Gui.PropertyBag;
using Core.Plugins.Charting.Property;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Charting.Test.Property
{
    [TestFixture]
    public class LineChartSeriesPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new LineChartSeriesProperties();

            // Assert
            Assert.IsInstanceOf<ChartSeriesProperties<ILineChartSeries>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var random = new Random(21);
            var interpolationType = InterpolationType.Linear;
            var titleLabelVisible = false;
            var color = Color.AliceBlue;
            var width = random.Next(1, 10);
            var dashStyle = DashStyle.Dash;
            var pointerSize = random.Next(1,10);
            var pointerLineColor = Color.AliceBlue;
            var pointerStyle = PointerStyles.SmallDot;
            var pointerVisible = false;
            var pointerLineVisible = false;
            var chartSeries = mocks.StrictMock<ILineChartSeries>();

            chartSeries.Expect(a => a.InterpolationType).Return(interpolationType);
            chartSeries.Expect(a => a.TitleLabelVisible).Return(titleLabelVisible);
            chartSeries.Expect(a => a.Color).Return(color);
            chartSeries.Expect(a => a.Width).Return(width);
            chartSeries.Expect(a => a.DashStyle).Return(dashStyle);
            chartSeries.Expect(a => a.PointerSize).Return(pointerSize);
            chartSeries.Expect(a => a.PointerLineColor).Return(pointerLineColor);
            chartSeries.Expect(a => a.PointerStyle).Return(pointerStyle);
            chartSeries.Expect(a => a.PointerVisible).Return(pointerVisible);
            chartSeries.Expect(a => a.PointerLineVisible).Return(pointerLineVisible);

            mocks.ReplayAll();

            var properties = new LineChartSeriesProperties{
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(interpolationType, properties.InterpolationType);
            Assert.AreEqual(titleLabelVisible, properties.TitleLabelVisible);
            Assert.AreEqual(color, properties.Color);
            Assert.AreEqual(width, properties.Width);
            Assert.AreEqual(dashStyle, properties.DashStyle);
            Assert.AreEqual(pointerSize, properties.PointerSize);
            Assert.AreEqual(pointerLineColor, properties.PointerLineColor);
            Assert.AreEqual(pointerStyle, properties.PointerStyle);
            Assert.AreEqual(pointerVisible, properties.PointerVisible);
            Assert.AreEqual(pointerLineVisible, properties.PointerLineVisible);

            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_CallsSetters()
        {
            // Setup
            var mocks = new MockRepository();
            var random = new Random(21);
            var interpolationType = InterpolationType.Linear;
            var titleLabelVisible = false;
            var color = Color.AliceBlue;
            var width = random.Next(1, 10);
            var dashStyle = DashStyle.Dash;
            var pointerSize = random.Next(1, 10);
            var pointerLineColor = Color.AliceBlue;
            var pointerStyle = PointerStyles.SmallDot;
            var pointerVisible = false;
            var pointerLineVisible = false;
            var chartSeries = mocks.StrictMock<ILineChartSeries>();

            chartSeries.Expect(a => a.InterpolationType).SetPropertyWithArgument(interpolationType);
            chartSeries.Expect(a => a.TitleLabelVisible).SetPropertyWithArgument(titleLabelVisible);
            chartSeries.Expect(a => a.Color).SetPropertyWithArgument(color);
            chartSeries.Expect(a => a.Width).SetPropertyWithArgument(width);
            chartSeries.Expect(a => a.DashStyle).SetPropertyWithArgument(dashStyle);
            chartSeries.Expect(a => a.PointerSize).SetPropertyWithArgument(pointerSize);
            chartSeries.Expect(a => a.PointerLineColor).SetPropertyWithArgument(pointerLineColor);
            chartSeries.Expect(a => a.PointerStyle).SetPropertyWithArgument(pointerStyle);
            chartSeries.Expect(a => a.PointerVisible).SetPropertyWithArgument(pointerVisible);
            chartSeries.Expect(a => a.PointerLineVisible).SetPropertyWithArgument(pointerLineVisible);

            mocks.ReplayAll();

            // Call
            new LineChartSeriesProperties{
                Data = chartSeries,
                InterpolationType = interpolationType,
                TitleLabelVisible = titleLabelVisible,
                Color = color,
                Width = width,
                DashStyle = dashStyle,
                PointerSize = pointerSize,
                PointerLineColor = pointerLineColor,
                PointerStyle = pointerStyle,
                PointerVisible = pointerVisible,
                PointerLineVisible = pointerLineVisible
            };

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_Always_ReturnsFourteenProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<ILineChartSeries>();

            mocks.ReplayAll();

            var bag = new DynamicPropertyBag(new LineChartSeriesProperties
            {
                Data = data
            });

            // Call
            var properties = bag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(14, properties.Count);
        }
    }
}