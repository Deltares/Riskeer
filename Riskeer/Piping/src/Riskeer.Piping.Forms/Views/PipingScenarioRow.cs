// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// Base class that represents a row of <typeparamref name="TCalculationScenario"/> in the <see cref="PipingScenariosView"/>.
    /// </summary>
    /// <typeparam name="TCalculationScenario">The type of the calculation scenario.</typeparam>
    public abstract class PipingScenarioRow<TCalculationScenario> : ScenarioRow<TCalculationScenario>, IPipingScenarioRow
        where TCalculationScenario : class, IPipingCalculationScenario<PipingInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingScenarioRow{TCalculationScenario}"/>.
        /// </summary>
        /// <param name="calculationScenario">The calculation scenario this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationScenario"/>
        /// is <c>null</c>.</exception>
        protected PipingScenarioRow(TCalculationScenario calculationScenario)
            : base(calculationScenario) {}

        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public abstract double SectionFailureProbability { get; }
    }
}