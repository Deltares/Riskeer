// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="HydraulicBoundaryLocation"/> 
    /// with <see cref="HydraulicBoundaryLocation.WaveHeight"/> for a given norm.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveHeightLocationContext : GrassCoverErosionOutwardsHydraulicBoundaryLocationContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveHeightLocationContext"/>.
        /// </summary>
        /// <param name="wrappedList">The <see cref="ObservableList{T}"/> which the <see cref="GrassCoverErosionOutwardsHydraulicBoundaryLocationContext"/> 
        /// belongs to.</param>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> 
        /// which the <see cref="GrassCoverErosionOutwardsWaveHeightLocationContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveHeightLocationContext(ObservableList<HydraulicBoundaryLocation> wrappedList,
                                                                  HydraulicBoundaryLocation hydraulicBoundaryLocation)
            : base(wrappedList, hydraulicBoundaryLocation) {}
    }
}