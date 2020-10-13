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

namespace Riskeer.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing an uplift sub calculation.
    /// </summary>
    public interface IUpliftCalculator
    {
        /// <summary>
        /// Gets or sets the effective stress.
        /// </summary>
        double EffectiveStress { get; set; }
        
        /// <summary>
        /// Gets the factor of safety.
        /// </summary>
        double FoSu { get; }

        /// <summary>
        /// Sets the phreatic level at the exit point.
        /// </summary>
        void SetHExit(double hExit);

        /// <summary>
        /// Sets the river water level.
        /// </summary>
        void SetHRiver(double hRiver);

        /// <summary>
        /// Sets the model factor.
        /// </summary>
        void SetModelFactorUplift(double modelFactorUplift);

        /// <summary>
        /// Sets the piezometric head at the exit point.
        /// </summary>
        void SetPhiExit(double phiExit);

        /// <summary>
        /// Sets the piezometric head in the hinterland.
        /// </summary>
        void SetPhiPolder(double phiPolder);

        /// <summary>
        /// Sets the damping factor at the exit point.
        /// </summary>
        void SetRExit(double rExit);

        /// <summary>
        /// Sets the volumetric weight of water.
        /// </summary>
        void SetVolumetricWeightOfWater(double volumetricWeightOfWater);

        /// <summary>
        /// Performs the uplift calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Validates the input for the uplift calculation.
        /// </summary>
        /// <returns>A list of validation strings, or an empty list if there are no validation errors.</returns>
        List<string> Validate();
    }
}