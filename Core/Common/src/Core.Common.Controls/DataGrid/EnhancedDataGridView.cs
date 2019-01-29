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
using System.Windows.Forms;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Enhanced version of <see cref="DataGridView"/>, containing logic for:
    /// <list type="bullet">
    /// <item>double buffering;</item>
    /// <item>suspending current cell change propagation while binding context.</item>
    /// </list>
    /// </summary>
    public sealed class EnhancedDataGridView : DataGridView
    {
        private bool suspendCurrentCellChanges;

        /// <summary>
        /// Creates a new instance of <see cref="EnhancedDataGridView"/>.
        /// </summary>
        public EnhancedDataGridView()
        {
            DoubleBuffered = true;
        }

        protected override void OnBindingContextChanged(EventArgs e)
        {
            suspendCurrentCellChanges = true;

            base.OnBindingContextChanged(e);

            suspendCurrentCellChanges = false;
        }

        protected override void OnCurrentCellChanged(EventArgs e)
        {
            if (suspendCurrentCellChanges)
            {
                return;
            }

            base.OnCurrentCellChanged(e);
        }
    }
}