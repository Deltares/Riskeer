// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="PipingFailureMechanismResult"/>.
    /// </summary>
    public partial class PipingFailureMechanismResultView : UserControl, IView
    {
        private PipingFailureMechanismResult pipingFailureMechanismResult;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismResultView"/>.
        /// </summary>
        public PipingFailureMechanismResultView()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            var sectionName = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = Resources.PipingFailureMechanismResultView_InitializeDataGridView_Section_name,
                Name = "column_Name"
            };

            var assessmentLayerOne = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerOne",
                HeaderText = Resources.PipingFailureMechanismResultView_InitializeDataGridView_Assessment_layer_one,
                Name = "column_AssessmentLayerOne"
            };

            var assessmentLayerTwoA = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoA",
                HeaderText = Resources.PipingFailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                Name = "column_AssessmentLayerTwoA"
            };

            var assessmentLayerTwoB = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoB",
                HeaderText = Resources.PipingFailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_b,
                Name = "column_AssessmentLayerTwoB"
            };

            var assessmentLayerThree = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerThree",
                HeaderText = Resources.PipingFailureMechanismResultView_InitializeDataGridView_Assessment_layer_three,
                Name = "column_AssessmentLayerThree"
            };

            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.AddRange(sectionName, assessmentLayerOne, assessmentLayerTwoA, assessmentLayerTwoB, assessmentLayerThree);

            foreach (var column in dataGridView.Columns.OfType<DataGridViewColumn>())
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        public object Data 
        {
            get
            {
                return pipingFailureMechanismResult;
            }
            set
            {
                pipingFailureMechanismResult = value as PipingFailureMechanismResult;
            }
        }
    }
}
