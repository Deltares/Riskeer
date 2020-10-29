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
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingProbabilityAssessmentOutputFactoryTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingProbabilityAssessmentOutputFactory.Create(null, new ProbabilisticPipingCalculationScenario(), new PipingFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingProbabilityAssessmentOutputFactory.Create(PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(), null, new PipingFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingProbabilityAssessmentOutputFactory.Create(PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(), new ProbabilisticPipingCalculationScenario(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingProbabilityAssessmentOutputFactory.Create(PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(), new ProbabilisticPipingCalculationScenario(), new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_WithData_ReturnsProbabilityAssessmentInput()
        {
            // Setup
            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            var calculation = new ProbabilisticPipingCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = failureMechanism.SurfaceLines.First()
                }
            };

            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            PartialProbabilisticPipingOutput output = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();

            // Call
            ProbabilityAssessmentOutput probabilityOutput = PipingProbabilityAssessmentOutputFactory.Create(output,
                                                                                                            calculation,
                                                                                                            failureMechanism,
                                                                                                            assessmentSection);

            // Assert
            ProbabilityAssessmentOutput expectedProbabilityOutput = ProbabilityAssessmentOutputFactory.Create(assessmentSection.FailureMechanismContribution.Norm,
                                                                                                              failureMechanism.Contribution,
                                                                                                              5.0,
                                                                                                              output.Reliability);

            Assert.AreEqual(expectedProbabilityOutput.RequiredProbability, probabilityOutput.RequiredProbability);
            Assert.AreEqual(expectedProbabilityOutput.RequiredReliability, probabilityOutput.RequiredReliability);
            Assert.AreEqual(expectedProbabilityOutput.Probability, probabilityOutput.Probability);
            Assert.AreEqual(expectedProbabilityOutput.Reliability, probabilityOutput.Reliability);
            Assert.AreEqual(expectedProbabilityOutput.FactorOfSafety, probabilityOutput.FactorOfSafety);
        }
    }
}