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

using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.Common.Service.TestUtil
{
    /// <summary>
    /// A test helper which can be used to assert instances of <see cref="HydraRingCalculationSettings"/>.
    /// </summary>
    public static class HydraRingCalculationSettingsTestHelper
    {
        /// <summary>
        /// Asserts whether the <see cref="HydraRingCalculationSettings"/> contains the correct
        /// data from <see cref="HydraulicBoundaryCalculationSettings"/>.
        /// </summary>
        /// <param name="expectedSettings">The <see cref="HydraulicBoundaryCalculationSettings"/>
        /// to assert against.</param>
        /// <param name="actualSettings">The <see cref="HydraRingCalculationSettings"/> to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The HLCD file paths do not match.</item>
        /// <item>The preprocessor directories do not match.</item>
        /// </list>
        /// </exception>
        public static void AssertHydraRingCalculationSettings(HydraulicBoundaryCalculationSettings expectedSettings,
                                                              HydraRingCalculationSettings actualSettings)
        {
            Assert.AreEqual(expectedSettings.HlcdFilePath, actualSettings.HlcdFilePath);
            Assert.AreEqual(expectedSettings.PreprocessorDirectory, actualSettings.PreprocessorDirectory);
            Assert.AreEqual(expectedSettings.UsePreprocessorClosure, actualSettings.UsePreprocessorClosure);
        }
    }
}