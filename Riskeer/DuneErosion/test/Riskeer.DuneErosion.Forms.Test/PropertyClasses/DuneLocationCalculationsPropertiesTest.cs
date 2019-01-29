// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PropertyClasses;

namespace Riskeer.DuneErosion.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DuneLocationCalculationsPropertiesTest
    {
        private const int requiredLocationsPropertyIndex = 0;

        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DuneLocationCalculationsProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_WithData_ReturnExpectedValues()
        {
            // Setup
            var calculation = new DuneLocationCalculation(new TestDuneLocation());
            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                calculation
            };

            // Call
            using (var properties = new DuneLocationCalculationsProperties(duneLocationCalculations))
            {
                // Assert
                Assert.IsInstanceOf<ObjectProperties<IObservableEnumerable<DuneLocationCalculation>>>(properties);
                Assert.IsInstanceOf<IDisposable>(properties);

                Assert.AreSame(duneLocationCalculations, properties.Data);

                Assert.AreEqual(1, properties.Calculations.Length);
                Assert.AreSame(calculation, properties.Calculations[0].Data);
            }
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var calculation = new DuneLocationCalculation(new TestDuneLocation());
            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                calculation
            };

            // Call
            using (var properties = new DuneLocationCalculationsProperties(duneLocationCalculations))
            {
                // Assert
                PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
                Assert.AreEqual(1, dynamicProperties.Count);

                PropertyDescriptor locationsProperty = dynamicProperties[requiredLocationsPropertyIndex];
                Assert.IsInstanceOf<ExpandableArrayConverter>(locationsProperty.Converter);
                PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationsProperty,
                                                                                "Algemeen",
                                                                                "Locaties",
                                                                                "Locaties uit de hydraulische belastingendatabase.",
                                                                                true);
            }
        }

        [Test]
        public void GivenPropertyControlWithData_WhenSingleCalculationUpdated_RefreshRequiredEventRaised()
        {
            // Given
            var calculation = new DuneLocationCalculation(new TestDuneLocation());
            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                calculation
            };

            using (var properties = new DuneLocationCalculationsProperties(duneLocationCalculations))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                // When
                calculation.NotifyObservers();

                // Then
                Assert.AreEqual(1, refreshRequiredRaised);
            }
        }

        [Test]
        public void GivenDisposedPropertyControlWithData_WhenSingleCalculationUpdated_RefreshRequiredEventNotRaised()
        {
            // Given
            var calculation = new DuneLocationCalculation(new TestDuneLocation());
            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                calculation
            };

            using (var properties = new DuneLocationCalculationsProperties(duneLocationCalculations))
            {
                var refreshRequiredRaised = 0;
                properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

                properties.Dispose();

                // When
                calculation.NotifyObservers();

                // Then
                Assert.AreEqual(0, refreshRequiredRaised);
            }
        }
    }
}