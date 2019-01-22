// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.Plugin.Handlers
{
    /// <summary>
    /// Interface for replacing dune locations of a <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public interface IDuneLocationsReplacementHandler
    {
        /// <summary>
        /// Replaces the dune locations of the <see cref="DuneErosionFailureMechanism"/>.
        /// </summary>
        /// <param name="newHydraulicBoundaryLocations">The new hydraulic boundary locations
        /// to update the dune locations for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newHydraulicBoundaryLocations"/>
        /// is <c>null</c>.</exception>
        void Replace(IEnumerable<HydraulicBoundaryLocation> newHydraulicBoundaryLocations);

        /// <summary>
        /// Performs post-replacement updates.
        /// </summary>
        void DoPostReplacementUpdates();
    }
}