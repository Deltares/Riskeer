﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Components.OxyPlot.Data;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Data
{
    [TestFixture]
    public class AreaDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new AreaData(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewICharData()
        {
            // Setup
            var points = new Collection<Tuple<double, double>>();

            // Call
            var data = new AreaData(points);

            // Assert
            Assert.IsInstanceOf<IChartData>(data);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewICharData()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            var data = new AreaData(points);

            // Assert
            Assert.IsInstanceOf<IChartData>(data);
        }

        [Test]
        public void AddTo_NoModel_ThrowsArgumentNullException()
        {
            // Setup
            var points = CreateTestPoints();
            var testData = new AreaData(points);

            // Call
            TestDelegate test = () => testData.AddTo(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void AddTo_Model_DataAddedToModelAsSeries()
        {
            // Setup
            var points = CreateTestPoints();
            var testData = new AreaData(points);
            var model = new PlotModel();

            // Call
            testData.AddTo(model);

            // Assert
            Assert.AreEqual(1, model.Series.Count);
            Assert.IsInstanceOf<AreaSeries>(model.Series.First());

            var series = (AreaSeries)model.Series.First();
            var equivalentPoints = points.Select(p => new DataPoint(p.Item1, p.Item2)).ToArray();
            CollectionAssert.AreEquivalent(equivalentPoints, series.Points);
            CollectionAssert.AreEquivalent(Arrays.CopyOf(equivalentPoints,1), series.Points2);
        }
        [Test]
        public void GivenBaseChart_WhenPointDataAdded_ThenSeriesHasPointStyle()
        {
            // Given
            var chart = new BaseChart();
            var pointData = new AreaData(new Collection<Tuple<double, double>>());

            // When
            chart.AddData(pointData);

            // Then
            Assert.IsInstanceOf<AreaSeries>(chart.Model.Series.First());
        }

        private Collection<Tuple<double, double>> CreateTestPoints()
        {
            return new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),    
                new Tuple<double, double>(1.0, 2.1),
                new Tuple<double, double>(1.6, 1.6)    
            };
        }
    }
} 