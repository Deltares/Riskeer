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
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsProbabilityAssessmentOutputFactoryTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsProbabilityAssessmentOutputFactory.Create(null,
                                                                                                        new GrassCoverErosionInwardsFailureMechanism(),
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
            TestDelegate call = () => GrassCoverErosionInwardsProbabilityAssessmentOutputFactory.Create(new TestOvertoppingOutput(0), 
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
            TestDelegate call = () => GrassCoverErosionInwardsProbabilityAssessmentOutputFactory.Create(new TestOvertoppingOutput(0),
                                                                                                        new GrassCoverErosionInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_WithData_ReturnsProbabilityAssessmentInput()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                GeneralInput =
                {
                    N = (RoundedDouble) 1
                },
                Contribution = 100
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);

            var output = new TestOvertoppingOutput(0);

            // Call
            ProbabilityAssessmentOutput probabilityOutput = GrassCoverErosionInwardsProbabilityAssessmentOutputFactory.Create(output,
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
        }
    }
}