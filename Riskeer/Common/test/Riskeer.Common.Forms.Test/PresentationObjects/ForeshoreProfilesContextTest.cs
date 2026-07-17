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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class ForeshoreProfilesContextTest
    {
        [Test]
        public void Constructor_ValidValues_ExpectedValues()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var foreshoresList = new ForeshoreProfileCollection();

            // Call
            var context = new ForeshoreProfilesContext(foreshoresList, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<ForeshoreProfileCollection>>(context);
            Assert.AreSame(foreshoresList, context.WrappedData);
            Assert.AreSame(failureMechanism, context.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, context.ParentAssessmentSection);
        }

        [Test]
        public void Constructor_FailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            // Call
            void Call() => new ForeshoreProfilesContext(new ForeshoreProfileCollection(), null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("parentFailureMechanism", paramName);
        }

        [Test]
        public void Constructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();

            // Call
            void Call() => new ForeshoreProfilesContext(new ForeshoreProfileCollection(), failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("parentAssessmentSection", paramName);
        }
    }
}