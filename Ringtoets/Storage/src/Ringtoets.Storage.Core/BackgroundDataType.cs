﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.Common.Data.AssessmentSection;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// Enumeration that defines the possible types of a <see cref="BackgroundData"/>.
    /// </summary>
    public enum BackgroundDataType
    {
        /// <summary>
        /// Web Map Tile Service background data type.
        /// </summary>
        Wmts = 1,

        /// <summary>
        /// Well known tile source background data type.
        /// </summary>
        WellKnown = 2
    }
}