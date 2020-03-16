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

namespace Riskeer.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing a piezometric head at exit sub calculation.
    /// </summary>
    public interface IPiezoHeadCalculator
    {
        /// <summary>
        /// Sets the piezometric head in the hinterland.
        /// </summary>
        void SetPhiPolder(double phiPolder);

        /// <summary>
        /// Sets the damping factor at the exit point.
        /// </summary>
        void SetRExit(double rExit);

        /// <summary>
        /// Sets the river water level.
        /// </summary>
        void SetHRiver(double hRiver);

        /// <summary>
        /// Gets the piezometric head exit result.
        /// </summary>
        double PhiExit { get; }

        /// <summary>
        /// Performs the piezometric head at exit calculation.
        /// </summary>
        void Calculate();
    }
}