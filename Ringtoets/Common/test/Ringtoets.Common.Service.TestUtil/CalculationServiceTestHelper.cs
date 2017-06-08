// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using NUnit.Framework;

namespace Ringtoets.Common.Service.TestUtil
{
    /// <summary>
    /// A helper for asserting results of using calculation services.
    /// </summary>
    public static class CalculationServiceTestHelper
    {
        /// <summary>
        /// Asserts that the given message equals the message stating the start of validation.
        /// </summary>
        /// <param name="calculationName">The name of the calculation.</param>
        /// <param name="actualMessage">The actual message.</param>
        public static void AssertValidationStartMessage(string calculationName, string actualMessage)
        {
            Assert.AreEqual($"Validatie van '{calculationName}' gestart.", actualMessage);
        }

        /// <summary>
        /// Asserts that the given message equals the message stating the end of validation.
        /// </summary>
        /// <param name="calculationName">The name of the calculation.</param>
        /// <param name="actualMessage">The actual message.</param>
        public static void AssertValidationEndMessage(string calculationName, string actualMessage)
        {
            Assert.AreEqual($"Validatie van '{calculationName}' beëindigd.", actualMessage);
        }

        /// <summary>
        /// Asserts that the given message equals the message stating the start of calculation.
        /// </summary>
        /// <param name="calculationName">The name of the calculation.</param>
        /// <param name="actualMessage">The actual message.</param>
        public static void AssertCalculationStartMessage(string calculationName, string actualMessage)
        {
            Assert.AreEqual($"Berekening van '{calculationName}' gestart.", actualMessage);
        }

        /// <summary>
        /// Asserts that the given message equals the message stating the end of calculation.
        /// </summary>
        /// <param name="calculationName">The name of the calculation.</param>
        /// <param name="actualMessage">The actual message.</param>
        public static void AssertCalculationEndMessage(string calculationName, string actualMessage)
        {
            Assert.AreEqual($"Berekening van '{calculationName}' beëindigd.", actualMessage);
        }
    }
}