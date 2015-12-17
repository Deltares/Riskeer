using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Property.Charting
{
    [TestFixture]
    public class AreaChartSeriesPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new AreaChartSeriesProperties();

            // Assert
            Assert.IsInstanceOf<ChartSeriesProperties<IAreaChartSeries>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var someInterpolationType = InterpolationType.Constant;
            var someColor = Color.FromArgb(255, 240, 240, 222); // Using Color constant fails, probably due to how Area.Color works (in AreaChartSeries.Color).
            var someTransparency = 0;
            var someUseHatch = false;
            var someHatchStyle = HatchStyle.BackwardDiagonal;
            var someHatchColor = Color.AliceBlue;
            var someLineColor = Color.AliceBlue;
            var someLineWidth = 2;
            var someLineVisible = false;
            var somePointerColor = Color.AliceBlue;
            var somePointerStyle = PointerStyles.Circle;
            var somePointerVisible = false;
            var somePointerSize = 2;
            var somePointerLineColor = Color.AliceBlue;
            var somePointerLineVisible = false;

            var chartSeries = new AreaChartSeries
            {
                InterpolationType = someInterpolationType,
                Color = someColor,
                Transparency = someTransparency,
                UseHatch = someUseHatch,
                HatchStyle = someHatchStyle,
                HatchColor = someHatchColor,
                LineColor = someLineColor,
                LineWidth = someLineWidth,
                LineVisible = someLineVisible,
                PointerColor = somePointerColor,
                PointerStyle = somePointerStyle,
                PointerVisible = somePointerVisible,
                PointerSize = somePointerSize,
                PointerLineColor = somePointerLineColor,
                PointerLineVisible = somePointerLineVisible
            };

            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someInterpolationType, properties.InterpolationType);
            Assert.AreEqual(someColor, properties.Color);
            Assert.AreEqual(someTransparency, properties.Transparency);
            Assert.AreEqual(someUseHatch, properties.UseHatch);
            Assert.AreEqual(someHatchStyle, properties.HatchStyle);
            Assert.AreEqual(someHatchColor, properties.HatchColor);
            Assert.AreEqual(someLineColor, properties.LineColor);
            Assert.AreEqual(someLineWidth, properties.LineWidth);
            Assert.AreEqual(someLineVisible, properties.LineVisible);
            Assert.AreEqual(somePointerColor, properties.PointerColor);
            Assert.AreEqual(somePointerStyle, properties.PointerStyle);
            Assert.AreEqual(somePointerVisible, properties.PointerVisible);
            Assert.AreEqual(somePointerSize, properties.PointerSize);
            Assert.AreEqual(somePointerLineColor, properties.PointerLineColor);
            Assert.AreEqual(somePointerLineVisible, properties.PointerLineVisible);
        }

        [Test]
        public void SetProperties_WithData_CallsSetters()
        {
            // Setup
            var mocks = new MockRepository();
            var chartSeries = mocks.StrictMock<IAreaChartSeries>();

            var someInterpolationType = InterpolationType.Constant;
            var someColor = Color.FromArgb(255, 240, 240, 222); // Using Color constant fails, probably due to how Area.Color works (in AreaChartSeries.Color).
            var someTransparency = 0;
            var someUseHatch = false;
            var someHatchStyle = HatchStyle.BackwardDiagonal;
            var someHatchColor = Color.AliceBlue;
            var someLineColor = Color.AliceBlue;
            var someLineWidth = 2;
            var someLineVisible = false;
            var somePointerColor = Color.AliceBlue;
            var somePointerStyle = PointerStyles.Circle;
            var somePointerVisible = false;
            var somePointerSize = 2;
            var somePointerLineColor = Color.AliceBlue;
            var somePointerLineVisible = false;

            chartSeries.Expect(cs => cs.InterpolationType).SetPropertyWithArgument(someInterpolationType);
            chartSeries.Expect(cs => cs.Color).SetPropertyWithArgument(someColor);
            chartSeries.Expect(cs => cs.Transparency).SetPropertyWithArgument(someTransparency);
            chartSeries.Expect(cs => cs.UseHatch).SetPropertyWithArgument(someUseHatch);
            chartSeries.Expect(cs => cs.HatchStyle).SetPropertyWithArgument(someHatchStyle);
            chartSeries.Expect(cs => cs.HatchColor).SetPropertyWithArgument(someHatchColor);
            chartSeries.Expect(cs => cs.LineColor).SetPropertyWithArgument(someLineColor);
            chartSeries.Expect(cs => cs.LineWidth).SetPropertyWithArgument(someLineWidth);
            chartSeries.Expect(cs => cs.LineVisible).SetPropertyWithArgument(someLineVisible);
            chartSeries.Expect(cs => cs.PointerColor).SetPropertyWithArgument(somePointerColor);
            chartSeries.Expect(cs => cs.PointerStyle).SetPropertyWithArgument(somePointerStyle);
            chartSeries.Expect(cs => cs.PointerVisible).SetPropertyWithArgument(somePointerVisible);
            chartSeries.Expect(cs => cs.PointerSize).SetPropertyWithArgument(somePointerSize);
            chartSeries.Expect(cs => cs.PointerLineColor).SetPropertyWithArgument(somePointerLineColor);
            chartSeries.Expect(cs => cs.PointerLineVisible).SetPropertyWithArgument(somePointerLineVisible);

            mocks.ReplayAll();

            // Call
            new AreaChartSeriesProperties
            {
                Data = chartSeries,
                InterpolationType = someInterpolationType,
                Color = someColor,
                Transparency = someTransparency,
                UseHatch = someUseHatch,
                HatchStyle = someHatchStyle,
                HatchColor = someHatchColor,
                LineColor = someLineColor,
                LineWidth = someLineWidth,
                LineVisible = someLineVisible,
                PointerColor = somePointerColor,
                PointerStyle = somePointerStyle,
                PointerVisible = somePointerVisible,
                PointerSize = somePointerSize,
                PointerLineColor = somePointerLineColor,
                PointerLineVisible = somePointerLineVisible
            };

            // Assert
            mocks.VerifyAll();
        }
    }
}