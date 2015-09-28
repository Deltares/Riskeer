using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Charting
{
    [TestFixture]
    public class ChartViewTest
    {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            LogHelper.ConfigureLogging();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogHelper.ResetLogging();
        }
        

        #endregion

        private static DataTable InitTable()
        {
            var dataTable = new DataTable("ShowTableWithValues");
            dataTable.Columns.Add("Y", typeof (double));
            dataTable.Columns.Add("Z", typeof (double));
            dataTable.Columns.Add("n", typeof (float));

            var y = new[] {  0.0,   5.0,  6.0,  7.0,  3.0,   0.0};
            var z = new[] {  0.0,  10.0, 15.0, 21.0, 15.0,   0.0};
            var n = new []{0.001, 0.001, 0.01, 0.01, 0.01, 0.001};

            for (int i = 0; i < 6; i++)
            {
                var row = dataTable.NewRow();

                row["Y"] = y[i];
                row["Z"] = z[i];
                row["n"] = n[i];

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
        
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void HidingPointWithZeroY()
        {
            var view = new ChartView();
            var series = ChartSeriesFactory.CreateLineSeries();
            
            series.NoDataValues.Add(1.0);

            series.Add(new double?[] { 0.0, 1.0, 3.0, 4.0, 5.0, 7.0, 8.0}, 
                       new double?[] { 0.0, 1.0, 0.5, 0.0, 5.0, 3.0, 4.0 });
            
            view.Chart.Series.Add(series);

            WindowsFormsTestHelper.ShowModal(view);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowFourSeriesTypesWithTheRightColors()
        {
            var chartView = new ChartView();

            var areaSeries = ChartSeriesFactory.CreateAreaSeries();
            var barSeries = ChartSeriesFactory.CreateBarSeries();
            var lineSeries = ChartSeriesFactory.CreateLineSeries();
            var pointSeries = ChartSeriesFactory.CreatePointSeries();

            var dataTable = new DataTable("ShowTableWithValues");

            dataTable.Columns.Add("X", typeof(double));
            dataTable.Columns.Add("area", typeof(double));
            dataTable.Columns.Add("line", typeof(double));
            dataTable.Columns.Add("point", typeof(double));
            dataTable.Columns.Add("bar", typeof(double));

            var x = Enumerable.Range(1, 65).Select(v => (double)v/10.0).ToArray();

            foreach (var xValue in x)
            {
                var row = dataTable.NewRow();

                row["X"] = xValue;
                row["area"] = Math.Sin(xValue) + 1;
                row["line"] = Math.Sin(xValue + 1) + 3;
                row["point"] = Math.Sin(xValue + 2) + 5;
                row["bar"] = Math.Sin(xValue + 3) + 7;

                dataTable.Rows.Add(row);
            }

            areaSeries.DataSource = dataTable;
            barSeries.DataSource = dataTable;
            lineSeries.DataSource = dataTable;
            pointSeries.DataSource = dataTable;

            areaSeries.XValuesDataMember = "X";
            barSeries.XValuesDataMember = "X";
            lineSeries.XValuesDataMember = "X";
            pointSeries.XValuesDataMember = "X";

            areaSeries.YValuesDataMember = "area";
            barSeries.YValuesDataMember = "bar";
            lineSeries.YValuesDataMember = "line";
            pointSeries.YValuesDataMember = "point";

            areaSeries.Color = Color.Red;
            areaSeries.HatchColor = Color.LemonChiffon;
            areaSeries.HatchStyle = HatchStyle.WideDownwardDiagonal;
            
            barSeries.Color = Color.ForestGreen;

            lineSeries.Color = Color.Blue;
            lineSeries.Width = 3;
            lineSeries.PointerVisible = false;

            pointSeries.Size = 4;
            pointSeries.Color = Color.Yellow;
            pointSeries.LineColor = Color.DodgerBlue;

            chartView.Chart.Series.AddRange(new[] {barSeries, pointSeries, lineSeries,areaSeries});
            chartView.Chart.Legend.Visible = true;
            chartView.Chart.Legend.ShowCheckBoxes = true;

            WindowsFormsTestHelper.ShowModal(chartView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void PolygonSeriesViewShowsTwoPolygonsInOneSeries()
        {
            var chartView = new ChartView();

            var series = new PolygonChartSeries
            {
                HatchStyle = HatchStyle.BackwardDiagonal,
                UseHatch = true,
                HatchColor = Color.Firebrick,
                LineColor = Color.BlueViolet,
                LineWidth = 3,
                LineVisible = true,
                Color = Color.Green,
                Title = "test",
                DefaultNullValue = 10.1, // Recommended to be in axes limits to prevent wrong scaling of axis
                AutoClose = true
            };

            var x = new double?[] { 10, 5, 15, null, 20,21, double.NaN ,22 };
            var y = new double?[] { 15, 10.0, 10, null, 1, 5, 3, 1 };

            series.Add(x, y);

            chartView.Chart.Series.Add(series);
            chartView.Chart.Legend.Visible = true;
            WindowsFormsTestHelper.ShowModal(chartView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void PolygonSeriesView()
        {
            var chartView = new ChartView();
            
            var series = new PolygonChartSeries
                             {
                                 HatchStyle = HatchStyle.BackwardDiagonal,
                                 UseHatch = true,
                                 HatchColor = Color.Firebrick,
                                 LineColor = Color.BlueViolet,
                                 LineWidth = 3,
                                 LineVisible = true,
                                 Color = Color.Green,
                                 Title = "test",
                                 AutoClose = true
                             };

            var x = new double?[] {0.0, 10, 5, 10};
            var y = new double?[] {0.0, -5, 0, 5};
            
            series.Add(x,y);

            var series2 = new PolygonChartSeries
            {
                UseHatch = false,
                LineColor = Color.Black,
                LineWidth = 3,
                LineVisible = true,
                Color = Color.DodgerBlue,
                Transparency = 70,
                Title = "test 2",
            };

            var x2 = new double?[] { 0.0, 1, 2, 3, 4, 3, 2, 1, 0 };
            var y2 = new double?[] {0.0, -5, 0, 5, 0, -5, 0, 5, 0};

            series2.Add(x2, y2);

            var series3 = new PolygonChartSeries
                              {
                                  UseHatch = false,
                                  LineColor = Color.Black,
                                  LineStyle = DashStyle.Dash,
                                  LineWidth = 2,
                                  LineVisible = true,
                                  Color = Color.OrangeRed,
                                  Title = "balloon",
                              };

            var x3 = new double?[] { 0.0, 1, 1.1, 1.2, 1.1, 1, 0.5, 0.4, 0.5, 1, 1.2 };
            var y3 = new double?[] { 0.0, 1, 2, 3, 4, 5, 6, 4, 3, 2, 0, -5 };

            series3.Add(x3, y3);

            chartView.Chart.Series.Add(series);
            chartView.Chart.Series.Add(series2);
            chartView.Chart.Series.Add(series3);
            chartView.Chart.Legend.Visible = true;
            chartView.Chart.Legend.ShowCheckBoxes = true;
            WindowsFormsTestHelper.ShowModal(chartView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void AreaSeriesView()
        {
            var chartView = new ChartView();

            var series = ChartSeriesFactory.CreateAreaSeries();
            var dataTable = new DataTable("ShowTableWithValues");

            dataTable.Columns.Add("Y", typeof(double));
            dataTable.Columns.Add("Z", typeof(double));

            var y = new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 };
            var z = new[] { 0.0, 10.0, 15.0, 21.0, 15.0, 0.0 };

            for (int i = 0; i < 6; i++)
            {
                var row = dataTable.NewRow();

                row["Y"] = y[i];
                row["Z"] = z[i];

                dataTable.Rows.Add(row);
            }

            series.DataSource = dataTable;
            series.XValuesDataMember = "Y";
            series.YValuesDataMember = "Z";

            chartView.Chart.Series.Add(series);
            chartView.Chart.Legend.Visible = true;

            WindowsFormsTestHelper.ShowModal(chartView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SeriesBandToolView()
        {
           var chartView = new ChartView();

            var dataTable1 = new DataTable();
            var dataTable2 = new DataTable();

            dataTable1.Columns.AddRange(new []
                                            {
                                                new DataColumn("Y", typeof(double)),
                                                new DataColumn("Z", typeof(double))
                                            });
            dataTable2.Columns.AddRange(new[]
                                            {
                                                new DataColumn("Y", typeof(double)),
                                                new DataColumn("Z", typeof(double))
                                            });

            var ySeries1 = new[] {0.0, 2.0,   5.0,  10.0, 13.0, 15.0};
            var zSeries1 = new[] {0.0, 0.0, -10.0, -10.0,  0.0,  0.0};
            var ySeries2 = new[] {0.0, 5.0,  5.0, 10.0, 10.0, 15.0};
            var zSeries2 = new[] {1.0, 1.0, -9.0, -9.0,  1.0,  1.0};

            for (int i = 0; i < ySeries1.Length; i++)
            {
                var row = dataTable1.NewRow();
                row["Y"] = ySeries1[i];
                row["Z"] = zSeries1[i];
                dataTable1.Rows.Add(row);
            }

            for (int i = 0; i < ySeries2.Length; i++)
            {
                var row = dataTable2.NewRow();
                row["Y"] = ySeries2[i];
                row["Z"] = zSeries2[i];
                dataTable2.Rows.Add(row);
            }

            var series1 = ChartSeriesFactory.CreateLineSeries();
            var series2 = ChartSeriesFactory.CreateLineSeries();

            series1.DataSource = dataTable1;
            series1.XValuesDataMember = "Y";
            series1.YValuesDataMember = "Z";
            
            series2.DataSource = dataTable2;
            series2.XValuesDataMember = "Y";
            series2.YValuesDataMember = "Z";
            
            chartView.Chart.Series.AddRange(new []{series1, series2});
            
            //tool
            var tool = chartView.NewSeriesBandTool(series1, series2, Color.Yellow, HatchStyle.BackwardDiagonal,Color.Red);
            chartView.Tools.Add(tool);

            WindowsFormsTestHelper.ShowModal(chartView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void TestStackedAreaSeries()
        {
            var chartView = new ChartView();

            var dataTable1 = new DataTable();
            var dataTable2 = new DataTable();

            dataTable1.Columns.AddRange(new[]
                                            {
                                                new DataColumn("Y", typeof (double)),
                                                new DataColumn("Z", typeof (double))
                                            });
            dataTable2.Columns.AddRange(new[]
                                            {
                                                new DataColumn("Y", typeof (double)),
                                                new DataColumn("Z", typeof (double))
                                            });

            var ySeries1 = new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0};
            var zSeries1 = new[] {0.0, 1.0, 2.0, 3.0, 2.0, 1.0, 0.0};

            var ySeries2 = new[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 };
            var zSeries2 = new[] { 3.0, 2.0, 1.0, 0.0, 1.0, 2.0, 3.0 };

            for (int i = 0; i < ySeries1.Length; i++)
            {
                var row = dataTable1.NewRow();
                row["Y"] = ySeries1[i];
                row["Z"] = zSeries1[i];
                dataTable1.Rows.Add(row);
            }

            for (int i = 0; i < ySeries2.Length; i++)
            {
                var row = dataTable2.NewRow();
                row["Y"] = ySeries2[i];
                row["Z"] = zSeries2[i];
                dataTable2.Rows.Add(row);
            }

            var series1 = ChartSeriesFactory.CreateAreaSeries();
            var series2 = ChartSeriesFactory.CreateAreaSeries();

            series1.DataSource = dataTable1;
            series1.XValuesDataMember = "Y";
            series1.YValuesDataMember = "Z";

            series2.DataSource = dataTable2;
            series2.XValuesDataMember = "Y";
            series2.YValuesDataMember = "Z";

            chartView.Chart.Series.AddRange(new[] {series1, series2});

            chartView.Chart.StackSeries = true;

            WindowsFormsTestHelper.ShowModal(chartView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ChangeYMemberSeriesView()
        {
            var chartView = new ChartView();

            var series = ChartSeriesFactory.CreateLineSeries();

            series.DataSource = InitTable();
            series.XValuesDataMember = "Y";
            series.YValuesDataMember = "Z";
            
            chartView.Chart.Series.Add(series);

            series.YValuesDataMember = "n";
            series.CheckDataSource();

            WindowsFormsTestHelper.ShowModal(chartView);
        }

        private static IChart CreateMultipleSeriesChart()
        {
            IChart chart = new Chart();

            var dataTable = new DataTable("sinandcosinus");
            dataTable.Columns.Add("X", typeof(double));
            dataTable.Columns.Add("sin", typeof(double));
            dataTable.Columns.Add("cos", typeof(double));

            const double pi2 = Math.PI * 2;

            for (int i = 0; i < 100; i++)
            {
                double angle = i * (pi2 / 100);
                DataRow row = dataTable.NewRow();
                row["X"] = angle;
                row["sin"] = Math.Sin(angle);
                row["cos"] = Math.Cos(angle);
                dataTable.Rows.Add(row);
            }

            ILineChartSeries sin = ChartSeriesFactory.CreateLineSeries();
            sin.Title = "sinus";
            sin.DataSource = dataTable;
            sin.XValuesDataMember = "X";
            sin.YValuesDataMember = "sin";
            chart.Series.Add(sin);
            sin.Color = Color.Red;
            sin.PointerVisible = false;

            ILineChartSeries cos = ChartSeriesFactory.CreateLineSeries();
            cos.Title = "cosinus";
            cos.DataSource = dataTable;
            cos.XValuesDataMember = "X";
            cos.YValuesDataMember = "cos";
            chart.Series.Add(cos);
            cos.Color = Color.Blue;
            cos.PointerVisible = false;

            chart.Legend.Visible = true;
            return chart;
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void MultipleSeriesView()
        {
            WindowsFormsTestHelper.ShowModal(new ChartView
                                                 {
                                                     Chart = CreateMultipleSeriesChart()
                                                 });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowDisabledChartView()
        {
            WindowsFormsTestHelper.ShowModal(new ChartView
                                                 {
                                                     Chart = CreateMultipleSeriesChart(),
                                                     Enabled = false
                                                 });
        }
        
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void MultipleSeriesExtraAxesView()
        {
            var chart = CreateMultipleSeriesChart();
            var chartSeries = chart.Series[0];
            
            chartSeries.VertAxis = VerticalAxis.Right;

            WindowsFormsTestHelper.ShowModal(new ChartView { Chart = chart });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ApplyCustomDateTimeFormatSeconds()
        {
            var random = new Random();
            var chartView = new ChartView();
            var startTime = DateTime.Now;
            
            var times = Enumerable.Range(1, 1000).Select(i => startTime.AddSeconds(i)).ToArray();
            var y = Enumerable.Range(1000, 1000).Select(i => Convert.ToDouble(random.Next(100))).ToArray();

            var pointList = new List<DelftTools.Utils.Tuple<DateTime, double>>();
            var lineSeries = ChartSeriesFactory.CreateLineSeries();
            var chart = chartView.Chart;

            chart.Series.Add(lineSeries);

            for (int i = 0; i < 1000; i++)
            {
                pointList.Add(new DelftTools.Utils.Tuple<DateTime, double>(times[i], y[i]));
            }

            lineSeries.DataSource = pointList;
            lineSeries.XValuesDataMember = "First";
            lineSeries.YValuesDataMember = "Second";
            lineSeries.CheckDataSource();
            WindowsFormsTestHelper.ShowModal(chartView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ApplyCustomDateTimeFormatYears()
        {
            var random = new Random();
            var chartView = new ChartView();
            var startTime = DateTime.Now;
            
            var times = Enumerable.Range(1, 1000).Select(startTime.AddYears).ToArray();
            var y = Enumerable.Range(1000, 1000).Select(i => Convert.ToDouble(random.Next(100))).ToArray();

            var pointList = new List<DelftTools.Utils.Tuple<DateTime, double>>();
            var lineSeries = ChartSeriesFactory.CreateLineSeries();
            var chart = chartView.Chart;

            chart.Series.Add(lineSeries);

            chartView.DateTimeLabelFormatProvider = new QuarterNavigatableLabelFormatProvider();

            for (int i = 0; i < 1000; i++)
            {
                pointList.Add(new DelftTools.Utils.Tuple<DateTime, double>(times[i], y[i]));
            }

            lineSeries.DataSource = pointList;
            lineSeries.XValuesDataMember = "First";
            lineSeries.YValuesDataMember = "Second";
            lineSeries.CheckDataSource();
            WindowsFormsTestHelper.ShowModal(chartView);
        }
    }
}