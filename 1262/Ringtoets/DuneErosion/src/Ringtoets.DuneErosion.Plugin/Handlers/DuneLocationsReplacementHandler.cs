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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Commands;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.IO;
using Ringtoets.DuneErosion.Service;

namespace Ringtoets.DuneErosion.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for replacing dune locations on a <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public class DuneLocationsReplacementHandler
    {
        private readonly IViewCommands viewCommands;
        private readonly DuneErosionFailureMechanism failureMechanism;
        private IEnumerable<ReadDuneLocation> newDuneLocations;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationsReplacementHandler"/>.
        /// </summary>
        /// <param name="viewCommands">The view commands used to close views for removed data.</param>
        /// <param name="failureMechanism">The failure mechanism to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneLocationsReplacementHandler(IViewCommands viewCommands, DuneErosionFailureMechanism failureMechanism)
        {
            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            this.viewCommands = viewCommands;
            this.failureMechanism = failureMechanism;
        }

        /// <summary>
        /// Replaces the dune locations of the <see cref="DuneErosionFailureMechanism"/>.
        /// </summary>
        /// <param name="newHydraulicBoundaryLocations">The new hydraulic boundary locations
        /// to update the dune locations for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newHydraulicBoundaryLocations"/>
        /// is <c>null</c>.</exception>
        public void Replace(HydraulicBoundaryLocation[] newHydraulicBoundaryLocations)
        {
            var duneLocationsReader = new DuneLocationsReader();
            newDuneLocations = duneLocationsReader.ReadDuneLocations();

            DuneErosionDataSynchronizationService.SetDuneLocations(failureMechanism,
                                                                   newHydraulicBoundaryLocations,
                                                                   newDuneLocations.ToArray());
        }

        /// <summary>
        /// Performs post-replacement updates.
        /// </summary>
        public void DoPostReplacementUpdates()
        {
            if (!failureMechanism.DuneLocations.Any())
            {
                viewCommands.RemoveAllViewsForItem(failureMechanism.DuneLocations);
            }
        }
    }
}