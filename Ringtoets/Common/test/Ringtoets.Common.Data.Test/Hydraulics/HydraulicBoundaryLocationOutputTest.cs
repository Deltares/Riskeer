// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationOutputTest
    {
        [Test]
        public void Constructor_InvalidTargetProbability_ThrowsArgumentOutOfRangeException(
            [Random(1)] double result,
            [Values(-0.01, 1.01)] double targetProbability,
            [Random(1)] double targetReliability,
            [Random(1)] double calculatedProbability,
            [Random(1)] double calculatedReliability,
            [Values(CalculationConvergence.CalculatedNotConverged, CalculationConvergence.CalculatedConverged,
                CalculationConvergence.NotCalculated)] CalculationConvergence convergence)
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationOutput(result, targetProbability,
                                                                          targetReliability,
                                                                          calculatedProbability,
                                                                          calculatedReliability,
                                                                          convergence);

            // Assert
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("targetProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0, 1] liggen.", exception.Message);
        }

        [Test]
        public void Constructor_InvalidCalculatedProbability_ThrowsArgumentOutOfRangeException(
            [Random(1)] double result,
            [Random(1)] double targetProbability,
            [Random(1)] double targetReliability,
            [Values(-0.01, 1.01)] double calculatedProbability,
            [Random(1)] double calculatedReliability,
            [Values(CalculationConvergence.CalculatedNotConverged, CalculationConvergence.CalculatedConverged,
                CalculationConvergence.NotCalculated)] CalculationConvergence convergence)
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationOutput(result, targetProbability,
                                                                          targetReliability,
                                                                          calculatedProbability,
                                                                          calculatedReliability,
                                                                          convergence);

            // Assert
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("calculatedProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0, 1] liggen.", exception.Message);
        }

        [Test]
        public void Constructor_ValidInput_ExpectedProperties(
            [Random(1)] double result,
            [Random(1)] double targetProbability,
            [Random(1)] double targetReliability,
            [Random(1)] double calculatedProbability,
            [Random(1)] double calculatedReliability,
            [Values(CalculationConvergence.CalculatedNotConverged, CalculationConvergence.CalculatedConverged,
                CalculationConvergence.NotCalculated)] CalculationConvergence convergence)
        {
            // Call
            var output = new HydraulicBoundaryLocationOutput(result, targetProbability,
                                                             targetReliability,
                                                             calculatedProbability,
                                                             calculatedReliability,
                                                             convergence);

            // Assert
            Assert.AreEqual(result, output.Result, output.Result.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability, output.TargetProbability.GetAccuracy());
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability, output.CalculatedProbability.GetAccuracy());
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }
    }
}