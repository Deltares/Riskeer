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

using System;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationsPropertiesTest
    {
        [Test]
        public void Constructor_HydraulicBoundaryLocations_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationsProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocations", paramName);
        }

        [Test]
        public void GivenPropertyControlWithData_WhenSingleLocationUpdated_RefreshRequiredEventRaised()
        {
            // Given
            HydraulicBoundaryLocation location = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.5);
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>
            {
                location
            };

            var properties = new TestHydraulicBoundaryLocationsProperties(hydraulicBoundaryLocations);

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
            HydraulicBoundaryLocation location = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.5);
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>
            {
                location
            };

            var properties = new TestHydraulicBoundaryLocationsProperties(hydraulicBoundaryLocations);

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
            HydraulicBoundaryLocation location1 = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.5);
            HydraulicBoundaryLocation location2 = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.5);
            var hydraulicBoundaryLocations1 = new ObservableList<HydraulicBoundaryLocation>
            {
                location1
            };
            var hydraulicBoundaryLocations2 = new ObservableList<HydraulicBoundaryLocation>
            {
                location2
            };

            var properties = new TestHydraulicBoundaryLocationsProperties(hydraulicBoundaryLocations1)
            {
                Data = hydraulicBoundaryLocations2
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
            HydraulicBoundaryLocation location1 = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.5);
            HydraulicBoundaryLocation location2 = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.5);
            var hydraulicBoundaryLocations1 = new ObservableList<HydraulicBoundaryLocation>
            {
                location1
            };
            var hydraulicBoundaryLocations2 = new ObservableList<HydraulicBoundaryLocation>
            {
                location2
            };

            var properties = new TestHydraulicBoundaryLocationsProperties(hydraulicBoundaryLocations1)
            {
                Data = hydraulicBoundaryLocations2
            };

            var refreshRequiredRaised = 0;
            properties.RefreshRequired += (sender, args) => refreshRequiredRaised++;

            // When
            location2.NotifyObservers();

            // Then
            Assert.AreEqual(1, refreshRequiredRaised);
        }

        private class TestHydraulicBoundaryLocationsProperties : HydraulicBoundaryLocationsProperties
        {
            public TestHydraulicBoundaryLocationsProperties(ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
                : base(hydraulicBoundaryLocations) {}
        }
    }
}