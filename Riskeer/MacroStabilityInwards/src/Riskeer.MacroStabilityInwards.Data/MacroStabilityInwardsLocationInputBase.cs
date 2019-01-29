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
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// Base class that holds all locations input for either daily or 
    /// extreme conditions for the macro stability inwards calculation.
    /// </summary>
    public abstract class MacroStabilityInwardsLocationInputBase : IMacroStabilityInwardsLocationInput, ICloneable
    {
        private RoundedDouble waterLevelPolder;
        private RoundedDouble phreaticLineOffsetBelowDikeTopAtRiver;
        private RoundedDouble phreaticLineOffsetBelowDikeTopAtPolder;
        private RoundedDouble phreaticLineOffsetBelowShoulderBaseInside;
        private RoundedDouble phreaticLineOffsetBelowDikeToeAtPolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsLocationInputBase"/> class.
        /// </summary>
        protected MacroStabilityInwardsLocationInputBase()
        {
            waterLevelPolder = new RoundedDouble(2, double.NaN);

            UseDefaultOffsets = true;

            phreaticLineOffsetBelowDikeTopAtRiver = new RoundedDouble(2, double.NaN);
            phreaticLineOffsetBelowDikeTopAtPolder = new RoundedDouble(2, double.NaN);
            phreaticLineOffsetBelowShoulderBaseInside = new RoundedDouble(2, double.NaN);
            phreaticLineOffsetBelowDikeToeAtPolder = new RoundedDouble(2, double.NaN);
        }

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

        public bool UseDefaultOffsets { get; set; }

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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}