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
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Gui.Converters;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveHeightCalculationsForUserDefinedTargetProbabilityPropertiesTest
    {
        private const int targetProbabilityPropertyIndex = 0;
        private const int calculationsPropertyIndex = 1;

        [Test]
        public void Constructor_TargetProbabilityChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new WaveHeightCalculationsForUserDefinedTargetProbabilityProperties(
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("targetProbabilityChangeHandler", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);

            // Call
            var properties = new WaveHeightCalculationsForUserDefinedTargetProbabilityProperties(
                calculationsForTargetProbability, targetProbabilityChangeHandler);

            // Assert
            Assert.IsInstanceOf<WaveHeightCalculationsProperties>(properties);
            Assert.AreSame(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations, properties.Data);
            TestHelper.AssertTypeConverter<WaveHeightCalculationsForUserDefinedTargetProbabilityProperties, ExpandableArrayConverter>(
                nameof(WaveHeightCalculationsForUserDefinedTargetProbabilityProperties.Calculations));
            TestHelper.AssertTypeConverter<WaveHeightCalculationsForUserDefinedTargetProbabilityProperties, NoProbabilityValueDoubleConverter>(
                nameof(WaveHeightCalculationsForUserDefinedTargetProbabilityProperties.TargetProbability));

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            var properties = new WaveHeightCalculationsForUserDefinedTargetProbabilityProperties(
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1), targetProbabilityChangeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor targetProbabilityProperty = dynamicProperties[targetProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetProbabilityProperty,
                                                                            "Algemeen",
                                                                            "Doelkans [1/jaar]",
                                                                            "Overschrijdingskans waarvoor de hydraulische belastingen worden berekend.");

            PropertyDescriptor locationsProperty = dynamicProperties[calculationsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationsProperty,
                                                                            "Algemeen",
                                                                            "Locaties",
                                                                            "Locaties uit de hydraulische belastingendatabase.",
                                                                            true);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var targetProbabilityChangeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1)
            {
                HydraulicBoundaryLocationCalculations =
                {
                    new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                }
            };

            // Call
            var properties = new WaveHeightCalculationsForUserDefinedTargetProbabilityProperties(
                calculationsForTargetProbability, targetProbabilityChangeHandler);

            // Assert
            Assert.AreEqual(calculationsForTargetProbability.TargetProbability, properties.TargetProbability);
            Assert.AreEqual(1, properties.Calculations.Length);

            mocks.VerifyAll();
        }

        [Test]
        public void TargetProbability_Always_InputChangedAndObservablesNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1)
            {
                HydraulicBoundaryLocationCalculations =
                {
                    new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                }
            };

            var properties = new WaveHeightCalculationsForUserDefinedTargetProbabilityProperties(
                calculationsForTargetProbability, customHandler);

            // Call
            properties.TargetProbability = 0.01;

            // Assert
            mocks.VerifyAll();
        }
    }
}