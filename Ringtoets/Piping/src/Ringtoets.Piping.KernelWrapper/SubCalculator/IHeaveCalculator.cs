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

namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing a heave sub calculation.
    /// </summary>
    public interface IHeaveCalculator
    {
        /// <summary>
        /// Sets the total thickness of the coverage layer.
        /// </summary>
        double DTotal { set; }

        /// <summary>
        /// Sets the phreatic level at the exit point.
        /// </summary>
        double HExit { set; }

        /// <summary>
        /// Sets the critical exit gradient.
        /// </summary>
        double Ich { set; }

        /// <summary>
        /// Sets the piezometric head at the exit point.
        /// </summary>
        double PhiExit { set; }

        /// <summary>
        /// Sets the piezometric head in the hinterland.
        /// </summary>
        double PhiPolder { set; }

        /// <summary>
        /// Sets the damping factor at the exit point.
        /// </summary>
        double RExit { set; }

        /// <summary>
        /// Gets the vertical outflow gradient.
        /// </summary>
        double Gradient { get; }

        /// <summary>
        /// Gets the z-value.
        /// </summary>
        double Zh { get; }

        /// <summary>
        /// Gets the factor of safety.
        /// </summary>
        double FoSh { get; }

        /// <summary>
        /// Performs the heave calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Validates the input for the heave calculation.
        /// </summary>
        /// <returns>A list of validation strings, or an empty list if there are no validation errors.</returns>
        List<string> Validate();
    }
}