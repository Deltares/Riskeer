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

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.Storage;

namespace Ringtoets.HydraRing.Data
{
    /// <summary>
    /// Interface for an object for which a hydraulic boundary location can be stored.
    /// </summary>
    public interface IHydraulicBoundaryLocation : IStorable
    {
        /// <summary>
        /// Gets the database id of <see cref="IHydraulicBoundaryLocation"/>.
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Gets the name of <see cref="IHydraulicBoundaryLocation"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the coordinates of <see cref="IHydraulicBoundaryLocation"/>.
        /// </summary>
        Point2D Location { get; }

        /// <summary>
        /// Gets or sets the design water level of <see cref="IHydraulicBoundaryLocation"/>.
        /// </summary>
        RoundedDouble DesignWaterLevel { get; set; }

        /// <summary>
        /// Gets or sets the wave height of <see cref="IHydraulicBoundaryLocation"/>.
        /// </summary>
        RoundedDouble WaveHeight { get; set; }

        /// <summary>
        /// Gets or sets the convergence status of the design waterlevel calculation.
        /// </summary>
        CalculationConvergence DesignWaterLevelCalculationConvergence { get; set; }

        /// <summary>
        /// Gets or sets the convergence status of the wave height calculation.
        /// </summary>
        CalculationConvergence WaveHeightCalculationConvergence { get; set; }
    }
}