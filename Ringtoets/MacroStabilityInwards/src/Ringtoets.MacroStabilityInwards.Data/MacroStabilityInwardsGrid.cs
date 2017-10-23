// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.MacroStabilityInwards.Data
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
        /// <param name="xLeft">The x left of the grid.</param>
        /// <param name="xRight">The x right of the grid.</param>
        /// <param name="zTop">The z top of the grid.</param>
        /// <param name="zBottom">The z bottom of the grid.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="xLeft"/> is not smaller than <paramref name="xRight"/>;</item>
        /// <item><paramref name="zTop"/> is not larger than <paramref name="zBottom"/>.</item>
        /// </list>
        /// </exception>
        public MacroStabilityInwardsGrid(double xLeft, double xRight, double zTop, double zBottom)
        {
            if (!IsSmallerOrNaN(xLeft, xRight))
            {
                throw new ArgumentException(Resources.MacroStabilityInwardsGrid_XLeft_should_be_smaller_than_XRight);
            }
            if (!IsSmallerOrNaN(zBottom, zTop))
            {
                throw new ArgumentException(Resources.MacroStabilityInwardsGrid_ZTop_should_be_larger_than_ZBottom);
            }

            this.xLeft = new RoundedDouble(2, xLeft);
            this.xRight = new RoundedDouble(2, xRight);
            this.zTop = new RoundedDouble(2, zTop);
            this.zBottom = new RoundedDouble(2, zBottom);

            NumberOfHorizontalPoints = 5;
            NumberOfVerticalPoints = 5;
        }

        /// <summary>
        /// Gets or sets the x left of the grid.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not smaller 
        /// than <see cref="XRight"/> or is <see cref="double.NaN"/>.</exception>
        public RoundedDouble XLeft
        {
            get
            {
                return xLeft;
            }
            set
            {
                if (!IsSmallerOrNaN(value, xRight))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsGrid_XLeft_should_be_smaller_than_XRight);
                }

                xLeft = value.ToPrecision(xLeft.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the x right of the grid.
        /// [m]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not larger 
        /// than <see cref="XLeft"/> or is <see cref="double.NaN"/>.</exception>
        public RoundedDouble XRight
        {
            get
            {
                return xRight;
            }
            set
            {
                if (!IsSmallerOrNaN(xLeft, value))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsGrid_XRight_should_be_larger_than_XLeft);
                }

                xRight = value.ToPrecision(xRight.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the z top of the grid.
        /// [m+NAP]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not larger 
        /// than <see cref="ZBottom"/> or is <see cref="double.NaN"/>.</exception>
        public RoundedDouble ZTop
        {
            get
            {
                return zTop;
            }
            set
            {
                if (!IsSmallerOrNaN(ZBottom, value))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsGrid_ZTop_should_be_larger_than_ZBottom);
                }

                zTop = value.ToPrecision(zTop.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the z bottom of the grid.
        /// [m+NAP]
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not smaller 
        /// than <see cref="ZTop"/> or is <see cref="double.NaN"/>.</exception>
        public RoundedDouble ZBottom
        {
            get
            {
                return zBottom;
            }
            set
            {
                if (!IsSmallerOrNaN(value, ZTop))
                {
                    throw new ArgumentException(Resources.MacroStabilityInwardsGrid_ZBottom_should_be_smaller_than_ZTop);
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

                    throw new ArgumentOutOfRangeException(nameof(value), message);
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

                    throw new ArgumentOutOfRangeException(nameof(value), message);
                }

                numberOfVerticalPoints = value;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        private static bool IsSmallerOrNaN(double value, double valueToCompareTo)
        {
            return double.IsNaN(value) || double.IsNaN(valueToCompareTo) || value.CompareTo(valueToCompareTo) < 0;
        }
    }
}