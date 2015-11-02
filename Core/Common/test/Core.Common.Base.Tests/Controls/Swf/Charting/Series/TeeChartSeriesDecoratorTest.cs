using System;
using System.Linq;
using Core.Common.Controls.Swf.Charting.Series;
using NUnit.Framework;

namespace Core.Common.Base.Tests.Controls.Swf.Charting.Series
{
    [TestFixture]
    public class TeeChartSeriesDecoratorTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Invalid argument for series datasource. Are you passing IEnumerable? IList and IListSource are supported")]
        public void ThrowExceptionOnSettingInvalidDataSource()
        {
            ILineChartSeries lineChartSeries = ChartSeriesFactory.CreateLineSeries();
            lineChartSeries.DataSource = Enumerable.Range(1, 3);
        }
    }
}