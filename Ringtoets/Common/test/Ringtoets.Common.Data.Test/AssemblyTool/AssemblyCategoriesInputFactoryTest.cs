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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyCategoriesInputFactoryTest
    {
        [Test]
        public void CreateAssemblyCategoriesInput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => AssemblyCategoriesInputFactory.CreateAssemblyCategoriesInput(new Random(39).NextDouble(),
                                                                                                   null,
                                                                                                   assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateAssemblyCategoriesInput_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => AssemblyCategoriesInputFactory.CreateAssemblyCategoriesInput(new Random(39).NextDouble(),
                                                                                                   failureMechanism,
                                                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateAssemblyCategoriesInput_WithValidInput_ReturnsExpectedAssemblyCategoriesInput()
        {
            // Setup
            var random = new Random(39);
            double n = random.NextDouble();
            var failureMechanism = new TestFailureMechanism
            {
                Contribution = random.Next(1, 100)
            };
            var assessmentSection = new AssessmentSectionStub();

            // Call
            AssemblyCategoriesInput assemblyCategoriesInput = AssemblyCategoriesInputFactory.CreateAssemblyCategoriesInput(n, failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(n, assemblyCategoriesInput.N);
            Assert.AreEqual(failureMechanism.Contribution / 100, assemblyCategoriesInput.FailureMechanismContribution);
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.SignalingNorm, assemblyCategoriesInput.SignalingNorm);
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.LowerLimitNorm, assemblyCategoriesInput.LowerLimitNorm);
        }
    }
}