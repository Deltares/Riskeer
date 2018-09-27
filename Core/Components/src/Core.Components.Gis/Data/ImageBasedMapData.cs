// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Data;
using Core.Components.Gis.Properties;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// Abstract class for <see cref="MapData"/> that depend on image data.
    /// </summary>
    public abstract class ImageBasedMapData : MapData
    {
        private const int transparencyNumberOfDecimals = 2;

        private static readonly Range<RoundedDouble> transparencyValidityRange = new Range<RoundedDouble>(new RoundedDouble(transparencyNumberOfDecimals),
                                                                                                          new RoundedDouble(transparencyNumberOfDecimals, 1));

        private RoundedDouble transparency;

        /// <summary>
        /// Initializes a new instance of <see cref="ImageBasedMapData"/> with a given name.
        /// </summary>
        /// <param name="name">The name of the map data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is <c>null</c> or only whitespace.</exception>
        protected ImageBasedMapData(string name) : base(name)
        {
            transparency = new RoundedDouble(transparencyNumberOfDecimals);
        }

        /// <summary>
        /// Gets or sets the transparency of the map data.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a new value
        /// that is not in the range [0.00, 1.00].</exception>
        public RoundedDouble Transparency
        {
            get
            {
                return transparency;
            }
            set
            {
                RoundedDouble newValue = value.ToPrecision(transparency.NumberOfDecimalPlaces);
                if (!transparencyValidityRange.InRange(newValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          string.Format(Resources.ImageBasedMapData_Transparency_Value_must_be_in_Range_0_,
                                                                        transparencyValidityRange));
                }

                transparency = newValue;
            }
        }

        /// <summary>
        /// Gets a value indicating if the map data is configured to use a tile source.
        /// </summary>
        public bool IsConfigured { get; protected set; }
    }
}