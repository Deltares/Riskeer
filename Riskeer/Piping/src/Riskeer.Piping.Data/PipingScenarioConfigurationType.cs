﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Core.Common.Util.Attributes;
using Riskeer.Piping.Data.Properties;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Specifies the types of the scenario configurations.
    /// </summary>
    public enum PipingScenarioConfigurationType
    {
        /// <summary>
        /// The semi probabilistic scenario configuration type.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SemiProbabilistic_DisplayName))]
        SemiProbabilistic = 1,

        /// <summary>
        /// The probabilistic scenario configuration type.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Probabilistic_DisplayName))]
        Probabilistic = 2,

        /// <summary>
        /// The per failure mechanism section scenario configuration type.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingScenarioConfigurationType_PerFailureMechanismSection_DisplayName))]
        PerFailureMechanismSection = 3
    }
}