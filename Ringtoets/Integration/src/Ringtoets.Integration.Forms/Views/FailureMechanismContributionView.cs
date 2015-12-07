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
                SetNormValue(value as FailureMechanismContribution);
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

        private void SetNormValue(FailureMechanismContribution value)
        {
            UnbindNormChange();
            DetachFromData();

            data = value;

            SetGridDataSource();
            SetNormText();

            AttachToData();
            BindNormChange();
        }

        private void SetGridDataSource()
        {
            if (data != null)
            {
                probabilityDistributionGrid.DataSource = data.Distribution;
            }
        }

        private void AttachToData()
        {
            if (data != null)
            {
                data.Attach(this);
            }
        }

        private void DetachFromData()
        {
            if (data != null)
            {
                data.Detach(this);
            }
        }

        private void BindNormChange()
        {
            normInput.ValueChanged += NormValueChanged;
        }

        private void UnbindNormChange()
        {
            normInput.ValueChanged -= NormValueChanged;
        }

        private void NormValueChanged(object sender, EventArgs eventArgs)
        {
            data.Norm = (int) normInput.Value;
            data.NotifyObservers();
        }

        private void SetNormText()
        {
            if (data != null)
            {
                normInput.Value = data.Norm;
            }
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
    }
}