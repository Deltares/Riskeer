// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Util.Settings;
using Core.Components.Gis.Data;
using Core.Components.Gis.Exceptions;
using Core.Components.Gis.Forms.Properties;
using Core.Components.Gis.IO.Readers;
using Core.Components.Gis.IO.Writers;
using log4net;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Components.Gis.Forms.Views
{
    /// <summary>
    /// This class represents a <see cref="Control"/> where a WMTS layer can be selected.
    /// </summary>
    public partial class WmtsLocationControl : BackgroundMapDataSelectionControl
    {
        private const string wmtsConnectionInfoFileName = "wmtsConnectionInfo.config";
        private static readonly ILog log = LogManager.GetLogger(typeof(WmtsLocationControl));
        private readonly WmtsMapData activeWmtsMapData;
        private readonly IWmtsCapabilityFactory wmtsCapabilityFactory;
        private readonly HashSet<WmtsConnectionInfo> wmtsConnectionInfos = new HashSet<WmtsConnectionInfo>();

        private readonly List<WmtsCapability> capabilities = new List<WmtsCapability>();
        private string wmtsConnectionInfoFilePath;

        private object lastSelectedCapability;
        private bool urlLocationComboBoxUpdating;

        /// <summary>
        /// Creates a new instance of <see cref="WmtsLocationControl"/>.
        /// </summary>
        /// <param name="activeWmtsMapData">The active <see cref="WmtsMapData"/> or <c>null</c> if none active.</param>
        /// <param name="wmtsCapabilityFactory">The <see cref="IWmtsCapabilityFactory"/> to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wmtsCapabilityFactory"/> is <c>null</c>.</exception>
        public WmtsLocationControl(WmtsMapData activeWmtsMapData, IWmtsCapabilityFactory wmtsCapabilityFactory)
            : base(Resources.WmtsLocationControl_DisplayName)
        {
            if (wmtsCapabilityFactory == null)
            {
                throw new ArgumentNullException(nameof(wmtsCapabilityFactory));
            }

            this.activeWmtsMapData = activeWmtsMapData;
            this.wmtsCapabilityFactory = wmtsCapabilityFactory;

            InitializeComponent();
            InitializeWmtsConnectionInfos();
            InitializeDataGridView();
            InitializeComboBox();
            InitializeEventHandlers();

            WmtsConnectionInfo selectedWmtsConnectionInfo = PreSelectComboBox();
            UpdateComboBoxDataSource(selectedWmtsConnectionInfo);

            UpdateButtons();
        }

        public override ImageBasedMapData SelectedMapData
        {
            get
            {
                WmtsCapability currentRow = GetSelectedWmtsCapability();
                if (currentRow == null)
                {
                    return null;
                }

                WmtsConnectionInfo selectedWmtsConnectionInfo = GetSelectedWmtsConnectionInfo();
                if (selectedWmtsConnectionInfo == null)
                {
                    return null;
                }

                return currentRow.ToWmtsMapdata(selectedWmtsConnectionInfo.Name, selectedWmtsConnectionInfo.Url);
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

        private WmtsConnectionInfo PreSelectComboBox()
        {
            WmtsConnectionInfo suggestedInfo = TryCreateWmtsConnectionInfo(activeWmtsMapData?.Name,
                                                                           activeWmtsMapData?.SourceCapabilitiesUrl);
            if (suggestedInfo == null)
            {
                return null;
            }

            if (!wmtsConnectionInfos.Contains(suggestedInfo))
            {
                wmtsConnectionInfos.Add(suggestedInfo);
            }

            return suggestedInfo;
        }

        private void PreSelectDataGridView()
        {
            DataGridViewRow dataGridViewRow = dataGridViewControl.Rows.OfType<DataGridViewRow>()
                                                                 .FirstOrDefault(row => IsMatch(
                                                                                     (WmtsCapability) row.DataBoundItem,
                                                                                     activeWmtsMapData));
            if (dataGridViewRow == null)
            {
                return;
            }

            DataGridViewCell cell = dataGridViewControl.GetCell(dataGridViewRow.Index, 0);
            dataGridViewControl.SetCurrentCell(cell);
        }

        private static bool IsMatch(WmtsCapability wmtsCapability, WmtsMapData wmtsMapData)
        {
            return string.Equals(wmtsCapability.Id, wmtsMapData.SelectedCapabilityIdentifier)
                   && string.Equals(wmtsCapability.Format, wmtsMapData.PreferredFormat);
        }

        private void InitializeWmtsConnectionInfos()
        {
            string folderPath = SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(SettingsHelper.Instance.ApplicationVersion);

            wmtsConnectionInfoFilePath = Path.Combine(folderPath, wmtsConnectionInfoFileName);
            wmtsConnectionInfos.UnionWith(GetSavedWmtsConnectionInfos());
        }

        private IEnumerable<WmtsConnectionInfo> GetSavedWmtsConnectionInfos()
        {
            var reader = new WmtsConnectionInfoReader();
            if (!File.Exists(wmtsConnectionInfoFilePath))
            {
                return reader.ReadDefaultWmtsConnectionInfos();
            }

            try
            {
                return reader.ReadWmtsConnectionInfos(wmtsConnectionInfoFilePath);
            }
            catch (CriticalFileReadException exception)
            {
                log.Error(exception.Message, exception);
            }

            return Enumerable.Empty<WmtsConnectionInfo>();
        }

        private void SaveWmtsConnectionInfos()
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
            if (wmtsConnectionName == null || string.IsNullOrWhiteSpace(wmtsConnectionUrl))
            {
                return null;
            }

            return new WmtsConnectionInfo(wmtsConnectionName, wmtsConnectionUrl);
        }

        private void UpdateButtons()
        {
            connectToButton.Enabled = urlLocationComboBox.SelectedItem != null;
            editLocationButton.Enabled = urlLocationComboBox.SelectedItem != null;
        }

        #region DataGridView

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapability.Id), Resources.WmtsCapability_MapLayer_Id,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapability.Format), Resources.WmtsCapability_MapLayer_Format,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapability.Title), Resources.WmtsCapability_MapLayer_Title,
                                                 true);
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapability.CoordinateSystem), Resources.WmtsCapability_MapLayer_CoordinateSystem,
                                                 true);
        }

        private void DataGridViewCurrentRowChangedHandler(object sender, EventArgs e)
        {
            SelectedMapDataChanged?.Invoke(this, e);
        }

        private void UpdateDataGridViewDataSource(IEnumerable<WmtsCapability> wmtsCapabilities)
        {
            capabilities.Clear();
            foreach (WmtsCapability wmtsCapability in wmtsCapabilities)
            {
                capabilities.Add(wmtsCapability);
            }

            UpdateDataGridViewDataSource();
        }

        private void UpdateDataGridViewDataSource()
        {
            dataGridViewControl.SetDataSource(capabilities.ToArray());
            dataGridViewControl.ClearCurrentCell();
        }

        private WmtsCapability GetSelectedWmtsCapability()
        {
            return dataGridViewControl.CurrentRow?.DataBoundItem as WmtsCapability;
        }

        private void RefreshWmtsCapabilities()
        {
            capabilities.Clear();
            UpdateDataGridViewDataSource();
        }

        #endregion

        #region ComboBox

        private void InitializeComboBox()
        {
            urlLocationComboBox.ValueMember = nameof(WmtsConnectionInfo.Url);
        }

        private void UpdateComboBoxDataSource(object selectedItem)
        {
            List<WmtsConnectionInfo> dataSource = wmtsConnectionInfos.OrderBy(i => i.Name).ToList();

            urlLocationComboBox.BeginUpdate();

            urlLocationComboBoxUpdating = true;
            urlLocationComboBox.DataSource = dataSource;
            urlLocationComboBox.DisplayMember = nameof(WmtsConnectionInfo.Name);
            urlLocationComboBox.SelectedItem = null;
            urlLocationComboBoxUpdating = false;

            urlLocationComboBox.EndUpdate();

            urlLocationComboBox.SelectedItem = selectedItem ?? dataSource.FirstOrDefault();

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
            dataGridViewControl.CurrentRowChanged += DataGridViewCurrentRowChangedHandler;
        }

        private void OnUrlLocationSelectedValueChanged(object sender, EventArgs e)
        {
            if (urlLocationComboBoxUpdating
                || urlLocationComboBox.SelectedIndex == -1
                || ReferenceEquals(lastSelectedCapability, urlLocationComboBox.SelectedValue))
            {
                return;
            }

            RefreshWmtsCapabilities();
            lastSelectedCapability = urlLocationComboBox.SelectedValue;
        }

        private void OnConnectToButtonClick(object sender, EventArgs e)
        {
            var selectedWmtsConnectionInfo = urlLocationComboBox.SelectedItem as WmtsConnectionInfo;
            ConnectToUrl(selectedWmtsConnectionInfo);

            if (activeWmtsMapData != null)
            {
                PreSelectDataGridView();
            }
        }

        private void ConnectToUrl(WmtsConnectionInfo selectedWmtsConnectionInfo)
        {
            if (selectedWmtsConnectionInfo == null)
            {
                return;
            }

            try
            {
                IEnumerable<WmtsCapability> wmtsCapabilities = wmtsCapabilityFactory.GetWmtsCapabilities(selectedWmtsConnectionInfo.Url).ToArray();
                UpdateDataGridViewDataSource(wmtsCapabilities);
            }
            catch (CannotFindTileSourceException)
            {
                Form parentForm = FindForm();
                MessageBox.Show(parentForm, string.Format(Resources.WmtsLocationControl_Unable_to_connect_to_0, selectedWmtsConnectionInfo.Name),
                                BaseResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnAddLocationButtonClick(object sender, EventArgs eventArgs)
        {
            Form parentForm = FindForm();
            using (var dialog = new WmtsConnectionDialog(parentForm))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                UpdateWmtsConnectionInfos(dialog);
            }
        }

        private void OnEditLocationButtonClick(object sender, EventArgs eventArgs)
        {
            var selectedWmtsConnectionInfo = urlLocationComboBox.SelectedItem as WmtsConnectionInfo;
            if (selectedWmtsConnectionInfo == null)
            {
                return;
            }

            Form parentForm = FindForm();
            using (var dialog = new WmtsConnectionDialog(parentForm, selectedWmtsConnectionInfo))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                UpdateWmtsConnectionInfos(dialog, selectedWmtsConnectionInfo);
            }
        }

        private void UpdateWmtsConnectionInfos(WmtsConnectionDialog dialog, WmtsConnectionInfo selectedWmtsConnectionInfo = null)
        {
            WmtsConnectionInfo createdWmtsConnectionInfo = TryCreateWmtsConnectionInfo(dialog.WmtsConnectionName,
                                                                                       dialog.WmtsConnectionUrl);
            if (createdWmtsConnectionInfo == null)
            {
                return;
            }

            if (selectedWmtsConnectionInfo != null)
            {
                wmtsConnectionInfos.Remove(selectedWmtsConnectionInfo);
            }

            wmtsConnectionInfos.Add(createdWmtsConnectionInfo);
            SaveWmtsConnectionInfos();
            UpdateComboBoxDataSource(createdWmtsConnectionInfo);
        }

        public override event EventHandler<EventArgs> SelectedMapDataChanged;

        #endregion
    }
}