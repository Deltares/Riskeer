using System.Collections;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Charting.Tools
{
    [TestFixture]
    public class SelectPointToolTest
    {
        private ChartView chartView;

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void Add()
        {
            var chart = new Chart();
            var lineSeries = ChartSeriesFactory.CreateLineSeries();

            lineSeries.DataSource = new ArrayList
                                        {
                                            new {X = 1, Y = 1},
                                            new {X = 2, Y = 3},
                                            new {X = 3, Y = 2},
                                            new {X = 4, Y = 4}
                                        };
            lineSeries.XValuesDataMember = "X";
            lineSeries.YValuesDataMember = "Y";

            chart.Series.Add(lineSeries);
            chartView = new ChartView { Chart = chart };
            chartView.NewSelectPointTool();
            //chartView.NewAddPointTool();
            WindowsFormsTestHelper.ShowModal(chartView);
        }      
    }
}