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
using System.ComponentModel;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.PropertyClasses;

namespace Riskeer.DuneErosion.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DuneLocationCalculationsForUserDefinedTargetProbabilityPropertiesTest
    {
        private const int calculationsPropertyIndex = 0;

        [Test]
        public void Constructor_CalculationsForTargetProbabilityNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new DuneLocationCalculationsForUserDefinedTargetProbabilityProperties(null, targetProbabilityChangeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbability", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_TargetProbabilityChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationCalculationsForUserDefinedTargetProbabilityProperties(
                new DuneLocationCalculationsForTargetProbability(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("targetProbabilityChangeHandler", paramName);
        }

        [Test]
        public void Constructor_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculation = new DuneLocationCalculation(new TestDuneLocation());
            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability
            {
                DuneLocationCalculations =
                {
                    calculation
                }
            };

            // Call
            using (var properties = new DuneLocationCalculationsForUserDefinedTargetProbabilityProperties(calculationsForTargetProbability, targetProbabilityChangeHandler))
            {
                // Assert
                Assert.IsInstanceOf<ObjectProperties<DuneLocationCalculationsForTargetProbability>>(properties);
                Assert.IsInstanceOf<IDisposable>(properties);

                Assert.AreSame(calculationsForTargetProbability, properties.Data);

                Assert.AreEqual(1, properties.Calculations.Length);
                Assert.AreSame(calculation, properties.Calculations[0].Data);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            using (var properties = new DuneLocationCalculationsForUserDefinedTargetProbabilityProperties(
                new DuneLocationCalculationsForTargetProbability(), targetProbabilityChangeHandler))
            {
                // Assert
                PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
                Assert.AreEqual(1, dynamicProperties.Count);

                PropertyDescriptor locationsProperty = dynamicProperties[calculationsPropertyIndex];
                Assert.IsInstanceOf<ExpandableArrayConverter>(locationsProperty.Converter);
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationsProperty,
                                                                                "Algemeen",
                                                                                "Locaties",
                                                                                "Locaties uit de hydraulische belastingendatabase.",
                                                                                true);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenPropertyControlWithData_WhenSingleCalculationUpdated_RefreshRequiredEventRaised()
        {
            // Given
            var mocks = new MockRepository();
            var targetProbabilityChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculation = new DuneLocationCalculation(new TestDuneLocation());
            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability
            {
                DuneLocationCalculations =
                {
                    calculation
                }
            };

            using (var properties = new DuneLocationCalculationsForUserDefinedTargetProbabilityProperties(calculationsForTargetProbability, targetProbabilityChangeHandler))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                // When
                calculation.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshRequiredRaised);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenDisposedPropertyControlWithData_WhenSingleCalculationUpdated_RefreshRequiredEventNotRaised()
        {
            // Given
            var mocks = new MockRepository();
            var targetProbabilityChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculation = new DuneLocationCalculation(new TestDuneLocation());
            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability
            {
                DuneLocationCalculations =
                {
                    calculation
                }
            };

            using (var properties = new DuneLocationCalculationsForUserDefinedTargetProbabilityProperties(calculationsForTargetProbability, targetProbabilityChangeHandler))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                properties.Dispose();

                // When
                calculation.NotifyObservers();

                // Then
                Assert.AreEqual(0, refreshRequiredRaised);

                mocks.VerifyAll();
            }
        }
    }
}