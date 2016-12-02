// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismContributionContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 1.0/30000);

            // Call
            var context = new FailureMechanismContributionContext(contribution, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<FailureMechanismContribution>>(context);
            Assert.AreSame(contribution, context.WrappedData);
            Assert.AreSame(assessmentSectionMock, context.Parent);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 1.0/30000);

            // Call
            TestDelegate call = () => new FailureMechanismContributionContext(contribution, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("contributionOwner", exception.ParamName);
        }
    }
}