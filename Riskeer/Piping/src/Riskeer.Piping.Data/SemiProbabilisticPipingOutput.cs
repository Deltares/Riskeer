﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using Core.Common.Base;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Class containing the results of a semi-probabilistic piping calculation.
    /// </summary>
    public class SemiProbabilisticPipingOutput : CloneableObservable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="SemiProbabilisticPipingOutput"/>. 
        /// </summary>
        /// <param name="constructionProperties">The container of the properties for the
        /// <see cref="SemiProbabilisticPipingOutput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="constructionProperties"/>
        /// is <c>null</c>.</exception>
        public SemiProbabilisticPipingOutput(ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            HeaveFactorOfSafety = constructionProperties.HeaveFactorOfSafety;
            UpliftFactorOfSafety = constructionProperties.UpliftFactorOfSafety;
            SellmeijerFactorOfSafety = constructionProperties.SellmeijerFactorOfSafety;
            UpliftEffectiveStress = new RoundedDouble(2, constructionProperties.UpliftEffectiveStress);
            HeaveGradient = new RoundedDouble(2, constructionProperties.HeaveGradient);
            SellmeijerCreepCoefficient = new RoundedDouble(1, constructionProperties.SellmeijerCreepCoefficient);
            SellmeijerCriticalFall = new RoundedDouble(2, constructionProperties.SellmeijerCriticalFall);
            SellmeijerReducedFall = new RoundedDouble(2, constructionProperties.SellmeijerReducedFall);
        }

        /// <summary>
        /// Gets the factor of safety for the uplift sub calculation.
        /// </summary>
        public double UpliftFactorOfSafety { get; }

        /// <summary>
        /// Gets the factor of safety for the heave sub calculation.
        /// </summary>
        public double HeaveFactorOfSafety { get; }

        /// <summary>
        /// Gets the factor of safety for the Sellmeijer sub calculation.
        /// </summary>
        public double SellmeijerFactorOfSafety { get; }

        /// <summary>
        /// Gets the effective stress that was calculated for the uplift sub calculation.
        /// </summary>
        public RoundedDouble UpliftEffectiveStress { get; }

        /// <summary>
        /// Gets the gradient that was calculated for the heave sub calculation.
        /// </summary>
        public RoundedDouble HeaveGradient { get; }

        /// <summary>
        /// Gets the creep coefficient that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public RoundedDouble SellmeijerCreepCoefficient { get; }

        /// <summary>
        /// Gets the critical fall that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public RoundedDouble SellmeijerCriticalFall { get; }

        /// <summary>
        /// Gets the reduced fall that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public RoundedDouble SellmeijerReducedFall { get; }

        /// <summary>
        /// Container for properties for constructing a <see cref="SemiProbabilisticPipingOutput"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                UpliftFactorOfSafety = double.NaN;
                HeaveFactorOfSafety = double.NaN;
                SellmeijerFactorOfSafety = double.NaN;
                UpliftEffectiveStress = double.NaN;
                HeaveGradient = double.NaN;
                SellmeijerCreepCoefficient = double.NaN;
                SellmeijerCriticalFall = double.NaN;
                SellmeijerReducedFall = double.NaN;
            }

            /// <summary>
            /// Sets the factor of safety for the uplift sub calculation.
            /// </summary>
            public double UpliftFactorOfSafety { internal get; set; }

            /// <summary>
            /// Sets the factor of safety for the heave sub calculation.
            /// </summary>
            public double HeaveFactorOfSafety { internal get; set; }

            /// <summary>
            /// Sets the factor of safety for the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerFactorOfSafety { internal get; set; }

            /// <summary>
            /// Gets the effective stress that was calculated for the uplift sub calculation.
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
    }
}