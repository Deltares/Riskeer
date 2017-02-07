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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a <seealso cref="Control"/> where WMTS locations can be administrated.
    /// </summary>
    public partial class WmtsLocationControl : UserControl, IView
    {
        private IEnumerable<WmtsCapabilityRow> capabilities;

        /// <summary>
        /// Creates a new instance of <see cref="WmtsLocationControl"/>.
        /// </summary>
        public WmtsLocationControl()
        {
            InitializeComponent();
            InitializeDataGridView();
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

        private void InitializeDataGridView()
        {
            dataGridViewControl.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewControl.MultiSelect = false;

            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapabilityRow.Id), "Kaartlaag", true);
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapabilityRow.Format), "Formaat", true);
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapabilityRow.Title), "Titel", true);
            dataGridViewControl.AddTextBoxColumn(nameof(WmtsCapabilityRow.CoordinateSystem), "Coördinatenstelsel", true);
        }

        private void UpdateDataGridViewDataSource()
        {
            dataGridViewControl.SetDataSource(capabilities?.ToArray());
        }
    }
}