using System.Windows.Forms;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone.SectionResult;
using Ringtoets.Integration.Forms.Views.SectionResultRow;

namespace Ringtoets.Integration.Forms.Views.SectionResultView
{
    public class StrengthStabilityPointConstructionResultView : FailureMechanismResultView<StrengthStabilityPointConstructionFailureMechanismSectionResult>
    {
        public StrengthStabilityPointConstructionResultView()
        {
            DataGridViewControl.AddCellFormattingHandler(OnCellFormatting);

            AddDataGridColumns();
        }

        private void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StrengthStabilityPointConstructionSectionResultRow>(sr => sr.Name),
                Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                true
                );
            DataGridViewControl.AddCheckBoxColumn(
                TypeUtils.GetMemberName<StrengthStabilityPointConstructionSectionResultRow>(sr => sr.AssessmentLayerOne),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StrengthStabilityPointConstructionSectionResultRow>(sr => sr.AssessmentLayerTwoA),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a
                );
            DataGridViewControl.AddTextBoxColumn(
                TypeUtils.GetMemberName<StrengthStabilityPointConstructionSectionResultRow>(sr => sr.AssessmentLayerThree),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three
                );
        }

        protected override object CreateFailureMechanismSectionResultRow(StrengthStabilityPointConstructionFailureMechanismSectionResult sectionResult)
        {
            return new StrengthStabilityPointConstructionSectionResultRow(sectionResult);
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