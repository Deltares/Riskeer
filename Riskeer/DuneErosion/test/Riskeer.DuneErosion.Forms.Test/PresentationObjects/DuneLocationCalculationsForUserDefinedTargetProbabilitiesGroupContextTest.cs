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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects;

namespace Riskeer.DuneErosion.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculations = new ObservableList<DuneLocationCalculationsForTargetProbability>();
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                calculations, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableList<DuneLocationCalculationsForTargetProbability>>>(context);
            Assert.AreSame(calculations, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                new ObservableList<DuneLocationCalculationsForTargetProbability>(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                new ObservableList<DuneLocationCalculationsForTargetProbability>(), new DuneErosionFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
    }
}