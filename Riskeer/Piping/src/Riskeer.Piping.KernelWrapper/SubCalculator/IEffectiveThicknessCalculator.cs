// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections.Generic;
using Deltares.WTIPiping;

namespace Riskeer.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing an effective thickness sub calculation.
    /// </summary>
    public interface IEffectiveThicknessCalculator
    {
        /// <summary>
        /// Sets the exit point's x-coordinate.
        /// </summary>
        double ExitPointXCoordinate { set; }

        /// <summary>
        /// Sets the phreatic level at the exit point.
        /// </summary>
        double PhreaticLevel { set; }

        /// <summary>
        /// Sets the volumic weight of water.
        /// </summary>
        double VolumicWeightOfWater { set; }

        /// <summary>
        /// Sets the soil profile.
        /// </summary>
        PipingProfile SoilProfile { set; }

        /// <summary>
        /// Sets the surface line.
        /// </summary>
        PipingSurfaceLine SurfaceLine { set; }

        /// <summary>
        /// Gets the effective thickness of the cover layer.
        /// </summary>
        double EffectiveHeight { get; }

        /// <summary>
        /// Validates the input for the effective thickness calculation.
        /// </summary>
        /// <returns>A list of validation strings, or an empty list if there are no validation errors.</returns>
        List<string> Validate();

        /// <summary>
        /// Performs the effective thickness calculation.
        /// </summary>
        void Calculate();
    }
}