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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Integration.Data.Test
{
    [TestFixture]
    public class AssessmentSectionExtensionsTest
    {
        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AssessmentSectionExtensions.GetNormativeAssessmentLevel(null, new TestHydraulicBoundaryLocation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionWithInvalidNormType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution((NormType) invalidValue));
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => assessmentSection.GetNormativeAssessmentLevel(new TestHydraulicBoundaryLocation());

            // Assert
            string expectedMessage = $"The value of argument 'normType' ({invalidValue}) is invalid for Enum type '{nameof(NormType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
            Assert.AreEqual("normType", parameterName);

            mocks.VerifyAll();
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationWithOutputAndNormTypeSignaling_ReturnsCorrespondingAssessmentLevel()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.Signaling));
            mocks.ReplayAll();

            HydraulicBoundaryLocation hydraulicBoundaryLocationWithOutput = CreateHydraulicBoundaryLocationWithOutput();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocationWithOutput);

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocationWithOutput.DesignWaterLevelCalculation2.Output.Result, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationWithOutputAndNormTypeLowerLimit_ReturnsCorrespondingAssessmentLevel()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.LowerLimit));
            mocks.ReplayAll();

            HydraulicBoundaryLocation hydraulicBoundaryLocationWithOutput = CreateHydraulicBoundaryLocationWithOutput();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocationWithOutput);

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocationWithOutput.DesignWaterLevelCalculation3.Output.Result, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        [TestCase(NormType.Signaling)]
        [TestCase(NormType.LowerLimit)]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationNull_ReturnsNaN(NormType normType)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(normType));
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(null);

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        [TestCase(NormType.Signaling)]
        [TestCase(NormType.LowerLimit)]
        public void GetNormativeAssessmentLevel_NoCorrespondingAssessmentLevelOutput_ReturnsNaN(NormType normType)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(normType));
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(new TestHydraulicBoundaryLocation());

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        private static FailureMechanismContribution CreateFailureMechanismContribution(NormType normType)
        {
            var random = new Random(21);
            int otherContribution = random.Next(0, 100);
            const double norm = 1.0 / 30000;

            return new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(),
                                                    otherContribution,
                                                    norm,
                                                    norm)
            {
                NormativeNorm = normType
            };
        }

        private static HydraulicBoundaryLocation CreateHydraulicBoundaryLocationWithOutput()
        {
            var random = new Random(32);

            return new TestHydraulicBoundaryLocation
            {
                DesignWaterLevelCalculation2 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                },
                DesignWaterLevelCalculation3 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                }
            };
        }
    }
}