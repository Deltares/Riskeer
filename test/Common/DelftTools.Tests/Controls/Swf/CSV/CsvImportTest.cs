using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Csv;
using DelftTools.TestUtils;
using DelftTools.Utils.Csv.Importer;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.CSV
{
    [TestFixture]
    public class CsvImportTest
    {
        [Test]
        [Category(TestCategory.DataAccess)]
        public void ConvertCsvFile()
        {
            var csvToDataTableConverter = new CsvImporter();
            
            var dataTable = csvToDataTableConverter.SplitToTable(TestHelper.GetTestFilePath("Timeseries.csv"),
                                                          new CsvSettings
                                                              {
                                                                  Delimiter = ',',
                                                                  FirstRowIsHeader = true,
                                                                  SkipEmptyLines = true
                                                              });

            var customDTFormat = (DateTimeFormatInfo) CultureInfo.InvariantCulture.DateTimeFormat.Clone();
            customDTFormat.FullDateTimePattern = "dd/MM/yyyy";

            var csvMapping = new Dictionary<CsvRequiredField, CsvColumnInfo>
            {
                { new CsvRequiredField("Date time", typeof (DateTime)), new CsvColumnInfo(0, customDTFormat) },
                { new CsvRequiredField("Value (m AD)", typeof (double)), new CsvColumnInfo(1, new NumberFormatInfo()) },
            };

            var typedDataTable = csvToDataTableConverter.Extract(dataTable, csvMapping);

            Assert.AreEqual(2, typedDataTable.Columns.Count);
            Assert.AreEqual(4, typedDataTable.Rows.Count);
            Assert.IsTrue(typedDataTable.Rows[0][0] is DateTime);
            Assert.AreEqual(10.005, typedDataTable.Rows[0][1]);
        }

        [Test]
        [Category(TestCategory.DataAccess)]
        public void ParseCsvFileAndFilterColumn()
        {
            var csvToDataTableConverter = new CsvImporter();

            var dataTable = csvToDataTableConverter.SplitToTable(TestHelper.GetTestFilePath("Timeseries.csv"),
                                                          new CsvSettings
                                                          {
                                                              Delimiter = ',',
                                                              FirstRowIsHeader = true,
                                                              SkipEmptyLines = true
                                                          });

            var csvMapping = new Dictionary<CsvRequiredField, CsvColumnInfo>
            {
                { new CsvRequiredField("Value (m AD)", typeof (double)), new CsvColumnInfo(1, new NumberFormatInfo()) },
            };
            var filteredDataTable = csvToDataTableConverter.Extract(dataTable, csvMapping, new[] { new CsvPassIfEqualFilter(0, "14/07/1990") });

            Assert.AreEqual(1, filteredDataTable.Columns.Count);
            Assert.AreEqual(1, filteredDataTable.Rows.Count);
            Assert.AreEqual(15.0, filteredDataTable.Rows[0][0]);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowDialogWithColumnSelectionAndFiltering()
        {
            var dataTable = new DataTable();
            
            dataTable.Columns.Add(new DataColumn("Time", typeof (string)));
            dataTable.Columns.Add(new DataColumn("Info", typeof (string)));
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)));

            dataTable.Rows.Add(new object[] {"01-02-2000", "blah", 1.0.ToString()});
            dataTable.Rows.Add(new object[] {"01-03-2000", "blah2", 3.434.ToString()});
            dataTable.Rows.Add(new object[] {"01-04-2000", "blah3", 5.0.ToString()});

            var selectionControl = new CsvDataSelectionControl {Dock = DockStyle.Fill};

            selectionControl.SetData(dataTable, new[]
                {
                    new CsvRequiredField("Date time", typeof (DateTime)),
                    new CsvRequiredField("Water level (m AD)", typeof (double))
                });

            var mapping = selectionControl.FieldToColumnMapping;
            var filters = selectionControl.Filters;

            Assert.IsNotNull(mapping);
            Assert.IsNotNull(filters);

            var form = new Form {Width = 600, Height = 800};
            form.Controls.Add(selectionControl);
            WindowsFormsTestHelper.ShowModal(form);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        [Category(TestCategory.DataAccess)]
        public void ShowDialogForActualFile()
        {
            var csvToDataTableConverter = new CsvImporter();

            var dataTable = csvToDataTableConverter.SplitToTable(TestHelper.GetTestFilePath("Timeseries.csv"),
                                                          new CsvSettings
                                                          {
                                                              Delimiter = ',',
                                                              FirstRowIsHeader = true,
                                                              SkipEmptyLines = true
                                                          });

            var pageTwo = new CsvDataSelectionControl();
            pageTwo.SetData(dataTable, new[]
                {
                    new CsvRequiredField("Datetime", typeof (DateTime)),
                    new CsvRequiredField("Value", typeof (double))
                });

            pageTwo.Dock = DockStyle.Fill;
            var form = new Form { Width = 600, Height = 800 };
            form.Controls.Add(pageTwo);
            WindowsFormsTestHelper.ShowModal(form);
        }
    }
}
