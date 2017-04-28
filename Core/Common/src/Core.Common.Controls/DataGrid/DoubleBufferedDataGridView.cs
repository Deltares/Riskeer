// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Windows.Forms;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Double buffered version of<see cref="DataGridView"/>.
    /// </summary>
    /// <remarks>
    /// Also see http://stackoverflow.com/questions/252689/why-does-the-doublebuffered-property-default-to-false-on-a-datagridview-and-why.
    /// </remarks>
    public sealed class DoubleBufferedDataGridView : DataGridView
    {
        /// <summary>
        /// Creates a new instance of <see cref="DoubleBufferedDataGridView"/>.
        /// </summary>
        public DoubleBufferedDataGridView()
        {
            DoubleBuffered = true;
        }
    }
}