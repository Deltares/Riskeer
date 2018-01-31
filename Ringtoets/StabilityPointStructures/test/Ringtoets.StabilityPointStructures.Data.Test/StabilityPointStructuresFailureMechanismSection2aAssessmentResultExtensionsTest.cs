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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismSection2aAssessmentResultExtensionsTest
    {
        [Test]
        public void GetAssessmentLayerTwoA_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => StabilityPointStructuresFailureMechanismSection2aAssessmentResultExtensions.GetAssessmentLayerTwoA(null,
                                                                                                                                 new StabilityPointStructuresFailureMechanism(),
                                                                                                                                 assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => failureMechanismSectionResult.GetAssessmentLayerTwoA(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => failureMechanismSectionResult.GetAssessmentLayerTwoA(new StabilityPointStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetAssessmentLayerTwoA_SectionResultWithoutCalculation_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new StabilityPointStructuresFailureMechanism(), assessmentSection);

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void GetAssessmentLayerTwoA_CalculationWithoutOutput_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                Calculation = new TestStabilityPointStructuresCalculation()
            };

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new StabilityPointStructuresFailureMechanism(), assessmentSection);

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void GetAssessmentLayerTwoA_CalculationWithOutput_ReturnsDerivedProbability()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                Calculation = new TestStabilityPointStructuresCalculation
                {
                    Output = new TestStructuresOutput()
                }
            };

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new StabilityPointStructuresFailureMechanism(), assessmentSection);

            // Assert
            Assert.AreEqual(0.5, assessmentLayerTwoA);
            mocks.VerifyAll();
        }
    }
}