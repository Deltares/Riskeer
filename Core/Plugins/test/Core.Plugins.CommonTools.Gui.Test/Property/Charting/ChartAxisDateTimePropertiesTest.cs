using System;
using Core.Common.Controls.Charting;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Property.Charting
{
    [TestFixture]
    public class ChartAxisDateTimePropertiesTest
    {
        [Test]
        public void Maximum_WithData_GetFromData()
        {
            // Setup
            var mocks = new MockRepository();
            var chartAxis = mocks.StrictMock<IChartAxis>();
            var oaDate = new Random(21).NextDouble();
            chartAxis.Expect(a => a.Maximum).Return(oaDate);

            mocks.ReplayAll();

            var properties = new ChartAxisDateTimeProperties(chartAxis);

            // Call & Assert
            Assert.AreEqual(DateTime.FromOADate(oaDate), properties.Maximum);

            mocks.VerifyAll();
        }

        [Test]
        public void Maximum_WithData_SetToData()
        {
            // Setup
            var mocks = new MockRepository();
            var dateTime = DateTime.Parse("2015/12/17");
            var chartAxis = mocks.StrictMock<IChartAxis>();
            chartAxis.Expect(a => a.Maximum).SetPropertyWithArgument(dateTime.ToOADate());

            mocks.ReplayAll();

            var properties = new ChartAxisDateTimeProperties(chartAxis);

            // Call
            properties.Maximum = dateTime;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Minimum_WithData_GetFromData()
        {
            // Setup
            var mocks = new MockRepository();
            var chartAxis = mocks.StrictMock<IChartAxis>();
            var oaDate = new Random(21).NextDouble();
            chartAxis.Expect(a => a.Minimum).Return(oaDate);

            mocks.ReplayAll();

            var properties = new ChartAxisDateTimeProperties(chartAxis);

            // Call & Assert
            Assert.AreEqual(DateTime.FromOADate(oaDate), properties.Minimum);

            mocks.VerifyAll();
        }

        [Test]
        public void Minimum_WithData_SetToData()
        {
            // Setup
            var mocks = new MockRepository();
            var dateTime = DateTime.Parse("2015/12/17");
            var chartAxis = mocks.StrictMock<IChartAxis>();
            chartAxis.Expect(a => a.Minimum).SetPropertyWithArgument(dateTime.ToOADate());

            mocks.ReplayAll();

            var properties = new ChartAxisDateTimeProperties(chartAxis);

            // Call
            properties.Minimum = dateTime;

            // Assert
            mocks.VerifyAll();
        }
    }
}