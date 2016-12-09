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
    /// Interface with operations for performing an uplift sub calculation.
    /// </summary>
    public interface IUpliftCalculator
    {
        /// <summary>
        /// Sets the EffectiveStress property to use in the uplift calculation.
        /// </summary>
        double EffectiveStress { set; }

        /// <summary>
        /// Sets the HExit property to use in the uplift calculation.
        /// </summary>
        double HExit { set; }

        /// <summary>
        /// Sets the HRiver property to use in the uplift calculation.
        /// </summary>
        double HRiver { set; }

        /// <summary>
        /// Sets the ModelFactorUplift property to use in the uplift calculation.
        /// </summary>
        double ModelFactorUplift { set; }

        /// <summary>
        /// Sets the PhiExit property to use in the uplift calculation.
        /// </summary>
        double PhiExit { set; }

        /// <summary>
        /// Sets the PhiPolder property to use in the uplift calculation.
        /// </summary>
        double PhiPolder { set; }

        /// <summary>
        /// Sets the RExit property to use in the uplift calculation.
        /// </summary>
        double RExit { set; }

        /// <summary>
        /// Sets the VolumetricWeightOfWater property to use in the uplift calculation.
        /// </summary>
        double VolumetricWeightOfWater { set; }

        /// <summary>
        /// Gets the Zu property of the uplift calculation.
        /// </summary>
        double Zu { get; }

        /// <summary>
        /// Gets the FoSu property of the uplift calculation.
        /// </summary>
        double FoSu { get; }

        /// <summary>
        /// Performs the uplift calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Performs the uplift validation.
        /// </summary>
        /// <returns>A list of validation strings, or an empty list if there are no validation errors.</returns>
        List<string> Validate();
    }
}