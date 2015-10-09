using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DelftTools.Controls.Swf.Csv;
using DelftTools.Controls.Swf.WizardPages;
using DelftTools.Utils.Csv.Importer;
using log4net;

namespace DelftTools.Controls.Swf.Table
{
    public class TableViewCsvImportWizardDialog : CsvImportWizardDialog
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TableViewCsvImportWizardDialog));
        private readonly TableView tableView;
        private readonly bool importFromClipboard;
        private readonly string clipboardText;

        public TableViewCsvImportWizardDialog(TableView tableView, string clipboardText = null)
        {
            this.tableView = tableView;
            importFromClipboard = !string.IsNullOrEmpty(clipboardText);

            if (importFromClipboard)
            {
                this.clipboardText = clipboardText;

                // we're importing from file; don't bother with file select:
                var filePage = Pages.OfType<SelectFileWizardPage>().First();
                RemovePage(filePage);

                // prepare the preview:
                var csvSeparatorPage = Pages.OfType<CsvToDataTableWizardPage>().First();
                using (var reader = new StringReader(clipboardText))
                {
                    int numLines = 0;
                    string line;
                    var first30Lines = new StringBuilder();
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (++numLines > 30)
                        {
                            break;
                        }
                        first30Lines.AppendLine(line);
                    }
                    csvSeparatorPage.PreviewText = first30Lines.ToString();
                }
            }
        }

        public void DoImport(CsvMappingData mappingData, string filePath = null)
        {
            var importer = new CsvImporter();
            using (var stream = importFromClipboard ? (Stream) new MemoryStream(Encoding.UTF8.GetBytes(clipboardText)) : File.OpenRead(filePath))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var importedData = importer.SplitToTable(streamReader, mappingData.Settings);
                    importedData = importer.Extract(importedData, mappingData.FieldToColumnMapping, mappingData.Filters);

                    // delete all existing data
                    tableView.SelectRows(Enumerable.Range(0, tableView.RowCount).ToArray());
                    tableView.DeleteCurrentSelection();

                    var exceptionMode = tableView.ExceptionMode;
                    tableView.ExceptionMode = TableView.ValidationExceptionMode.ThrowException; //throw exception

                    try
                    {
                        tableView.BeginInit();
                        TableView.DoActionInEditAction(
                            tableView, importFromClipboard ? "Pasting values" : "Importing values",
                            () =>
                            {
                                // start importing data
                                int rowIndex = 0;
                                bool skipNextAdd = false;
                                foreach (DataRow row in importedData.Rows)
                                {
                                    if (!skipNextAdd)
                                    {
                                        tableView.AddNewRowToDataSource();
                                    }

                                    skipNextAdd = false;
                                    bool success;
                                    var message = "";
                                    try
                                    {
                                        success = tableView.SetRowCellValues(rowIndex, 0, row.ItemArray);
                                    }
                                    catch (Exception e)
                                    {
                                        message = e.Message;
                                        success = false;
                                    }

                                    if (!success)
                                    {
                                        Log.ErrorFormat("Unable to import row {0}. {1}", rowIndex,
                                                        string.IsNullOrEmpty(message) ? "" : message);
                                        skipNextAdd = true; // failed to set values, reuse row
                                        continue;
                                    }
                                    rowIndex++;
                                }
                            });
                        tableView.EndInit();
                    }
                    finally
                    {
                        tableView.ExceptionMode = exceptionMode;
                    }
                }
            }
        }

        protected override IEnumerable<CsvRequiredField> GetRequiredFields()
        {
            return
                tableView.Columns.Where(c => c.Visible)
                         .Select(column => new CsvRequiredField(column.Name, column.ColumnType));
        }

        protected override void OnUserFinishedMapping(string filePath, CsvMappingData mappingData)
        {
            DoImport(mappingData, filePath);
        }
    }
}