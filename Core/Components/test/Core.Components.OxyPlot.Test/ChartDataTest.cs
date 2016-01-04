using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test
{
    [TestFixture]
    public class ChartDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartData(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithEmptyPoints_DoesNotThrow()
        {
            // Setup
            var points = new Collection<Tuple<double, double>>();

            // Call
            TestDelegate test = () => new ChartData(points);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Constructor_WithPoints_DoesNotThrow()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            TestDelegate test = () => new ChartData(points);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AddTo_NoModel_ThrowsArgumentNullException()
        {
            // Setup
            var points = CreateTestPoints();
            var testData = new ChartData(points);

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
            var testData = new ChartData(points);
            var model = new PlotModel();

            // Call
            testData.AddTo(model);

            // Assert
            Assert.AreEqual(1, model.Series.Count);
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