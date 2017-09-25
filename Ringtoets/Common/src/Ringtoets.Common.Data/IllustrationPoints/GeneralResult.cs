﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils.Extensions;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.IllustrationPoints
{
    /// <summary>
    /// The general illustration point result.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="TopLevelIllustrationPointBase"/>
    /// that the general result holds.</typeparam>
    public class GeneralResult<T> : ICloneable
        where T : TopLevelIllustrationPointBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult{T}"/>
        /// </summary>
        /// <param name="governingWindDirection">The governing wind direction.</param>
        /// <param name="stochasts">The general alpha values.</param>
        /// <param name="topLevelIllustrationPoints">A collection of illustration points
        /// for every combination of wind directions and closing situations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input
        /// parameters is <c>null</c>.</exception>
        public GeneralResult(WindDirection governingWindDirection,
                             IEnumerable<Stochast> stochasts,
                             IEnumerable<T> topLevelIllustrationPoints)
        {
            if (governingWindDirection == null)
            {
                throw new ArgumentNullException(nameof(governingWindDirection));
            }
            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }
            if (topLevelIllustrationPoints == null)
            {
                throw new ArgumentNullException(nameof(topLevelIllustrationPoints));
            }

            ValidateStochasts(stochasts);
            ValidateTopLevelIllustrationPoints(topLevelIllustrationPoints);

            GoverningWindDirection = governingWindDirection;
            Stochasts = stochasts;
            TopLevelIllustrationPoints = topLevelIllustrationPoints;
        }

        /// <summary>
        /// Gets the governing wind direction.
        /// </summary>
        public WindDirection GoverningWindDirection { get; private set; }

        /// <summary>
        /// Gets the general alpha values.
        /// </summary>
        public IEnumerable<Stochast> Stochasts { get; private set; }

        /// <summary>
        /// Gets the collection of illustration points for every combination of a wind direction and a closing situation.
        /// </summary>
        public IEnumerable<T> TopLevelIllustrationPoints { get; private set; }

        public object Clone()
        {
            var clone = (GeneralResult<T>) MemberwiseClone();

            clone.GoverningWindDirection = (WindDirection) GoverningWindDirection.Clone();
            clone.Stochasts = Stochasts.Select(s => (Stochast) s.Clone()).ToArray();
            clone.TopLevelIllustrationPoints = TopLevelIllustrationPoints.Select(s => (T) s.Clone()).ToArray();

            return clone;
        }

        private static void ValidateTopLevelIllustrationPoints(IEnumerable<T> topLevelIllustrationPoints)
        {
            bool hasNonDistinctIllustrationPointsPerWindDirection =
                topLevelIllustrationPoints.AnyNonDistinct(t => $"{t.ClosingSituation} {t.WindDirection.Angle}");
            if (hasNonDistinctIllustrationPointsPerWindDirection)
            {
                throw new ArgumentException(string.Format(Resources.GeneralResult_Imported_non_unique_closing_situations_or_wind_direction));
            }
        }

        private static void ValidateStochasts(IEnumerable<Stochast> stochasts)
        {
            bool hasNonDistinctStochasts = stochasts.AnyNonDistinct(s => s.Name);
            if (hasNonDistinctStochasts)
            {
                throw new ArgumentException(string.Format(Resources.GeneralResult_Imported_non_unique_stochasts));
            }
        }
    }
}