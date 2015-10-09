using System.Data;
using System.Windows.Forms;
using DelftTools.Utils.Csv.Importer;

namespace DelftTools.Controls.Swf.Csv
{
    public partial class CsvToDataTableWizardPage : UserControl, IWizardPage
    {
        public CsvToDataTableWizardPage()
        {
            InitializeComponent();
        }

        public string PreviewText
        {
            get
            {
                return csvToDataTableControl1.PreviewText;
            }
            set
            {
                csvToDataTableControl1.PreviewText = value;
            }
        }

        public DataTable PreviewDataTable
        {
            get
            {
                return csvToDataTableControl1.PreviewDataTable;
            }
        }

        public CsvSettings Settings
        {
            get
            {
                return csvToDataTableControl1.Settings;
            }
        }

        public bool CanFinish()
        {
            return CanDoNext();
        }

        public bool CanDoNext()
        {
            return !PreviewDataTable.HasErrors && PreviewDataTable.Rows.Count > 0;
        }

        public bool CanDoPrevious()
        {
            return true;
        }
    }
}