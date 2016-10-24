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

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.ClosingStructures.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class ClosingStructuresInputContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new ClosingStructuresInput();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            var context = new ClosingStructuresInputContext(input, failureMechanism, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<FailureMechanismItemContextBase<ClosingStructuresInput, ClosingStructuresFailureMechanism>>(context);
            Assert.AreEqual(input, context.WrappedData);
            Assert.AreEqual(failureMechanism, context.FailureMechanism);
            Assert.AreEqual(assessmentSectionMock, context.AssessmentSection);
            mocks.VerifyAll();
        }
    }
}