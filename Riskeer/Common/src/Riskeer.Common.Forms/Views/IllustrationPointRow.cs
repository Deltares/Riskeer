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

using System;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Util;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="Views.IllustrationPointControlItem"/> 
    /// in the <see cref="IllustrationPointsTableControl"/>.
    /// </summary>
    internal class IllustrationPointRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointRow"/>.
        /// </summary>
        /// <param name="illustrationPointControlItem">The illustration point to create the row for.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="illustrationPointControlItem"/> is <c>null</c>.</exception>
        public IllustrationPointRow(IllustrationPointControlItem illustrationPointControlItem)
        {
            if (illustrationPointControlItem == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointControlItem));
            }

            IllustrationPointControlItem = illustrationPointControlItem;
        }

        /// <summary>
        /// Gets the wind direction.
        /// </summary>
        public string WindDirection
        {
            get
            {
                return IllustrationPointControlItem.WindDirectionName;
            }
        }

        /// <summary>
        /// Gets the closing situation.
        /// </summary>
        public string ClosingSituation
        {
            get
            {
                return IllustrationPointControlItem.ClosingSituation;
            }
        }

        /// <summary>
        /// Gets the calculated probability.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double Probability
        {
            get
            {
                return StatisticsConverter.ReliabilityToProbability(IllustrationPointControlItem.Beta);
            }
        }

        /// <summary>
        /// Gets the calculated reliability.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble Reliability
        {
            get
            {
                return IllustrationPointControlItem.Beta;
            }
        }

        /// <summary>
        /// Gets the wrapped illustration point control item.
        /// </summary>
        public IllustrationPointControlItem IllustrationPointControlItem { get; }
    }
}