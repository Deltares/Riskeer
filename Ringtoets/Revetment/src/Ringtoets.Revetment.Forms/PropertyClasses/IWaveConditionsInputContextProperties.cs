// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.Gui.PropertyBag;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;

namespace Ringtoets.Revetment.Forms.PropertyClasses
{
    /// <summary>
    /// Interface for wave conditions input properties classes.
    /// </summary>
    /// <typeparam name="T">The type of the wave conditions input presentation object.</typeparam>
    public interface IWaveConditionsInputContextProperties<out T> : IObjectProperties where T : WaveConditionsInputContext
    {
        /// <summary>
        /// Gets and sets the selected <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets and sets the selected <see cref="ForeshoreProfile"/>.
        /// </summary>
        ForeshoreProfile ForeshoreProfile { get; set; }

        /// <summary>
        /// Gets the available <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of available <see cref="HydraulicBoundaryLocation"/>.</returns>
        IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations();

        /// <summary>
        /// Gets the available <see cref="ForeshoreProfile"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of available <see cref="ForeshoreProfile"/>.</returns>
        IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles();
    }
}