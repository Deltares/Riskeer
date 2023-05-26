﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContextTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext(
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);

            // Call
            var context = new TestHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext(
                calculationsForTargetProbability, assessmentSection);

            // Assert
            Assert.IsInstanceOf<LocationCalculationsContext<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsForTargetProbability>>(context);
            Assert.AreSame(calculationsForTargetProbability, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mockRepository.VerifyAll();
        }

        private class TestHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext
            : HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext
        {
            public TestHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilityContext(
                HydraulicBoundaryLocationCalculationsForTargetProbability wrappedData, IAssessmentSection assessmentSection)
                : base(wrappedData, assessmentSection) {}

            protected override IObservableEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> LocationCalculationsEnumerationToObserve { get; }
                = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>();
        }
    }
}