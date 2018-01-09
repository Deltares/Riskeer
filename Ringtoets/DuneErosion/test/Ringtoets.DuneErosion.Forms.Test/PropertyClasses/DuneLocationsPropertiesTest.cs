// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PropertyClasses;

namespace Ringtoets.DuneErosion.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DuneLocationsPropertiesTest
    {
        private const int requiredLocationsPropertyIndex = 0;

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var location = new TestDuneLocation();
            var locations = new ObservableList<DuneLocation>
            {
                location
            };

            // Call
            var properties = new DuneLocationsProperties(locations);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(properties.Locations, typeof(DuneLocationProperties));
            Assert.AreEqual(1, properties.Locations.Length);
            TestHelper.AssertTypeConverter<DuneLocationsProperties, ExpandableArrayConverter>(
                nameof(DuneLocationsProperties.Locations));

            DuneLocationProperties duneLocationProperties = properties.Locations.First();
            Assert.AreEqual(location.Id, duneLocationProperties.Id);
            Assert.AreEqual(location.Name, duneLocationProperties.Name);
            Assert.AreEqual(location.CoastalAreaId, duneLocationProperties.CoastalAreaId);
            Assert.AreEqual(location.Offset.ToString("0.#", CultureInfo.InvariantCulture), duneLocationProperties.Offset);
            Assert.AreEqual(location.Location, duneLocationProperties.Location);

            Assert.IsNaN(duneLocationProperties.WaterLevel);
            Assert.IsNaN(duneLocationProperties.WaveHeight);
            Assert.IsNaN(duneLocationProperties.WavePeriod);
            Assert.AreEqual(location.D50, duneLocationProperties.D50);

            Assert.IsNaN(duneLocationProperties.TargetProbability);
            Assert.IsNaN(duneLocationProperties.TargetReliability);
            Assert.IsNaN(duneLocationProperties.CalculatedProbability);
            Assert.IsNaN(duneLocationProperties.CalculatedReliability);

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(CalculationConvergence.NotCalculated).DisplayName;
            Assert.AreEqual(convergenceValue, duneLocationProperties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var location = new TestDuneLocation();
            var locations = new ObservableList<DuneLocation>
            {
                location
            };

            // Call
            var properties = new DuneLocationsProperties(locations);

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
        public void GivenPropertyControlWithData_WhenSingleLocationUpdated_RefreshRequiredEventRaised()
        {
            // Given
            DuneLocation location = new TestDuneLocation();
            var duneLocations = new ObservableList<DuneLocation>
            {
                location
            };

            var properties = new DuneLocationsProperties(duneLocations);

            var refreshRequiredRaised = 0;
            properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

            // When
            location.NotifyObservers();

            // Then
            Assert.AreEqual(1, refreshRequiredRaised);
        }

        [Test]
        public void GivenDisposedPropertyControlWithData_WhenSingleLocationUpdated_RefreshRequiredEventNotRaised()
        {
            // Given
            DuneLocation location = new TestDuneLocation();
            var duneLocations = new ObservableList<DuneLocation>
            {
                location
            };

            var properties = new DuneLocationsProperties(duneLocations);

            var refreshRequiredRaised = 0;
            properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

            properties.Dispose();

            // When
            location.NotifyObservers();

            // Then
            Assert.AreEqual(0, refreshRequiredRaised);
        }

        [Test]
        public void GivenPropertyControlWithNewData_WhenSingleLocationUpdatedInPreviouslySetData_RefreshRequiredEventNotRaised()
        {
            // Given
            DuneLocation location1 = new TestDuneLocation();
            DuneLocation location2 = new TestDuneLocation();
            var duneLocations1 = new ObservableList<DuneLocation>
            {
                location1
            };
            var duneLocations2 = new ObservableList<DuneLocation>
            {
                location2
            };

            var properties = new DuneLocationsProperties(duneLocations1)
            {
                Data = duneLocations2
            };

            var refreshRequiredRaised = 0;
            properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

            // When
            location1.NotifyObservers();

            // Then
            Assert.AreEqual(0, refreshRequiredRaised);
        }

        [Test]
        public void GivenPropertyControlWithNewData_WhenSingleLocationUpdatedInNewlySetData_RefreshRequiredEventRaised()
        {
            // Given
            DuneLocation location1 = new TestDuneLocation();
            DuneLocation location2 = new TestDuneLocation();
            var duneLocations1 = new ObservableList<DuneLocation>
            {
                location1
            };
            var duneLocations2 = new ObservableList<DuneLocation>
            {
                location2
            };

            var properties = new DuneLocationsProperties(duneLocations1)
            {
                Data = duneLocations2
            };

            var refreshRequiredRaised = 0;
            properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

            // When
            location2.NotifyObservers();

            // Then
            Assert.AreEqual(1, refreshRequiredRaised);
        }
    }
}