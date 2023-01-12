﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class WaveHeightCalculationsForUserDefinedTargetProbabilityContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);

            // Call
            var context = new WaveHeightCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                           assessmentSection);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext>(context);
            Assert.AreSame(calculationsForTargetProbability, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenContextWithObserverAttached_WhenNotifyingObserversOfWaveHeightCalculationsForUserDefinedTargetProbabilities_ThenObserverCorrectlyNotified()
        {
            // Given
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
            var context = new WaveHeightCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                           assessmentSection);

            context.Attach(observer);

            // When
            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

            // Then
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenContextWithObserverAttached_WhenNotifyingObserversOfWaveHeightCalculationsForUserDefinedTargetProbability_ThenObserverCorrectlyNotified()
        {
            // Given
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
            var context = new WaveHeightCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                           assessmentSection);

            context.Attach(observer);

            // When
            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.First().NotifyObservers();

            // Then
            mockRepository.VerifyAll();
        }
    }
}