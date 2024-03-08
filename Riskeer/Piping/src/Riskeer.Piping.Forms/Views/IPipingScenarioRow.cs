﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using Core.Common.Base.Data;
using Riskeer.Piping.Data;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// Interface that represents a row of <see cref="IPipingCalculationScenario{TPipingInput}"/> in the <see cref="PipingScenariosView"/>.
    /// </summary>
    public interface IPipingScenarioRow
    {
        /// <summary>
        /// Gets whether the <see cref="IPipingCalculationScenario{TPipingInput}"/> is relevant.
        /// </summary>
        bool IsRelevant { get; }
        
        /// <summary>
        /// Gets the contribution of <see cref="IPipingCalculationScenario{TPipingInput}"/>.
        /// </summary>
        RoundedDouble Contribution { get; }
        
        /// <summary>
        /// Gets the section failure probability of the <see cref="IPipingCalculationScenario{TPipingInput}"/>.
        /// </summary>
        double SectionFailureProbability { get; }

        /// <summary>
        /// Updates the row based on the current output of the <see cref="IPipingCalculationScenario{TPipingInput}"/>.
        /// </summary>
        void Update();
    }
}