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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Helper class for dealing with classes that have inherently limited precision.
    /// </summary>
    public static class LimitedPrecisionHelper
    {
        /// <summary>
        /// Gets the accuracy for a <see cref="RoundedDouble"/>.
        /// </summary>
        public static double GetAccuracy(this RoundedDouble roundedDouble)
        {
            return 0.5 * Math.Pow(10.0, -roundedDouble.NumberOfDecimalPlaces);
        }

        /// <summary>
        /// Gets the accuracy for a <see cref="IDistribution"/>.
        /// </summary>
        /// <remarks>Assumes that all the parameters of the distribution share the same accuracy.</remarks>
        public static double GetAccuracy(this IDistribution distribution)
        {
            return distribution.Mean.GetAccuracy();
        }

        /// <summary>
        /// Gets the accuracy for a <see cref="IVariationCoefficientDistribution"/>.
        /// </summary>
        /// <remarks>Assumes that all the parameters of the distribution share the same accuracy.</remarks>
        public static double GetAccuracy(this IVariationCoefficientDistribution distribution)
        {
            return distribution.Mean.GetAccuracy();
        }

        /// <summary>
        /// Gets the accuracy for a <see cref="RoundedPoint2DCollection"/>.
        /// </summary>
        public static double GetAccuracy(this RoundedPoint2DCollection collection)
        {
            return 0.5 * Math.Pow(10.0, -collection.NumberOfDecimalPlaces);
        }
    }
}