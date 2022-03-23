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

using System;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PresentationObjects;

namespace Riskeer.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class SpecificFailurePathContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePath = new SpecificFailureMechanism();

            // Call
            var context = new SpecificFailurePathContext(failurePath, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<SpecificFailureMechanism>>(context);
            Assert.IsInstanceOf<IFailureMechanismContext<SpecificFailureMechanism>>(context);
            Assert.AreSame(assessmentSection, context.Parent);
            Assert.AreSame(failurePath, context.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failurePath = new SpecificFailureMechanism();

            // Call
            void Call() => new SpecificFailurePathContext(failurePath, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("parent", exception.ParamName);
        }
    }
}