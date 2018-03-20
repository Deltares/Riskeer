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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.AssessmentSection
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

        #region Norm type signaling

        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionWithOutputAndNormTypeSignaling_ReturnsCorrespondingAssessmentLevel()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            double expectedNormativeAssessmentLevel = new Random(21).NextDouble();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.Signaling));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalingNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                {
                    Output = new TestHydraulicBoundaryLocationOutput(expectedNormativeAssessmentLevel)
                },
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            });
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(expectedNormativeAssessmentLevel, normativeAssessmentLevel, normativeAssessmentLevel.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationNullAndNormTypeSignaling_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.Signaling));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalingNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(null);

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationNotPartOfAssessmentSectionAndNormTypeSignaling_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.Signaling));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalingNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(new TestHydraulicBoundaryLocation());

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionWithoutOutputAndNormTypeSignaling_ReturnsNaN()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.Signaling));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalingNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            });
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        #endregion

        #region Norm type lower limit

        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionWithOutputAndNormTypeLowerLimit_ReturnsCorrespondingAssessmentLevel()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            double expectedNormativeAssessmentLevel = new Random(21).NextDouble();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.LowerLimit));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForLowerLimitNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                {
                    Output = new TestHydraulicBoundaryLocationOutput(expectedNormativeAssessmentLevel)
                },
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            });
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(expectedNormativeAssessmentLevel, normativeAssessmentLevel, normativeAssessmentLevel.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationNullAndNormTypeLowerLimit_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.LowerLimit));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForLowerLimitNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(null);

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationNotPartOfAssessmentSectionAndNormTypeLowerLimit_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.LowerLimit));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForLowerLimitNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(new TestHydraulicBoundaryLocation());

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionWithoutOutputAndNormTypeLowerLimit_ReturnsNaN()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(CreateFailureMechanismContribution(NormType.LowerLimit));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForLowerLimitNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            });
            mocks.ReplayAll();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);

            mocks.VerifyAll();
        }

        #endregion
    }
}