using System;
using System.Collections.ObjectModel;
using Core.Components.OxyPlot.Data;
using NUnit.Framework;

namespace Core.Components.OxyPlot.Test.Data
{
    [TestFixture]
    public class CollectionDataTest
    {
        [Test]
        public void DefaultConstructor_NewInstanceOfIChartData()
        {
            // Call
            var data = new CollectionData();

            // Assert
            Assert.IsInstanceOf<IChartData>(data);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(11)]
        public void GivenANumberOfChartData_WhenAddedToChart_AddsSameNumberOfSeriesToModel(int numberOfSeries)
        {
            // Given
            var chart = new BaseChart();
            var data = new CollectionData();
            for (int i = 0; i < numberOfSeries; i++)
            {
                data.Add(new LineData(new Collection<Tuple<double,double>>()));
            }

            // When
            chart.AddData(data);

            // Assert
            Assert.AreEqual(numberOfSeries, chart.Model.Series.Count);
        }
    }
}