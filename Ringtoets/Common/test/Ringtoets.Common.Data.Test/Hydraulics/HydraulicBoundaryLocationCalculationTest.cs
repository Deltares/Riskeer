﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var calculation = new HydraulicBoundaryLocationCalculation();

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationInput>(calculation.InputParameters);
            Assert.IsNull(calculation.Output);
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void HasOutput_WithOrWithoutOutput_ReturnsExpectedValue(
            [Values(true, false)] bool setOutput)
        {
            // Setup
            var calculation = new HydraulicBoundaryLocationCalculation();
            if (setOutput)
            {
                calculation.Output = new TestHydraulicBoundaryLocationOutput(5);
            }

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.AreEqual(setOutput, hasOutput);
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculations))]
        public void IsCalculated_NotFullyCalculated_ReturnIsCalculated(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                                       bool expectedIsCalculated)
        {
            // Call
            bool isCalculated = hydraulicBoundaryLocationCalculation.IsCalculated();

            // Assert
            Assert.AreEqual(expectedIsCalculated, isCalculated);
        }

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryLocationCalculations()
        {
            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                },
                Output = new TestHydraulicBoundaryLocationOutput(1.0, CalculationConvergence.CalculatedConverged)
            }, false);

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(), false);

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation
            {
                Output = new TestHydraulicBoundaryLocationOutput(1.0, CalculationConvergence.CalculatedConverged)
            }, true);
        }
    }
}