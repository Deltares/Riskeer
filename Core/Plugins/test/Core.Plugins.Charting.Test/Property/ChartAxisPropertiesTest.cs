using System;
using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.Charting;
using Core.Common.Utils.PropertyBag;
using Core.Plugins.Charting.Property;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Charting.Test.Property
{
    [TestFixture]
    public class ChartAxisPropertiesTest
    {
        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var visible = true;
            var automatic = true;
            var title = "some title";
            var labels = true;
            var labelsFont = new Font(FontFamily.GenericSansSerif, 12);
            var titleFont = new Font(FontFamily.GenericSansSerif, 12);

            var chartAxis = mocks.StrictMock<IChartAxis>();
            chartAxis.Expect(a => a.Visible).Return(visible);
            chartAxis.Expect(a => a.Automatic).Return(automatic);
            chartAxis.Expect(a => a.Title).Return(title);
            chartAxis.Expect(a => a.Labels).Return(labels);
            chartAxis.Expect(a => a.LabelsFont).Return(labelsFont);
            chartAxis.Expect(a => a.TitleFont).Return(titleFont);

            mocks.ReplayAll();

            var properties = new TestChartAxisProperties(chartAxis);

            // Call & Assert
            Assert.AreEqual(visible, properties.Visible);
            Assert.AreEqual(automatic, properties.Automatic);
            Assert.AreEqual(title, properties.Title);
            Assert.AreEqual(labels, properties.Labels);
            Assert.AreEqual(labelsFont, properties.LabelsFont);
            Assert.AreEqual(titleFont, properties.TitleFont);

            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_CallsSetters()
        {
            // Setup
            var mocks = new MockRepository();
            var visible = true;
            var automatic = true;
            var title = "some title";
            var labels = true;
            var labelsFont = new Font(FontFamily.GenericSansSerif, 12);
            var titleFont = new Font(FontFamily.GenericSansSerif, 12);

            var chartAxis = mocks.StrictMock<IChartAxis>();
            chartAxis.Expect(a => a.Visible).SetPropertyWithArgument(visible);
            chartAxis.Expect(a => a.Automatic).SetPropertyWithArgument(automatic);
            chartAxis.Expect(a => a.Title).SetPropertyWithArgument(title);
            chartAxis.Expect(a => a.Labels).SetPropertyWithArgument(labels);
            chartAxis.Expect(a => a.LabelsFont).SetPropertyWithArgument(labelsFont);
            chartAxis.Expect(a => a.TitleFont).SetPropertyWithArgument(titleFont);
            mocks.ReplayAll();

            // Call
            new TestChartAxisProperties(chartAxis)
            {
                Visible = visible,
                Automatic = automatic,
                Title = title,
                Labels = labels,
                LabelsFont = labelsFont,
                TitleFont = titleFont
            };

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_Always_ReturnsSixProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var chartAxis = mocks.Stub<IChartAxis>();

            mocks.ReplayAll();

            var bag = new DynamicPropertyBag(new TestChartAxisProperties(chartAxis));

            // Call
            var properties = bag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(6, properties.Count);
        }
    }

    public class TestChartAxisProperties : ChartAxisProperties
    {
        public TestChartAxisProperties(IChartAxis chartAxis) : base(chartAxis) {}
    }
}