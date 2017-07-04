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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class WaveHeightCalculationTest
    {
        private static IEnumerable<TestCaseData> HydraulicBoundaryLocationsToCalculate
        {
            get
            {
                yield return new TestCaseData(new TestHydraulicBoundaryLocation("WithOutputWithoutIllustrationPoints")
                {
                    WaveHeightCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = new TestHydraulicBoundaryLocationOutput(1.0, CalculationConvergence.CalculatedConverged)
                    }
                }, true);

                yield return new TestCaseData(new TestHydraulicBoundaryLocation("WithoutOutput"), true);

                yield return new TestCaseData(new TestHydraulicBoundaryLocation("WithValidOutput")
                {
                    WaveHeightCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(1.0, CalculationConvergence.CalculatedConverged)
                    }
                }, false);
            }
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WaveHeightCalculation(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_ValidHydraulicBoundaryLocation_SetsProperties(
            [Values(true, false)] bool calculateIllustrationPoints)
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 1, 1)
            {
                WaveHeightCalculation =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = calculateIllustrationPoints
                    }
                }
            };

            // Call
            var calculation = new WaveHeightCalculation(hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<IWaveHeightCalculation>(calculation);
            Assert.AreEqual(hydraulicBoundaryLocation.Id, calculation.Id);
            Assert.AreSame(hydraulicBoundaryLocation.Name, calculation.Name);

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = hydraulicBoundaryLocation.WaveHeightCalculation;
            Assert.AreEqual(hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated, calculation.CalculateIllustrationPoints);

            Assert.AreSame(hydraulicBoundaryLocation, calculation.GetObservableObject());
        }

        [Test]
        [TestCaseSource(nameof(HydraulicBoundaryLocationsToCalculate))]
        public void IsCalculated_NotFullyCalculated_ReturnIsCalculated(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                       bool shouldBeCalculated)
        {
            // Setup
            var calculation = new WaveHeightCalculation(hydraulicBoundaryLocation);

            // Call
            bool isCalculated = calculation.IsCalculated();

            // Assert
            Assert.AreEqual(!shouldBeCalculated, isCalculated);
        }

        [Test]
        public void Output_ValidOutput_SetsOutput()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new WaveHeightCalculation(hydraulicBoundaryLocation);
            var output = new TestHydraulicBoundaryLocationOutput(1);

            // Call
            calculation.Output = output;

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation.WaveHeightCalculation.Output, output);
        }
    }
}