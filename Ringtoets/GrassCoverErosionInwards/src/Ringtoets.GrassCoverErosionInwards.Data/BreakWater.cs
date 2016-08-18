﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Container for break water related data.
    /// </summary>
    public class BreakWater
    {
        private RoundedDouble height;

        /// <summary>
        /// Creates a new instance of <see cref="BreakWater"/>.
        /// </summary>
        /// <param name="type">The break water type.</param>
        /// <param name="height">The break water height.</param>
        public BreakWater(BreakWaterType type, double height)
        {
            Type = type;
            this.height = new RoundedDouble(2, height);
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public BreakWaterType Type { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public RoundedDouble Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value.ToPrecision(height.NumberOfDecimalPlaces);
            }
        }
    }
}