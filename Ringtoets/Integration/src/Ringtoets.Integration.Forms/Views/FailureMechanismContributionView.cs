using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Forms.Views
{
    public partial class FailureMechanismContributionView : UserControl, IView, IObserver
    {
        private FailureMechanismContribution data;

        public FailureMechanismContributionView()
        {
            InitializeComponent();
            InitializeGridColumns();
            BindNormChange();
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                if (data != null)
                {
                    data.Detach(this);
                }

                data = (FailureMechanismContribution)value;

                if (data != null)
                {
                    data.Attach(this);
                    probabilityDistributionGrid.DataSource = data.Distribution;
                    SetNormText();
                }
            }
        }

        public Image Image { get; set; }
        public ViewInfo ViewInfo { get; set; }

        public void UpdateObserver()
        {
            SetNormText();
            probabilityDistributionGrid.Refresh();
        }

        public void EnsureVisible(object item) {}

        private void BindNormChange()
        {
            normTextBox.LostFocus += NormTextBoxOnLostFocus;
        }

        private void NormTextBoxOnLostFocus(object sender, EventArgs eventArgs)
        {
            data.Norm = Int32.Parse(normTextBox.Text);
            data.NotifyObservers();
        }

        private void InitializeGridColumns()
        {
            var assessmentColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Assessment",
                HeaderText = "Toetsspoor",
                Name = "column_Assessment",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader
            };

            var probabilityColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Probability",
                HeaderText = "Toegestane bijdrage aan faalkans [%]",
                Name = "column_Probability",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            };

            var probabilityPerYearColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProbabilityPerYear",
                HeaderText = "Faalkansruimte (per jaar)",
                Name = "column_ProbabilityPerYear",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            probabilityDistributionGrid.AutoGenerateColumns = false;
            probabilityDistributionGrid.Columns.AddRange(assessmentColumn, probabilityColumn, probabilityPerYearColumn);
        }

        private void SetNormText()
        {
            normTextBox.Text = string.Format("{0}", data.Norm);
        }
    }
}