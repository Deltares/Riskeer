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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.AssessmentSection
{
    /// <summary>
    /// Class that holds information about configured background data.
    /// </summary>
    public class BackgroundData : Observable
    {
        private const int transparencyNumberOfDecimals = 2;

        private static readonly Range<RoundedDouble> transparencyValidityRange = new Range<RoundedDouble>(new RoundedDouble(transparencyNumberOfDecimals),
                                                                                                          new RoundedDouble(transparencyNumberOfDecimals, 1));

        private RoundedDouble transparency;

        /// <summary>
        /// Creates a new <see cref="BackgroundData"/>.
        /// </summary>
        public BackgroundData()
        {
            IsVisible = true;
            transparency = new RoundedDouble(transparencyNumberOfDecimals);
            Parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the name of the background data.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the background is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the transparency of the background.
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
                var newValue = value.ToPrecision(transparency.NumberOfDecimalPlaces);
                if (!transparencyValidityRange.InRange(newValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          string.Format(Resources.BackgroundData_Transparency_Value_must_be_in_Range_0_,
                                                                        transparencyValidityRange));
                }

                transparency = newValue;
            }
        }

        /// <summary>
        /// Gets or sets the type of the background map data.
        /// </summary>
        public BackgroundMapDataType BackgroundMapDataType { get; set; }

        /// <summary>
        /// Gets or sets whether a background map data is configured.
        /// </summary>
        public bool IsConfigured { get; set; }

        /// <summary>
        /// Gets the parameters that are configured for a background map data.
        /// </summary>
        public IDictionary<string, string> Parameters { get; }
    }
}