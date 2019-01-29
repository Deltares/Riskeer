// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.Common.Service.TestUtil
{
    /// <summary>
    /// A helper for asserting results of using calculation services.
    /// </summary>
    public static class CalculationServiceTestHelper
    {
        /// <summary>
        /// Asserts that the given message equals the message stating the start of validation.
        /// </summary>
        /// <param name="actualMessage">The actual message.</param>
        public static void AssertValidationStartMessage(string actualMessage)
        {
            Assert.AreEqual("Validatie is gestart.", actualMessage);
        }

        /// <summary>
        /// Asserts that the given message equals the message stating the end of validation.
        /// </summary>
        /// <param name="actualMessage">The actual message.</param>
        public static void AssertValidationEndMessage(string actualMessage)
        {
            Assert.AreEqual("Validatie is beëindigd.", actualMessage);
        }

        /// <summary>
        /// Asserts that the given message equals the message stating the start of calculation.
        /// </summary>
        /// <param name="actualMessage">The actual message.</param>
        public static void AssertCalculationStartMessage(string actualMessage)
        {
            Assert.AreEqual("Berekening is gestart.", actualMessage);
        }

        /// <summary>
        /// Asserts that the given message equals the message stating the end of calculation.
        /// </summary>
        /// <param name="actualMessage">The actual message.</param>
        public static void AssertCalculationEndMessage(string actualMessage)
        {
            Assert.AreEqual("Berekening is beëindigd.", actualMessage);
        }
    }
}