// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.Forms.Controls
{
    /// <summary>
    /// Interaction logic for <see cref="AssemblyOverviewControl"/>.
    /// </summary>
    public partial class AssemblyOverviewControl
    {
        private readonly AssemblyOverviewViewModel viewModel;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyOverviewControl"/>.
        /// </summary>
        public AssemblyOverviewControl(AssemblyOverviewViewModel viewModel)
        {
            this.viewModel = viewModel;
            InitializeComponent();

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Binding = new Binding("Name")
            });
            
            var index = 0;
            foreach (CombinedFailureMechanismSectionAssemblyResult combinedAssemblyResult in viewModel.CombinedAssemblyResults)
            {
                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Binding = new Binding($"Results_{index}"),
                    Width = CalculateWidth(viewModel.AssessmentSection.ReferenceLine, combinedAssemblyResult)
                });
            }

            var rowNumber = 0;
            foreach (Tuple<string,Dictionary<FailureMechanismSection,FailureMechanismSectionAssemblyResult>> failureMechanism in viewModel.FailureMechanisms)
            {
                dataGrid.Items.Add(new
                {
                    Name = failureMechanism.Item1,
                    Results = failureMechanism.Item2.Select(i => FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName(i.Value.AssemblyGroup))
                });

                var failureMechanismSectionIndex = 0;
                var combinedFailureMechanismSectionIndex = 0;
                double previousSectionLength = 0;
                
                // foreach (CombinedFailureMechanismSectionAssemblyResult combinedAssemblyResult in viewModel.CombinedAssemblyResults)
                // {
                    // FailureMechanismSection failureMechanismSection = failureMechanism.Item2.Keys.ElementAt(failureMechanismSectionIndex);
                    // double combinedAssemblyResultLength = combinedAssemblyResult.SectionEnd - combinedAssemblyResult.SectionStart;
                    //
                    // if (failureMechanismSection.Length > combinedAssemblyResultLength - previousSectionLength)
                    // {
                        // var dataGridCellInfo = new DataGridCellInfo(dataGrid.Items[rowNumber], dataGrid.Columns[combinedFailureMechanismSectionIndex + 1]);
                        // FrameworkElement frameworkElement = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
                        // var cell = (DataGridCell) frameworkElement.Parent;
                        // cell.BorderThickness = new Thickness(0);

                        // previousSectionLength = combinedAssemblyResultLength;
                        // combinedFailureMechanismSectionIndex++;
                    // }

                    // failureMechanismSectionIndex++;
                // }
                //
                // rowNumber++;
            }
        }

        private DataGridLength CalculateWidth(ReferenceLine referenceLine, CombinedFailureMechanismSectionAssemblyResult combinedAssemblyResult)
        {
            double referenceLineLength = referenceLine.Length;
            double combinedAssemblyResultSectionLength = combinedAssemblyResult.SectionEnd - combinedAssemblyResult.SectionStart;

            return new DataGridLength(dataGrid.ActualWidth * (combinedAssemblyResultSectionLength / referenceLineLength) / 100);
        }
    }
}