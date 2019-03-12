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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class IllustrationPointsHelperTest
    {
        [Test]
        public void HasIllustrationPoints_HydraulicBoundaryLocationCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => IllustrationPointsHelper.HasIllustrationPoints((IEnumerable<HydraulicBoundaryLocationCalculation>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculations))]
        public void HasIllustrationPoints_WithHydraulicBoundaryLocationCalculations_ReturnsExpectedResult(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                                                          bool expectedHasIllustrationPoints)
        {
            // Call
            bool hasIllustrationPoints = IllustrationPointsHelper.HasIllustrationPoints(calculations);

            // Assert
            Assert.AreEqual(expectedHasIllustrationPoints, hasIllustrationPoints);
        }

        [Test]
        public void HasIllustrationPoints_StructureCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => IllustrationPointsHelper.HasIllustrationPoints((IEnumerable<TestStructuresCalculation>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetStructureCalculationCollections))]
        public void HasIllustrationPoints_WithStructuresCalculations_ReturnsExpectedResult(IEnumerable<TestStructuresCalculation> calculations,
                                                                                           bool expectedHasIllustrationPoints)
        {
            // Call
            bool hasIllustrationPoints = IllustrationPointsHelper.HasIllustrationPoints(calculations);

            // Assert
            Assert.AreEqual(expectedHasIllustrationPoints, hasIllustrationPoints);
        }

        [Test]
        public void HasIllustrationPoints_StructureCalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => IllustrationPointsHelper.HasIllustrationPoints((TestStructuresCalculation) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetStructureCalculations))]
        public void HasIllustrationPoints_WithStructuresCalculation_ReturnsExpectedResult(TestStructuresCalculation calculation,
                                                                                          bool expectedHasIllustrationPoints)
        {
            // Call
            bool hasIllustrationPoints = IllustrationPointsHelper.HasIllustrationPoints(calculation);

            // Assert
            Assert.AreEqual(expectedHasIllustrationPoints, hasIllustrationPoints);
        }

        #region Test data

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryLocationCalculations()
        {
            var random = new Random(21);
            var calculationWithIllustrationPoints = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(),
                                                                            new TestGeneralResultSubMechanismIllustrationPoint())
            };

            var calculationWithOutput1 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
            };
            var calculationWithOutput2 = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
            };

            yield return new TestCaseData(new[]
            {
                calculationWithOutput1,
                calculationWithIllustrationPoints,
                calculationWithOutput2
            }, true);

            yield return new TestCaseData(new[]
            {
                calculationWithOutput1,
                calculationWithOutput2
            }, false);

            yield return new TestCaseData(new[]
            {
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            }, false);

            yield return new TestCaseData(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), false);
        }

        private static IEnumerable<TestCaseData> GetStructureCalculationCollections()
        {
            var calculationWithIllustrationPoints = new TestStructuresCalculation
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput1 = new TestStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };
            var calculationWithOutput2 = new TestStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };

            yield return new TestCaseData(new[]
            {
                calculationWithOutput1,
                calculationWithIllustrationPoints,
                calculationWithOutput2
            }, true);

            yield return new TestCaseData(new[]
            {
                calculationWithOutput1,
                calculationWithOutput2
            }, false);

            yield return new TestCaseData(new[]
            {
                new TestStructuresCalculation()
            }, false);

            yield return new TestCaseData(Enumerable.Empty<TestStructuresCalculation>(), false);
        }

        private static IEnumerable<TestCaseData> GetStructureCalculations()
        {
            yield return new TestCaseData(new TestStructuresCalculation
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            }, true);

            yield return new TestCaseData(new TestStructuresCalculation
            {
                Output = new TestStructuresOutput()
            }, false);

            yield return new TestCaseData(new TestStructuresCalculation(), false);
        }

        #endregion
    }
}