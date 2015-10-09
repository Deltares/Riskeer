using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DelftTools.Controls.Swf.WizardPages;
using DelftTools.Utils.Csv.Importer;

namespace DelftTools.Controls.Swf.Csv
{
    public abstract partial class CsvImportWizardDialog : WizardDialog
    {
        private readonly SelectFileWizardPage fileSelectPage;
        private readonly CsvToDataTableWizardPage csvSeparatorPage;
        private readonly ICsvDataSelectionWizardPage csvMappingPage;

        public CsvImportWizardDialog()
        {
            InitializeComponent();

            Height = 768;

            WelcomeMessage = "This wizard will allow you to import CSV data";
            FinishedPageMessage = "Press Finish to close the wizard.";

            fileSelectPage = new SelectFileWizardPage
            {
                Filter = "CSV files|*.csv"
            };
            csvSeparatorPage = new CsvToDataTableWizardPage();
            csvMappingPage = CreateCsvMappingPage();

            AddPage(fileSelectPage, "Select CSV file", "Select the CSV-file containing the data");
            AddPage(csvSeparatorPage, "Parse columns", "Please indicate how the data should be parsed into columns");
            AddPage(csvMappingPage, "Value parsing and column mapping", "Please indicate how the values should be parsed and which columns should be used.");
        }

        protected virtual ICsvDataSelectionWizardPage CreateCsvMappingPage()
        {
            return new CsvDataSelectionWizardPage();
        }

        protected override void OnPageCompleted(IWizardPage page)
        {
            if (page == fileSelectPage)
            {
                //read first 30 lines (or less) of file
                csvSeparatorPage.PreviewText = string.Join(Environment.NewLine, File.ReadLines(FilePath).Take(30).ToArray());
            }
            else if (page == csvSeparatorPage)
            {
                //set data table from separator page into mapping page
                var table = csvSeparatorPage.PreviewDataTable;

                //set result + datatable (not yet exposed?) into csvMappingPage
                csvMappingPage.SetData(table, GetRequiredFields());
            }
        }

        protected abstract void OnUserFinishedMapping(string filePath, CsvMappingData mappingData);

        protected override void OnDialogFinished()
        {
            OnUserFinishedMapping(FilePath, MappingData);
        }

        protected abstract IEnumerable<CsvRequiredField> GetRequiredFields();

        private CsvMappingData MappingData
        {
            get
            {
                return new CsvMappingData
                {
                    Settings = csvSeparatorPage.Settings,
                    FieldToColumnMapping = csvMappingPage.FieldToColumnMapping,
                    Filters = csvMappingPage.Filters
                };
            }
        }

        private string FilePath
        {
            get
            {
                return fileSelectPage.FileName;
            }
        }
    }
}