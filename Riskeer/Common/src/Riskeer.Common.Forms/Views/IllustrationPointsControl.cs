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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Control to show illustration points.
    /// </summary>
    public partial class IllustrationPointsControl : UserControl, ISelectionProvider
    {
        private IEnumerable<IllustrationPointControlItem> data;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointsControl"/>.
        /// </summary>
        public IllustrationPointsControl()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        /// <summary>
        /// Gets or sets the data of the control.
        /// </summary>
        public IEnumerable<IllustrationPointControlItem> Data
        {
            get
            {
                return data;
            }
            set
            {
                if (data != null && data.Equals(value)
                    || data == null && value == null)
                {
                    return;
                }

                data = value;
                illustrationPointsChartControl.Data = data;
                illustrationPointsTableControl.Data = data;
            }
        }

        public object Selection
        {
            get
            {
                return illustrationPointsTableControl.Selection;
            }
        }

        private void InitializeEventHandlers()
        {
            illustrationPointsTableControl.SelectionChanged += IllustrationPointsTableControlOnSelectionChanged;
        }

        private void IllustrationPointsTableControlOnSelectionChanged(object sender, EventArgs e)
        {
            OnSelectionChanged(e);
        }

        private void OnSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}