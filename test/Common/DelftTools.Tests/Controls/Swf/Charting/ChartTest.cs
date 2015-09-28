using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Charting
{
    [TestFixture]
    public class ChartTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ChartWithTitle()
        {
            var chartView1 = new ChartView {Title = "TestTitle"};

            var form = new Form { Width = 600, Height = 400 };
            form.Controls.Add(chartView1);
            WindowsFormsTestHelper.ShowModal(form);
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void AddingSeriesToChartTriggersChartCollectionChanged()
        {
            var chart = new Chart();
            var count = 0;

            ((INotifyCollectionChanged)chart).CollectionChanged += (s, e) => { count++; };

            chart.Series.Add(new AreaChartSeries());

            Assert.AreEqual(1, count);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ChartWithDataTableAsSeriesSource()
        {
            var table = new DataTable();
            table.Columns.Add("x", typeof (double));
            table.Columns.Add("y", typeof (double));

            table.Rows.Add(2.5, 33.3);
            table.Rows.Add(0.5, 13.3);

            // create chart and add function as a data source using object adapter class FunctionSeriesDataSource
            //IChart chart = new Chart();

            IChartSeries series = ChartSeriesFactory.CreateLineSeries();

            series.DataSource = table;
            series.XValuesDataMember = "x";
            series.YValuesDataMember = "y";

            var chartView1 = new ChartView() ;

            chartView1.Chart.Series.Add(series);

            var form = new Form {Width = 600, Height = 100};
            form.Controls.Add(chartView1);
            WindowsFormsTestHelper.ShowModal(form);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ChartWithObjectsAsSeriesSource()
        {
            IList objects = new ArrayList
                                {
                                    new {X = 2.5, Y = 33.3}, 
                                    new {X = 0.5, Y = 13.3}
                                };

            // create chart and add function as a data source using object adapter class FunctionSeriesDataSource

            IChartSeries series = ChartSeriesFactory.CreateLineSeries();
            series.DataSource = objects;
            series.XValuesDataMember = "X";
            series.YValuesDataMember = "Y";

            var chartView1 = new ChartView();

            chartView1.Chart.Series.Add(series);

            // show form
            var form = new Form {Width = 600, Height = 100};
            form.Controls.Add(chartView1);
            WindowsFormsTestHelper.ShowModal(form);
        }

        [Test]
        public void ExportAsImageWorks()
        {
            SaveDeleteAndAssertExport("test.png");
        }

        [Test]
        public void ExportAsVectorGraphicsImageWorks()
        {
            SaveDeleteAndAssertExport("test.svg");
        }

        [Test]
        public void ExportAsVectorGraphicsImagesGivesWarningForIgnoringHatchStyle()
        {
            var chart = new Chart();
            var areaSeries = new AreaChartSeries { UseHatch = true };
            chart.Series.Add(areaSeries);

            TestHelper.AssertLogMessageIsGenerated(() => SaveDeleteAndAssertExport("test.svg", chart), "Hatch style is not supported for exports and will be ignored.", 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Argument did not contain a filename\r\nParameter name: filename")]
        public void ExportAsImageThrowsOnIncompleteFileName()
        {
            SaveDeleteAndAssertExport(".noname");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Extension (.ext) not supported\r\nParameter name: filename")]
        public void ExportAsImageThrowsOnUnSupportedExtension()
        {
            SaveDeleteAndAssertExport("incorrect.ext");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Argument should not be null\r\nParameter name: filename")]
        public void ExportAsImageThrowsOnNullArgument()
        {
            SaveDeleteAndAssertExport(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Argument should have an extension\r\nParameter name: filename")]
        public void ExportAsImageThrowsOnMissingExtension()
        {
            SaveDeleteAndAssertExport("noextension");
        }

        private static void SaveDeleteAndAssertExport(string exportFileName, IChart chart = null)
        {
            try
            {
                if (chart == null)
                {
                    new Chart().ExportAsImage(exportFileName,null,null);
                }
                else
                {
                    chart.ExportAsImage(exportFileName, null, null);
                }
                Assert.IsTrue(File.Exists(exportFileName));
            }
            finally
            {
                if (File.Exists(exportFileName))
                {
                    File.Delete(exportFileName);
                }
            }
        }
    }
}