﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismSectionsContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            IEnumerable<FailureMechanismSection> sectionsSequence = new[]
            {
                new FailureMechanismSection("A", new[]
                {
                    new Point2D(1, 2),
                    new Point2D(3, 4)
                }),
                new FailureMechanismSection("B", new[]
                {
                    new Point2D(3, 4),
                    new Point2D(5, 6)
                })
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism<FailureMechanismSectionResult>>();
            failureMechanism.Stub(fm => fm.Sections).Return(sectionsSequence);
            mocks.ReplayAll();

            // Call
            var context = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IFailureMechanism<FailureMechanismSectionResult>>>(context);
            Assert.AreSame(failureMechanism, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism<FailureMechanismSectionResult>>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismSectionsContext(failureMechanism, null);

            // Assert
            Assert.Throws<ArgumentNullException>(Call);
            mocks.VerifyAll();
        }
    }
}