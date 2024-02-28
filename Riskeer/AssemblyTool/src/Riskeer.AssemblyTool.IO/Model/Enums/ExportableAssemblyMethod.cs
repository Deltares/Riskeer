// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using Core.Common.Util.Attributes;
using Riskeer.AssemblyTool.IO.Properties;

namespace Riskeer.AssemblyTool.IO.Model.Enums
{
    /// <summary>
    /// Enum defining the exportable assembly methods.
    /// </summary>
    public enum ExportableAssemblyMethod
    {
        /// <summary>
        /// Represents the assembly method BOI-0A-1.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI0A1_DisplayName))]
        BOI0A1 = 1,

        /// <summary>
        /// Represents the assembly method BOI-0A-2.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI0A2_DisplayName))]
        BOI0A2 = 2,

        /// <summary>
        /// Represents the assembly method BOI-0B-1.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI0B1_DisplayName))]
        BOI0B1 = 3,

        /// <summary>
        /// Represents the assembly method BOI-0C-1.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI0C1_DisplayName))]
        BOI0C1 = 4,

        /// <summary>
        /// Represents the assembly method BOI-0C-2.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI0C2_DisplayName))]
        BOI0C2 = 5,

        /// <summary>
        /// Represents the assembly method BOI-1A-1.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI1A1_DisplayName))]
        BOI1A1 = 6,

        /// <summary>
        /// Represents the assembly method BOI-1A-2.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI1A2_DisplayName))]
        BOI1A2 = 7,

        /// <summary>
        /// Represents a manual failure mechanism assembly.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_Manual_DisplayName))]
        Manual = 8,

        /// <summary>
        /// Represents the assembly method BOI-2A-1.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI2A1_DisplayName))]
        BOI2A1 = 9,

        /// <summary>
        /// Represents the assembly method BOI-2A-2.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI2A2_DisplayName))]
        BOI2A2 = 10,

        /// <summary>
        /// Represents the assembly method BOI-2B-1.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssemblyMethod_BOI2B1_DisplayName))]
        BOI2B1 = 11
    }
}