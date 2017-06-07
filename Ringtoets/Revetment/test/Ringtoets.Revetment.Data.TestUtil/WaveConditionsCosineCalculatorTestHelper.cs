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

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.Revetment.TestUtil
{
    /// <summary>
    /// Class that provide test cases which can be used to test objects using <see cref="IWaveConditionsCosineCalculator"/>.
    /// </summary>
    public static class WaveConditionsCosineCalculatorTestHelper
    {
        /// <summary>
        /// Gets test cases of wave conditions cosine calculators that will fail when called.
        /// </summary>
        public static IEnumerable<TestCaseData> FailingWaveConditionsCosineCalculators
        {
            get
            {
                yield return new TestCaseData(new TestWaveConditionsCosineCalculator
                    {
                        LastErrorFileContent = "LastErrorFileContent"
                    }, $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}LastErrorFileContent")
                    .SetName("{m}(LastErrorFileContent)");
                yield return new TestCaseData(new TestWaveConditionsCosineCalculator
                    {
                        EndInFailure = true
                    }, "Er is geen foutrapport beschikbaar.")
                    .SetName("{m}(EndInFailure)");
                yield return new TestCaseData(new TestWaveConditionsCosineCalculator
                    {
                        EndInFailure = true,
                        LastErrorFileContent = "LastErrorFileContentAndEndInFailure"
                    }, $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}LastErrorFileContentAndEndInFailure")
                    .SetName("{m}(LastErrorFileContentAndEndInFailure)");
            }
        }
    }
}