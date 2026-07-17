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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;

namespace Riskeer.StabilityStoneCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityStoneCoverContextTest
    {
        [Test]
        public void Constructor_WrappedDataNull_ThrowArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate call = () => new SimpleStabilityStoneCoverContext(null, failureMechanism, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wrappedData", paramName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var observable = Substitute.For<IObservable>();

            // Call
            TestDelegate call = () => new SimpleStabilityStoneCoverContext(observable, null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var observable = Substitute.For<IObservable>();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate call = () => new SimpleStabilityStoneCoverContext(observable, failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var observable = Substitute.For<IObservable>();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            var context = new SimpleStabilityStoneCoverContext(observable, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IObservable>>(context);
            Assert.AreSame(observable, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, context.ForeshoreProfiles);
        }

        private class SimpleStabilityStoneCoverContext : StabilityStoneCoverContext<IObservable>
        {
            public SimpleStabilityStoneCoverContext(IObservable wrappedData,
                                                    StabilityStoneCoverFailureMechanism failureMechanism,
                                                    IAssessmentSection assessmentSection) :
                base(wrappedData, failureMechanism, assessmentSection) {}
        }
    }
}