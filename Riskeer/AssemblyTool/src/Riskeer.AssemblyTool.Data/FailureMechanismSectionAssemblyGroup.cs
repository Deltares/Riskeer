// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    /// Enum defining the assembly categories for a failure mechanism section.
    /// </summary>
    public enum FailureMechanismSectionAssemblyGroup
    {
        /// <summary>
        /// Represents the interpretation category ND (Not Dominant) for a failure mechanism section.
        /// </summary>
        ND = 1,

        /// <summary>
        /// Represents the interpretation category III for a failure mechanism section.
        /// </summary>
        III = 2,

        /// <summary>
        /// Represents the interpretation category II for a failure mechanism section.
        /// </summary>
        II = 3,

        /// <summary>
        /// Represents the interpretation category I for a failure mechanism section.
        /// </summary>
        I = 4,

        /// <summary>
        /// Represents the interpretation category 0+ for a failure mechanism section.
        /// </summary>
        ZeroPlus = 5,

        /// <summary>
        /// Represents the interpretation category 0 for a failure mechanism section.
        /// </summary>
        Zero = 6,

        /// <summary>
        /// Represents the interpretation category I- for a failure mechanism section.
        /// </summary>
        IMin = 7,

        /// <summary>
        /// Represents the interpretation category II- for a failure mechanism section.
        /// </summary>
        IIMin = 8,

        /// <summary>
        /// Represents the interpretation category III- for a failure mechanism section.
        /// </summary>
        IIIMin = 9,

        /// <summary>
        /// Represents the interpretation category D (Dominant) for a failure mechanism section.
        /// </summary>
        D = 10,

        /// <summary>
        /// Represents the interpretation category GR for a failure mechanism section.
        /// </summary>
        Gr = 11
    }
}