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

using System.Collections.Generic;

namespace Riskeer.Storage.Core.DbContext
{
    /// <summary>
    /// Interface for an entity that contains failure path information.
    /// </summary>
    public interface IFailurePathEntity
    {
        /// <summary>
        /// Gets or sets an indicator whether the failure path is part of the assembly.
        /// </summary>
        byte InAssembly { get; set; }

        /// <summary>
        /// Gets or sets the source path of the imported collection of failure mechanism sections.
        /// </summary>
        string FailureMechanismSectionCollectionSourcePath { get; set; }

        /// <summary>
        /// Gets or sets the comments associated with the input when it is part of the assembly.
        /// </summary>
        string InAssemblyInputComments { get; set; }

        /// <summary>
        /// Gets or sets the comments associated with the output when it is part of the assembly.
        /// </summary>
        string InAssemblyOutputComments { get; set; }

        /// <summary>
        /// Gets or sets the comments associated when the failure path is set to not be part of the assembly.
        /// </summary>
        string NotInAssemblyComments { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="FailureMechanismSectionEntity"/>.
        /// </summary>
        ICollection<FailureMechanismSectionEntity> FailureMechanismSectionEntities { get; set; }
        
        /// <summary>
        /// Gets or sets the failure path assembly probability result type.
        /// </summary>
        byte FailurePathAssemblyProbabilityResultType { get; set; }

        /// <summary>
        /// Gets or sets the manual failure path assembly probability.
        /// </summary>
        double? ManualFailurePathAssemblyProbability { get; set; }
    }
}