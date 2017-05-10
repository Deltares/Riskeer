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

using System;
using System.Collections.Generic;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for the <see cref="WaveConditionsInput"/>.
    /// </summary>
    public abstract class WaveConditionsInputContext : ObservableWrappedObjectContextBase<WaveConditionsInput>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaveConditionsInputContext"/> class.
        /// </summary>
        /// <param name="wrappedData">The wrapped <see cref="WaveConditionsInput"/>.</param>
        /// <param name="calculation">The calculation having <paramref name="wrappedData"/> as input.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        protected WaveConditionsInputContext(WaveConditionsInput wrappedData, ICalculation<WaveConditionsInput> calculation)
            : base(wrappedData)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            Calculation = calculation;
        }

        /// <summary>
        /// Gets the calculation containing the wrapped <see cref="WaveConditionsInput"/>.
        /// </summary>
        public ICalculation<WaveConditionsInput> Calculation { get; private set; }

        /// <summary>
        /// Gets the hydraulic boundary locations.
        /// </summary>
        public abstract IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations { get; }

        /// <summary>
        /// Gets the foreshore profiles.
        /// </summary>
        public abstract IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
    }
}