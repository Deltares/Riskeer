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
using System.Windows.Forms;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// This class makes it easier to temporarily disable automatic resizing of a column,
    /// for example when its data is being changed or you are replacing the list items
    /// available in a combo-box for that column.
    /// 
    /// This resolves the "DataGridViewComboBoxCell value is not valid" error when updating the <see cref="DataGridViewComboBoxColumn"/>.
    /// That error turns out not to refer to the content of the value, 
    /// but to the string representation of the value not fitting inside the cell current width.
    /// </summary>
    public class SuspendDataGridViewColumnResizes : IDisposable
    {
        private readonly DataGridViewColumn column;
        private readonly DataGridViewAutoSizeColumnMode originalValue;

        /// <summary>
        /// Creates a new instance of <see cref="SuspendDataGridViewColumnResizes"/>.
        /// </summary>
        /// <param name="columnToSuspend">The column to suspend.</param>
        public SuspendDataGridViewColumnResizes(DataGridViewColumn columnToSuspend)
        {
            column = columnToSuspend;
            originalValue = columnToSuspend.AutoSizeMode;
            columnToSuspend.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                column.AutoSizeMode = originalValue;
            }
        }
    }
}