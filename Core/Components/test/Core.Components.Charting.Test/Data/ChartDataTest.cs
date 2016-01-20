using Core.Common.Base;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    [TestFixture]
    public class ChartDataTest
    {
        [Test]
        public void DefaultConstructor_Observable()
        {
            // Call
            var chartData = new TestChartData();

            // Assert
            Assert.IsInstanceOf<Observable>(chartData);
        } 
    }

    public class TestChartData : ChartData {}
}