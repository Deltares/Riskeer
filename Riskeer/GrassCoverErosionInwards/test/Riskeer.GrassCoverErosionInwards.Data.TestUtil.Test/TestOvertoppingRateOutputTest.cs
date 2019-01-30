﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.GrassCoverErosionInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class TestOvertoppingRateOutputTest
    {
        [Test]
        public void TestOvertoppingRateOutput_WithoutConvergence_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(12);
            double result = random.NextDouble();

            // Call
            var output = new TestOvertoppingRateOutput(result);

            // Assert
            Assert.IsInstanceOf<OvertoppingRateOutput>(output);
            Assert.AreEqual(result, output.OvertoppingRate, output.OvertoppingRate.GetAccuracy());
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.NotCalculated, output.CalculationConvergence);
            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void TestOvertoppingRateOutput_WithConvergence_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(12);
            double result = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            var output = new TestOvertoppingRateOutput(result, convergence);

            // Assert
            Assert.IsInstanceOf<OvertoppingRateOutput>(output);
            Assert.AreEqual(result, output.OvertoppingRate, output.OvertoppingRate.GetAccuracy());
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(convergence, output.CalculationConvergence);
            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void TestOvertoppingRateOutput_WithGeneralResult_ReturnsExpectedValues()
        {
            // Setup
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();

            // Call
            var output = new TestOvertoppingRateOutput(generalResult);

            // Assert
            Assert.IsInstanceOf<OvertoppingRateOutput>(output);
            Assert.IsNaN(output.OvertoppingRate);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, output.CalculationConvergence);
            Assert.AreSame(generalResult, output.GeneralResult);
        }
    }
}