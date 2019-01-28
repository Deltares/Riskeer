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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Util.Extensions;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms
{
    /// <summary>
    /// Base class for selection dialogs which should be derived in order to get a consistent look and feel.
    /// </summary>
    public abstract partial class SelectionDialogBase<T> : DialogBase where T : class
    {
        private const int selectItemColumnIndex = 0;

        /// <summary>
        /// Creates a new instance of <see cref="SelectionDialogBase{T}"/>.
        /// </summary>
        /// <param name="dialogParent">The owner of the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> is <c>null</c>.</exception>
        protected SelectionDialogBase(IWin32Window dialogParent) : base(dialogParent, Resources.GenerateScenariosIcon, 370, 550)
        {
            InitializeComponent();
            InitializeEventHandlers();
            Localize();

            SelectedItems = new List<T>();
        }

        /// <summary>
        /// Gets a collection of selected <see cref="T"/> if they were selected
        /// in the dialog and a confirmation was given. If no confirmation was given or no 
        /// <see cref="T"/> was selected, then an empty collection is returned.
        /// </summary>
        public IEnumerable<T> SelectedItems { get; private set; }

        /// <summary>
        /// Sets the datasource on the <see cref="DataGridView"/>.
        /// </summary>
        protected void SetDataSource(IEnumerable data)
        {
            DataGridViewControl.SetDataSource(data);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override Button GetCancelButton()
        {
            return CustomCancelButton;
        }

        /// <summary>
        /// Initializes the <see cref="DataGridView"/>.
        /// <param name="nameColumnHeader">Display name of the column header for <see cref="SelectableRow{T}.Name"/>.</param>
        /// </summary>
        protected void InitializeDataGridView(string nameColumnHeader)
        {
            DataGridViewControl.AddCheckBoxColumn(nameof(SelectableRow<T>.Selected),
                                                  Resources.SelectionDialogBase_ColumnSelect_DisplayName);
            DataGridViewControl.AddTextBoxColumn(nameof(SelectableRow<T>.Name),
                                                 nameColumnHeader, true, DataGridViewAutoSizeColumnMode.Fill);
        }

        private void Localize()
        {
            SelectAllButton.Text = Resources.SelectionDialogBase_SelectionDialogBase_Select_all;
            DeselectAllButton.Text = Resources.SelectionDialogBase_SelectionDialogBase_Deselect_all;
            DoForSelectedButton.Text = Resources.SelectionDialogBase_SelectionDialogBase_Generate;
            CustomCancelButton.Text = Resources.SelectionDialogBase_SelectionDialogBase_Cancel;
        }

        private void SetSelectedItems()
        {
            SelectedItems = GetSelectedItems();
        }

        private IEnumerable<SelectableRow<T>> GetSelectableRows()
        {
            return DataGridViewControl.Rows.Cast<DataGridViewRow>().Select(row => row.DataBoundItem).Cast<SelectableRow<T>>().ToArray();
        }

        private IEnumerable<T> GetSelectedItems()
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