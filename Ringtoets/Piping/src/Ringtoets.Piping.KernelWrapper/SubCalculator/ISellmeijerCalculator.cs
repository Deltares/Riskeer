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
    /// Interface with operations for performing a sellmeijer sub calculation.
    /// </summary>
    public interface ISellmeijerCalculator
    {
        /// <summary>
        /// Sets the BeddingAngle property to use in the Sellmeijer calculation.
        /// </summary>
        double BeddingAngle { set; }

        /// <summary>
        /// Sets the D70 property to use in the Sellmeijer calculation.
        /// </summary>
        double D70 { set; }

        /// <summary>
        /// Sets the D70Mean property to use in the Sellmeijer calculation.
        /// </summary>
        double D70Mean { set; }

        /// <summary>
        /// Sets the DAquifer property to use in the Sellmeijer calculation.
        /// </summary>
        double DAquifer { set; }

        /// <summary>
        /// Sets the DarcyPermeability property to use in the Sellmeijer calculation.
        /// </summary>
        double DarcyPermeability { set; }

        /// <summary>
        /// Sets the DTotal property to use in the Sellmeijer calculation.
        /// </summary>
        double DTotal { set; }

        /// <summary>
        /// Sets the GammaSubParticles property to use in the Sellmeijer calculation.
        /// </summary>
        double GammaSubParticles { set; }

        /// <summary>
        /// Sets the Gravity property to use in the Sellmeijer calculation.
        /// </summary>
        double Gravity { set; }

        /// <summary>
        /// Sets the HExit property to use in the Sellmeijer calculation.
        /// </summary>
        double HExit { set; }

        /// <summary>
        /// Sets the HRiver property to use in the Sellmeijer calculation.
        /// </summary>
        double HRiver { set; }

        /// <summary>
        /// Sets the KinematicViscosityWater property to use in the Sellmeijer calculation.
        /// </summary>
        double KinematicViscosityWater { set; }

        /// <summary>
        /// Sets the ModelFactorPiping property to use in the Sellmeijer calculation.
        /// </summary>
        double ModelFactorPiping { set; }

        /// <summary>
        /// Sets the Rc property to use in the Sellmeijer calculation.
        /// </summary>
        double Rc { set; }

        /// <summary>
        /// Sets the SeepageLength property to use in the Sellmeijer calculation.
        /// </summary>
        double SeepageLength { set; }

        /// <summary>
        /// Sets the VolumetricWeightOfWater property to use in the Sellmeijer calculation.
        /// </summary>
        double VolumetricWeightOfWater { set; }

        /// <summary>
        /// Sets the WhitesDragCoefficient property to use in the Sellmeijer calculation.
        /// </summary>
        double WhitesDragCoefficient { set; }

        /// <summary>
        /// Sets the BottomLevelAquitardAboveExitPointZ property to use in the Sellmeijer calculation.
        /// </summary>
        double BottomLevelAquitardAboveExitPointZ { set; }

        /// <summary>
        /// Gets the CreepCoefficient property of the Sellmeijer calculation. 
        /// </summary>
        double CreepCoefficient { get; }

        /// <summary>
        /// Gets the CriticalFall property of the Sellmeijer calculation. 
        /// </summary>
        double CriticalFall { get; }

        /// <summary>
        /// Gets the ReducedFall property of the Sellmeijer calculation. 
        /// </summary>
        double ReducedFall { get; }

        /// <summary>
        /// Gets the Zp property of the Sellmeijer calculation.
        /// </summary>
        double Zp { get; }

        /// <summary>
        /// Gets the FoSp property of the Sellmeijer calculation.
        /// </summary>
        double FoSp { get; }

        /// <summary>
        /// Performs the Sellmeijer calculation.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Performs the Sellmeijer validation.
        /// </summary>
        /// <returns>A list of validation strings.</returns>
        List<string> Validate();
    }
}