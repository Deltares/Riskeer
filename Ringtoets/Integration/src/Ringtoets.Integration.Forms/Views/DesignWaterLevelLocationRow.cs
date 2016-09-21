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

using System;
using System.ComponentModel;
using Core.Common.Base.Data;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="HydraulicBoundaryLocation"/> of which the design water
    /// level is presented.
    /// </summary>
    public class DesignWaterLevelLocationRow : HydraulicBoundaryLocationRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelLocationRow"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="DesignWaterLevelLocationContext"/> for this row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public DesignWaterLevelLocationRow(HydraulicBoundaryLocation hydraulicBoundaryLocation) : base(hydraulicBoundaryLocation) { }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>.
        /// </summary>
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble DesignWaterLevel
        {
            get
            {
                return HydraulicBoundaryLocation.DesignWaterLevel;
            }
        }
    }
}