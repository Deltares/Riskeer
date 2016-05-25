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

using System.Drawing;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Class describing the different cell styles for the <see cref="DataGridViewControl"/>.
    /// </summary>
    public class CellStyle
    {
        /// <summary>
        /// The cell style for enabled cells.
        /// </summary>
        public static readonly CellStyle Enabled = new CellStyle
        {
            TextColor = Color.FromKnownColor(KnownColor.ControlText),
            BackgroundColor = Color.FromKnownColor(KnownColor.White)
        };

        /// <summary>
        /// The cell style of disabled cells.
        /// </summary>
        public static readonly CellStyle Disabled = new CellStyle
        {
            TextColor = Color.FromKnownColor(KnownColor.GrayText),
            BackgroundColor = Color.FromKnownColor(KnownColor.DarkGray)
        };

        public Color TextColor { get; private set; }
        public Color BackgroundColor { get; private set; }
    }
}