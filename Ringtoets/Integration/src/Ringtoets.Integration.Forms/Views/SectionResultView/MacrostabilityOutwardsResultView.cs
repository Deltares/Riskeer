using System.Windows.Forms;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone.SectionResult;
using Ringtoets.Integration.Forms.Views.SectionResultRow;

namespace Ringtoets.Integration.Forms.Views.SectionResultView
{
    public class MacrostabilityOutwardsResultView : FailureMechanismResultView<MacrostabilityOutwardsFailureMechanismSectionResult>
    {
        public MacrostabilityOutwardsResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(OnCellFormatting);

            AddDataGridColumns();
        }

        private void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<MacrostabilityOutwardsSectionResultRow>(sr => sr.Name),
                Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                true
                );
            DataGridViewControl.AddCheckBoxColumn(
                TypeUtils.GetMemberName<MacrostabilityOutwardsSectionResultRow>(sr => sr.AssessmentLayerOne),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<MacrostabilityOutwardsSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<MacrostabilityOutwardsSectionResultRow>(sr => sr.AssessmentLayerThree),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three
                );
        }

        protected override object CreateFailureMechanismSectionResultRow(MacrostabilityOutwardsFailureMechanismSectionResult sectionResult)
        {
            return new MacrostabilityOutwardsSectionResultRow(sectionResult);
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > 1)
            {
                if (HasPassedLevelOne(eventArgs.RowIndex))
                {
                    DataGridViewControl.DisableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
                else
                {
                    DataGridViewControl.RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            }
        }
    }
}