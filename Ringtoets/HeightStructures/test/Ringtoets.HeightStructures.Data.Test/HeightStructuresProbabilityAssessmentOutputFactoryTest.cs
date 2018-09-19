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
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresProbabilityAssessmentOutputFactoryTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => HeightStructuresProbabilityAssessmentOutputFactory.Create(null,
                                                                                                new HeightStructuresFailureMechanism(),
                                                                                                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
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
            TestDelegate call = () => HeightStructuresProbabilityAssessmentOutputFactory.Create(new TestStructuresOutput(),
                                                                                                null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresProbabilityAssessmentOutputFactory.Create(new TestStructuresOutput(),
                                                                                                new HeightStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_WithData_ReturnsProbabilityAssessmentInput()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                Contribution = 100
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var output = new TestStructuresOutput();

            // Call
            ProbabilityAssessmentOutput probabilityOutput = HeightStructuresProbabilityAssessmentOutputFactory.Create(output,
                                                                                                                      failureMechanism,
                                                                                                                      assessmentSection);

            // Assert
            ProbabilityAssessmentOutput expectedProbabilityOutput = ProbabilityAssessmentOutputFactory.Create(assessmentSection.FailureMechanismContribution.Norm,
                                                                                                              failureMechanism.Contribution,
                                                                                                              failureMechanism.GeneralInput.N,
                                                                                                              output.Reliability);
            Assert.AreEqual(expectedProbabilityOutput.FactorOfSafety, probabilityOutput.FactorOfSafety);
            Assert.AreEqual(expectedProbabilityOutput.Probability, probabilityOutput.Probability);
            Assert.AreEqual(expectedProbabilityOutput.Reliability, probabilityOutput.Reliability);
            Assert.AreEqual(expectedProbabilityOutput.RequiredProbability, probabilityOutput.RequiredProbability);
            Assert.AreEqual(expectedProbabilityOutput.RequiredReliability, probabilityOutput.RequiredReliability);
            mocks.VerifyAll();
        }
    }
}