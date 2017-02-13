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
using Core.Common.Controls.Views;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a <seealso cref="Control"/> where WMTS locations can be administrated.
    /// </summary>
    public partial class WmtsLocationControl : UserControl, IView
    {
        private readonly List<WmtsConnectionInfo> wmtsConnectionInfos;
        private IEnumerable<WmtsCapabilityRow> capabilities;

        /// <summary>
        /// Creates a new instance of <see cref="WmtsLocationControl"/>.
        /// </summary>
        public WmtsLocationControl()
        {
            wmtsConnectionInfos = new List<WmtsConnectionInfo>();

            InitializeComponent();
            InitializeDataGridView();
            InitializeComboBoxDataSource();
            InitializeEventHandlers();
        }

        public object Data
        {
            get
            {
                return capabilities;
            }
            set
            {
                capabilities = value as IEnumerable<WmtsCapabilityRow>;
                UpdateDataGridViewDataSource();
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
        }

        private void UpdateDataGridViewDataSource()
        {
            dataGridViewControl.SetDataSource(capabilities?.ToArray());
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

            urlLocationComboBox.SelectedItem = selectedItem;
        }

        #endregion

        #region Event handlers

        private void InitializeEventHandlers()
        {
            addLocationButton.Click += AddLocationButtonOnClick;
            editLocationButton.Click += EditLocationButtonOnClick;
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
                }
            }
        }

        #endregion
    }
}