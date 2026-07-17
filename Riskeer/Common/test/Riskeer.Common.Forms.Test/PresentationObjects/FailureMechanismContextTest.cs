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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = Substitute.For<IFailureMechanism>();
            // Call
            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IFailureMechanism>>(context);
            Assert.IsInstanceOf<IFailureMechanismContext<IFailureMechanism>>(context);
            Assert.AreSame(assessmentSection, context.Parent);
            Assert.AreSame(failureMechanism, context.WrappedData);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();

            // Call
            void Call() => new SimpleFailureMechanismContext(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("parent", exception.ParamName);
        }

        private class SimpleFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public SimpleFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) : base(wrappedFailureMechanism, parent) {}
        }
    }
}