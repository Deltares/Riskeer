﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Enumeration that defines tree node checked states.
    /// </summary>
    public enum TreeNodeCheckedState
    {
        /// <summary>
        /// Tree node checked state.
        /// </summary>
        Checked = 1,

        /// <summary>
        /// Tree node unchecked state.
        /// </summary>
        Unchecked = 2,

        /// <summary>
        /// Tree node mixed state.
        /// </summary>
        Mixed = 3
    }
}