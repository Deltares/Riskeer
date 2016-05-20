using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Utils;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class defines a view where <see cref="SimpleFailureMechanismSectionResult"/> are displayed in a grid
    /// and can be modified.
    /// </summary>
    public class SimpleFailureMechanismResultView : FailureMechanismResultView<SimpleFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="SimpleFailureMechanismResultView"/>
        /// </summary>
        public SimpleFailureMechanismResultView()
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
                    EnableCell(eventArgs.RowIndex, eventArgs.ColumnIndex);
                }
            } 
        }

        protected override IEnumerable<DataGridViewColumn> GetDataGridColumns()
        {
            foreach (var baseColumn in base.GetDataGridColumns())
            {
                yield return baseColumn;
            }

            yield return new DataGridViewComboBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoA",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                Name = "column_AssessmentLayerTwoA",
                DataSource = Enum.GetValues(typeof(AssessmentLayerTwoAResult))
                    .OfType<AssessmentLayerTwoAResult>()
                    .Select(el => new EnumDisplayWrapper<AssessmentLayerTwoAResult>(el))
                    .ToList(),
                ValueMember = "Value",
                DisplayMember = "DisplayName"
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

        protected override object CreateFailureMechanismSectionResultRow(SimpleFailureMechanismSectionResult sectionResult)
        {
            return new SimpleFailureMechanismSectionResultRow(sectionResult);
        }
    }
}