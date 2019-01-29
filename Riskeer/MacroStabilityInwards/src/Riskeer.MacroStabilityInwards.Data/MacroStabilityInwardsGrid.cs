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
using Ringtoets.MacroStabilityInwards.Data.Properties;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// A grid used in the macro stability calculation.
    /// </summary>
    public class MacroStabilityInwardsGrid : ICloneable
    {
        private static readonly Range<int> numberOfPointsValidityRange = new Range<int>(1, 100);

        private RoundedDouble xLeft;
        private RoundedDouble xRight;
        private RoundedDouble zTop;
        private RoundedDouble zBottom;
        private int numberOfHorizontalPoints;
        private int numberOfVerticalPoints;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsGrid"/>.
        /// </summary>
        /// <param name="xLeft">The left boundary of the grid.</param>
        /// <param name="xRight">The right boundary of the grid.</param>
        /// <param name="zTop">The top boundary of the grid.</param>
        /// <param name="zBottom">The bottom boundary of the grid.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="xRight"/> is smaller than <paramref name="xLeft"/>;</item>
        /// <item><paramref name="zBottom"/> is larger than <paramref name="zTop"/>.</item>
        /// </list>
        /// </exception>
        public MacroStabilityInwardsGrid(double xLeft, double xRight, double zTop, double zBottom)
        {
            if (!IsSmallerEqualOrNaN(xLeft, xRight))
            {
                throw new ArgumentException(Resources.MacroStabilityInwardsGrid_XLeft_should_be_smaller_than_or_equal_to_XRight);
            }

            if (!IsSmallerEqualOrNaN(zBottom, zTop))
            {
                throw new ArgumentException(Resources.MacroStabilityInwardsGrid_ZTop_should_be_larger_than_or_equal_to_ZBottom);
            }

            this.xLeft = new RoundedDouble(2, xLeft);
            this.xRight = new RoundedDouble(2, xRight);
            this.zTop = new RoundedDouble(2, zTop);
            this.zBottom = new RoundedDouble(2, zBottom);

            NumberOfHorizontalPoints = 5;
            NumberOfVerticalPoints = 5;
        }

        /// <summary>
        /// Gets or sets the left boundary of the grid.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is larger
        /// than <see cref="XRight"/> and is not <see cref="double.NaN"/>.</exception>
        public RoundedDouble XLeft
        {
            get
            {
                return xLeft;
            }
            set
            {
                if (!IsSmallerEqualOrNaN(value, xRight))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsGrid_XLeft_should_be_smaller_than_or_equal_to_XRight);
                }

                xLeft = value.ToPrecision(xLeft.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the right boundary of the grid.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is smaller 
        /// than <see cref="XLeft"/> and is not <see cref="double.NaN"/>.</exception>
        public RoundedDouble XRight
        {
            get
            {
                return xRight;
            }
            set
            {
                if (!IsSmallerEqualOrNaN(xLeft, value))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsGrid_XRight_should_be_larger_than_or_equal_to_XLeft);
                }

                xRight = value.ToPrecision(xRight.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the top boundary of the grid.
        /// [m+NAP]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is smaller 
        /// than <see cref="ZBottom"/> and is not <see cref="double.NaN"/>.</exception>
        public RoundedDouble ZTop
        {
            get
            {
                return zTop;
            }
            set
            {
                if (!IsSmallerEqualOrNaN(ZBottom, value))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsGrid_ZTop_should_be_larger_than_or_equal_to_ZBottom);
                }

                zTop = value.ToPrecision(zTop.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the bottom boundary of the grid.
        /// [m+NAP]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is larger 
        /// than <see cref="ZTop"/> and is not <see cref="double.NaN"/>.</exception>
        public RoundedDouble ZBottom
        {
            get
            {
                return zBottom;
            }
            set
            {
                if (!IsSmallerEqualOrNaN(value, ZTop))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsGrid_ZBottom_should_be_smaller_than_or_equal_to_ZTop);
                }

                zBottom = value.ToPrecision(zBottom.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or set the number of horizontal points.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not in range [1, 100].</exception>
        public int NumberOfHorizontalPoints
        {
            get
            {
                return numberOfHorizontalPoints;
            }
            set
            {
                if (!numberOfPointsValidityRange.InRange(value))
                {
                    string message = string.Format(Resources.NumberOfHorizontalPoints_must_be_in_Range_0_,
                                                   numberOfPointsValidityRange);

                    throw new ArgumentOutOfRangeException(null, message);
                }

                numberOfHorizontalPoints = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of vertical points.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not in range [1, 100].</exception>
        public int NumberOfVerticalPoints
        {
            get
            {
                return numberOfVerticalPoints;
            }
            set
            {
                if (!numberOfPointsValidityRange.InRange(value))
                {
                    string message = string.Format(Resources.NumberOfVerticalPoints_must_be_in_Range_0_,
                                                   numberOfPointsValidityRange);

                    throw new ArgumentOutOfRangeException(null, message);
                }

                numberOfVerticalPoints = value;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        private static bool IsSmallerEqualOrNaN(double value, double valueToCompareTo)
        {
            return double.IsNaN(value) || double.IsNaN(valueToCompareTo) || value.CompareTo(valueToCompareTo + 1e-3) <= 0;
        }
    }
}