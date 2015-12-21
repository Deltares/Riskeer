using System;
using System.ComponentModel;
using Core.Common.Controls.Charting;
using Core.Common.Utils.PropertyBag;
using Core.Plugins.Charting.Property;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Charting.Test.Property
{
    [TestFixture]
    public class ChartAxisDateTimePropertiesTest
    {
        [Test]
        public void Constructor_WithAxis_ExpectedValues()
        {
            // Call
            var mocks = new MockRepository();
            var chartAxis = mocks.StrictMock<IChartAxis>();
            var properties = new ChartAxisDateTimeProperties(chartAxis);

            // Assert
            Assert.IsInstanceOf<ChartAxisProperties>(properties);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var random = new Random(21);
            var maximum = random.NextDouble();
            var minimum = random.NextDouble();
            var chartAxis = mocks.StrictMock<IChartAxis>();
            chartAxis.Expect(a => a.Maximum).Return(maximum);
            chartAxis.Expect(a => a.Minimum).Return(minimum);

            mocks.ReplayAll();

            var properties = new ChartAxisDateTimeProperties(chartAxis);

            // Call & Assert
            Assert.AreEqual(DateTime.FromOADate(maximum), properties.Maximum);
            Assert.AreEqual(DateTime.FromOADate(minimum), properties.Minimum);

            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_CallsSetters()
        {
            // Setup
            var mocks = new MockRepository();
            var chartAxis = mocks.StrictMock<IChartAxis>();
            var maximum = DateTime.Parse("2015/12/17");
            var minimum = DateTime.Parse("2015/12/3");
            chartAxis.Expect(a => a.Maximum).SetPropertyWithArgument(maximum.ToOADate());
            chartAxis.Expect(a => a.Minimum).SetPropertyWithArgument(minimum.ToOADate());

            mocks.ReplayAll();

            // Call
            new ChartAxisDateTimeProperties(chartAxis)
            {
                Maximum = maximum,
                Minimum = minimum
            };

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_Always_ReturnsEightProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var chartAxis = mocks.Stub<IChartAxis>();

            mocks.ReplayAll();

            var bag = new DynamicPropertyBag(new ChartAxisDateTimeProperties(chartAxis));

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