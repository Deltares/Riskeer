﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationsPropertiesTest
    {
        private const int requiredLocationsPropertyIndex = 0;

        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationsProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocationCalculations", paramName);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new TestHydraulicBoundaryLocationsProperties(new ObservableList<HydraulicBoundaryLocationCalculation>());

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<TypeConverter>(classTypeConverter);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor locationsProperty = dynamicProperties[requiredLocationsPropertyIndex];
            Assert.IsInstanceOf<ExpandableArrayConverter>(locationsProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationsProperty,
                                                                            "Algemeen",
                                                                            "Locaties",
                                                                            "Locaties uit de hydraulische randvoorwaardendatabase.",
                                                                            true);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            // Call
            using (var properties = new TestHydraulicBoundaryLocationsProperties(hydraulicBoundaryLocationCalculations))
            {
                // Assert
                Assert.IsInstanceOf<ObjectProperties<ObservableList<HydraulicBoundaryLocationCalculation>>>(properties);
                Assert.IsInstanceOf<IDisposable>(properties);
                Assert.AreSame(hydraulicBoundaryLocationCalculations, properties.Data);
            }
        }

        [Test]
        public void GivenPropertyControlWithData_WhenSingleCalculationUpdated_RefreshRequiredEventRaised()
        {
            // Given
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                hydraulicBoundaryLocationCalculation
            };

            using (var properties = new TestHydraulicBoundaryLocationsProperties(hydraulicBoundaryLocationCalculations))
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
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                hydraulicBoundaryLocationCalculation
            };

            using (var properties = new TestHydraulicBoundaryLocationsProperties(hydraulicBoundaryLocationCalculations))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                properties.Dispose();

                // When
                hydraulicBoundaryLocationCalculation.NotifyObservers();

                // Then
                Assert.AreEqual(0, refreshRequiredRaised);
            }
        }

        private class TestHydraulicBoundaryLocationsProperties : HydraulicBoundaryLocationsProperties
        {
            public TestHydraulicBoundaryLocationsProperties(ObservableList<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations)
                : base(hydraulicBoundaryLocationCalculations) {}
        }
    }
}