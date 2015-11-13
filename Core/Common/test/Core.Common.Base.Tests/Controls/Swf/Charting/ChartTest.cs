using System;
using System.IO;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Controls.Swf.Charting.Series;
using Core.Common.TestUtils;
using Core.Common.Utils.Collections;
using NUnit.Framework;

namespace Core.Common.Base.Tests.Controls.Swf.Charting
{
    [TestFixture]
    public class ChartTest
    {
        [Test]
        public void AddingSeriesToChartTriggersChartCollectionChanged()
        {
            var chart = new Chart();
            var count = 0;

            ((INotifyCollectionChanged) chart).CollectionChanged += (s, e) => { count++; };

            chart.Series.Add(new AreaChartSeries());

            Assert.AreEqual(1, count);
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
            chart.Series.Add(areaSeries);

            TestHelper.AssertLogMessageIsGenerated(() => SaveDeleteAndAssertExport("test.svg", chart), "Hatch stijl wordt niet ondersteund voor exporteren en zal genegeerd worden.", 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Argument bevat geen bestandsnaam\r\nParameter name: filename")]
        public void ExportAsImageThrowsOnIncompleteFileName()
        {
            SaveDeleteAndAssertExport(".noname");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Extensie (.ext) wordt niet ondersteund\r\nParameter name: filename")]
        public void ExportAsImageThrowsOnUnSupportedExtension()
        {
            SaveDeleteAndAssertExport("incorrect.ext");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Argument kan niet de waarde 'null' hebben\r\nParameter name: filename")]
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