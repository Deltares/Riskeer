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

using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects.RegistrationState;

namespace Riskeer.DuneErosion.Forms.Test.PresentationObjects.RegistrationState
{
    [TestFixture]
    public class DuneErosionFailureMechanismContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            var context = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismContext<DuneErosionFailureMechanism>>(context);
            Assert.AreSame(failureMechanism, context.WrappedData);
            Assert.AreSame(assessmentSection, context.Parent);
            mocks.VerifyAll();
        }
    }
}