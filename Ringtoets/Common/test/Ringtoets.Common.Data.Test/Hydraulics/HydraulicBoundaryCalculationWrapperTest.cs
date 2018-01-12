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

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryCalculationWrapperTest
    {
        private static IEnumerable<TestCaseData> HydraulicBoundaryLocationCalculationsToPerform
        {
            get
            {
                yield return new TestCaseData(new HydraulicBoundaryLocationCalculation
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(1.0, CalculationConvergence.CalculatedConverged)
                }, true);

                yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(), true);

                yield return new TestCaseData(new HydraulicBoundaryLocationCalculation
                {
                    Output = new TestHydraulicBoundaryLocationOutput(1.0, CalculationConvergence.CalculatedConverged)
                }, false);
            }
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryCalculationWrapper(null, new HydraulicBoundaryLocationCalculation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryCalculationWrapper(new TestHydraulicBoundaryLocation(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocationCalculation", paramName);
        }

        [Test]
        public void Constructor_ValidInputParameters_SetsProperties([Values(true, false)] bool calculateIllustrationPoints)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = calculateIllustrationPoints
                }
            };

            // Call
            var calculationWrapper = new HydraulicBoundaryCalculationWrapper(hydraulicBoundaryLocation, hydraulicBoundaryLocationCalculation);

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocation.Id, calculationWrapper.Id);
            Assert.AreSame(hydraulicBoundaryLocation.Name, calculationWrapper.Name);
            Assert.AreEqual(hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated, calculationWrapper.CalculateIllustrationPoints);
            Assert.AreSame(hydraulicBoundaryLocation, calculationWrapper.ObservableObject);
        }

        [Test]
        [TestCaseSource(nameof(HydraulicBoundaryLocationCalculationsToPerform))]
        public void IsCalculated_NotFullyCalculated_ReturnIsCalculated(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                                       bool shouldBeCalculated)
        {
            // Setup
            var calculationWrapper = new HydraulicBoundaryCalculationWrapper(new TestHydraulicBoundaryLocation(),
                                                                             hydraulicBoundaryLocationCalculation);

            // Call
            bool isCalculated = calculationWrapper.IsCalculated();

            // Assert
            Assert.AreEqual(!shouldBeCalculated, isCalculated);
        }

        [Test]
        public void Output_ValidOutput_SetsOutput()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation();
            var calculationWrapper = new HydraulicBoundaryCalculationWrapper(hydraulicBoundaryLocation, hydraulicBoundaryLocationCalculation);
            var output = new TestHydraulicBoundaryLocationOutput(1);

            // Call
            calculationWrapper.Output = output;

            // Assert
            Assert.AreSame(output, calculationWrapper.Output);
            Assert.AreSame(output, hydraulicBoundaryLocationCalculation.Output);
        }
    }
}