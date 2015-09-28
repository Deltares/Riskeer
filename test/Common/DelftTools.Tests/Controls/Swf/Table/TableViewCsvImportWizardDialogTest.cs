using System.Collections.Generic;
using System.Data;
using System.Globalization;
using DelftTools.Controls.Swf.Table;
using DelftTools.Utils.Csv.Importer;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Table
{
    [TestFixture]
    public class TableViewCsvImportWizardDialogTest
    {
        [Test]
        public void ImportDataFromClipboard()
        {
            var tableView = new TableView();
            var dataTable = new DataTable();
            dataTable.Columns.Add("Foo", typeof (int));
            dataTable.Columns.Add("Bar", typeof (int));
            tableView.Data = dataTable;

            var dialog = new TableViewCsvImportWizardDialog(tableView, "0\t0\n" +
                                                                       "1\t2\n");

            Assert.AreEqual(0, tableView.RowCount);

            dialog.DoImport(new CsvMappingData
                {
                    Settings = new CsvSettings {Delimiter = '\t', FirstRowIsHeader = false, SkipEmptyLines = true},
                    Filters = new CsvFilter[0],
                    FieldToColumnMapping = new Dictionary<CsvRequiredField, CsvColumnInfo>
                        {
                            {
                                new CsvRequiredField("Foo", typeof (int)),
                                new CsvColumnInfo(0, CultureInfo.CurrentCulture.NumberFormat)
                            },
                            {
                                new CsvRequiredField("Bar", typeof (int)),
                                new CsvColumnInfo(1, CultureInfo.CurrentCulture.NumberFormat)
                            }
                        },
                });

            Assert.AreEqual(2, tableView.RowCount);
        }
    }
}