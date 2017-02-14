// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Components.DotSpatial.Forms.IO;
using Core.Components.DotSpatial.Forms.Properties;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.Gis.Data;
using log4net;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Components.DotSpatial.Forms.Views
{
    /// <summary>
    /// This class represents a <seealso cref="Control"/> where WMTS locations can be administrated.
    /// </summary>
    public partial class WmtsLocationControl : UserControl, IHasMapData
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WmtsLocationControl));
        private readonly List<WmtsConnectionInfo> wmtsConnectionInfos;
        private readonly List<WmtsCapabilityRow> capabilities;

        /// <summary>
        /// Creates a new instance of <see cref="WmtsLocationControl"/>.
        /// </summary>
        public WmtsLocationControl()
        {
            wmtsConnectionInfos = new List<WmtsConnectionInfo>();
            capabilities = new List<WmtsCapabilityRow>();

            InitializeComponent();
            InitializeDataGridView();
            InitializeComboBoxDataSource();
            InitializeEventHandlers();
        }

        public string DisplayName
        {
            get
            {
                return Resources.WmtsLocationControl_DisplayName;
            }
        }

        public MapData SelectedMapData
        {
            get
            {
                WmtsCapabilityRow currentRow = GetSelectedWmtsCapabilityRow();
                if (currentRow == null)
                {
                    return null;
                }

                WmtsConnectionInfo selectedWmtsConnectionInfo = GetSelectedWmtsConnectionInfo();
                if (selectedWmtsConnectionInfo == null)
                {
                    return null;
                }

                return currentRow.WmtsCapability.ToWmtsMapdata(selectedWmtsConnectionInfo.Name, selectedWmtsConnectionInfo.Url);
            }
        }

        public UserControl UserControl
        {
            get
            {
                return this;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private WmtsConnectionInfo TryCreateWmtsConnectionInfo(string wmtsConnectionName, string wmtsConnectionUrl)
        {
            try
            {
                return new WmtsConnectionInfo(wmtsConnectionName, wmtsConnectionUrl);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        #region DataGridView

        private void InitializeDataGridView()
        {
            dataGridViewControl.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewControl.MultiSelect = false;

            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapabilityRow.Id), Resources.WmtsCapability_MapLayer_Id,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapabilityRow.Format), Resources.WmtsCapability_MapLayer_Format,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapabilityRow.Title), Resources.WmtsCapability_MapLayer_Title,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapabilityRow.CoordinateSystem), Resources.WmtsCapability_MapLayer_CoordinateSystem,
                                                 true);
            dataGridViewControl.AddCurrentCellChangedHandler(DataGridViewCurrentCellChangedHandler);
        }

        private void DataGridViewCurrentCellChangedHandler(object sender, EventArgs e) {}

        private void UpdateDataGridViewDataSource(IEnumerable<WmtsCapability> wmtsCapabilities)
        {
            capabilities.Clear();
            foreach (var wmtsCapability in wmtsCapabilities)
            {
                capabilities.Add(new WmtsCapabilityRow(wmtsCapability));
            }

            UpdateDataGridViewDataSource();
        }

        private void UpdateDataGridViewDataSource()
        {
            dataGridViewControl.SetDataSource(capabilities);
        }

        private WmtsCapabilityRow GetSelectedWmtsCapabilityRow()
        {
            return dataGridViewControl.CurrentRow?.DataBoundItem as WmtsCapabilityRow;
        }

        #endregion

        #region ComboBox

        private void InitializeComboBoxDataSource()
        {
            urlLocationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            urlLocationComboBox.Sorted = true;
            UpdateComboBoxDataSource();
        }

        private void UpdateComboBoxDataSource()
        {
            object selectedItem = urlLocationComboBox.SelectedItem;
            urlLocationComboBox.DataSource = null;
            urlLocationComboBox.DataSource = wmtsConnectionInfos;
            urlLocationComboBox.DisplayMember = nameof(WmtsConnectionInfo.Name);
            urlLocationComboBox.ValueMember = nameof(WmtsConnectionInfo.Url);

            urlLocationComboBox.SelectedItem = selectedItem ?? wmtsConnectionInfos.FirstOrDefault();

            UpdateConnectToButton();
        }

        private WmtsConnectionInfo GetSelectedWmtsConnectionInfo()
        {
            return urlLocationComboBox.SelectedItem as WmtsConnectionInfo;
        }

        #endregion

        #region Event handlers

        private void InitializeEventHandlers()
        {
            UpdateConnectToButton();
            connectToButton.Click += ConnectToButtonOnClick;
            addLocationButton.Click += AddLocationButtonOnClick;
            editLocationButton.Click += EditLocationButtonOnClick;
        }

        private void UpdateConnectToButton()
        {
            connectToButton.Enabled = urlLocationComboBox.SelectedItem != null;
        }

        private void ConnectToButtonOnClick(object sender, EventArgs e)
        {
            var selectedWmtsConnectionInfo = urlLocationComboBox.SelectedItem as WmtsConnectionInfo;

            try
            {
                IEnumerable<WmtsCapability> wmtsCapabilities = WmtsCapabilityFactory.GetWmtsCapabilities(selectedWmtsConnectionInfo?.Url).ToArray();
                UpdateDataGridViewDataSource(wmtsCapabilities);
            }
            catch (CannotFindTileSourceException exception)
            {
                Form controlForm = FindForm();
                MessageBox.Show(controlForm, exception.Message, BaseResources.Error, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                log.Error(exception.Message, exception);
            }
        }

        private void AddLocationButtonOnClick(object sender, EventArgs eventArgs)
        {
            Form controlForm = FindForm();
            using (var dialog = new WmtsConnectionDialog(controlForm))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                WmtsConnectionInfo createdWmtsConnectionInfos = TryCreateWmtsConnectionInfo(dialog.WmtsConnectionName,
                                                                                            dialog.WmtsConnectionUrl);
                if (createdWmtsConnectionInfos != null)
                {
                    wmtsConnectionInfos.Add(createdWmtsConnectionInfos);
                    UpdateComboBoxDataSource();
                }
            }
        }

        private void EditLocationButtonOnClick(object sender, EventArgs eventArgs)
        {
            var selectedWmtsConnectionInfo = urlLocationComboBox.SelectedItem as WmtsConnectionInfo;
            if (selectedWmtsConnectionInfo == null)
            {
                return;
            }
            Form controlForm = FindForm();
            using (var dialog = new WmtsConnectionDialog(controlForm, selectedWmtsConnectionInfo))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                WmtsConnectionInfo createdWmtsConnectionInfos = TryCreateWmtsConnectionInfo(dialog.WmtsConnectionName,
                                                                                            dialog.WmtsConnectionUrl);
                if (createdWmtsConnectionInfos != null)
                {
                    wmtsConnectionInfos.Remove(selectedWmtsConnectionInfo);
                    wmtsConnectionInfos.Add(createdWmtsConnectionInfos);
                    UpdateComboBoxDataSource();

                    urlLocationComboBox.SelectedItem = createdWmtsConnectionInfos;
                    UpdateConnectToButton();
                }
            }
        }

        #endregion
    }
}