using System;
using System.Collections.ObjectModel;
using Core.Components.OxyPlot.Data;
using NUnit.Framework;

namespace Core.Components.OxyPlot.Test.Data
{
    [TestFixture]
    public class PointDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PointData(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewICharData()
        {
            // Setup
            var points = new Collection<Tuple<double, double>>();

            // Call
            var data = new PointData(points);

            // Assert
            Assert.IsInstanceOf<IChartData>(data);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewICharData()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            var data = new PointData(points);

            // Assert
            Assert.IsInstanceOf<IChartData>(data);
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