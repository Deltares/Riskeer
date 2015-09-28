using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DeltaShell.Plugins.CommonTools.Gui.Forms.Charting;
using NUnit.Framework;

namespace DeltaShell.Plugins.CommonTools.Tests.Forms.Charting
{
    [TestFixture]
    public class ChartLegendViewTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowChartLegendView()
        {
            var legendView = new ChartLegendView(null);
            WindowsFormsTestHelper.ShowModal(legendView);
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void ShowChartLegendViewWithLineSeriesData()
        {
            var chart = new Chart {Title = "Chart 1"};

            var lineSeries1 = new LineChartSeries { Title = "Line series 1", Color = Color.ForestGreen };
            var lineSeries2 = new LineChartSeries { Title = "Line series 2", Color = Color.Firebrick };

            var range = Enumerable.Range(0, 50).Select(n => (double)n / 10).ToArray();
            var xValues = range.Cast<double?>().ToArray();

            lineSeries1.Add(xValues.ToArray(), range.Select(Math.Sin).Cast<double?>().ToArray());
            lineSeries2.Add(xValues.ToArray(), range.Select(Math.Cos).Cast<double?>().ToArray());
            
            chart.Series.AddRange(new []{ lineSeries1, lineSeries2 });

            WindowsFormsTestHelper.ShowModal(CreateChartLegendViewWithChartView(null, chart));
        }
        
        [Test, Category(TestCategory.WindowsForms)]
        public void ShowChartLegendViewWithAreaSeriesData()
        {
            var chart = new Chart { Title = "Chart 1" };
            chart.Legend.Visible = true;

            var series1 = new AreaChartSeries { Title = "Series 1", LineColor = Color.ForestGreen };
            var series2 = new AreaChartSeries { Title = "Series 2", LineColor = Color.Firebrick };
            var series3 = new AreaChartSeries { Title = "Series 3", LineColor = Color.DodgerBlue };
            var series4 = new AreaChartSeries { Title = "Series 4", LineColor = Color.DarkOrange };
            var series5 = new AreaChartSeries { Title = "Series 5", LineColor = Color.Tomato };

            var range = Enumerable.Range(0, 65).Select(n => (double)n / 10).ToArray();

            foreach (var value in range)
            {
                series1.Add(value, Math.Sin(value) + 1);
                series2.Add(value, Math.Sin(value + 1) + 1);
                series3.Add(value, Math.Sin(value + 2) + 1);
                series4.Add(value, Math.Sin(value + 3) + 1);
                series5.Add(value, Math.Sin(value + 4) + 1);
            }

            chart.Series.AddRange(new[] { series1, series2, series3, series4, series5 });

            WindowsFormsTestHelper.ShowModal(CreateChartLegendViewWithChartView(null, chart));
        }

        private static Control CreateChartLegendViewWithChartView(GuiPlugin guiPlugin, IChart chart)
        {
            var chartView = new ChartView
                                { 
                                    Chart = chart,
                                    Dock = DockStyle.Fill
                                };

            var chartLegendView = new ChartLegendView(guiPlugin)
                                      {
                                          Data = chart, 
                                          Dock = DockStyle.Right
                                      };

            var control = new Control{Size = new Size(1200, 600)};
            
            control.Controls.Add(chartView);
            control.Controls.Add(chartLegendView);

            return control;
        }
    }
}