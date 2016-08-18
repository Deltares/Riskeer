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

using Deltares.WTIPiping;

namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Interface with operations for performing an effective thickness sub calculation.
    /// </summary>
    public interface IEffectiveThicknessCalculator
    {
        /// <summary>
        /// Sets the ExitPointXCoordinate property of the effective thickness calculation.
        /// </summary>
        double ExitPointXCoordinate { set; }

        /// <summary>
        /// Sets the PhreaticLevel property of the effective thickness calculation.
        /// </summary>
        double PhreaticLevel { set; }

        /// <summary>
        /// Sets the VolumicWeightOfWater property of the effective thickness calculation.
        /// </summary>
        double VolumicWeightOfWater { set; }

        /// <summary>
        /// Sets the SoilProfile property of the effective thickness calculation.
        /// </summary>
        PipingProfile SoilProfile { set; }

        /// <summary>
        /// Sets the SurfaceLine property of the effective thickness calculation.
        /// </summary>
        PipingSurfaceLine SurfaceLine { set; }

        /// <summary>
        /// Gets the EffectiveHeight property of the effective thickness calculation.
        /// </summary>
        double EffectiveHeight { get; }

        /// <summary>
        /// Performs the effective thickness calculation.
        /// </summary>
        void Calculate();
    }
}