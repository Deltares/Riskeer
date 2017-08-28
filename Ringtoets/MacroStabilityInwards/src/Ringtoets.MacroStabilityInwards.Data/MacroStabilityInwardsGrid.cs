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

using Core.Common.Base.Data;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// A grid used in the <see cref="MacroStabilityInwardsInput"/>.
    /// </summary>
    public class MacroStabilityInwardsGrid
    {
        private RoundedDouble xLeft;
        private RoundedDouble xRight;
        private RoundedDouble zTop;
        private RoundedDouble zBottom;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsGrid"/>.
        /// </summary>
        public MacroStabilityInwardsGrid()
        {
            xLeft = new RoundedDouble(2, double.NaN);
            xRight = new RoundedDouble(2, double.NaN);
            zTop = new RoundedDouble(2, double.NaN);
            zBottom = new RoundedDouble(2, double.NaN);
        }

        /// <summary>
        /// Gets or sets the x left of the grid.
        /// [m]
        /// </summary>
        public RoundedDouble XLeft
        {
            get
            {
                return xLeft;
            }
            set
            {
                xLeft = value.ToPrecision(xLeft.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the x right of the grid.
        /// [m]
        /// </summary>
        public RoundedDouble XRight
        {
            get
            {
                return xRight;
            }
            set
            {
                xRight = value.ToPrecision(xRight.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the z top of the grid.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble ZTop
        {
            get
            {
                return zTop;
            }
            set
            {
                zTop = value.ToPrecision(ZTop.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the z bottom of the grid.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble ZBottom
        {
            get
            {
                return zBottom;
            }
            set
            {
                zBottom = value.ToPrecision(zBottom.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or set the number of horizontal points.
        /// </summary>
        public int NumberOfHorizontalPoints { get; set; }

        /// <summary>
        /// Gets or sets the number of vertical points.
        /// </summary>
        public int NumberOfVerticalPoints { get; set; }
    }
}