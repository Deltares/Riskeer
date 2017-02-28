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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Gui.Settings;
using Core.Common.IO.Exceptions;
using Core.Components.DotSpatial.Forms.IO;
using Core.Components.DotSpatial.Forms.Properties;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.Gis.Data;
using log4net;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Components.DotSpatial.Forms.Views
{
    /// <summary>
    /// This class represents a <see cref="Control"/> where a WMTS layer can be selected.
    /// </summary>
    public partial class WmtsLocationControl : UserControl, IBackgroundMapDataSelectionControl
    {
        private const string wmtsConnectionInfoFileName = "wmtsConnectionInfo.config";
        private static readonly ILog log = LogManager.GetLogger(typeof(WmtsLocationControl));
        private readonly List<WmtsConnectionInfo> wmtsConnectionInfos;

        private readonly List<WmtsCapabilityRow> capabilities;
        private string wmtsConnectionInfoFilePath;

        /// <summary>
        /// Creates a new instance of <see cref="WmtsLocationControl"/>.
        /// </summary>
        /// <param name="activeWmtsMapData">The active <see cref="WmtsMapData"/> or <c>null</c> if none active.</param>
        public WmtsLocationControl(WmtsMapData activeWmtsMapData)
        {
            wmtsConnectionInfos = new List<WmtsConnectionInfo>();
            capabilities = new List<WmtsCapabilityRow>();

            InitializeComponent();
            InitializeWmtsConnectionInfos();
            InitializeDataGridView();
            InitializeComboBoxDataSource();
            InitializeEventHandlers();

            SetActiveWmtsMapData(activeWmtsMapData);

            UpdateButtons();
        }

        public string DisplayName
        {
            get
            {
                return Resources.WmtsLocationControl_DisplayName;
            }
        }

        public WmtsMapData SelectedMapData
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

        private void SetActiveWmtsMapData(WmtsMapData activeWmtsMapData)
        {
            WmtsConnectionInfo suggestedInfo = TryCreateWmtsConnectionInfo(activeWmtsMapData?.Name,
                                                                           activeWmtsMapData?.SourceCapabilitiesUrl);
            if (activeWmtsMapData == null || suggestedInfo == null)
            {
                return;
            }

            if (!wmtsConnectionInfos.Any(wi => wi.Equals(suggestedInfo)))
            {
                wmtsConnectionInfos.Add(suggestedInfo);
            }

            UpdateComboBoxDataSource(suggestedInfo);

            DataGridViewRow dataGridViewRow = dataGridViewControl.Rows.OfType<DataGridViewRow>()
                                                                 .FirstOrDefault(row => IsMatch(
                                                                                     (WmtsCapabilityRow) row.DataBoundItem,
                                                                                     activeWmtsMapData));
            if (dataGridViewRow == null)
            {
                return;
            }
            DataGridViewCell cell = dataGridViewControl.GetCell(dataGridViewRow.Index, 0);
            dataGridViewControl.SetCurrentCell(cell);
        }

        private static bool IsMatch(WmtsCapabilityRow wmtsCapabilityRow, WmtsMapData wmtsMapData)
        {
            return string.Equals(wmtsCapabilityRow.Id, wmtsMapData.SelectedCapabilityIdentifier)
                   && string.Equals(wmtsCapabilityRow.Format, wmtsMapData.PreferredFormat);
        }

        private void InitializeWmtsConnectionInfos()
        {
            string localUserSettingsDirectory = SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(SettingsHelper.Instance.ApplicationVersion);

            wmtsConnectionInfoFilePath = Path.Combine(localUserSettingsDirectory, wmtsConnectionInfoFileName);
            wmtsConnectionInfos.AddRange(TryGetSavedWmtsConnectionInfos());
        }

        private IEnumerable<WmtsConnectionInfo> TryGetSavedWmtsConnectionInfos()
        {
            try
            {
                var reader = new WmtsConnectionInfoReader();
                return reader.ReadWmtsConnectionInfos(wmtsConnectionInfoFilePath);
            }
            catch (CriticalFileReadException exception)
            {
                log.Error(exception.Message, exception);
            }
            return Enumerable.Empty<WmtsConnectionInfo>();
        }

        private void TrySaveWmtsConnectionInfos()
        {
            try
            {
                var writer = new WmtsConnectionInfoWriter(wmtsConnectionInfoFilePath);
                writer.WriteWmtsConnectionInfo(wmtsConnectionInfos);
            }
            catch (CriticalFileWriteException exception)
            {
                log.Error(exception.Message, exception);
            }
        }

        private static WmtsConnectionInfo TryCreateWmtsConnectionInfo(string wmtsConnectionName, string wmtsConnectionUrl)
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

        private void UpdateButtons()
        {
            connectToButton.Enabled = urlLocationComboBox.SelectedItem != null;
            editLocationButton.Enabled = urlLocationComboBox.SelectedItem != null;
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
        }

        private void DataGridViewCurrentCellChangedHandler(object sender, EventArgs e)
        {
            SelectedMapDataChanged?.Invoke(this, e);
        }

        private void UpdateDataGridViewDataSource(IEnumerable<WmtsCapability> wmtsCapabilities)
        {
            capabilities.Clear();
            foreach (WmtsCapability wmtsCapability in wmtsCapabilities)
            {
                capabilities.Add(new WmtsCapabilityRow(wmtsCapability));
            }

            UpdateDataGridViewDataSource();
        }

        private void UpdateDataGridViewDataSource()
        {
            dataGridViewControl.SetDataSource(capabilities.ToArray());
            dataGridViewControl.ClearCurrentCell();
        }

        private WmtsCapabilityRow GetSelectedWmtsCapabilityRow()
        {
            return dataGridViewControl.CurrentRow?.DataBoundItem as WmtsCapabilityRow;
        }

        private void ClearDataGridViewDataSource()
        {
            capabilities.Clear();
            UpdateDataGridViewDataSource();
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
            UpdateComboBoxDataSource(urlLocationComboBox.SelectedItem);
        }

        private void UpdateComboBoxDataSource(object selectedItem)
        {
            urlLocationComboBox.DataSource = null;
            urlLocationComboBox.DataSource = wmtsConnectionInfos;
            urlLocationComboBox.DisplayMember = nameof(WmtsConnectionInfo.Name);
            urlLocationComboBox.ValueMember = nameof(WmtsConnectionInfo.Url);

            if (selectedItem != null)
            {
                urlLocationComboBox.SelectedItem = selectedItem;
            }

            UpdateButtons();
        }

        private WmtsConnectionInfo GetSelectedWmtsConnectionInfo()
        {
            return urlLocationComboBox.SelectedItem as WmtsConnectionInfo;
        }

        #endregion

        #region Event handlers

        private void InitializeEventHandlers()
        {
            dataGridViewControl.AddCurrentCellChangedHandler(DataGridViewCurrentCellChangedHandler);

            urlLocationComboBox.SelectedIndexChanged += OnUrlLocationSelectedIndexChanged;

            connectToButton.Click += OnConnectToButtonClick;
            addLocationButton.Click += OnAddLocationButtonClick;
            editLocationButton.Click += OnEditLocationButtonClick;
        }

        private void OnUrlLocationSelectedIndexChanged(object sender, EventArgs e)
        {
            ClearDataGridViewDataSource();

            var selectedWmtsConnectionInfo = urlLocationComboBox.SelectedItem as WmtsConnectionInfo;
            ConnectToUrl(selectedWmtsConnectionInfo);
        }

        private void OnConnectToButtonClick(object sender, EventArgs e)
        {
            var selectedWmtsConnectionInfo = urlLocationComboBox.SelectedItem as WmtsConnectionInfo;
            ConnectToUrl(selectedWmtsConnectionInfo);
        }

        private void ConnectToUrl(WmtsConnectionInfo selectedWmtsConnectionInfo)
        {
            if (selectedWmtsConnectionInfo == null)
            {
                return;
            }
            try
            {
                IEnumerable<WmtsCapability> wmtsCapabilities = WmtsCapabilityFactory.GetWmtsCapabilities(selectedWmtsConnectionInfo.Url).ToArray();
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

        private void OnAddLocationButtonClick(object sender, EventArgs eventArgs)
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
                    TrySaveWmtsConnectionInfos();
                    UpdateComboBoxDataSource(createdWmtsConnectionInfos);
                }
            }
        }

        private void OnEditLocationButtonClick(object sender, EventArgs eventArgs)
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
                    TrySaveWmtsConnectionInfos();
                    UpdateComboBoxDataSource(createdWmtsConnectionInfos);
                }
            }
        }

        public event EventHandler<EventArgs> SelectedMapDataChanged;

        #endregion
    }
}