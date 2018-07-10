using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Merge
{
    public partial class AssessmentSectionProviderDialog : DialogBase, IMergeDataProvider
    {
        public AssessmentSectionProviderDialog(IWin32Window dialogParent)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 500, 350)
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        public AssessmentSection SelectedAssessmentSection { get; }
        public IEnumerable<IFailureMechanism> SelectedFailureMechanisms { get; }

        public bool SelectData(IEnumerable<AssessmentSection> assessmentSections)
        {
            throw new NotImplementedException();
        }

        protected override Button GetCancelButton()
        {
            return cancelButton;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.IsSelected), "Selecteer");
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismMergeDataRow.Name), "Toetsspoor", true);
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.IsRelevant), "Is relevant", true);
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.HasSections), "Heeft vakindeling", true);
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismMergeDataRow.NumberOfCalculations), "Aantal berekeningen", true);
        }
    }
}