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
using System.Linq;
using Core.Gui.Commands;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for removing dune locations of a <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public class DuneLocationsRemoveHandler : IDuneLocationsRemoveHandler
    {
        private readonly DuneErosionFailureMechanism failureMechanism;
        private readonly IViewCommands viewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationsRemoveHandler"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to update.</param>
        /// <param name="viewCommands">The view commands used to close views for removed data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneLocationsRemoveHandler(DuneErosionFailureMechanism failureMechanism, IViewCommands viewCommands)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            this.failureMechanism = failureMechanism;
            this.viewCommands = viewCommands;
        }

        public void RemoveLocations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            DuneLocation[] locationsToRemove = failureMechanism.DuneLocations.Where(
                dl => hydraulicBoundaryLocations.Contains(dl.HydraulicBoundaryLocation)).ToArray();

            failureMechanism.RemoveDuneLocations(locationsToRemove);
        }

        public void DoPostRemoveActions()
        {
            if (!failureMechanism.DuneLocations.Any())
            {
                viewCommands.RemoveAllViewsForItem(failureMechanism);
            }
        }
    }
}