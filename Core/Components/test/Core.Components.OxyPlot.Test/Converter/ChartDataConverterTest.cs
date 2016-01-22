using System;
using System.Collections.Generic;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartDataConverterTest
    {
        [Test]
        public void TupleToDataPoint_RandomTupleDoubleDouble_ReturnsDataPoint()
        {
            // Setup
            var random = new Random(21);
            var a = random.NextDouble();
            var b = random.NextDouble();
            var tuple = new Tuple<double,double>(a,b);
            var testConverter = new TestChartDataConverter<ChartData>();

            // Call
            var point = testConverter.PublicTupleToDataPoint(tuple);

            // Assert
            Assert.AreEqual(a, point.X);
            Assert.AreEqual(b, point.Y);
        }

        [Test]
        public void CanConvertSeries_DifferentInherritingTypes_OnlySupportsExactType()
        {
            // Setup
            var testConverter = new TestChartDataConverter<Class>();

            // Call
            var chartDataResult = testConverter.CanConvertSeries(new TestChartData());
            var classResult = testConverter.CanConvertSeries(new Class());
            var childResult = testConverter.CanConvertSeries(new Child());

            // Assert
            Assert.IsFalse(chartDataResult);
            Assert.IsTrue(classResult);
            Assert.IsFalse(childResult);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestChartDataConverter<Class>();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new TestChartDataConverter<Class>();
            var testChartData = new TestChartData();
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testChartData.GetType());
            // Precondition
            Assert.IsFalse(testConverter.CanConvertSeries(testChartData));

            // Call
            TestDelegate test = () => testConverter.Convert(testChartData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        private class Class : ChartData {}

        private class Child : Class {}

        private class TestChartDataConverter<T> : ChartDataConverter<T> where T : ChartData
        {
            protected override IList<Series> Convert(T data)
            {
                throw new NotImplementedException();
            }

            public DataPoint PublicTupleToDataPoint(object obj)
            {
                return TupleToDataPoint(obj);
            }
        }
    }
}