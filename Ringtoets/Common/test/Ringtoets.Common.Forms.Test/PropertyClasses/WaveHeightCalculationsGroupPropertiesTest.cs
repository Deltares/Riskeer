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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveHeightCalculationsGroupPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            IEnumerable<HydraulicBoundaryLocation> locations = Enumerable.Empty<HydraulicBoundaryLocation>();

            // Call
            var properties = new WaveHeightCalculationsGroupProperties(locations,
                                                                       Enumerable.Empty<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>>());

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationsGroupBaseProperties>(properties);
            Assert.AreSame(locations, properties.Data);

            TestHelper.AssertTypeConverter<WaveHeightCalculationsGroupProperties, ExpandableArrayConverter>(
                nameof(WaveHeightCalculationsGroupProperties.Locations));
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new WaveHeightCalculationsGroupProperties(Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                       Enumerable.Empty<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor locationsProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationsProperty,
                                                                            "Algemeen",
                                                                            "Locaties",
                                                                            "Locaties uit de hydraulische randvoorwaardendatabase.",
                                                                            true);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new WaveHeightCalculationsGroupProperties(new[]
                                                                       {
                                                                           location
                                                                       },
                                                                       new[]
                                                                       {
                                                                           new Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>("A", new[]
                                                                           {
                                                                               new HydraulicBoundaryLocationCalculation(location)
                                                                           })
                                                                       });

            // Assert
            WaveHeightHydraulicBoundaryLocationProperties locationProperties = properties.Locations.Single();
            Assert.AreSame(location, locationProperties.Data);
            Assert.AreEqual(1, locationProperties.CategoryBoundaries.Length);
        }
    }
}