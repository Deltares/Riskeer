﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Components.BruTile.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection from a given set of background layers. Upon
    /// closing of the dialog, the selected background layer can be obtained.
    /// </summary>
    public partial class BackgroundMapDataSelectionDialog : DialogBase
    {
        private readonly HashSet<IBackgroundMapDataSelectionControl> mapDatas;
        private IBackgroundMapDataSelectionControl currentBackgroundMapDataSelectionControl;
        private bool mapLayerComboBoxUpdating;

        /// <summary>
        /// Creates a new instance of <see cref="BackgroundMapDataSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="mapData">The active map data or <c>null</c> if none is active.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> is <c>null</c>.</exception>
        public BackgroundMapDataSelectionDialog(IWin32Window dialogParent, ImageBasedMapData mapData)
            : base(dialogParent, RingtoetsCommonFormsResources.SelectionDialogIcon, 500, 350)
        {
            mapDatas = new HashSet<IBackgroundMapDataSelectionControl>
            {
                new WellKnownMapDataControl(mapData as WellKnownTileSourceMapData),
                new WmtsLocationControl(mapData as WmtsMapData, new BruTileWmtsCapabilityFactory())
            };

            InitializeComponent();
            InitializeButtons();
            InitializeComboBox();

            IBackgroundMapDataSelectionControl controlToSelect = GetBackgroundMapDataControlToSelect(mapData);
            PreSelectComboBox(controlToSelect);
        }

        /// <summary>
        /// Gets the selected <see cref="ImageBasedMapData"/> or <c>null</c> if none selected.
        /// </summary>
        public ImageBasedMapData SelectedMapData { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private IBackgroundMapDataSelectionControl GetBackgroundMapDataControlToSelect(ImageBasedMapData mapData)
        {
            if (mapData is WellKnownTileSourceMapData)
            {
                return mapDatas.OfType<WellKnownMapDataControl>().Single();
            }
            if (mapData is WmtsMapData)
            {
                return mapDatas.OfType<WmtsLocationControl>().Single();
            }
            return mapDatas.First();
        }

        private void UpdatePropertiesGroupBox(IBackgroundMapDataSelectionControl newBackgroundMapDataSelectionControl)
        {
            if (currentBackgroundMapDataSelectionControl != null)
            {
                currentBackgroundMapDataSelectionControl.SelectedMapDataChanged -= OnSelectedMapDataSelectionChanged;
            }

            propertiesGroupBox.Controls.Clear();
            Control userControl = newBackgroundMapDataSelectionControl.UserControl;
            propertiesGroupBox.Controls.Add(userControl);
            userControl.Dock = DockStyle.Fill;
            newBackgroundMapDataSelectionControl.SelectedMapDataChanged += OnSelectedMapDataSelectionChanged;

            currentBackgroundMapDataSelectionControl = newBackgroundMapDataSelectionControl;
        }

        #region Buttons

        private void InitializeButtons()
        {
            UpdateSelectButton();
        }

        private void UpdateSelectButton()
        {
            selectButton.Enabled = currentBackgroundMapDataSelectionControl?.SelectedMapData != null;
        }

        protected override Button GetCancelButton()
        {
            return cancelButton;
        }

        #endregion

        #region ComboBox

        private void InitializeComboBox()
        {
            mapLayerComboBox.ValueMember = nameof(IBackgroundMapDataSelectionControl.UserControl);
        }

        private void PreSelectComboBox(IBackgroundMapDataSelectionControl controlToSelect)
        {
            mapLayerComboBox.BeginUpdate();

            mapLayerComboBoxUpdating = true;
            mapLayerComboBox.DataSource = mapDatas.ToArray();
            mapLayerComboBox.DisplayMember = nameof(IBackgroundMapDataSelectionControl.DisplayName);
            mapLayerComboBox.SelectedItem = null;
            mapLayerComboBoxUpdating = false;

            mapLayerComboBox.EndUpdate();

            mapLayerComboBox.SelectedItem = controlToSelect;
            currentBackgroundMapDataSelectionControl = controlToSelect;
        }

        #endregion

        #region Event handlers

        private void OnSelectButtonClick(object sender, EventArgs e)
        {
            SelectedMapData = currentBackgroundMapDataSelectionControl?.SelectedMapData;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnSelectedMapDataSelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectButton();
        }

        private void MapLayerComboBox_OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (mapLayerComboBoxUpdating || mapLayerComboBox.SelectedIndex == -1)
            {
                return;
            }

            UpdatePropertiesGroupBox((IBackgroundMapDataSelectionControl) mapLayerComboBox.SelectedItem);
        }

        #endregion
    }
}