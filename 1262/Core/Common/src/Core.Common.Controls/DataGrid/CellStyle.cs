﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Class describing the different cell styles for the <see cref="DataGridViewControl"/>.
    /// </summary>
    public sealed class CellStyle
    {
        /// <summary>
        /// The cell style for enabled cells.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly CellStyle Enabled = new CellStyle
        {
            TextColor = Color.FromKnownColor(KnownColor.ControlText),
            BackgroundColor = Color.FromKnownColor(KnownColor.White)
        };

        /// <summary>
        /// The cell style of disabled cells.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly CellStyle Disabled = new CellStyle
        {
            TextColor = Color.FromKnownColor(KnownColor.GrayText),
            BackgroundColor = Color.FromKnownColor(KnownColor.DarkGray)
        };

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color TextColor { get; private set; }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        public Color BackgroundColor { get; private set; }
    }
}