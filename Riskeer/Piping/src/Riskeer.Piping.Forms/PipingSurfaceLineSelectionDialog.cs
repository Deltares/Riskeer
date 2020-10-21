// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Controls.Dialogs;
using Core.Common.Util.Extensions;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Forms.Properties;
using Riskeer.Piping.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection from a given set of <see cref="PipingSurfaceLine"/>. Upon
    /// closing of the dialog, the selected <see cref="PipingSurfaceLine"/> can be obtained.
    /// </summary>
    public partial class PipingSurfaceLineSelectionDialog : DialogBase
    {
        private const int selectItemColumnIndex = 0;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSurfaceLineSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="surfaceLines">The collection of <see cref="PipingSurfaceLine"/> to show in the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingSurfaceLineSelectionDialog(IWin32Window dialogParent, IEnumerable<PipingSurfaceLine> surfaceLines)
            : base(dialogParent, RiskeerCommonFormsResources.GenerateScenariosIcon, 370, 550)
        {
            if (surfaceLines == null)
            {
                throw new ArgumentNullException(nameof(surfaceLines));
            }

            InitializeComponent();
            InitializeEventHandlers();
            Localize();

            SelectedItems = new List<PipingSurfaceLine>();

            Text = Resources.PipingSurfaceLineSelectionDialog_Select_SurfaceLines;
            InitializeDataGridView();

            DataGridViewControl.SetDataSource(surfaceLines.Select(sl => new SelectableRow<PipingSurfaceLine>(sl, sl.Name)).ToArray());
        }

        /// <summary>
        /// Gets a collection of selected <see cref="PipingSurfaceLine"/> if they were selected
        /// in the dialog and a confirmation was given. If no confirmation was given or no 
        /// <see cref="PipingSurfaceLine"/> was selected, then an empty collection is returned.
        /// </summary>
        public IEnumerable<PipingSurfaceLine> SelectedItems { get; private set; }

        protected override Button GetCancelButton()
        {
            return CustomCancelButton;
        }

        private void InitializeDataGridView()
        {
            DataGridViewControl.AddCheckBoxColumn(nameof(SelectableRow<PipingSurfaceLine>.Selected),
                                                  RiskeerCommonFormsResources.SelectionDialogBase_ColumnSelect_DisplayName);
            DataGridViewControl.AddTextBoxColumn(nameof(SelectableRow<PipingSurfaceLine>.Name),
                                                 RiskeerCommonFormsResources.SurfaceLine_DisplayName, true, DataGridViewAutoSizeColumnMode.Fill);
        }

        private void Localize()
        {
            SelectAllButton.Text = RiskeerCommonFormsResources.SelectionDialogBase_SelectionDialogBase_Select_all;
            DeselectAllButton.Text = RiskeerCommonFormsResources.SelectionDialogBase_SelectionDialogBase_Deselect_all;
            DoForSelectedButton.Text = RiskeerCommonFormsResources.SelectionDialogBase_SelectionDialogBase_Generate;
            CustomCancelButton.Text = RiskeerCommonFormsResources.SelectionDialogBase_SelectionDialogBase_Cancel;
            SemiProbabilisticCheckbox.Text = Resources.PipingSurfaceLineSelectionDialog_SemiProbabilisticCheckbox;
            ProbabilisticCheckbox.Text = Resources.PipingSurfaceLineSelectionDialog_ProbabilisticCheckbox;
        }

        private void SetSelectedItems()
        {
            SelectedItems = GetSelectedItems();
        }

        private IEnumerable<SelectableRow<PipingSurfaceLine>> GetSelectableRows()
        {
            return DataGridViewControl.Rows.Cast<DataGridViewRow>().Select(row => row.DataBoundItem).Cast<SelectableRow<PipingSurfaceLine>>().ToArray();
        }

        private IEnumerable<PipingSurfaceLine> GetSelectedItems()
        {
            return GetSelectableRows().Where(row => row.Selected).Select(row => row.Item).ToArray();
        }

        #region Event handling

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            GetSelectableRows().ForEachElementDo(row => row.Selected = true);
            DataGridViewControl.RefreshDataGridView();
            UpdateDoForSelectedButton();
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            GetSelectableRows().ForEachElementDo(row => row.Selected = false);
            DataGridViewControl.RefreshDataGridView();
            UpdateDoForSelectedButton();
        }

        private void InitializeEventHandlers()
        {
            DataGridViewControl.CellValueChanged += DataGridViewCellValueChanged;
        }

        private void DataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != selectItemColumnIndex)
            {
                return;
            }

            UpdateDoForSelectedButton();
        }

        private void DoForSelectedButton_Click(object sender, EventArgs e)
        {
            SetSelectedItems();
            Close();
        }

        private void CustomCancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateDoForSelectedButton()
        {
            DoForSelectedButton.Enabled = GetSelectableRows().Any(row => row.Selected);
        }

        #endregion
    }
}