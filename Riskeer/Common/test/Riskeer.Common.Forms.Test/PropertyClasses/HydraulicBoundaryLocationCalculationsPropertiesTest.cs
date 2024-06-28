// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsPropertiesTest
    {
        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestHydraulicBoundaryLocationCalculationsProperties(null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocationCalculations", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestHydraulicBoundaryLocationCalculationsProperties(new ObservableList<HydraulicBoundaryLocationCalculation>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            // Call
            using (var properties = new TestHydraulicBoundaryLocationCalculationsProperties(hydraulicBoundaryLocationCalculations, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<ObjectProperties<IObservableEnumerable<HydraulicBoundaryLocationCalculation>>>(properties);
                Assert.IsInstanceOf<IDisposable>(properties);
                Assert.AreSame(hydraulicBoundaryLocationCalculations, properties.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertyControlWithData_WhenSingleCalculationUpdated_ThenRefreshRequiredEventRaised()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                hydraulicBoundaryLocationCalculation
            };

            using (var properties = new TestHydraulicBoundaryLocationCalculationsProperties(hydraulicBoundaryLocationCalculations, assessmentSection))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                // When
                hydraulicBoundaryLocationCalculation.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshRequiredRaised);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenDisposedPropertyControlWithData_WhenSingleCalculationUpdated_ThenRefreshRequiredEventNotRaised()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                hydraulicBoundaryLocationCalculation
            };

            var properties = new TestHydraulicBoundaryLocationCalculationsProperties(hydraulicBoundaryLocationCalculations, assessmentSection);
            var refreshRequiredRaised = 0;
            properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

            properties.Dispose();

            // When
            hydraulicBoundaryLocationCalculation.NotifyObservers();

            // Then
            Assert.AreEqual(0, refreshRequiredRaised);
            mocks.VerifyAll();
        }

        private class TestHydraulicBoundaryLocationCalculationsProperties : HydraulicBoundaryLocationCalculationsProperties
        {
            public TestHydraulicBoundaryLocationCalculationsProperties(IObservableEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations,
                                                                       IAssessmentSection assessmentSection)
                : base(hydraulicBoundaryLocationCalculations, assessmentSection) {}
        }
    }
}