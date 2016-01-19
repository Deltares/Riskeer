using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var testConverter = new TestChartDataConverter(typeof(object));

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
            var testConverter = new TestChartDataConverter(typeof(Class));

            // Call
            var chartDataResult = testConverter.CanConvertSeries(new TestChartData());
            var classResult = testConverter.CanConvertSeries(new Class());
            var childResult = testConverter.CanConvertSeries(new Child());

            // Assert
            Assert.IsFalse(chartDataResult);
            Assert.IsTrue(classResult);
            Assert.IsFalse(childResult);
        }

        private class Class : ChartData {
            public Class() : base(new Collection<Tuple<double, double>>()) { }
        }

        private class Child : Class {}

        private class TestChartDataConverter : ChartDataConverter 
        {
            private readonly Type supportedType;

            public TestChartDataConverter(Type type)
            {
                supportedType = type;
            }

            protected override Type SupportedType
            {
                get
                {
                    return supportedType;
                }
            }

            internal override Series Convert(ChartData data)
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