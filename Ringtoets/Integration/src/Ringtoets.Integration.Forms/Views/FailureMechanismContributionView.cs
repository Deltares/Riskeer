using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Reflection;
using Ringtoets.Integration.Data.Contribution;

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
                SetNormValue((FailureMechanismContribution)value);
            }
        }

        public Image Image { get; set; }
        public ViewInfo ViewInfo { get; set; }
        public void EnsureVisible(object item) { }

        public void UpdateObserver()
        {
            SetNormText();
            probabilityDistributionGrid.Refresh();
        }

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
            var assessmentName = TypeUtils.GetMemberName<FailureMechanismContributionItem>(e => e.Assessment);
            var columnNameFormat = "column_{0}";
            var assessmentColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = assessmentName,
                HeaderText = Resources.FailureMechanismContributionView_GridColumn_Assessment,
                Name = string.Format(columnNameFormat, assessmentName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader
            };

            var contributionName = TypeUtils.GetMemberName<FailureMechanismContributionItem>(e => e.Contribution);
            var probabilityColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = contributionName,
                HeaderText = Resources.FailureMechanismContributionView_GridColumn_Contribution,
                Name = string.Format(columnNameFormat, contributionName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            };

            var probabilitySpaceName = TypeUtils.GetMemberName<FailureMechanismContributionItem>(e => e.ProbabilitySpace);
            var probabilityPerYearColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = probabilitySpaceName,
                HeaderText = Resources.FailureMechanismContributionView_GridColumn_ProbabilitySpace,
                Name = string.Format(columnNameFormat, probabilitySpaceName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            probabilityDistributionGrid.AutoGenerateColumns = false;
            probabilityDistributionGrid.Columns.AddRange(assessmentColumn, probabilityColumn, probabilityPerYearColumn);
        }
    }
}