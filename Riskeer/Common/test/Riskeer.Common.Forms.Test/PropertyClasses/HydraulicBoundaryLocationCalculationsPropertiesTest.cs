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
using Core.Gui.PropertyBag;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsPropertiesTest
    {
        [Test]
        public void Constructor_CalculationsForTargetProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestHydraulicBoundaryLocationCalculationsProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("calculationsForTargetProbability", paramName);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);

            // Call
            using (var properties = new TestHydraulicBoundaryLocationCalculationsProperties(calculationsForTargetProbability))
            {
                // Assert
                Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryLocationCalculationsForTargetProbability>>(properties);
                Assert.IsInstanceOf<IDisposable>(properties);
                Assert.AreSame(calculationsForTargetProbability, properties.Data);
            }
        }

        [Test]
        public void GivenPropertyControlWithData_WhenSingleCalculationUpdated_RefreshRequiredEventRaised()
        {
            // Given
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01)
            {
                HydraulicBoundaryLocationCalculations =
                {
                    hydraulicBoundaryLocationCalculation
                }
            };

            using (var properties = new TestHydraulicBoundaryLocationCalculationsProperties(calculationsForTargetProbability))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                // When
                hydraulicBoundaryLocationCalculation.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshRequiredRaised);
            }
        }

        [Test]
        public void GivenDisposedPropertyControlWithData_WhenSingleCalculationUpdated_RefreshRequiredEventNotRaised()
        {
            // Given
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01)
            {
                HydraulicBoundaryLocationCalculations =
                {
                    hydraulicBoundaryLocationCalculation
                }
            };

            var properties = new TestHydraulicBoundaryLocationCalculationsProperties(calculationsForTargetProbability);
            var refreshRequiredRaised = 0;
            properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

            properties.Dispose();

            // When
            hydraulicBoundaryLocationCalculation.NotifyObservers();

            // Then
            Assert.AreEqual(0, refreshRequiredRaised);
        }

        private class TestHydraulicBoundaryLocationCalculationsProperties : HydraulicBoundaryLocationCalculationsProperties
        {
            public TestHydraulicBoundaryLocationCalculationsProperties(HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability)
                : base(calculationsForTargetProbability) {}
        }
    }
}