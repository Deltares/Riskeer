using System;
using System.Linq;
using Core.Common.Controls.Swf.Charting.Series;
using NUnit.Framework;

namespace Core.Common.Controls.Swf.Test.Charting.Series
{
    [TestFixture]
    public class TeeChartSeriesDecoratorTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Ongeldig argument voor databron van gegevensreeks. Biedt u een IEnumerable aan? IList en IListSource worden ondersteund.")]
        public void ThrowExceptionOnSettingInvalidDataSource()
        {
            ILineChartSeries lineChartSeries = ChartSeriesFactory.CreateLineSeries();
            lineChartSeries.DataSource = Enumerable.Range(1, 3);
        }
    }
}