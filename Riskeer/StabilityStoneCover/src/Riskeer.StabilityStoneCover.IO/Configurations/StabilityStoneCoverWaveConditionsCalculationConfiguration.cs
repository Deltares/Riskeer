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
using Riskeer.Revetment.IO.Configurations;

namespace Riskeer.StabilityStoneCover.IO.Configurations
{
    /// <summary>
    /// Class that represents a wave conditions calculation configuration
    /// containing <see cref="ConfigurationStabilityStoneCoverCalculationType"/>.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationConfiguration : AssessmentSectionCategoryWaveConditionsCalculationConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="StabilityStoneCoverWaveConditionsCalculationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsCalculationConfiguration(string name) 
            : base(name) {}

        /// <summary>
        /// Gets or sets the calculation type.
        /// </summary>
        public ConfigurationStabilityStoneCoverCalculationType? CalculationType { get; set; }
    }
}