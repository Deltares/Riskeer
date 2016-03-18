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

namespace Ringtoets.Piping.KernelWrapper
{
    /// <summary>
    /// This class contains all the results of a complete piping calculation.
    /// </summary>
    public class PipingCalculatorResult
    {
        private readonly double upliftZValue;
        private readonly double upliftFactorOfSafety;
        private readonly double heaveZValue;
        private readonly double heaveFactorOfSafety;
        private readonly double sellmeijerZValue;
        private readonly double sellmeijerFactorOfSafety;

        /// <summary>
        /// Constructs a new <see cref="PipingCalculatorResult"/>. The result will hold all the values which were given.
        /// </summary>
        /// <param name="upliftZValue">The z-value of the Uplift sub calculation.</param>
        /// <param name="upliftFactorOfSafety">The factory of safety of the Uplift sub calculation.</param>
        /// <param name="heaveZValue">The z-value of the Heave sub calculation.</param>
        /// <param name="heaveFactorOfSafety">The factory of safety of the Heave sub calculation.</param>
        /// <param name="sellmeijerZValue">The z-value of the Sellmeijer sub calculation.</param>
        /// <param name="sellmeijerFactorOfSafety">The factory of safety of the Sellmeijer sub calculation.</param>
        public PipingCalculatorResult(double upliftZValue, double upliftFactorOfSafety, double heaveZValue, double heaveFactorOfSafety, double sellmeijerZValue, double sellmeijerFactorOfSafety)
        {
            this.upliftZValue = upliftZValue;
            this.upliftFactorOfSafety = upliftFactorOfSafety;
            this.heaveZValue = heaveZValue;
            this.heaveFactorOfSafety = heaveFactorOfSafety;
            this.sellmeijerZValue = sellmeijerZValue;
            this.sellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
        }

        #region properties

        /// <summary>
        /// Gets the z-value of the Uplift sub calculation.
        /// </summary>
        public double UpliftZValue
        {
            get
            {
                return upliftZValue;
            }
        }

        /// <summary>
        /// Gets the factory of safety of the Uplift sub calculation.
        /// </summary>
        public double UpliftFactorOfSafety
        {
            get
            {
                return upliftFactorOfSafety;
            }
        }

        /// <summary>
        /// Gets the z-value of the Heave sub calculation.
        /// </summary>
        public double HeaveZValue
        {
            get
            {
                return heaveZValue;
            }
        }

        /// <summary>
        /// Gets the factory of safety of the Heave sub calculation.
        /// </summary>
        public double HeaveFactorOfSafety
        {
            get
            {
                return heaveFactorOfSafety;
            }
        }

        /// <summary>
        /// Gets the z-value of the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerZValue
        {
            get
            {
                return sellmeijerZValue;
            }
        }

        /// <summary>
        /// Gets the factory of safety of the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerFactorOfSafety
        {
            get
            {
                return sellmeijerFactorOfSafety;
            }
        }

        #endregion
    }
}