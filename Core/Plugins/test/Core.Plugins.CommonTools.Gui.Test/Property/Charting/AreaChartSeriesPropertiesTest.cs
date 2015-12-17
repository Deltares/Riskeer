using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Gui.Test.Property.Charting
{
    [TestFixture]
    public class AreaChartSeriesPropertiesTest
    {
        [Test]
        public void InterpolationType_WithData_SameAsData()
        {
            // Setup
            var someInterpolationType = InterpolationType.Constant;
            var otherInterpolationType = InterpolationType.Linear;

            var chartSeries = new AreaChartSeries
            {
                InterpolationType = someInterpolationType
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someInterpolationType, properties.InterpolationType);

            // Call
            properties.InterpolationType = otherInterpolationType;

            // Assert
            Assert.AreEqual(otherInterpolationType, chartSeries.InterpolationType);
        }

        [Test]
        public void Color_WithData_SameAsData()
        {
            // Setup
            var someColor = Color.AliceBlue;
            var otherColor = Color.AntiqueWhite;

            var chartSeries = new AreaChartSeries
            {
                Color = someColor
            };
            var properties = new AreaChartSeriesProperties
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
        public void Transparency_WithData_SameAsData()
        {
            // Setup
            var someTransparency = 0;
            var otherTransparency = 1;

            var chartSeries = new AreaChartSeries
            {
                Transparency = someTransparency
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someTransparency, properties.Transparency);

            // Call
            properties.Transparency = otherTransparency;

            // Assert
            Assert.AreEqual(otherTransparency, chartSeries.Transparency);
        }

        [Test]
        public void UseHatch_WithData_SameAsData()
        {
            // Setup
            var someHatch = false;
            var otherHatch = true;

            var chartSeries = new AreaChartSeries
            {
                UseHatch = someHatch
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someHatch, properties.UseHatch);

            // Call
            properties.UseHatch = otherHatch;

            // Assert
            Assert.AreEqual(otherHatch, chartSeries.UseHatch);
        }

        [Test]
        public void HatchStyle_WithData_SameAsData()
        {
            // Setup
            var someHatchStyle = HatchStyle.BackwardDiagonal;
            var otherHatchStyle = HatchStyle.Cross;

            var chartSeries = new AreaChartSeries
            {
                HatchStyle = someHatchStyle
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someHatchStyle, properties.HatchStyle);

            // Call
            properties.HatchStyle = otherHatchStyle;

            // Assert
            Assert.AreEqual(otherHatchStyle, chartSeries.HatchStyle);
        }

        [Test]
        public void HatchColor_WithData_SameAsData()
        {
            // Setup
            var someHatchColor = Color.AliceBlue;
            var otherHatchColor = Color.AntiqueWhite;

            var chartSeries = new AreaChartSeries
            {
                HatchColor = someHatchColor
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someHatchColor, properties.HatchColor);

            // Call
            properties.HatchColor = otherHatchColor;

            // Assert
            Assert.AreEqual(otherHatchColor, chartSeries.HatchColor);
        }

        [Test]
        public void LineColor_WithData_SameAsData()
        {
            // Setup
            var someLineColor = Color.AliceBlue;
            var otherLineColor = Color.AntiqueWhite;

            var chartSeries = new AreaChartSeries
            {
                LineColor = someLineColor
            };
            var properties = new AreaChartSeriesProperties
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
        public void LineWidth_WithData_SameAsData()
        {
            // Setup
            var someLineWidth = 2;
            var otherLineWidth = 3;

            var chartSeries = new AreaChartSeries
            {
                LineWidth = someLineWidth
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(someLineWidth, properties.LineWidth);

            // Call
            properties.LineWidth = otherLineWidth;

            // Assert
            Assert.AreEqual(otherLineWidth, chartSeries.LineWidth);
        }

        [Test]
        public void LineVisible_WithData_SameAsData()
        {
            // Setup
            var someLineVisible = false;
            var otherLineVisible = true;

            var chartSeries = new AreaChartSeries
            {
                LineVisible = someLineVisible
            };
            var properties = new AreaChartSeriesProperties
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
        public void PointerColor_WithData_SameAsData()
        {
            // Setup
            var somePointerColor = Color.AliceBlue;
            var otherPointerColor = Color.AntiqueWhite;

            var chartSeries = new AreaChartSeries
            {
                PointerColor = somePointerColor
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(somePointerColor, properties.PointerColor);

            // Call
            properties.PointerColor = otherPointerColor;

            // Assert
            Assert.AreEqual(otherPointerColor, chartSeries.PointerColor);
        }

        [Test]
        public void PointerStyle_WithData_SameAsData()
        {
            // Setup
            var somePointerStyle = PointerStyles.Circle;
            var otherPointerStyle = PointerStyles.Diamond;

            var chartSeries = new AreaChartSeries
            {
                PointerStyle = somePointerStyle
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(somePointerStyle, properties.PointerStyle);

            // Call
            properties.PointerStyle = otherPointerStyle;

            // Assert
            Assert.AreEqual(otherPointerStyle, chartSeries.PointerStyle);
        }

        [Test]
        public void PointerVisible_WithData_SameAsData()
        {
            // Setup
            var somePointerVisible = true;
            var otherPointerVisible = false;

            var chartSeries = new AreaChartSeries
            {
                PointerVisible = somePointerVisible
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(somePointerVisible, properties.PointerVisible);

            // Call
            properties.PointerVisible = otherPointerVisible;

            // Assert
            Assert.AreEqual(otherPointerVisible, chartSeries.PointerVisible);
        }

        [Test]
        public void PointerSize_WithData_SameAsData()
        {
            // Setup
            var somePointerSize = 2;
            var otherPointerSize = 3;

            var chartSeries = new AreaChartSeries
            {
                PointerSize = somePointerSize
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(somePointerSize, properties.PointerSize);

            // Call
            properties.PointerSize = otherPointerSize;

            // Assert
            Assert.AreEqual(otherPointerSize, chartSeries.PointerSize);
        }

        [Test]
        public void PointerLineColor_WithData_SameAsData()
        {
            // Setup
            var somePointerLineColor = Color.AliceBlue;
            var otherPointerLineColor = Color.AntiqueWhite;

            var chartSeries = new AreaChartSeries
            {
                PointerLineColor = somePointerLineColor
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(somePointerLineColor, properties.PointerLineColor);

            // Call
            properties.PointerLineColor = otherPointerLineColor;

            // Assert
            Assert.AreEqual(otherPointerLineColor, chartSeries.PointerLineColor);
        }

        [Test]
        public void PointerLineVisible_WithData_SameAsData()
        {
            // Setup
            var somePointerLineVisible = true;
            var otherPointerLineVisible = false;

            var chartSeries = new AreaChartSeries
            {
                PointerLineVisible = somePointerLineVisible
            };
            var properties = new AreaChartSeriesProperties
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(somePointerLineVisible, properties.PointerLineVisible);

            // Call
            properties.PointerLineVisible = otherPointerLineVisible;

            // Assert
            Assert.AreEqual(otherPointerLineVisible, chartSeries.PointerLineVisible);
        }
    }
}