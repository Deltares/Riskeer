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
    /// Factory responsible for creating the sub calculators required for a piping calculation.
    /// </summary>
    public interface IPipingSubCalculatorFactory
    {
        /// <summary>
        /// Creates the uplift calculator.
        /// </summary>
        /// <returns>A new <see cref="IUpliftCalculator"/>.</returns>
        IUpliftCalculator CreateUpliftCalculator();

        /// <summary>
        /// Creates the heave calculator.
        /// </summary>
        /// <returns>A new <see cref="IHeaveCalculator"/>.</returns>
        IHeaveCalculator CreateHeaveCalculator();

        /// <summary>
        /// Creates the Sellmeijer calculator.
        /// </summary>
        /// <returns>A new <see cref="ISellmeijerCalculator"/>.</returns>
        ISellmeijerCalculator CreateSellmeijerCalculator();

        /// <summary>
        /// Creates the effective thickness calculator.
        /// </summary>
        /// <returns>A new <see cref="IEffectiveThicknessCalculator"/>.</returns>
        IEffectiveThicknessCalculator CreateEffectiveThicknessCalculator();

        /// <summary>
        /// Creates the piezometric head at exit calculator.
        /// </summary>
        /// <returns>A new <see cref="IPiezoHeadCalculator"/>.</returns>
        IPiezoHeadCalculator CreatePiezometricHeadAtExitCalculator();

        /// <summary>
        /// Creates the piping profile property calculator.
        /// </summary>
        /// <returns>A new <see cref="IPipingProfilePropertyCalculator"/>.</returns>
        IPipingProfilePropertyCalculator CreatePipingProfilePropertyCalculator();
    }
}