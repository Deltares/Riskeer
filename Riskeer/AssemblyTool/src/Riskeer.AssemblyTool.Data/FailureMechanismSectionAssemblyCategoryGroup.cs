// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
    public enum FailureMechanismSectionAssemblyCategoryGroup
    {
        /// <summary>
        /// Represents the assembly category GR (No result) for a failure mechanism section.
        /// </summary>
        None = 1,

        /// <summary>
        /// Represents the assembly category NVT (Not applicable) for a failure mechanism section.
        /// </summary>
        NotApplicable = 2,

        /// <summary>
        /// Represents the assembly category Iv for a failure mechanism section.
        /// </summary>
        Iv = 3,

        /// <summary>
        /// Represents the assembly category IIv for a failure mechanism section.
        /// </summary>
        IIv = 4,

        /// <summary>
        /// Represents the assembly category IIIv for a failure mechanism section.
        /// </summary>
        IIIv = 5,

        /// <summary>
        /// Represents the assembly category IVv for a failure mechanism section.
        /// </summary>
        IVv = 6,

        /// <summary>
        /// Represents the assembly category Vv for a failure mechanism section.
        /// </summary>
        Vv = 7,

        /// <summary>
        /// Represents the assembly category VIv for a failure mechanism section.
        /// </summary>
        VIv = 8,

        /// <summary>
        /// Represents the assembly category VIIv for a failure mechanism section.
        /// </summary>
        VIIv = 9
    }
}