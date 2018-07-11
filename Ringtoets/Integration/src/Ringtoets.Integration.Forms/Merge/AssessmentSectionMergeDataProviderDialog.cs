using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Forms.Merge
{
    /// <summary>
    /// A dialog which allows the user to make a selection of which <see cref="AssessmentSection"/>
    /// and its <see cref="IFailureMechanism"/> to use for merging the data. The selections
    /// can be obtained upon closing the dialog.
    /// </summary>
    public partial class AssessmentSectionMergeDataProviderDialog : DialogBase, IMergeDataProvider
    {
        private FailureMechanismMergeDataRow[] failureMechanismMergeDataRows;
        private bool assessmentSectionComboBoxUpdating;
        private bool isDataSelected;

        /// <summary>
        /// Creates a new instance of the <see cref="AssessmentSectionMergeDataProviderDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        public AssessmentSectionMergeDataProviderDialog(IWin32Window dialogParent)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 720, 590)
        {
            InitializeComponent();
            InitializeTooltip();
            InitializeDataGridView();
        }

        public AssessmentSection SelectedAssessmentSection { get; private set; }
        public IEnumerable<IFailureMechanism> SelectedFailureMechanisms { get; private set; }

        public bool SelectData(IEnumerable<AssessmentSection> assessmentSections)
        {
            if (assessmentSections == null)
            {
                throw new ArgumentNullException(nameof(assessmentSections));
            }

            SetComboBoxData(assessmentSections);
            ShowDialog();

            return isDataSelected;
        }

        protected override Button GetCancelButton()
        {
            return cancelButton;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.IsSelected),
                                                  Resources.FailureMechanismMergeDataRow_IsSelected_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismMergeDataRow.Name),
                                                 Resources.FailureMechanism_Name_DisplayName,
                                                 true);
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.IsRelevant),
                                                  Resources.FailureMechanismMergeDataRow_IsRelevant_DisplayName,
                                                  true);
            dataGridViewControl.AddCheckBoxColumn(nameof(FailureMechanismMergeDataRow.HasSections),
                                                  Resources.FailureMechanismMergeDataRow_HasSections_DisplayName,
                                                  true);
            dataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismMergeDataRow.NumberOfCalculations),
                                                 Resources.FailureMechanismMergeDataRow_NumberOfCalculations_DisplayName,
                                                 true);
        }

        private void InitializeTooltip()
        {
            infoIcon.BackgroundImage = CoreCommonGuiResources.information;
            toolTip.SetToolTip(infoIcon, Resources.AssessmentSectionMergeDataProviderDialog_InfoToolTip);
        }

        #region Data Setters

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
            failureMechanismMergeDataRows = new[]
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

            dataGridViewControl.SetDataSource(failureMechanismMergeDataRows);
        }

        #endregion

        #region Event Handling

        private void AssessmentSectionComboBox_OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (assessmentSectionComboBoxUpdating || assessmentSectionComboBox.SelectedIndex == -1)
            {
                return;
            }

            SetDataGridViewData((AssessmentSection) assessmentSectionComboBox.SelectedItem);
        }

        private void OnImportButtonClick(object sender, EventArgs eventArgs)
        {
            SelectedAssessmentSection = (AssessmentSection) assessmentSectionComboBox.SelectedItem;
            SelectedFailureMechanisms = failureMechanismMergeDataRows?.Where(row => row.IsSelected)
                                                                     .Select(row => row.FailureMechanism)
                                                                     .ToArray();

            isDataSelected = true;
            Close();
        }

        #endregion
    }
}