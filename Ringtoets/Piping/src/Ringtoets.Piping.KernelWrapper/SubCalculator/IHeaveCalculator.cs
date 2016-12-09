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

namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing a heave sub calculation.
    /// </summary>
    public interface IHeaveCalculator
    {
        /// <summary>
        /// Sets the DTotal property to use in the heave calculation.
        /// </summary>
        double DTotal { set; }

        /// <summary>
        /// Sets the HExit property to use in the heave calculation.
        /// </summary>
        double HExit { set; }

        /// <summary>
        /// Sets the Ich property to use in the heave calculation.
        /// </summary>
        double Ich { set; }

        /// <summary>
        /// Sets the PhiExit property to use in the heave calculation.
        /// </summary>
        double PhiExit { set; }

        /// <summary>
        /// Sets the PhiPolder property to use in the heave calculation.
        /// </summary>
        double PhiPolder { set; }

        /// <summary>
        /// Sets the RExit property to use in the heave calculation.
        /// </summary>
        double RExit { set; }

        /// <summary>
        /// Sets the bottom level of the bottom most aquitard that is above the exit point's z-coordinate.
        /// </summary>
        double BottomLevelAquitardAboveExitPointZ { set; }

        /// <summary>
        /// Gets the Gradient property to use in the heave calculation.
        /// </summary>
        double Gradient { get; }

        /// <summary>
        /// Gets the Zh property of the heave calculation.
        /// </summary>
        double Zh { get; }

        /// <summary>
        /// Gets the FoSh property of the heave calculation.
        /// </summary>
        double FoSh { get; }

        /// <summary>
        /// Performs the heave calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Performs the heave validation.
        /// </summary>
        /// <returns>A list of validation strings, or an empty list if there are no validation errors.</returns>
        List<string> Validate();
    }
}