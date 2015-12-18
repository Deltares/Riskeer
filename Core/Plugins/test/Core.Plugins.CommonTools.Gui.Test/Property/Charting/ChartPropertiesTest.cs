using System;
using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Gui;
using Core.Common.Utils.PropertyBag;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Property.Charting
{
    [TestFixture]
    public class ChartPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new ChartProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IChart>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var chartAxis = mocks.StrictMock<IChartAxis>();
            var chart = mocks.StrictMock<IChart>();
            var legend = mocks.StrictMock<IChartLegend>();

            var backgroundColor = Color.AliceBlue;
            var showLegend = false;
            var legendAlignment = LegendAlignment.Top;
            var legendFont = new Font(FontFamily.GenericSansSerif, 12);
            var title = "some title";
            var titleFont = new Font(FontFamily.GenericSansSerif, 12);
            var titleVisible = false;

            chart.Expect(a => a.BackGroundColor).Return(backgroundColor);
            chart.Expect(a => a.Title).Return(title);
            chart.Expect(a => a.Font).Return(titleFont);
            chart.Expect(a => a.TitleVisible).Return(titleVisible);
            chart.Expect(a => a.LeftAxis).Return(chartAxis).Repeat.Twice();
            chart.Expect(a => a.BottomAxis).Return(chartAxis).Repeat.Twice();
            chart.Expect(a => a.RightAxis).Return(chartAxis).Repeat.Twice();

            chart.Expect(a => a.Legend).Return(legend).Repeat.Times(3);
            legend.Expect(a => a.Visible).Return(showLegend);
            legend.Expect(a => a.Alignment).Return(legendAlignment);
            legend.Expect(a => a.Font).Return(legendFont);

            chartAxis.Expect(a => a.IsDateTime).Return(false).Repeat.Times(3);

            mocks.ReplayAll();

            var properties = new ChartProperties
            {
                Data = chart
            };

            // Call & Assert
            Assert.AreEqual(backgroundColor, properties.BackgroundColor);
            Assert.AreEqual(showLegend, properties.ShowLegend);
            Assert.AreEqual(legendAlignment, properties.LegendAlignment);
            Assert.AreEqual(legendFont, properties.LegendFont);
            Assert.AreEqual(title, properties.Title);
            Assert.AreEqual(titleFont, properties.TitleFont);
            Assert.AreEqual(titleVisible, properties.TitleVisibile);
            Assert.IsInstanceOf<ChartAxisProperties>(properties.LeftAxis);
            Assert.IsInstanceOf<ChartAxisProperties>(properties.BottomAxis);
            Assert.IsInstanceOf<ChartAxisProperties>(properties.RightAxis);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_Always_ReturnsTenProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<IChart>();
            var chartAxis = mocks.Stub<IChartAxis>();
            var legend = mocks.Stub<IChartLegend>();

            data.Expect(a => a.Legend).Return(legend).Repeat.AtLeastOnce();
            data.Expect(a => a.LeftAxis).Return(chartAxis).Repeat.AtLeastOnce();
            data.Expect(a => a.BottomAxis).Return(chartAxis).Repeat.AtLeastOnce();
            data.Expect(a => a.RightAxis).Return(chartAxis).Repeat.AtLeastOnce();

            mocks.ReplayAll();

            var bag = new DynamicPropertyBag(new ChartProperties
            {
                Data = data
            });

            // Call
            var properties = bag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(10, properties.Count);
        }
    }
}