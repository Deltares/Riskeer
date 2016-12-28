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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;

namespace Ringtoets.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneErosionFailureMechanismExtensionsTest
    {
        [Test]
        public void GetMechanismSpecificNorm_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => DuneErosionFailureMechanismExtensions.GetMechanismSpecificNorm(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetMechanismSpecificNorm_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.GetMechanismSpecificNorm(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetMechanismSpecificNorm_WithZeroContributionForFailureMechanism_ThrowsArgumentException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 0
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, 1.0 / 300));
            mocks.ReplayAll();

            // Call
            TestDelegate action = () => failureMechanism.GetMechanismSpecificNorm(assessmentSection);

            // Assert
            const string expectedMessage = "De bijdrage van dit toetsspoor is nul. Daardoor kunnen de berekeningen niet worden uitgevoerd.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(action, expectedMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void GetMechanismSpecificNorm_WithValidData_ReturnMechanismSpecificNorm()
        {
            // Setup
            const double norm = 1.0 / 200;
            const double contribution = 10;
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = contribution
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, norm));
            mocks.ReplayAll();

            // Call
            double mechanismSpecificNorm = failureMechanism.GetMechanismSpecificNorm(assessmentSection);

            // Assert
            Assert.AreEqual(0.0005375, mechanismSpecificNorm);
            mocks.VerifyAll();
        }
    }
}