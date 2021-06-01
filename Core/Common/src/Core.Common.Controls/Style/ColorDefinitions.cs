// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Drawing;

namespace Core.Common.Controls.Style
{
    /// <summary>
    /// Class with all color definitions that can be used in winforms controls.
    /// </summary>
    public static class ColorDefinitions
    {
        /// <summary>
        /// Gets the background color for controls.
        /// </summary>
        public static Color ControlBackgroundColor { get; } = Color.FromArgb(241, 241, 238);

        /// <summary>
        /// Gets the button background color.
        /// </summary>
        public static Color ButtonBackgroundColor { get; } = Color.FromArgb(228, 228, 223);

        /// <summary>
        /// Gets the button border color.
        /// </summary>
        public static Color ButtonBorderColor { get; } = Color.FromArgb(210, 210, 202);

        /// <summary>
        /// Gets the front color for active buttons.
        /// </summary>
        public static Color ButtonActiveFrontColor { get; } = Color.FromArgb(0, 139, 191);

        /// <summary>
        /// Gets the front color for inactive buttons.
        /// </summary>
        public static Color ButtonInactiveFrontColor { get; } = Color.Black;
    }
}