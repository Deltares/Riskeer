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
using System.ComponentModel;
using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;
using DuneErosionDataResources = Ringtoets.DuneErosion.Data.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="DuneLocation"/>.
    /// </summary>
    public class DuneLocationRow : CalculatableRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationRow"/>.
        /// </summary>
        /// <param name="duneLocation">The <see cref="DuneLocation"/> to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocation"/> is <c>null</c>.</exception>
        public DuneLocationRow(DuneLocation duneLocation)
        {
            if (duneLocation == null)
            {
                throw new ArgumentNullException(nameof(duneLocation));
            }

            DuneLocation = duneLocation;
        }

        /// <summary>
        /// Gets the wrapped <see cref="DuneLocation"/>.
        /// </summary>
        public DuneLocation DuneLocation { get; }

        /// <summary>
        /// Gets the <see cref="Data.DuneLocation.Name"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return DuneLocation.Name;
            }
        }

        /// <summary>
        /// Gets the <see cref="Data.DuneLocation.Id"/>.
        /// </summary>
        public long Id
        {
            get
            {
                return DuneLocation.Id;
            }
        }

        /// <summary>
        /// Gets the <see cref="Data.DuneLocation.Location"/>.
        /// </summary>
        public Point2D Location
        {
            get
            {
                return DuneLocation.Location;
            }
        }

        /// <summary>
        /// Gets the <see cref="Data.DuneLocation.CoastalAreaId"/>.
        /// </summary>
        public int CoastalAreaId
        {
            get
            {
                return DuneLocation.CoastalAreaId;
            }
        }

        /// <summary>
        /// Gets the <see cref="Data.DuneLocation.Offset"/>.
        /// </summary>
        public string Offset
        {
            get
            {
                return DuneLocation.Offset.ToString(DuneErosionDataResources.DuneLocation_Offset_format, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocationOutput.WaterLevel"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaterLevel
        {
            get
            {
                return DuneLocation.Output?.WaterLevel ?? RoundedDouble.NaN;
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocationOutput.WaveHeight"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaveHeight
        {
            get
            {
                return DuneLocation.Output?.WaveHeight ?? RoundedDouble.NaN;
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocationOutput.WavePeriod"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WavePeriod
        {
            get
            {
                return DuneLocation.Output?.WavePeriod ?? RoundedDouble.NaN;
            }
        }

        /// <summary>
        /// Gets the <see cref="Data.DuneLocation.D50"/>.
        /// </summary>
        public RoundedDouble D50
        {
            get
            {
                return DuneLocation.D50;
            }
        }
    }
}