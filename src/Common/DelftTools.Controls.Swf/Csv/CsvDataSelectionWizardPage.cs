using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DelftTools.Utils.Csv.Importer;

namespace DelftTools.Controls.Swf.Csv
{
    public partial class CsvDataSelectionWizardPage : UserControl, ICsvDataSelectionWizardPage
    {
        public CsvDataSelectionWizardPage()
        {
            InitializeComponent();
        }

        public bool FilteringVisible
        {
            get
            {
                return csvDataSelectionControl1.FilteringVisible;
            }
            set
            {
                csvDataSelectionControl1.FilteringVisible = value;
            }
        }

        public bool ColumnSelectionVisible
        {
            get
            {
                return csvDataSelectionControl1.ColumnSelectionVisible;
            }
            set
            {
                csvDataSelectionControl1.ColumnSelectionVisible = value;
            }
        }

        public IDictionary<CsvRequiredField, CsvColumnInfo> FieldToColumnMapping
        {
            get
            {
                return csvDataSelectionControl1.FieldToColumnMapping;
            }
        }

        public IEnumerable<CsvFilter> Filters
        {
            get
            {
                return csvDataSelectionControl1.Filters;
            }
        }

        public bool CanFinish()
        {
            return CanDoNext();
        }

        public bool CanDoNext()
        {
            return !csvDataSelectionControl1.HasErrors;
        }

        public bool CanDoPrevious()
        {
            return true;
        }

        public void SetData(DataTable dataTable, IEnumerable<CsvRequiredField> requiredFields)
        {
            csvDataSelectionControl1.SetData(dataTable, requiredFields);
        }
    }
}