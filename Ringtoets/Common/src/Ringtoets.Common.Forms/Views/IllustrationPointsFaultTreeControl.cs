// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Control to show fault tree illustration points.
    /// </summary>
    public partial class IllustrationPointsFaultTreeControl : UserControl, ISelectionProvider
    {
        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointsFaultTreeControl"/>.
        /// </summary>
        public IllustrationPointsFaultTreeControl()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        /// <summary>
        /// Gets or sets the data of the control.
        /// </summary>
        public object Data { get; set; }

        public object Selection
        {
            get
            {
                return null;
            }
        }

        private void InitializeEventHandlers()
        {
            pointedTreeGraphControl.SelectionChanged += FaultTreeIllustrationPointsControlOnSelectionChanged;
        }

        private void FaultTreeIllustrationPointsControlOnSelectionChanged(object sender, EventArgs e)
        {
            OnSelectionChanged(e);
        }

        private void OnSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}