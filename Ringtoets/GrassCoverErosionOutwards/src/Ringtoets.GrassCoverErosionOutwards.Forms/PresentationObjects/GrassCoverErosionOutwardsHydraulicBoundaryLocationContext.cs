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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="HydraulicBoundaryLocationCalculation"/>
    /// for <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public abstract class GrassCoverErosionOutwardsHydraulicBoundaryLocationContext
        : ObservableWrappedObjectContextBase<HydraulicBoundaryLocationCalculation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsHydraulicBoundaryLocationContext"/>.
        /// </summary>
        /// <param name="wrappedData">The <see cref="HydraulicBoundaryLocationCalculation"/> which the
        /// <see cref="GrassCoverErosionOutwardsHydraulicBoundaryLocationContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/> is <c>null</c>.</exception>
        protected GrassCoverErosionOutwardsHydraulicBoundaryLocationContext(HydraulicBoundaryLocationCalculation wrappedData)
            : base(wrappedData) {}
    }
}