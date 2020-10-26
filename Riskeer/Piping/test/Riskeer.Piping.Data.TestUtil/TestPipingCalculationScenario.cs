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
using Core.Common.Base.Data;

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Piping calculation scenario that can be used for testing.
    /// </summary>
    public class TestPipingCalculationScenario : TestPipingCalculation, IPipingCalculationScenario<PipingInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestPipingCalculationScenario"/> with default <see cref="PipingInput"/>.
        /// </summary>
        /// <param name="hasOutput">Whether or not the calculation should have output.</param>
        public TestPipingCalculationScenario(bool hasOutput = false) : this(new TestPipingInput(), hasOutput) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestPipingCalculationScenario"/>.
        /// </summary>
        /// <param name="pipingInput">The input parameters to perform the piping calculation with.</param>
        /// <param name="hasOutput">Whether or not the calculation should have output.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/>
        /// is <c>null</c>.</exception>
        public TestPipingCalculationScenario(PipingInput pipingInput, bool hasOutput = false) : base(pipingInput, hasOutput) {}

        public bool IsRelevant { get; set; }

        public RoundedDouble Contribution { get; set; }
    }
}