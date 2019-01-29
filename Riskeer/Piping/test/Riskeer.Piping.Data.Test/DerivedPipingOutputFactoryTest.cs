// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.TestUtil;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class DerivedPipingOutputFactoryTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => DerivedPipingOutputFactory.Create(null,
                                                                        new PipingFailureMechanism(),
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
            TestDelegate call = () => DerivedPipingOutputFactory.Create(new PipingOutput(new PipingOutput.ConstructionProperties()),
                                                                        null,
                                                                        assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => DerivedPipingOutputFactory.Create(new PipingOutput(new PipingOutput.ConstructionProperties()),
                                                                        new PipingFailureMechanism(),
                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_ValidData_ReturnsExpectedValue()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                Contribution = 100
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculatorResult = new PipingOutput(new PipingOutput.ConstructionProperties
            {
                UpliftFactorOfSafety = 1.2,
                HeaveFactorOfSafety = 1.4,
                SellmeijerFactorOfSafety = 0.9
            });

            // Call
            DerivedPipingOutput derivedOutput = DerivedPipingOutputFactory.Create(calculatorResult, failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(0.0030333773290253025, derivedOutput.UpliftProbability, 1e-6);
            Assert.AreEqual(0.00017624686431291146, derivedOutput.HeaveProbability, 1e-6);
            Assert.AreEqual(0.13596896289025881, derivedOutput.SellmeijerProbability, 1e-6);
            Assert.AreEqual(3.57331, derivedOutput.PipingReliability, derivedOutput.PipingReliability.GetAccuracy());
            Assert.AreEqual(1.28155, derivedOutput.RequiredReliability, derivedOutput.RequiredReliability.GetAccuracy());
            Assert.AreEqual(2.788, derivedOutput.PipingFactorOfSafety, derivedOutput.PipingFactorOfSafety.GetAccuracy());
            mocks.VerifyAll();
        }
    }
}