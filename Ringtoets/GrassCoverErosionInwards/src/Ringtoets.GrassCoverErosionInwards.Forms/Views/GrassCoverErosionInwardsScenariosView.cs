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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Utils;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Creates a new instance of <see cref="GrassCoverErosionInwardsScenariosView"/>.
    /// </summary>
    public partial class GrassCoverErosionInwardsScenariosView : UserControl, IView
    {
        private const int calculationsColumnIndex = 1;
        private readonly RecursiveObserver<CalculationGroup, ICalculationInput> calculationInputObserver;
        private readonly RecursiveObserver<CalculationGroup, ICalculationBase> calculationGroupObserver;
        private readonly Observer failureMechanismObserver;
        private GrassCoverErosionInwardsFailureMechanism failureMechanism;
        private CalculationGroup data;

        /// <summary>
        /// Creates a new instance of the <see cref="GrassCoverErosionInwardsScenariosView"/> class.
        /// </summary>
        public GrassCoverErosionInwardsScenariosView()
        {
            InitializeComponent();

            AddDataGridColumns();

            failureMechanismObserver = new Observer(UpdateDataGridViewDataSource);

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, ICalculationInput>(UpdateDataGridViewDataSource, cg => cg.Children.Concat<object>(cg.Children.OfType<ICalculation>().Select(c => c.GetObservableInput())));
            calculationGroupObserver = new RecursiveObserver<CalculationGroup, ICalculationBase>(UpdateDataGridViewDataSource, c => c.Children);
        }

        /// <summary>
        /// Gets or sets the failure mechanism.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism FailureMechanism
        {
            get
            {
                return failureMechanism;
            }
            set
            {
                failureMechanism = value;
                failureMechanismObserver.Observable = failureMechanism;
                UpdateDataGridViewDataSource();
            }
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as CalculationGroup;

                calculationInputObserver.Observable = data;
                calculationGroupObserver.Observable = data;
                UpdateDataGridViewDataSource();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            // Necessary to correctly load the content of the dropdown lists of the comboboxes...
            UpdateDataGridViewDataSource();
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (failureMechanismObserver != null)
            {
                failureMechanismObserver.Dispose();
            }
            calculationInputObserver.Dispose();
            calculationGroupObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void AddDataGridColumns()
        {
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<GrassCoverErosionInwardsScenarioRow>(sr => sr.Name),
                                                 Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                                                 true);

            dataGridViewControl.AddComboBoxColumn<DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>>(
                TypeUtils.GetMemberName<GrassCoverErosionInwardsScenarioRow>(sr => sr.Calculation),
                Properties.Resources.GrassCoverErosionInwardsScenariosView_AddDataGridColumns_Calculation,
                null,
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>>(wrapper => wrapper.WrappedObject),
                TypeUtils.GetMemberName<DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>>(wrapper => wrapper.DisplayName)
                );
        }

        private void UpdateDataGridViewDataSource()
        {
            dataGridViewControl.EndEdit();

            if (FailureMechanism == null || FailureMechanism.SectionResults == null || data == null || data.Children == null)
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            using (new SuspendDataGridViewColumnResizes(dataGridViewControl.GetColumnFromIndex(calculationsColumnIndex)))
            {
                var columnItems = ((DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(calculationsColumnIndex)).Items;
                var items = data.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>().Select(c => new DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>(c));
                SetItemsOnObjectCollection(columnItems, items.Cast<object>().ToArray());
            }

            dataGridViewControl.SetDataSource(FailureMechanism.SectionResults.Select(sectionResult => new GrassCoverErosionInwardsScenarioRow(sectionResult)).ToList());

            using (new SuspendDataGridViewColumnResizes(dataGridViewControl.GetColumnFromIndex(calculationsColumnIndex)))
            {
                UpdateDataGridViewDataComboBoxesContent();
            }
        }

        private void UpdateDataGridViewDataComboBoxesContent()
        {
            Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> calculationsPerSegmentName =
                GrassCoverErosionInwardsHelper.CollectCalculationsPerSegment(failureMechanism.SectionResults, data.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>());

            foreach (DataGridViewRow dataGridViewRow in dataGridViewControl.Rows)
            {
                FillAvailableCalculationsList(dataGridViewRow, calculationsPerSegmentName);
            }
        }

        private void FillAvailableCalculationsList(DataGridViewRow dataGridViewRow, Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> calculationsPerSegmentName)
        {
            var rowData = (GrassCoverErosionInwardsScenarioRow) dataGridViewRow.DataBoundItem;
            string sectionName = rowData.Name;

            var items = new List<DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>>
            {
                new DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>(null)
            };
            if (calculationsPerSegmentName.ContainsKey(sectionName))
            {
                items.AddRange(calculationsPerSegmentName[sectionName].Select(c => new DataGridViewComboBoxItemWrapper<GrassCoverErosionInwardsCalculation>(c)));
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