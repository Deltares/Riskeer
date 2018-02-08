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
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class ProbabilityFailureMechanismSectionResultContextTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ProbabilityFailureMechanismSectionResultContext<FailureMechanismSectionResult>(
                new ObservableList<FailureMechanismSectionResult>(), new TestFailureMechanism(), null);

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

            IObservableEnumerable<FailureMechanismSectionResult> sectionResults = new ObservableList<FailureMechanismSectionResult>();
            var failureMechanism = new TestFailureMechanism();

            // Call
            var context = new ProbabilityFailureMechanismSectionResultContext<FailureMechanismSectionResult>(sectionResults, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultContext<FailureMechanismSectionResult>>(context);
            Assert.AreSame(sectionResults, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocks.VerifyAll();
        }
    }
}