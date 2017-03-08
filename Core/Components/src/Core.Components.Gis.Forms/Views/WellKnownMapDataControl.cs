﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Linq;
using System.Windows.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms.Properties;

namespace Core.Components.Gis.Forms.Views
{
    /// <summary>
    /// This class represents a <see cref="Control"/> where a well known map data layer can be selected.
    /// </summary>
    public partial class WellKnownMapDataControl : UserControl, IBackgroundMapDataSelectionControl
    {
        public event EventHandler<EventArgs> SelectedMapDataChanged;

        /// <summary>
        /// Creates a new instance of <see cref="WellKnownMapDataControl"/>.
        /// </summary>
        /// <param name="activeWellKnownTileSourceMapData">The active <see cref="WellKnownTileSourceMapData"/> 
        /// or <c>null</c> if none active.</param>
        public WellKnownMapDataControl(WellKnownTileSourceMapData activeWellKnownTileSourceMapData)
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeEventHandlers();

            PreSelectDataGridView(activeWellKnownTileSourceMapData);
        }

        public string DisplayName
        {
            get
            {
                return Resources.WellKnownMapDataControl_DisplayName;
            }
        }

        public ImageBasedMapData SelectedMapData
        {
            get
            {
                return dataGridViewControl.CurrentRow?.DataBoundItem as WellKnownTileSourceMapData;
            }
        }

        public UserControl UserControl => this;

        private void PreSelectDataGridView(WellKnownTileSourceMapData activeWellKnownTileSourceMapData)
        {
            if (activeWellKnownTileSourceMapData == null)
            {
                return;
            }
            DataGridViewRow dataGridViewRow = dataGridViewControl.Rows.OfType<DataGridViewRow>()
                                                                 .FirstOrDefault(row => IsMatch((WellKnownTileSourceMapData) row.DataBoundItem,
                                                                                                activeWellKnownTileSourceMapData));
            if (dataGridViewRow == null)
            {
                return;
            }
            DataGridViewCell cell = dataGridViewControl.GetCell(dataGridViewRow.Index, 0);
            dataGridViewControl.SetCurrentCell(cell);
        }

        private static bool IsMatch(WellKnownTileSourceMapData mapData, WellKnownTileSourceMapData otherMapData)
        {
            return mapData.TileSource == otherMapData.TileSource && string.Equals(mapData.Name, otherMapData.Name);
        }

        #region Event handlers

        private void InitializeEventHandlers()
        {
            dataGridViewControl.AddCurrentCellChangedHandler(DataGridViewCurrentCellChangedHandler);
        }

        #endregion

        #region DataGridView

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(nameof(WellKnownTileSourceMapData.Name), Resources.WellKnownTileSourceMapData_Description,
                                                 true, DataGridViewAutoSizeColumnMode.Fill);
            UpdateDataGridViewDataSource();
        }

        private void UpdateDataGridViewDataSource()
        {
            WellKnownTileSourceMapData[] knownTileSourceMapDatas = GetSortedWellKnownTileSourceMapDatas();

            dataGridViewControl.SetDataSource(knownTileSourceMapDatas);
            dataGridViewControl.ClearCurrentCell();
        }

        private static WellKnownTileSourceMapData[] GetSortedWellKnownTileSourceMapDatas()
        {
            var enumValues = (WellKnownTileSource[]) Enum.GetValues(typeof(WellKnownTileSource));
            return enumValues.Select(enumValue => new WellKnownTileSourceMapData(enumValue)).ToList().OrderBy(w => w.Name).ToArray();
        }

        private void DataGridViewCurrentCellChangedHandler(object sender, EventArgs e)
        {
            SelectedMapDataChanged?.Invoke(this, e);
        }

        #endregion
    }
}