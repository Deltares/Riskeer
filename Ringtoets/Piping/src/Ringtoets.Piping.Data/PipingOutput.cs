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

using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Simple class containing the results of a Piping calculation.
    /// </summary>
    public class PipingOutput : Observable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingOutput"/>. 
        /// </summary>
        /// <param name="upliftZValue">The calculated z-value for the uplift sub calculation.</param>
        /// <param name="upliftFactorOfSafety">The factor of safety for the uplift sub calculation.</param>
        /// <param name="heaveZValue">The calculated z-value for the heave sub calculation.</param>
        /// <param name="heaveFactorOfSafety">The factor of safety for the heave sub calculation.</param>
        /// <param name="sellmeijerZValue">The calculated z-value for the Sellmeijer sub calculation.</param>
        /// <param name="sellmeijerFactorOfSafety">The factor of safety for the Sellmeijer sub calculation.</param>
        /// <param name="heaveGradient">The gradient calculated for the heave sub calculation.</param>
        /// <param name="sellmeijerCreepCoefficient">The creep coefficient calculated for the Sellmeijer sub calculation.</param>
        /// <param name="sellmeijerCriticalFall">The critical fall calculated for the Sellmeijer sub calculation.</param>
        /// <param name="sellmeijerReducedFall">The reduced fall calculated for the Sellmeijer sub calculation.</param>
        public PipingOutput(double upliftZValue,
            double upliftFactorOfSafety, 
            double heaveZValue, 
            double heaveFactorOfSafety, 
            double sellmeijerZValue, 
            double sellmeijerFactorOfSafety,
            double heaveGradient,
            double sellmeijerCreepCoefficient,
            double sellmeijerCriticalFall,
            double sellmeijerReducedFall)
        {
            HeaveFactorOfSafety = heaveFactorOfSafety;
            HeaveZValue = heaveZValue;
            UpliftFactorOfSafety = upliftFactorOfSafety;
            UpliftZValue = upliftZValue;
            SellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
            SellmeijerZValue = sellmeijerZValue;
            HeaveGradient = heaveGradient;
            SellmeijerCreepCoefficient = sellmeijerCreepCoefficient;
            SellmeijerCriticalFall = sellmeijerCriticalFall;
            SellmeijerReducedFall = sellmeijerReducedFall;
        }

        /// <summary>
        /// The calculated z-value for the uplift sub calculation.
        /// </summary>
        public double UpliftZValue { get; private set; }

        /// <summary>
        /// The factor of safety for the uplift sub calculation.
        /// </summary>
        public double UpliftFactorOfSafety { get; private set; }

        /// <summary>
        /// The calculated z-value for the heave sub calculation.
        /// </summary>
        public double HeaveZValue { get; private set; }

        /// <summary>
        /// The factor of safety for the heave sub calculation.
        /// </summary>
        public double HeaveFactorOfSafety { get; private set; }

        /// <summary>
        /// The calculated z-value for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerZValue { get; private set; }

        /// <summary>
        /// The factor of safety for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerFactorOfSafety { get; private set; }

        /// <summary>
        /// The gradient that was calculated for the heave sub calculation.
        /// </summary>
        public double HeaveGradient { get; private set; }

        /// <summary>
        /// The creep coefficient that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerCreepCoefficient { get; private set; }

        /// <summary>
        /// The critical fall that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerCriticalFall { get; private set; }

        /// <summary>
        /// The reduced fall that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerReducedFall { get; private set; }
    }
}