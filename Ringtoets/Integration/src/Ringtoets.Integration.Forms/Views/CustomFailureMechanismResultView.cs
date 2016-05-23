using System.Collections.Generic;
using System.Windows.Forms;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class defines a view where <see cref="SimpleFailureMechanismSectionResult"/> are displayed in a grid
    /// and can be modified.
    /// </summary>
    public class CustomFailureMechanismResultView : FailureMechanismResultView<CustomFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="CustomFailureMechanismResultView"/>
        /// </summary>
        public CustomFailureMechanismResultView()
        {
            AddCellFormattingHandler(OnCellFormatting);
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex > 1)
            {
                if (HasPassedLevelZero(eventArgs.RowIndex))
                {
                    DisableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
                else
                {
                    RestoreCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            } 
        }

        protected override IEnumerable<DataGridViewColumn> GetDataGridColumns()
        {
            foreach (var baseColumn in base.GetDataGridColumns())
            {
                yield return baseColumn;
            }

            yield return new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoA",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                Name = "column_AssessmentLayerTwoA"
            };

            yield return new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoB",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_b,
                Name = "column_AssessmentLayerTwoB"
            };

            yield return new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerThree",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three,
                Name = "column_AssessmentLayerThree"
            };
        }

        protected override object CreateFailureMechanismSectionResultRow(CustomFailureMechanismSectionResult sectionResult)
        {
            return new CustomFailureMechanismSectionResultRow(sectionResult);
        }
    }
}