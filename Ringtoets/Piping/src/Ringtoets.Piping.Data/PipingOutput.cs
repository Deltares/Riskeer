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

using System;
using Core.Common.Base;
using Core.Common.Base.Data;
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
        /// <param name="constructionProperties">The container of the properties for the
        /// <see cref="PipingOutput"/></param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="constructionProperties"/>
        /// is <c>null</c>.</exception>
        public PipingOutput(ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException("constructionProperties");
            }
            HeaveFactorOfSafety = constructionProperties.HeaveFactorOfSafety;
            HeaveZValue = constructionProperties.HeaveZValue;
            UpliftFactorOfSafety = constructionProperties.UpliftFactorOfSafety;
            UpliftZValue = constructionProperties.UpliftZValue;
            SellmeijerFactorOfSafety = constructionProperties.SellmeijerFactorOfSafety;
            SellmeijerZValue = constructionProperties.SellmeijerZValue;
            HeaveGradient = new RoundedDouble(2, constructionProperties.HeaveGradient);
            SellmeijerCreepCoefficient = new RoundedDouble(1, constructionProperties.SellmeijerCreepCoefficient);
            SellmeijerCriticalFall = new RoundedDouble(2, constructionProperties.SellmeijerCriticalFall);
            SellmeijerReducedFall = new RoundedDouble(2, constructionProperties.SellmeijerReducedFall);
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
        public RoundedDouble HeaveGradient { get; private set; }

        /// <summary>
        /// The creep coefficient that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public RoundedDouble SellmeijerCreepCoefficient { get; private set; }

        /// <summary>
        /// The critical fall that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public RoundedDouble SellmeijerCriticalFall { get; private set; }

        /// <summary>
        /// The reduced fall that was calculated for the Sellmeijer sub calculation.
        /// </summary>
        public RoundedDouble SellmeijerReducedFall { get; private set; }

        /// <summary>
        /// Container for properties for constructing a <see cref="PipingOutput"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// The calculated z-value for the uplift sub calculation.
            /// </summary>
            public double UpliftZValue { internal get; set; }

            /// <summary>
            /// The factor of safety for the uplift sub calculation.
            /// </summary>
            public double UpliftFactorOfSafety { internal get; set; }

            /// <summary>
            /// The calculated z-value for the heave sub calculation.
            /// </summary>
            public double HeaveZValue { internal get; set; }

            /// <summary>
            /// The factor of safety for the heave sub calculation.
            /// </summary>
            public double HeaveFactorOfSafety { internal get; set; }

            /// <summary>
            /// The calculated z-value for the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerZValue { internal get; set; }

            /// <summary>
            /// The factor of safety for the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerFactorOfSafety { internal get; set; }

            /// <summary>
            /// The gradient that was calculated for the heave sub calculation.
            /// </summary>
            public double HeaveGradient { internal get; set; }

            /// <summary>
            /// The creep coefficient that was calculated for the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerCreepCoefficient { internal get; set; }

            /// <summary>
            /// The critical fall that was calculated for the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerCriticalFall { internal get; set; }

            /// <summary>
            /// The reduced fall that was calculated for the Sellmeijer sub calculation.
            /// </summary>
            public double SellmeijerReducedFall { internal get; set; }

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
                HeaveGradient = double.NaN;
                SellmeijerCreepCoefficient = double.NaN;
                SellmeijerCriticalFall = double.NaN;
                SellmeijerReducedFall = double.NaN;
            }
        }
    }
}