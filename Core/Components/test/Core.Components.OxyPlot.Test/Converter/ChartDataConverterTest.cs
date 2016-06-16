// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
            var tuple = Tuple.Create(a,b);
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
            var classResult = testConverter.CanConvertSeries(new Class("test data"));
            var childResult = testConverter.CanConvertSeries(new Child("test data"));

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

        private class Class : ChartData
        {
            public Class(string name) : base(name) {}
        }

        private class Child : Class
        {
            public Child(string name) : base(name) { }
        }

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