using System;
using System.IO;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Controls.Swf.Charting.Series;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Controls.Swf.Test.Charting
{
    [TestFixture]
    public class ChartTest
    {
        [Test]
        public void ChartReferenceSetToChartSeriesAfterAddingItToChart()
        {
            var chart = new Chart();
            var chartSeries = new AreaChartSeries();

            Assert.IsNull(chartSeries.Chart);

            chart.AddChartSeries(chartSeries);

            Assert.AreSame(chart, chartSeries.Chart);
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
            var areaSeries = new AreaChartSeries
            {
                UseHatch = true
            };
            chart.AddChartSeries(areaSeries);

            TestHelper.AssertLogMessageIsGenerated(() => SaveDeleteAndAssertExport("test.svg", chart), "Gearceerde stijl wordt niet ondersteund voor exporteren en zal genegeerd worden.", 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Argument bevat geen bestandsnaam.\r\nParameter name: filename")]
        public void ExportAsImageThrowsOnIncompleteFileName()
        {
            SaveDeleteAndAssertExport(".noname");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Extensie (.ext) wordt niet ondersteund.\r\nParameter name: filename")]
        public void ExportAsImageThrowsOnUnSupportedExtension()
        {
            SaveDeleteAndAssertExport("incorrect.ext");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Argument kan niet de waarde 'null' hebben.\r\nParameter name: filename")]
        public void ExportAsImageThrowsOnNullArgument()
        {
            SaveDeleteAndAssertExport(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Bestandsnaam moet een extensie hebben.\r\nParameter name: filename")]
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
                    new Chart().ExportAsImage(exportFileName, null, null);
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