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

using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveHeightLocationsPropertiesTest
    {
        [Test]
        public void Constructor_WithHydraulicBoundaryLocationCalculations_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            // Call
            var properties = new WaveHeightLocationsProperties(hydraulicBoundaryLocationCalculations);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationsProperties>(properties);
            Assert.AreSame(hydraulicBoundaryLocationCalculations, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                Output = new TestHydraulicBoundaryLocationOutput(1.5)
            };

            // Call
            var properties = new WaveHeightLocationsProperties(new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                hydraulicBoundaryLocationCalculation
            });

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(properties.Locations, typeof(WaveHeightLocationProperties));
            Assert.AreEqual(1, properties.Locations.Length);
            TestHelper.AssertTypeConverter<WaveHeightLocationsProperties,
                ExpandableArrayConverter>(nameof(WaveHeightLocationsProperties.Locations));

            WaveHeightLocationProperties waveHeightLocationProperties = properties.Locations.First();
            Assert.AreEqual(hydraulicBoundaryLocation.Name, waveHeightLocationProperties.Name);
            Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightLocationProperties.Id);
            Assert.AreEqual(hydraulicBoundaryLocation.Location, waveHeightLocationProperties.Location);

            RoundedDouble waveHeight = hydraulicBoundaryLocationCalculation.Output.Result;
            Assert.AreEqual(waveHeight, waveHeightLocationProperties.WaveHeight, waveHeight.GetAccuracy());
        }
    }
}