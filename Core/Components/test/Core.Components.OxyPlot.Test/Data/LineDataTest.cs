using System;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Components.OxyPlot.Data;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Data
{
    [TestFixture]
    public class LineDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new LineData(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewICharData()
        {
            // Setup
            var points = new Collection<Tuple<double, double>>();

            // Call
            var data = new LineData(points);

            // Assert
            Assert.IsInstanceOf<IChartData>(data);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewICharData()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            var data = new LineData(points);

            // Assert
            Assert.IsInstanceOf<IChartData>(data);
        }

        [Test]
        public void AddTo_NoModel_ThrowsArgumentNullException()
        {
            // Setup
            var points = CreateTestPoints();
            var testData = new LineData(points);

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
            var testData = new LineData(points);
            var model = new PlotModel();

            // Call
            testData.AddTo(model);

            // Assert
            Assert.AreEqual(1, model.Series.Count);
            Assert.IsInstanceOf<LineSeries>(model.Series.First());
            
            var series = (LineSeries)model.Series.First();
            Assert.AreSame(points, series.ItemsSource);
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