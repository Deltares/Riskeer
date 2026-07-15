// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismSectionResultContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = Substitute.For<IFailureMechanism>();
            var sectionResults = Substitute.For<IObservableEnumerable<FailureMechanismSectionResult>>();
            // Call
            var context = new FailureMechanismSectionResultContext<FailureMechanismSectionResult>(sectionResults, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<IObservableEnumerable<FailureMechanismSectionResult>>>(context);
            Assert.AreSame(sectionResults, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var sectionResults = Substitute.For<IObservableEnumerable<FailureMechanismSectionResult>>();

            // Call
            void Call() => new FailureMechanismSectionResultContext<FailureMechanismSectionResult>(sectionResults, null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResults = Substitute.For<IObservableEnumerable<FailureMechanismSectionResult>>();
            var failureMechanism = Substitute.For<IFailureMechanism>();

            // Call
            void Call() => new FailureMechanismSectionResultContext<FailureMechanismSectionResult>(sectionResults, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
    }
}