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
    /// Interface with operations for performing a sellmeijer sub calculation.
    /// </summary>
    public interface ISellmeijerCalculator
    {
        /// <summary>
        /// Gets the creep factor. 
        /// </summary>
        double CreepCoefficient { get; }

        /// <summary>
        /// Gets the critical fall. 
        /// </summary>
        double CriticalFall { get; }

        /// <summary>
        /// Gets the reduced fall. 
        /// </summary>
        double ReducedFall { get; }

        /// <summary>
        /// Gets the z-value.
        /// </summary>
        double Zp { get; }

        /// <summary>
        /// Gets the factor of safety.
        /// </summary>
        double FoSp { get; }

        /// <summary>
        /// Sets the bedding angle.
        /// </summary>
        void SetBeddingAngle(double beddingAngle);

        /// <summary>
        /// Sets the D70.
        /// </summary>
        void SetD70(double d70);

        /// <summary>
        /// Sets the D70 reference value.
        /// </summary>
        void SetD70Mean(double d70Mean);

        /// <summary>
        /// Sets the total thickness of the aquifer.
        /// </summary>
        void SetDAquifer(double dAquifer);

        /// <summary>
        /// Sets the hydraulic conductivity.
        /// </summary>
        void SetDarcyPermeability(double darcyPermeability);

        /// <summary>
        /// Sets the total thickness of the cover layers.
        /// </summary>
        void SetDTotal(double dTotal);

        /// <summary>
        /// Sets the submerged volumetric weight of sand particles.
        /// </summary>
        void SetGammaSubParticles(double gammaSubParticles);

        /// <summary>
        /// Sets the gravitational constant.
        /// </summary>
        void SetGravity(double gravity);

        /// <summary>
        /// Sets the phreatic level at the exit point.
        /// </summary>
        void SetHExit(double hExit);

        /// <summary>
        /// Sets the river water level.
        /// </summary>
        void SetHRiver(double hRiver);

        /// <summary>
        /// Sets the kinematic viscosity of water at 10 degrees Celsius.
        /// </summary>
        void SetKinematicViscosityWater(double kinematicViscosityWater);

        /// <summary>
        /// Sets the model factor.
        /// </summary>
        void SetModelFactorPiping(double modelFactorPiping);

        /// <summary>
        /// Sets the damping factor.
        /// </summary>
        void SetRc(double rc);

        /// <summary>
        /// Sets the horizontal seepage length.
        /// </summary>
        void SetSeepageLength(double seepageLength);

        /// <summary>
        /// Sets the volumetric weight of water.
        /// </summary>
        void SetVolumetricWeightOfWater(double volumetricWeightOfWater);

        /// <summary>
        /// Sets White's drag coefficient.
        /// </summary>
        void SetWhitesDragCoefficient(double whitesDragCoefficient);

        /// <summary>
        /// Sets the bottom level of the bottommost aquitard that is above the exit point's z-coordinate.
        /// </summary>
        void SetBottomLevelAquitardAboveExitPointZ(double bottomLevelAquitardAboveExitPointZ);

        /// <summary>
        /// Performs the Sellmeijer calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Validates the input for the Sellmeijer calculation.
        /// </summary>
        /// <returns>A list of validation strings, or an empty list if there are no validation errors.</returns>
        List<string> Validate();
    }
}