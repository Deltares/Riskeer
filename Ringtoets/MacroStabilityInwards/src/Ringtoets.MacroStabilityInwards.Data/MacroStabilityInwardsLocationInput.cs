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

using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Base class that holds all locations input for either daily or 
    /// extreme conditions for the macro stability inwards calculation.
    /// </summary>
    public abstract class MacroStabilityInwardsLocationInput : Observable, ICalculationInput
    {
        protected RoundedDouble waterLevelPolder;
        protected RoundedDouble phreaticLineOffsetBelowDikeTopAtRiver;
        protected RoundedDouble phreaticLineOffsetBelowDikeTopAtPolder;
        protected RoundedDouble phreaticLineOffsetBelowShoulderBaseInside;
        protected RoundedDouble phreaticLineOffsetBelowDikeToeAtPolder;

        protected MacroStabilityInwardsLocationInput()
        {
            waterLevelPolder = new RoundedDouble(2, double.NaN);

            UseDefaultOffsets = true;

            phreaticLineOffsetBelowDikeTopAtRiver = new RoundedDouble(2, double.NaN);
            phreaticLineOffsetBelowDikeTopAtPolder = new RoundedDouble(2, double.NaN);
            phreaticLineOffsetBelowShoulderBaseInside = new RoundedDouble(2, double.NaN);
            phreaticLineOffsetBelowDikeToeAtPolder = new RoundedDouble(2, double.NaN);
        }

        /// <summary>
        /// Gets or sets the polder water level.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble WaterLevelPolder
        {
            get
            {
                return waterLevelPolder;
            }
            set
            {
                waterLevelPolder = value.ToPrecision(waterLevelPolder.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets whether the default offsets should be used.
        /// </summary>
        public bool UseDefaultOffsets { get; set; }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below dike top at river.
        /// [m]
        /// </summary>
        public RoundedDouble PhreaticLineOffsetBelowDikeTopAtRiver
        {
            get
            {
                return phreaticLineOffsetBelowDikeTopAtRiver;
            }
            set
            {
                phreaticLineOffsetBelowDikeTopAtRiver = value.ToPrecision(phreaticLineOffsetBelowDikeTopAtRiver.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below dike top at polder.
        /// [m]
        /// </summary>
        public RoundedDouble PhreaticLineOffsetBelowDikeTopAtPolder
        {
            get
            {
                return phreaticLineOffsetBelowDikeTopAtPolder;
            }
            set
            {
                phreaticLineOffsetBelowDikeTopAtPolder = value.ToPrecision(phreaticLineOffsetBelowDikeTopAtPolder.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below shoulder base inside.
        /// [m]
        /// </summary>
        public RoundedDouble PhreaticLineOffsetBelowShoulderBaseInside
        {
            get
            {
                return phreaticLineOffsetBelowShoulderBaseInside;
            }
            set
            {
                phreaticLineOffsetBelowShoulderBaseInside = value.ToPrecision(phreaticLineOffsetBelowShoulderBaseInside.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below dike toe at polder.
        /// [m]
        /// </summary>
        public RoundedDouble PhreaticLineOffsetBelowDikeToeAtPolder
        {
            get
            {
                return phreaticLineOffsetBelowDikeToeAtPolder;
            }
            set
            {
                phreaticLineOffsetBelowDikeToeAtPolder = value.ToPrecision(phreaticLineOffsetBelowDikeToeAtPolder.NumberOfDecimalPlaces);
            }
        }
    }
}