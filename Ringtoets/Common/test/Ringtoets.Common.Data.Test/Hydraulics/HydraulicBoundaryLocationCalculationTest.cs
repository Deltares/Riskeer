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
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationTest
    {
        [Test]
        public void Constructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocationCalculation(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            var calculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreSame(hydraulicBoundaryLocation, calculation.HydraulicBoundaryLocation);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void HasOutput_WithOrWithoutOutput_ReturnsExpectedValue(
            [Values(true, false)] bool setOutput)
        {
            // Setup
            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
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
        public void IsCalculated_Always_ReturnIsCalculated(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                           bool expectedIsCalculated)
        {
            // Call
            bool isCalculated = hydraulicBoundaryLocationCalculation.IsCalculated;

            // Assert
            Assert.AreEqual(expectedIsCalculated, isCalculated);
        }

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryLocationCalculations()
        {
            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                },
                Output = new TestHydraulicBoundaryLocationOutput(1.0, CalculationConvergence.CalculatedConverged)
            }, false);

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()), false);

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationOutput(1.0, CalculationConvergence.CalculatedConverged)
            }, true);

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                },
                Output = new TestHydraulicBoundaryLocationOutput(1.0, new TestGeneralResultSubMechanismIllustrationPoint())
            }, true);

            yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationOutput(1.0, new TestGeneralResultSubMechanismIllustrationPoint())
            }, false);
        }
    }
}