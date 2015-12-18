﻿using System;
using Core.Common.Controls.Charting;
using Core.Common.Gui;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Property.Charting
{
    [TestFixture]
    public class ChartSeriesPropertiesTest
    {
        [Test]
        public void Constructor_WithAxis_ExpectedValues()
        {
            // Call
            var mocks = new MockRepository();
            var chartSeries = mocks.StrictMock<IChartSeries>();
            var properties = new ChartSeriesProperties<IChartSeries>
            {
                Data = chartSeries
            };

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IChartSeries>>(properties);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var showInLegend = false;
            var title = "some title";
            var verticalAxis = new VerticalAxis();
            var chartSeries = mocks.StrictMock<IChartSeries>();
            chartSeries.Expect(a => a.ShowInLegend).Return(showInLegend);
            chartSeries.Expect(a => a.Title).Return(title);
            chartSeries.Expect(a => a.VertAxis).Return(verticalAxis);

            mocks.ReplayAll();

            var properties = new ChartSeriesProperties<IChartSeries>
            {
                Data = chartSeries
            };

            // Call & Assert
            Assert.AreEqual(showInLegend, properties.ShowInLegend);
            Assert.AreEqual(title, properties.Title);
            Assert.AreEqual(verticalAxis, properties.VerticalAxis);

            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_WithData_CallsSetters()
        {
            // Setup
            var mocks = new MockRepository();
            var chartSeries = mocks.StrictMock<IChartSeries>();
            var showInLegend = false;
            var title = "some title";
            var verticalAxis = new VerticalAxis();
            chartSeries.Expect(a => a.ShowInLegend).SetPropertyWithArgument(showInLegend);
            chartSeries.Expect(a => a.Title).SetPropertyWithArgument(title);
            chartSeries.Expect(a => a.VertAxis).SetPropertyWithArgument(verticalAxis);

            mocks.ReplayAll();

            // Call
            new ChartSeriesProperties<IChartSeries>
            {
                Data = chartSeries,
                ShowInLegend = showInLegend,
                Title = title,
                VerticalAxis = verticalAxis
            };

            // Assert
            mocks.VerifyAll();
        }
         
    }
}