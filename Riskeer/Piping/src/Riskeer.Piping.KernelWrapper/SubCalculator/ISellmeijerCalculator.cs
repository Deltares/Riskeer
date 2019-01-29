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

namespace Riskeer.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing a sellmeijer sub calculation.
    /// </summary>
    public interface ISellmeijerCalculator
    {
        /// <summary>
        /// Sets the bedding angle.
        /// </summary>
        double BeddingAngle { set; }

        /// <summary>
        /// Sets the D70.
        /// </summary>
        double D70 { set; }

        /// <summary>
        /// Sets the D70 reference value.
        /// </summary>
        double D70Mean { set; }

        /// <summary>
        /// Sets the total thickness of the aquifer.
        /// </summary>
        double DAquifer { set; }

        /// <summary>
        /// Sets the hydraulic conductivity.
        /// </summary>
        double DarcyPermeability { set; }

        /// <summary>
        /// Sets the total thickness of the cover layers.
        /// </summary>
        double DTotal { set; }

        /// <summary>
        /// Sets the submerged volumetric weight of sand particles.
        /// </summary>
        double GammaSubParticles { set; }

        /// <summary>
        /// Sets the gravitational constant.
        /// </summary>
        double Gravity { set; }

        /// <summary>
        /// Sets the phreatic level at the exit point.
        /// </summary>
        double HExit { set; }

        /// <summary>
        /// Sets the river water level.
        /// </summary>
        double HRiver { set; }

        /// <summary>
        /// Sets the kinematic viscosity of water at 10 degrees Celsius.
        /// </summary>
        double KinematicViscosityWater { set; }

        /// <summary>
        /// Sets the model factor.
        /// </summary>
        double ModelFactorPiping { set; }

        /// <summary>
        /// Sets the damping factor.
        /// </summary>
        double Rc { set; }

        /// <summary>
        /// Sets the horizontal seepage length.
        /// </summary>
        double SeepageLength { set; }

        /// <summary>
        /// Sets the volumetric weight of water.
        /// </summary>
        double VolumetricWeightOfWater { set; }

        /// <summary>
        /// Sets White's drag coefficient.
        /// </summary>
        double WhitesDragCoefficient { set; }

        /// <summary>
        /// Sets the bottom level of the bottommost aquitard that is above the exit point's z-coordinate.
        /// </summary>
        double BottomLevelAquitardAboveExitPointZ { set; }

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