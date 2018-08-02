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
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class FailureMechanismContributionTestFactoryTest
    {
        [Test]
        public void CreateFailureMechanismContribution_ReturnsExpectedFailureMechanismContribution()
        {
            // Call
            FailureMechanismContribution contribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Assert
            Assert.AreEqual(1.0 / 30000, contribution.LowerLimitNorm);
            Assert.AreEqual(1.0 / 30000, contribution.SignalingNorm);
            Assert.AreEqual(NormType.LowerLimit, contribution.NormativeNorm);
        }

        [Test]
        public void CreateFailureMechanismContribution_FailureMechanismsNull_ReturnsExpectedFailureMechanismContribution()
        {
            // Call
            TestDelegate test = () => FailureMechanismContributionTestFactory.CreateFailureMechanismContribution(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void CreateFailureMechanismContribution_WithFailureMechanism_ReturnsExpectedFailureMechanismContribution()
        {
            // Call
            var mockRepository = new MockRepository();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            var failureMechanism2 = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var failureMechanisms = new[]
            {
                failureMechanism,
                failureMechanism2
            };
            FailureMechanismContribution contribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution(failureMechanisms);

            // Assert
            Assert.AreEqual(1.0 / 30000, contribution.LowerLimitNorm);
            Assert.AreEqual(1.0 / 30000, contribution.SignalingNorm);
            Assert.AreEqual(NormType.LowerLimit, contribution.NormativeNorm);

            mockRepository.VerifyAll();
        }
    }
}