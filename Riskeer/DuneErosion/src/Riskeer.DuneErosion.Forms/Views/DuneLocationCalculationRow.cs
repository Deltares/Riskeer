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
using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.DuneErosion.Data;
using DuneErosionDataResources = Riskeer.DuneErosion.Data.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="DuneLocationCalculation"/>.
    /// </summary>
    public class DuneLocationCalculationRow : CalculatableRow<DuneLocationCalculation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationRow"/>.
        /// </summary>
        /// <param name="duneLocationCalculation">The <see cref="DuneLocationCalculation"/> to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocationCalculation"/> is <c>null</c>.</exception>
        public DuneLocationCalculationRow(DuneLocationCalculation duneLocationCalculation)
            : base(duneLocationCalculation) {}

        /// <summary>
        /// Gets the <see cref="DuneLocation.Name"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return CalculatableObject.DuneLocation.Name;
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocation.Id"/>.
        /// </summary>
        public long Id
        {
            get
            {
                return CalculatableObject.DuneLocation.Id;
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocation.Location"/>.
        /// </summary>
        public Point2D Location
        {
            get
            {
                return CalculatableObject.DuneLocation.Location;
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocation.CoastalAreaId"/>.
        /// </summary>
        public int CoastalAreaId
        {
            get
            {
                return CalculatableObject.DuneLocation.CoastalAreaId;
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocation.Offset"/>.
        /// </summary>
        public string Offset
        {
            get
            {
                return CalculatableObject.DuneLocation.Offset.ToString(DuneErosionDataResources.DuneLocation_Offset_format, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocationCalculationOutput.WaterLevel"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaterLevel
        {
            get
            {
                return CalculatableObject.Output?.WaterLevel ?? RoundedDouble.NaN;
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocationCalculationOutput.WaveHeight"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaveHeight
        {
            get
            {
                return CalculatableObject.Output?.WaveHeight ?? RoundedDouble.NaN;
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocationCalculationOutput.WavePeriod"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WavePeriod
        {
            get
            {
                return CalculatableObject.Output?.WavePeriod ?? RoundedDouble.NaN;
            }
        }

        /// <summary>
        /// Gets the <see cref="DuneLocation.D50"/>.
        /// </summary>
        public RoundedDouble D50
        {
            get
            {
                return CalculatableObject.DuneLocation.D50;
            }
        }
    }
}