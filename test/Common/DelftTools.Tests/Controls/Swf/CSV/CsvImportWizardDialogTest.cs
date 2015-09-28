using System;
using System.Collections.Generic;
using DelftTools.Controls.Swf.Csv;
using DelftTools.TestUtils;
using DelftTools.Utils.Csv.Importer;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.CSV
{
    [TestFixture]
    public class CsvImportWizardDialogTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void Show()
        {
            var dialog = new TestCsvImportWizardDialog();
            WindowsFormsTestHelper.ShowModal(dialog);
        }

        private class TestCsvImportWizardDialog : CsvImportWizardDialog
        {
            protected override IEnumerable<CsvRequiredField> GetRequiredFields()
            {
                yield return new CsvRequiredField("Datetime", typeof(DateTime));
                yield return new CsvRequiredField("Value (m AD)", typeof(double));
                yield return new CsvRequiredField("Test (m AD)", typeof(double));
                yield return new CsvRequiredField("Blah (m AD)", typeof(double));
            }

            protected override void OnUserFinishedMapping(string filePath, CsvMappingData mappingData)
            {
            }
        }
    }
}