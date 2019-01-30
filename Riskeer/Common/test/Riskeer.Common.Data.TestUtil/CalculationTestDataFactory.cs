﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Factory for creating test calculation objects.
    /// </summary>
    public static class CalculationTestDataFactory
    {
        /// <summary>
        /// Creates a test calculation without output.
        /// </summary>
        /// <returns>The created calculation.</returns>
        public static ICalculation CreateCalculationWithoutOutput()
        {
            return new TestCalculation();
        }

        /// <summary>
        /// Creates a test calculation with output.
        /// </summary>
        /// <returns>The created calculation.</returns>
        public static ICalculation CreateCalculationWithOutput()
        {
            return new TestCalculation
            {
                Output = new object()
            };
        }
    }
}