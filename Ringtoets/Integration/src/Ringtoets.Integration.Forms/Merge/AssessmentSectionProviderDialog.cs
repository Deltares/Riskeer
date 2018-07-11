using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Merge
{
    public partial class AssessmentSectionProviderDialog : DialogBase, IMergeDataProvider
    {
        private bool assessmentSectionComboBoxUpdating;

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
            if (assessmentSections == null)
            {
                throw new ArgumentNullException(nameof(assessmentSections));
            }

            SetComboBoxData(assessmentSections);
            Show();

            return false;
        }

        private void SetComboBoxData(IEnumerable<AssessmentSection> assessmentSections)
        {
            assessmentSectionComboBox.BeginUpdate();

            assessmentSectionComboBoxUpdating = true;
            assessmentSectionComboBox.DataSource = assessmentSections.ToArray();
            assessmentSectionComboBox.DisplayMember = nameof(AssessmentSection.Name);
            assessmentSectionComboBox.SelectedItem = null;
            assessmentSectionComboBoxUpdating = false;

            assessmentSectionComboBox.SelectedItem = assessmentSections.FirstOrDefault();

            assessmentSectionComboBox.EndUpdate();
        }

        private void SetDataGridViewData(AssessmentSection assessmentSection)
        {
            var rows = new[]
            {
                new FailureMechanismMergeDataRow(assessmentSection.Piping),
                new FailureMechanismMergeDataRow(assessmentSection.GrassCoverErosionInwards),
                new FailureMechanismMergeDataRow(assessmentSection.MacroStabilityInwards),
                new FailureMechanismMergeDataRow(assessmentSection.MacroStabilityOutwards),
                new FailureMechanismMergeDataRow(assessmentSection.Microstability),
                new FailureMechanismMergeDataRow(assessmentSection.StabilityStoneCover),
                new FailureMechanismMergeDataRow(assessmentSection.WaveImpactAsphaltCover),
                new FailureMechanismMergeDataRow(assessmentSection.WaterPressureAsphaltCover),
                new FailureMechanismMergeDataRow(assessmentSection.GrassCoverErosionOutwards),
                new FailureMechanismMergeDataRow(assessmentSection.GrassCoverSlipOffOutwards),
                new FailureMechanismMergeDataRow(assessmentSection.GrassCoverSlipOffInwards),
                new FailureMechanismMergeDataRow(assessmentSection.HeightStructures),
                new FailureMechanismMergeDataRow(assessmentSection.ClosingStructures),
                new FailureMechanismMergeDataRow(assessmentSection.PipingStructure),
                new FailureMechanismMergeDataRow(assessmentSection.StabilityPointStructures),
                new FailureMechanismMergeDataRow(assessmentSection.StrengthStabilityLengthwiseConstruction),
                new FailureMechanismMergeDataRow(assessmentSection.DuneErosion),
                new FailureMechanismMergeDataRow(assessmentSection.TechnicalInnovation)
            };

            dataGridViewControl.SetDataSource(rows);
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

        private void AssessmentSectionComboBox_OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (assessmentSectionComboBoxUpdating || assessmentSectionComboBox.SelectedIndex == -1)
            {
                return;
            }

            SetDataGridViewData((AssessmentSection) assessmentSectionComboBox.SelectedItem);
        }
    }
}