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
using Core.Components.DotSpatial.Forms.Views;
using Core.Components.Gis.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection from a given set of background layers. Upon
    /// closing of the dialog, the selected background layer can be obtained.
    /// </summary>
    public partial class BackgroundMapDataSelectionDialog : DialogBase
    {
        private readonly List<IHasMapData> mapDatas;
        private IHasMapData currentMapDataControl;

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineMetaSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="mapData">The active map data or <c>null</c> if none is active.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> is <c>null</c>.</exception>
        public BackgroundMapDataSelectionDialog(IWin32Window dialogParent, WmtsMapData mapData)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 500, 350)
        {
            mapDatas = new List<IHasMapData>
            {
                new WmtsLocationControl(mapData)
            };

            SelectedMapData = mapData;

            InitializeComponent();
            InitializeButtons();
            InitializeComboBox();
            InitializeEventHandlers();
        }

        /// <summary>
        /// Gets the selected <see cref="WmtsMapData"/> or <c>null</c> if none selected.
        /// </summary>
        public WmtsMapData SelectedMapData { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SetSelectedMapData()
        {
            SelectedMapData = currentMapDataControl?.SelectedMapData;
        }

        private void UpdatePropertiesGroupBox()
        {
            if (currentMapDataControl != null)
            {
                var currentHasMapData = propertiesGroupBox.Controls.OfType<UserControl>().FirstOrDefault() as IHasMapData;
                if (currentHasMapData != null)
                {
                    currentHasMapData.SelectedMapDataChanged -= OnSelectedMapDataChanged;
                }

                propertiesGroupBox.Controls.Clear();
                Control userControl = currentMapDataControl.UserControl;
                propertiesGroupBox.Controls.Add(userControl);
                userControl.Dock = DockStyle.Fill;
                currentMapDataControl.SelectedMapDataChanged += OnSelectedMapDataChanged;
            }
        }

        #region Buttons

        private void InitializeButtons()
        {
            UpdateSelectButton();
        }

        private void UpdateSelectButton()
        {
            selectButton.Enabled = currentMapDataControl?.SelectedMapData != null;
        }

        protected override Button GetCancelButton()
        {
            return cancelButton;
        }

        #endregion

        #region ComboBox

        private void InitializeComboBox()
        {
            mapLayerComboBox.SelectedIndexChanged += MapLayerComboBox_OnSelectedIndexChanged;
            mapLayerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            mapLayerComboBox.Sorted = true;
            UpdateComboBoxDataSource();

            mapLayerComboBox.Enabled = false;
        }

        private void UpdateComboBoxDataSource()
        {
            mapLayerComboBox.DataSource = mapDatas;
            mapLayerComboBox.DisplayMember = nameof(IHasMapData.DisplayName);
        }

        #endregion

        #region Event handlers

        private void InitializeEventHandlers()
        {
            selectButton.Click += OnSelectButtonClick;
        }

        private void OnSelectButtonClick(object sender, EventArgs e)
        {
            SetSelectedMapData();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnSelectedMapDataChanged(object sender, EventArgs e)
        {
            UpdateSelectButton();
        }

        private void MapLayerComboBox_OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            var selectedItem = mapLayerComboBox.SelectedItem as IHasMapData;
            if (selectedItem == null)
            {
                return;
            }

            currentMapDataControl = selectedItem;
            UpdatePropertiesGroupBox();
        }

        #endregion
    }
}