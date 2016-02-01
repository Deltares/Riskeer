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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Theme
{
    /// <summary>
    /// Extension methods for the <see cref="ColorTheme"/> class
    /// </summary>
    public static class ColorThemeExtensions
    {
        /// <summary>
        /// Gets the localized string from the <see cref="Resources"/> based on the 
        /// name of the <see cref="ColorTheme"/>.
        /// </summary>
        /// <param name="theme">The <see cref="ColorTheme"/> for which to get the localized name.</param>
        /// <returns>The localized name from the <see cref="Resources"/>.</returns>
        public static string Localized(this ColorTheme theme)
        {
            return Resources.ResourceManager.GetString(Enum.GetName(typeof(ColorTheme), theme));
        }
    }
}