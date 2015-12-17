using System;
using Core.Common.Controls.Charting;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Property.Charting
{
    [TestFixture]
    public class ChartAxisDoublePropertiesTest
    {
        [Test]
        public void Constructor_WithAxis_ExpectedValues()
        {
            // Call
            var mocks = new MockRepository();
            var chartAxis = mocks.StrictMock<IChartAxis>();
            var properties = new ChartAxisDoubleProperties(chartAxis);

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
            var logaritmic = true;
            var chartAxis = mocks.StrictMock<IChartAxis>();
            chartAxis.Expect(a => a.Maximum).Return(maximum);
            chartAxis.Expect(a => a.Minimum).Return(minimum);
            chartAxis.Expect(a => a.Logaritmic).Return(logaritmic);

            mocks.ReplayAll();

            var properties = new ChartAxisDoubleProperties(chartAxis);

            // Call & Assert
            Assert.AreEqual(maximum, properties.Maximum);
            Assert.AreEqual(minimum, properties.Minimum);
            Assert.AreEqual(logaritmic, properties.Logaritmic);

            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_CallsSetters()
        {
            // Setup
            var mocks = new MockRepository();
            var chartAxis = mocks.StrictMock<IChartAxis>();
            var random = new Random(21);
            var maximum = random.NextDouble();
            var minimum = random.NextDouble();
            var logaritmic = true;
            chartAxis.Expect(a => a.Maximum).SetPropertyWithArgument(maximum);
            chartAxis.Expect(a => a.Minimum).SetPropertyWithArgument(minimum);
            chartAxis.Expect(a => a.Logaritmic).SetPropertyWithArgument(logaritmic);

            mocks.ReplayAll();

            // Call
            new ChartAxisDoubleProperties(chartAxis)
            {
                Maximum = maximum,
                Minimum = minimum,
                Logaritmic = logaritmic
            };

            // Assert
            mocks.VerifyAll();
        }
    }
}