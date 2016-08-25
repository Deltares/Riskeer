﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.HydraRing.Calculation.IO
{
    /// <summary>
    /// The columns of the design tables settings file.
    /// </summary>
    public enum DesignTablesSettingsColumns
    {
        /// <summary>
        /// The ring id column.
        /// </summary>
        RingId,

        /// <summary>
        /// The variable column.
        /// </summary>
        Variable,

        /// <summary>
        /// The min column.
        /// </summary>
        Min,

        /// <summary>
        /// The max column.
        /// </summary>
        Max
    }
}