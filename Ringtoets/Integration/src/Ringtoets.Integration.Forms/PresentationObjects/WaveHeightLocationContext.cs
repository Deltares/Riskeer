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

using System;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="HydraulicBoundaryLocation"/> 
    /// with <see cref="HydraulicBoundaryLocation.WaveHeight"/>.
    /// </summary>
    public class WaveHeightLocationContext : HydraulicBoundaryLocationContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightLocationContext"/>.
        /// </summary>
        /// <param name="wrappedData">The <see cref="HydraulicBoundaryDatabase"/> which the <see cref="WaveHeightLocationContext"/> belongs to.</param>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> which the <see cref="WaveHeightLocationContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public WaveHeightLocationContext(HydraulicBoundaryDatabase wrappedData, HydraulicBoundaryLocation hydraulicBoundaryLocation)
            : base(wrappedData, hydraulicBoundaryLocation) {}
    }
}