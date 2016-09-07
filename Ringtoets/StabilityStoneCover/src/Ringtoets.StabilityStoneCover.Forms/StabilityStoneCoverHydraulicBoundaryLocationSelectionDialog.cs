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
using Core.Common.Controls.Dialogs;
using Core.Common.Utils.Extensions;
using Core.Common.Utils.Reflection;
using Ringtoets.HydraRing.Data;
using Ringtoets.StabilityStoneCover.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection form a given set of <see cref="IHydraulicBoundaryLocation"/>. Upon
    /// closing of the dialog, the selected <see cref="IHydraulicBoundaryLocation"/> can be obtained.
    /// </summary>
    public partial class StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog : DialogBase
    {
        private const int locationCalculateColumnIndex = 0;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="hydraulicBoundaryLocations">The collection of <see cref="IHydraulicBoundaryLocation"/> to show in the dialog.</param>
        public StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(IWin32Window dialogParent,
                                                                           IEnumerable<IHydraulicBoundaryLocation> hydraulicBoundaryLocations)
            : base(dialogParent, RingtoetsCommonFormsResources.GenerateScenariosIcon, 370, 550)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocations");
            }

            InitializeComponent();
            InitializeEventHandlers();
            InitializeDataGridView();

            SetDataSource(hydraulicBoundaryLocations);
            SelectedLocations = new List<IHydraulicBoundaryLocation>();
        }

        /// <summary>
        /// Gets a collection of selected <see cref="IHydraulicBoundaryLocation"/> if they were selected
        /// in the dialog and a confirmation was given. If no confirmation was given or no 
        /// <see cref="IHydraulicBoundaryLocation"/> was selected an empty collection is returned.
        /// </summary>
        public IEnumerable<IHydraulicBoundaryLocation> SelectedLocations { get; private set; }

        protected override Button GetCancelButton()
        {
            return CustomCancelButton;
        }

        /// <summary>
        /// Gets the currently selected hydraulic boundary locations from the data grid view.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="IHydraulicBoundaryLocation"/>
        /// which were selected in the view.</returns>
        private IEnumerable<IHydraulicBoundaryLocation> GetSelectedLocations()
        {
            return GetHydraulicBoundaryLocationContextRows().Where(row => row.Selected).Select(row => row.Location).ToArray();
        }

        private void SetDataSource(IEnumerable<IHydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            dataGridViewControl.SetDataSource(hydraulicBoundaryLocations.Select(loc => new HydraulicBoundaryLocationContextRow(loc)).ToArray());
        }

        /// <summary>
        /// Initializes the <see cref="DataGridView"/>.
        /// </summary>
        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationContextRow>(row => row.Selected),
                                                  Resources.StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog_Select_Location);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationContextRow>(row => row.Name),
                                                 Resources.StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog_Location_Name,
                                                 false, DataGridViewAutoSizeColumnMode.Fill);
        }

        private IEnumerable<HydraulicBoundaryLocationContextRow> GetHydraulicBoundaryLocationContextRows()
        {
            return dataGridViewControl.Rows.Cast<DataGridViewRow>().Select(row => (HydraulicBoundaryLocationContextRow) row.DataBoundItem);
        }

        private void UpdateGenerateForSelectedButton()
        {
            GenerateForSelectedButton.Enabled = GetHydraulicBoundaryLocationContextRows().Any(r => r.Selected);
        }

        private class HydraulicBoundaryLocationContextRow
        {
            public HydraulicBoundaryLocationContextRow(IHydraulicBoundaryLocation location)
            {
                Selected = false;
                Name = location.Name;
                Location = location;
            }

            public bool Selected { get; set; }
            public string Name { get; private set; }
            public IHydraulicBoundaryLocation Location { get; private set; }
        }

        #region Event handling

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            GetHydraulicBoundaryLocationContextRows().ForEachElementDo(row => row.Selected = true);
            dataGridViewControl.RefreshDataGridView();
            UpdateGenerateForSelectedButton();
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            GetHydraulicBoundaryLocationContextRows().ForEachElementDo(row => row.Selected = false);
            dataGridViewControl.RefreshDataGridView();
            UpdateGenerateForSelectedButton();
        }

        private void InitializeEventHandlers()
        {
            dataGridViewControl.AddCellValueChangedHandler(DataGridViewCellValueChanged);
        }

        private void DataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != locationCalculateColumnIndex)
            {
                return;
            }
            UpdateGenerateForSelectedButton();
        }

        private void GenerateForSelectedButton_Click(object sender, EventArgs e)
        {
            SelectedLocations = GetSelectedLocations();
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}