using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DelftTools.Utils.Csv.Importer;

namespace DelftTools.Controls.Swf.Csv
{
    public partial class CsvToDataTableControl : UserControl
    {
        private DataTable previewDataTable = new DataTable();
        private string previewText;

        public CsvToDataTableControl()
        {
            InitializeComponent();
            rbTab.CheckedChanged += OnRadioButtonsChanged;
            rbSemicolon.CheckedChanged += OnRadioButtonsChanged;
            rbSpace.CheckedChanged += OnRadioButtonsChanged;
            rbComma.CheckedChanged += OnRadioButtonsChanged;

            chkHeader.CheckedChanged += OnCheckedChanged;
            chkEmptyLines.CheckedChanged += OnCheckedChanged;

            dgvPreview.DataSource = previewDataTable;
            rbComma.Checked = true;
            chkHeader.Checked = true;
            chkEmptyLines.Checked = true;
        }

        public CsvSettings Settings
        {
            get
            {
                return new CsvSettings
                {
                    Delimiter = rbComma.Checked ? ',' :
                                    rbSemicolon.Checked ? ';' :
                                        rbSpace.Checked ? ' ' :
                                            rbTab.Checked ? '\t' : ',',
                    FirstRowIsHeader = chkHeader.Checked,
                    SkipEmptyLines = chkEmptyLines.Checked
                };
            }
        }

        public DataTable PreviewDataTable
        {
            get
            {
                return previewDataTable;
            }
        }

        public string PreviewText
        {
            get
            {
                return previewText;
            }
            set
            {
                previewText = value;

                // do some auto detection:
                if (previewText != null && previewText.Length > 1)
                {
                    if (previewText.Contains("\t"))
                    {
                        rbTab.Checked = true;
                    }
                    else if (previewText.Contains(";"))
                    {
                        rbSemicolon.Checked = true;
                    }
                    chkHeader.Checked = !Char.IsNumber(previewText[0]);
                }

                UpdatePreview();
            }
        }

        private void OnRadioButtonsChanged(object sender, EventArgs eventArgs)
        {
            var button = sender as RadioButton;
            // ensure one update per change:
            if (button != null && button.Checked)
            {
                UpdatePreview();
            }
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (previewText == null)
            {
                return;
            }

            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(previewText)))
            {
                using (var reader = new StreamReader(stream))
                {
                    previewDataTable = new CsvImporter().SplitToTable(reader, Settings);
                    EscapeCommasInColumnHeaders(previewDataTable);
                    dgvPreview.DataSource = previewDataTable;
                }
            }
        }

        private static void EscapeCommasInColumnHeaders(DataTable dataTable)
        {
            foreach (DataColumn column in dataTable.Columns)
            {
                column.ColumnName = column.ColumnName.Replace(',', ' ');
            }
        }
    }
}