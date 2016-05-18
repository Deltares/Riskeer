using System.Collections.Generic;
using System.Windows.Forms;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Integration.Forms.Views
{
    public class CustomFailureMechanismResultView : FailureMechanismResultView
    {
        protected override IEnumerable<DataGridViewColumn> GetDataGridColumns()
        {
            foreach (var baseColumn in base.GetDataGridColumns())
            {
                yield return baseColumn;
            }

            yield return new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "AssessmentLayerOne",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one,
                Name = "column_AssessmentLayerOne"
            };

            yield return new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoA",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                Name = "column_AssessmentLayerTwoA",
                ReadOnly = true
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
    }
}