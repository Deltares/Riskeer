// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects;

namespace Riskeer.DuneErosion.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DuneLocationCalculationsForUserDefinedTargetProbabilityContextTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(0.1),
                                                                                              null,
                                                                                              new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(0.1),
                                                                                              new DuneErosionFailureMechanism(),
                                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            var duneLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(0.1);

            // Call
            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(duneLocationCalculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            // Assert
            Assert.IsInstanceOf<LocationCalculationsContext<DuneLocationCalculationsForTargetProbability, DuneLocationCalculationsForTargetProbability>>(context);
            Assert.AreSame(duneLocationCalculationsForTargetProbability, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
        }

        [Test]
        public void GivenContextWithObserverAttached_WhenNotifyingObserversOfDuneLocationCalculationsForUserDefinedTargetProbabilities_ThenObserverCorrectlyNotified()
        {
            // Given
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(0.1);
            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            context.Attach(observer);

            // When
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

            // Then
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenContextWithObserverAttached_WhenNotifyingObserversOfDuneLocationCalculationsForUserDefinedTargetProbability_ThenObserverCorrectlyNotified()
        {
            // Given
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(0.1);
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(calculationsForTargetProbability);
            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            context.Attach(observer);

            // When
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.First().NotifyObservers();

            // Then
            mockRepository.VerifyAll();
        }
    }
}