﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Ringtoets.HydraRing.Data
{
    /// <summary>
    /// Location of a hydraulic boundary.
    /// </summary>
    public class HydraulicBoundaryLocation
    {
        private RoundedDouble designWaterLevel;
        private RoundedDouble waveHeight;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="id">Id of the <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <param name="name">Name of the <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <param name="coordinateX">X-coordinate of the <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <param name="coordinateY">Y-coordinate of the <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public HydraulicBoundaryLocation(long id, string name, double coordinateX, double coordinateY)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            Id = id;
            Name = name;
            Location = new Point2D(coordinateX, coordinateY);
            designWaterLevel = new RoundedDouble(2, double.NaN);
            waveHeight = new RoundedDouble(2, double.NaN);
        }

        /// <summary>
        /// Gets or sets the design water level of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public RoundedDouble DesignWaterLevel
        {
            get
            {
                return designWaterLevel;
            }
            set
            {
                designWaterLevel = value.ToPrecision(designWaterLevel.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the wave height of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public RoundedDouble WaveHeight
        {
            get
            {
                return waveHeight;
            }
            set
            {
                waveHeight = value.ToPrecision(waveHeight.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the convergence status of the design waterlevel calculation.
        /// </summary>
        public CalculationConvergence DesignWaterLevelCalculationConvergence { get; set; }

        /// <summary>
        /// Gets or sets the convergence status of the waveheight calculation.
        /// </summary>
        public CalculationConvergence WaveHeightCalculationConvergence { get; set; }

        public long Id { get; private set; }

        public string Name { get; private set; }

        public Point2D Location { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}