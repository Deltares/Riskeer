using System;
using System.Collections.Generic;
using System.Globalization;
using DelftTools.TestUtils;
using DelftTools.Utils.Csv.Importer;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.CSV
{
    [TestFixture]
    public class CsvImportTest
    {
        [Test]
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
                { new CsvRequiredField("Value (m AD)", typeof (double)), new CsvColumnInfo(1, new NumberFormatInfo()) }
            };

            var typedDataTable = csvToDataTableConverter.Extract(dataTable, csvMapping);

            Assert.AreEqual(2, typedDataTable.Columns.Count);
            Assert.AreEqual(4, typedDataTable.Rows.Count);
            Assert.IsTrue(typedDataTable.Rows[0][0] is DateTime);
            Assert.AreEqual(10.005, typedDataTable.Rows[0][1]);
        }

        [Test]
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
                { new CsvRequiredField("Value (m AD)", typeof (double)), new CsvColumnInfo(1, new NumberFormatInfo()) }
            };
            var filteredDataTable = csvToDataTableConverter.Extract(dataTable, csvMapping, new[] { new CsvPassIfEqualFilter(0, "14/07/1990") });

            Assert.AreEqual(1, filteredDataTable.Columns.Count);
            Assert.AreEqual(1, filteredDataTable.Rows.Count);
            Assert.AreEqual(15.0, filteredDataTable.Rows[0][0]);
        }
    }
}
