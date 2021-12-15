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

namespace Riskeer.Storage.Core.DbContext
{
    /// <summary>
    /// Interface for an entity that contains failure path assembly result
    /// </summary>
    public interface IHasFailurePathAssemblyResultEntity
    {
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