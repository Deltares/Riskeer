// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PropertyClasses;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.ClosingStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ClosingStructuresOutputPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var output = new TestStructuresOutput();

            // Call
            var properties = new ClosingStructuresOutputProperties(output);

            // Assert
            Assert.IsInstanceOf<StructuresOutputProperties>(properties);
            Assert.AreSame(output, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var random = new Random(39);
            var structuresOutput = new TestStructuresOutput(random.NextDouble());

            // Call
            var properties = new ClosingStructuresOutputProperties(structuresOutput);

            // Assert
            ProbabilityAssessmentOutput expectedProbabilityAssessmentOutput = ClosingStructuresProbabilityAssessmentOutputFactory.Create(
                structuresOutput, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedProbabilityAssessmentOutput.Probability), properties.Probability);
            Assert.AreEqual(expectedProbabilityAssessmentOutput.Reliability, properties.Reliability, properties.Reliability.GetAccuracy());
            mocks.VerifyAll();
        }
    }
}