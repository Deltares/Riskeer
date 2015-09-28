using System;
using System.IO;
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
        public void AddingSeriesToChartTriggersChartCollectionChanged()
        {
            var chart = new Chart();
            var count = 0;

            ((INotifyCollectionChanged)chart).CollectionChanged += (s, e) => { count++; };

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