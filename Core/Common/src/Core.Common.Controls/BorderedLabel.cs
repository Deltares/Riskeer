// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Windows.Forms;

namespace Core.Common.Controls
{
    /// <summary>
    /// Custom <see cref="Label"/> control with a border.
    /// </summary>
    public sealed class BorderedLabel : Label
    {
        /// <summary>
        /// Creates a new instance of <see cref="BorderedLabel"/>.
        /// </summary>
        public BorderedLabel()
        {
            AutoSize = true;
            BorderStyle = BorderStyle.FixedSingle;
            Dock = DockStyle.Fill;
            MinimumSize = new Size(50, 0);
            Padding = new Padding(5, 0, 5, 0);
            TextAlign = ContentAlignment.MiddleLeft;
        }
    }
}