// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Helpers;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.Forms
{
    /// <summary>
    /// Control for displaying and configuring scenarios.
    /// </summary>
    public partial class ScenarioSelectionControl : UserControl
    {
        private const int calculationsColumnIndex = 1;

        /// <summary>
        /// Creates a new instance of <see cref="ScenarioSelectionControl"/>.
        /// </summary>
        public ScenarioSelectionControl()
        {
            InitializeComponent();
            AddDataGridColumns();
        }

        /// <summary>
        /// Update the source of the grid view by adding rows and filling the combo boxes for each row with the allowed calculations.
        /// </summary>
        /// <typeparam name="T">The type of rows to be added to the grid view.</typeparam>
        /// <param name="calculations">The collection of calculations known in the failure mechanism.</param>
        /// <param name="scenarioRows">The collection of rows to be added to the grid view.</param>
        /// <param name="calculationsPerSection">The allowed calculations grouped by the name of the failure mechanism sections.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public void UpdateDataGridViewDataSource<T>(
            IEnumerable<ICalculation> calculations, IEnumerable<T> scenarioRows, IDictionary<string, List<ICalculation>> calculationsPerSection)
            where T : IScenarioRow<ICalculation>
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (scenarioRows == null)
            {
                throw new ArgumentNullException(nameof(scenarioRows));
            }

            if (calculationsPerSection == null)
            {
                throw new ArgumentNullException(nameof(calculationsPerSection));
            }

            using (new SuspendDataGridViewColumnResizes(dataGridViewControl.GetColumnFromIndex(calculationsColumnIndex)))
            {
                dataGridViewControl.SetDataSource(scenarioRows.ToArray());

                DataGridViewComboBoxCell.ObjectCollection columnItems = ((DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(calculationsColumnIndex)).Items;
                IEnumerable<DataGridViewComboBoxItemWrapper<ICalculation>> items = calculations.Select(c => new DataGridViewComboBoxItemWrapper<ICalculation>(c));
                SetItemsOnObjectCollection(columnItems, items.Cast<object>().ToArray());

                UpdateDataGridViewDataComboBoxesContent(calculationsPerSection);
            }
        }

        /// <summary>
        /// Empties the data grid view.
        /// </summary>
        public void ClearDataSource()
        {
            dataGridViewControl.SetDataSource(null);
        }

        /// <summary>
        /// Ends the edit mode of the grid view.
        /// </summary>
        public void EndEdit()
        {
            dataGridViewControl.EndEdit();
        }

        private void UpdateDataGridViewDataComboBoxesContent(IDictionary<string, List<ICalculation>> calculationsPerSegmentName)
        {
            foreach (DataGridViewRow dataGridViewRow in dataGridViewControl.Rows)
            {
                FillAvailableCalculationsList(dataGridViewRow, calculationsPerSegmentName);
            }
        }

        private void AddDataGridColumns()
        {
            dataGridViewControl.AddTextBoxColumn(nameof(IScenarioRow<ICalculation>.Name),
                                                 RingtoetsCommonFormsResources.Section_DisplayName,
                                                 true);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<ICalculation>>(
                nameof(IScenarioRow<ICalculation>.Calculation),
                RingtoetsCommonDataResources.ICalculation_DisplayName,
                null,
                nameof(DataGridViewComboBoxItemWrapper<ICalculation>.WrappedObject),
                nameof(DataGridViewComboBoxItemWrapper<ICalculation>.DisplayName));
        }

        private static void FillAvailableCalculationsList(DataGridViewRow dataGridViewRow, IDictionary<string, List<ICalculation>> calculationsPerSegmentName)
        {
            var rowData = (IScenarioRow<ICalculation>) dataGridViewRow.DataBoundItem;
            string sectionName = rowData.Name;

            var items = new List<DataGridViewComboBoxItemWrapper<ICalculation>>
            {
                new DataGridViewComboBoxItemWrapper<ICalculation>(null)
            };
            if (calculationsPerSegmentName.ContainsKey(sectionName))
            {
                items.AddRange(calculationsPerSegmentName[sectionName].Select(c => new DataGridViewComboBoxItemWrapper<ICalculation>(c)).ToArray());
            }

            var cell = (DataGridViewComboBoxCell) dataGridViewRow.Cells[calculationsColumnIndex];
            SetItemsOnObjectCollection(cell.Items, items.Cast<object>().ToArray());
        }

        private static void SetItemsOnObjectCollection(DataGridViewComboBoxCell.ObjectCollection objectCollection, object[] comboBoxItems)
        {
            objectCollection.Clear();
            objectCollection.AddRange(comboBoxItems);
        }
    }
}