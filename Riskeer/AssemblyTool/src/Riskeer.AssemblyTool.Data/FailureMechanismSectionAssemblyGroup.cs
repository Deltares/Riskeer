﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// Enum defining the assembly groups for a failure mechanism section.
    /// </summary>
    public enum FailureMechanismSectionAssemblyGroup
    {
        /// <summary>
        /// Represents group Not Dominant.
        /// </summary>
        NotDominant = 1,

        /// <summary>
        /// Represents group III.
        /// </summary>
        III = 2,

        /// <summary>
        /// Represents group II.
        /// </summary>
        II = 3,

        /// <summary>
        /// Represents group I.
        /// </summary>
        I = 4,

        /// <summary>
        /// Represents group 0.
        /// </summary>
        Zero = 5,

        /// <summary>
        /// Represents group I-.
        /// </summary>
        IMin = 6,

        /// <summary>
        /// Represents group II-.
        /// </summary>
        IIMin = 7,

        /// <summary>
        /// Represents group III-.
        /// </summary>
        IIIMin = 8,

        /// <summary>
        /// Represents group Dominant.
        /// </summary>
        Dominant = 9,

        /// <summary>
        /// Represents group GR.
        /// </summary>
        Gr = 10
    }
}