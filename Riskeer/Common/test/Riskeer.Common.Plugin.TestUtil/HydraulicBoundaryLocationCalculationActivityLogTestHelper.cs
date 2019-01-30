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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.Common.Service.TestUtil;

namespace Riskeer.Common.Plugin.TestUtil
{
    /// <summary>
    /// Class containing helper methods used to assert the log messages of a calculation activity
    /// for hydraulic boundary location calculations.
    /// </summary>
    public static class HydraulicBoundaryLocationCalculationActivityLogTestHelper
    {
        /// <summary>
        /// Asserts whether <paramref cref="actualMessages"/> contains the correct items given the other parameters.
        /// </summary>
        /// <param name="locationName">The name of the location.</param>
        /// <param name="calculationTypeDisplayName">The display name of the type of calculation being performed.</param>
        /// <param name="calculationDisplayName">The display name of the calculation being performed.</param>
        /// <param name="categoryName">The category boundary name of the calculation.</param>
        /// <param name="actualMessages">The log messages to assert.</param>
        /// <param name="startIndex">The index to start asserting from.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actualMessages"/>
        /// contains incorrect messages.</exception>
        public static void AssertHydraulicBoundaryLocationCalculationMessages(string locationName,
                                                                              string calculationTypeDisplayName,
                                                                              string calculationDisplayName,
                                                                              string categoryName,
                                                                              IEnumerable<string> actualMessages,
                                                                              int startIndex)
        {
            Assert.AreEqual($"{calculationTypeDisplayName} berekenen voor locatie '{locationName}' (Categoriegrens {categoryName}) is gestart.",
                            actualMessages.ElementAt(startIndex));
            CalculationServiceTestHelper.AssertValidationStartMessage(actualMessages.ElementAt(startIndex + 1));
            CalculationServiceTestHelper.AssertValidationEndMessage(actualMessages.ElementAt(startIndex + 2));
            CalculationServiceTestHelper.AssertCalculationStartMessage(actualMessages.ElementAt(startIndex + 3));
            Assert.AreEqual($"{calculationDisplayName} voor locatie '{locationName}' (Categoriegrens {categoryName}) is niet geconvergeerd.",
                            actualMessages.ElementAt(startIndex + 4));
            StringAssert.StartsWith($"{calculationDisplayName} is uitgevoerd op de tijdelijke locatie",
                                    actualMessages.ElementAt(startIndex + 5));
            CalculationServiceTestHelper.AssertCalculationEndMessage(actualMessages.ElementAt(startIndex + 6));
            Assert.AreEqual($"{calculationTypeDisplayName} berekenen voor locatie '{locationName}' (Categoriegrens {categoryName}) is gelukt.",
                            actualMessages.ElementAt(startIndex + 7));
        }
    }
}