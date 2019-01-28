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

using System.Collections.Generic;
using Deltares.WTIPiping;

namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface for a calculator of properties for a surface line combined with soil profiles.
    /// </summary>
    public interface IPipingProfilePropertyCalculator
    {
        /// <summary>
        /// Sets the soil profile.
        /// </summary>
        PipingProfile SoilProfile { set; }

        /// <summary>
        /// Sets the surface line.
        /// </summary>
        PipingSurfaceLine SurfaceLine { set; }

        /// <summary>
        /// Sets the x-coordinate of the exit point.
        /// </summary>
        double ExitPointX { set; }

        /// <summary>
        /// Gets the bottom level of the bottommost aquitard that is above the exit point's z-coordinate.
        /// </summary>
        double BottomAquitardLayerAboveExitPointZ { get; }

        /// <summary>
        /// Performs the piping profile property calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Performs the validation of the input properties.
        /// </summary>
        /// <returns>A list of validation strings, or an empty list if there are no validation errors.</returns>
        List<string> Validate();
    }
}