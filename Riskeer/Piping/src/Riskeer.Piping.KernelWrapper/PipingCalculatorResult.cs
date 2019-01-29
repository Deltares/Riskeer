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

using System;

namespace Riskeer.Piping.KernelWrapper
{
    /// <summary>
    /// This class contains all the results of a complete piping calculation.
    /// </summary>
    public class PipingCalculatorResult
    {
        /// <summary>
        /// Constructs a new <see cref="PipingCalculatorResult"/>. The result will hold all the values which were given.
        /// </summary>
        /// <param name="constructionProperties">The container of the properties for the
        /// <see cref="PipingCalculatorResult"/></param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="constructionProperties"/>
        /// is <c>null</c>.</exception>
        internal PipingCalculatorResult(ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            UpliftZValue = constructionProperties.UpliftZValue;
            UpliftFactorOfSafety = constructionProperties.UpliftFactorOfSafety;
            HeaveZValue = constructionProperties.HeaveZValue;
            HeaveFactorOfSafety = constructionProperties.HeaveFactorOfSafety;
            SellmeijerZValue = constructionProperties.SellmeijerZValue;
            SellmeijerFactorOfSafety = constructionProperties.SellmeijerFactorOfSafety;
            UpliftEffectiveStress = constructionProperties.UpliftEffectiveStress;
            HeaveGradient = constructionProperties.HeaveGradient;
            SellmeijerCreepCoefficient = constructionProperties.SellmeijerCreepCoefficient;
            SellmeijerCriticalFall = constructionProperties.SellmeijerCriticalFall;
            SellmeijerReducedFall = constructionProperties.SellmeijerReducedFall;
        }

        /// <summary>
        /// Container for properties for constructing a <see cref="PipingCalculatorResult"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                UpliftZValue = double.NaN;
                UpliftFactorOfSafety = double.NaN;
                HeaveZValue = double.NaN;
                HeaveFactorOfSafety = double.NaN;
                SellmeijerZValue = double.NaN;
                SellmeijerFactorOfSafety = double.NaN;
                UpliftEffectiveStress = double.NaN;
                HeaveGradient = double.NaN;
                SellmeijerCreepCoefficient = double.NaN;
                SellmeijerCriticalFall = double.NaN;
                SellmeijerReducedFall = double.NaN;
            }

            /// <summary>
            /// Sets the z-value of the Uplift sub calculation.
            /// </summary>
            public double UpliftZValue { internal get; set; }

            /// <summary>
            /// Sets the factor of safety of the Uplift sub calculation.
            /// </summary>
            public double UpliftFactorOfSafety { internal get; set; }

            /// <summary>
            /// Sets the z-value of the Heave sub calculation.
            /// </summary>
            public double HeaveZValue { internal get; set; }

            /// <summary>
            /// Sets the factor of safety of the Heave sub calculation.
            /// </summary>
            public double HeaveFactorOfSafety { internal get; set; }

            /// <summary>
            /// Sets the z-value of the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerZValue { internal get; set; }

            /// <summary>
            /// Sets the factor of safety of the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerFactorOfSafety { internal get; set; }

            /// <summary>
            /// Sets the effective stress that was calculated for the uplift sub calculation.
            /// </summary>
            public double UpliftEffectiveStress { internal get; set; }

            /// <summary>
            /// Sets the gradient that was calculated for the heave sub calculation.
            /// </summary>
            public double HeaveGradient { internal get; set; }

            /// <summary>
            /// Sets the creep coefficient that was calculated for the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerCreepCoefficient { internal get; set; }

            /// <summary>
            /// Sets the critical fall that was calculated for the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerCriticalFall { internal get; set; }

            /// <summary>
            /// Sets the reduced fall that was calculated for the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerReducedFall { internal get; set; }
        }

        #region Properties

        /// <summary>
        /// Gets the z-value of the Uplift sub calculation.
        /// </summary>
        public double UpliftZValue { get; }

        /// <summary>
        /// Gets the factor of safety of the Uplift sub calculation.
        /// </summary>
        public double UpliftFactorOfSafety { get; }

        /// <summary>
        /// Gets the z-value of the Heave sub calculation.
        /// </summary>
        public double HeaveZValue { get; }

        /// <summary>
        /// Gets the factor of safety of the Heave sub calculation.
        /// </summary>
        public double HeaveFactorOfSafety { get; }

        /// <summary>
        /// Gets the z-value of the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerZValue { get; }

        /// <summary>
        /// Gets the factor of safety of the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerFactorOfSafety { get; }

        /// <summary>
        /// Gets the effective stress that was calculated for the uplift sub calculation.
        /// </summary>
        public double UpliftEffectiveStress { get; }

        /// <summary>
        /// Gets the gradient that was calculated for the heave sub calculation.
        /// </summary>
        public double HeaveGradient { get; }

        /// <summary>
        /// Gets the creep coefficient that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerCreepCoefficient { get; }

        /// <summary>
        /// Gets the critical fall that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerCriticalFall { get; }

        /// <summary>
        /// Gets the reduced fall that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerReducedFall { get; }

        #endregion
    }
}