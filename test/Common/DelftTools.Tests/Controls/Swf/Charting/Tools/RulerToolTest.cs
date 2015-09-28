using System;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Charting.Tools
{
    [TestFixture]
    public class RulerToolTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void UseToolWithDoubleAxes()
        {
            var chartView = new ChartView();
            var tool = chartView.NewRulerTool();
            tool.Active = true; 
            tool.Enabled = true;

            var lineChartSeries = new LineChartSeries();
            lineChartSeries.Add(0.0, 1);
            lineChartSeries.Add(1.0, 3);
            lineChartSeries.Add(2.0, 2);
            lineChartSeries.Add(3.0, 3.13245);
            lineChartSeries.Add(4.0, 1.478901);
            chartView.Chart.Series.Add(lineChartSeries);
            WindowsFormsTestHelper.ShowModal(chartView);
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void UseToolWithTimeAxes()
        {
            var chartView = new ChartView();
            var tool = chartView.NewRulerTool();
            tool.Active = true;
            tool.Enabled = true;
            tool.Cursor = Cursors.Cross;

            var lineChartSeries = new LineChartSeries();
            lineChartSeries.Add(new DateTime(2010,1,1), 1);
            lineChartSeries.Add(new DateTime(2010, 1, 2), 3);
            lineChartSeries.Add(new DateTime(2010, 1, 3), 2);
            lineChartSeries.Add(new DateTime(2010, 1, 4), 3.13245);
            lineChartSeries.Add(new DateTime(2010, 1, 5), 1.478901);
            chartView.Chart.Series.Add(lineChartSeries);
            WindowsFormsTestHelper.ShowModal(chartView);
        }
    }
}
