// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System;
using System.Collections.Generic;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.Plugin.Handlers
{
    /// <summary>
    /// Interface for removing dune locations from a <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public interface IDuneLocationsRemoveHandler
    {
        /// <summary>
        /// Removes the dune locations of the <see cref="DuneErosionFailureMechanism"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to remove the dune locations for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/>
        /// is <c>null</c>.</exception>
        void RemoveLocations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations);

        /// <summary>
        /// Performs post-remove actions.
        /// </summary>
        void DoPostRemoveActions();
    }
}