﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.PropertyClasses;

namespace Riskeer.HeightStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HeightStructuresOutputPropertiesTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new HeightStructuresOutputProperties(new TestStructuresOutput(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HeightStructuresOutputProperties(new TestStructuresOutput(), new HeightStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var output = new TestStructuresOutput();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            var properties = new HeightStructuresOutputProperties(output, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<StructuresOutputProperties>(properties);
            Assert.AreSame(output, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var random = new Random(39);
            double reliability = random.NextDouble();

            var structuresOutput = new TestStructuresOutput(reliability);

            // Call
            var properties = new HeightStructuresOutputProperties(structuresOutput, failureMechanism, assessmentSection);

            // Assert
            ProbabilityAssessmentOutput expectedProbabilityAssessmentOutput = HeightStructuresProbabilityAssessmentOutputFactory.Create(
                structuresOutput, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedProbabilityAssessmentOutput.Probability), properties.Probability);
            Assert.AreEqual(expectedProbabilityAssessmentOutput.Reliability, properties.Reliability, properties.Reliability.GetAccuracy());
            mocks.VerifyAll();
        }
    }
}