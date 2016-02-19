﻿using System;
using System.Collections.ObjectModel;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
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
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreNotSame(points, data.Points);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewICharData()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            var data = new PointData(points);

            // Assert
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreNotSame(points, data.Points);
            CollectionAssert.AreEqual(points, data.Points);
        }

        private Collection<Tuple<double, double>> CreateTestPoints()
        {
            return new Collection<Tuple<double, double>>
            {
                Tuple.Create(0.0, 1.1),    
                Tuple.Create(1.0, 2.1),
                Tuple.Create(1.6, 1.6)    
            };
        }
    }
}