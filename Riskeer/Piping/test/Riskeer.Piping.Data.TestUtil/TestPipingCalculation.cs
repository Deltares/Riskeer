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

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Piping calculation that can be used for testing.
    /// </summary>
    public class TestPipingCalculation : PipingCalculation<PipingInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestPipingCalculation"/> with default <see cref="PipingInput"/>.
        /// </summary>
        public TestPipingCalculation() : base(new PipingInput(new GeneralPipingInput())) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestPipingCalculation"/>.
        /// </summary>
        /// <param name="pipingInput">The input parameters to perform the piping calculation with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pipingInput"/>
        /// is <c>null</c>.</exception>
        public TestPipingCalculation(PipingInput pipingInput) : base(pipingInput) {}
    }
}