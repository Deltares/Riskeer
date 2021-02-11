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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;

namespace Riskeer.Piping.Forms.Test.PresentationObjects.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingOutputContextTest
    {
        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new ProbabilisticPipingOutputContext(null, new PipingFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("wrappedData", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new ProbabilisticPipingOutputContext(new ProbabilisticPipingCalculationScenario(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticPipingOutputContext(new ProbabilisticPipingCalculationScenario(), new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();

            // Call
            var context = new ProbabilisticPipingOutputContext(calculation, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ProbabilisticPipingCalculationScenario>>(context);
            Assert.AreSame(calculation, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);

            mocks.VerifyAll();
        }
    }
}