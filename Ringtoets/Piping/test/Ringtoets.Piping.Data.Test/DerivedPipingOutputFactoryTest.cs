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
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class DerivedPipingOutputFactoryTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DerivedPipingOutputFactory.Create(null,
                                                                        new PipingProbabilityAssessmentInput(),
                                                                        double.NaN,
                                                                        double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Create_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DerivedPipingOutputFactory.Create(new PipingOutput(new PipingOutput.ConstructionProperties()),
                                                                        null,
                                                                        double.NaN,
                                                                        double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("probabilityAssessmentInput", exception.ParamName);
        }

        [Test]
        [TestCase(30000, 1.2, 7.36633055700265E-06)]
        [TestCase(30000, 1.0, 4.13743266617776E-05)]
        [TestCase(20000, 1.2, 9.53352884976163E-06)]
        [TestCase(20000, 1.0, 5.24016937211752E-05)]
        public void Create_ValidData_ReturnsExpectedValue(int returnPeriod, double factorOfSafety, double expectedResult)
        {
            // Setup
            var probabilityAssessmentInput = new PipingProbabilityAssessmentInput
            {
                SectionLength = 6000
            };

            var calculatorResult = new PipingOutput(new PipingOutput.ConstructionProperties
            {
                UpliftFactorOfSafety = 1.2,
                HeaveFactorOfSafety = 1.4,
                SellmeijerFactorOfSafety = 0.9
            });
            const double norm = 1.0 / 30000;

            // Call
            DerivedPipingOutput derivedOutput = DerivedPipingOutputFactory.Create(calculatorResult, probabilityAssessmentInput, norm, 1000);

            // Assert
            Assert.AreEqual(7.3e-6, derivedOutput.UpliftProbability, 1e-6);
            Assert.AreEqual(0.004, derivedOutput.HeaveProbability, 1e-6);
            Assert.AreEqual(1.0988e-5, derivedOutput.SellmeijerProbability, 1e-6);
            Assert.AreEqual(1.33, derivedOutput.PipingReliability, derivedOutput.PipingReliability.GetAccuracy());
            Assert.AreEqual(4.77, derivedOutput.RequiredReliability, derivedOutput.RequiredReliability.GetAccuracy());
            Assert.AreEqual(0.907, derivedOutput.PipingFactorOfSafety, derivedOutput.PipingFactorOfSafety.GetAccuracy());
        }
    }
}