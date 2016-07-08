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

using System;

namespace Core.Common.Gui.Forms.ViewManager
{
    /// <summary>
    /// Flags on where views can be located.
    /// </summary>
    [Flags]
    public enum ViewLocation
    {
        /// <summary>
        /// The location reserved for Document Views.
        /// </summary>
        Document = 0x0,
        /// <summary>
        /// Left of the location reserved for Document Views.
        /// </summary>
        Left = 0x1,
        /// <summary>
        /// Right of the location reserved for Document Views.
        /// </summary>
        Right = 0x2,
        /// <summary>
        /// Above the location reserved for Document Views.
        /// </summary>
        Top = 0x4,
        /// <summary>
        /// Below the location reserved for Document Views.
        /// </summary>
        Bottom = 0x8,
        /// <summary>
        /// Floating panel.
        /// </summary>
        Floating = 0x16
    };
}