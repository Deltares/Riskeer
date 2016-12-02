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
using Deltares.WTIPiping;

namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface with operations for calculating properties for combinations of piping profiles and
    /// surface line.
    /// </summary>
    public interface IPipingProfilePropertyCalculator
    {
        /// <summary>
        /// Sets the <see cref="PipingProfile"/> to use in the calculation.
        /// </summary>
        PipingProfile SoilProfile { set; }
        
        /// <summary>
        /// Sets the <see cref="PipingSurfaceLine"/> to use in the calculation.
        /// </summary>
        PipingSurfaceLine SurfaceLine { set; }

        /// <summary>
        /// Sets the x-coordinate of the exit point.
        /// </summary>
        double ExitPointX { set; }

        /// <summary>
        /// Gets the BottomAquitardLayerAboveExitPointZ property of the piping profile property calculator.
        /// </summary>
        double BottomAquitardLayerAboveExitPointZ { get; }

        /// <summary>
        /// Performs the piping profile property calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Performs the validation of the input properties.
        /// </summary>
        /// <returns>A list of validation strings.</returns>
        List<string> Validate();
    }
}