using System;
using System.Windows.Forms;

namespace Core.Common.Controls.Swf.Table
{
    public partial class TableViewDataToolBar : UserControl
    {
        private TableView tableView;

        public TableViewDataToolBar()
        {
            InitializeComponent();
        }

        public void SetTableView(TableView tableView)
        {
            this.tableView = tableView;
        }

        public void RefreshButtons()
        {
            pasteAssistBtn.Visible = CanAddRemove();
            csvImportBtn.Visible = CanAddRemove();
        }

        public void AddButton(ToolStripButton button)
        {
            toolStrip1.Items.RemoveByKey(button.Name);
            toolStrip1.Items.Add(button);
        }

        private bool CanAddRemove()
        {
            return tableView.AllowAddNewRow && tableView.AllowDeleteRow;
        }

        private void pasteAssistBtn_Click(object sender, EventArgs e)
        {
            if (!CanAddRemove())
            {
                RefreshButtons();
                return;
            }

            var clipboardText = Clipboard.GetText();

            if (string.IsNullOrEmpty(clipboardText))
            {
                MessageBox.Show("Clipboard appears empty, please copy data first.");
                return;
            }

            var dialog = new TableViewCsvImportWizardDialog(tableView, clipboardText);
            dialog.ShowDialog();
        }

        private void csvImportBtn_Click(object sender, EventArgs e)
        {
            if (!CanAddRemove())
            {
                RefreshButtons();
                return;
            }

            var dialog = new TableViewCsvImportWizardDialog(tableView);
            dialog.ShowDialog();
        }

        private void csvExportBtn_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Csv file (*.csv)|*.csv"
            };
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            tableView.ExportAsCsv(dialog.FileName);
        }
    }
}