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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightLocationsPropertiesTest
    {
        [Test]
        public void Constructor_GetCalculationFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsWaveHeightLocationsProperties(hydraulicBoundaryLocations, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getCalculationFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_WithLocations_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>();

            // Call
            var properties = new GrassCoverErosionOutwardsWaveHeightLocationsProperties(hydraulicBoundaryLocations,
                                                                                        hbl => hbl.DesignWaterLevelCalculation1);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationsProperties>(properties);
            Assert.AreSame(hydraulicBoundaryLocations, properties.Data);
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            const string expectedLocationsDisplayName = "Locaties";
            const string expectedLocationsDescription = "Locaties uit de hydraulische randvoorwaardendatabase.";
            const string expectedLocationsCategory = "Algemeen";
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);

            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor locationsProperty = dynamicProperties.Find("Locations", false);

            Assert.IsInstanceOf<TypeConverter>(classTypeConverter);
            Assert.IsNotNull(locationsProperty);
            Assert.IsInstanceOf<ExpandableArrayConverter>(locationsProperty.Converter);
            Assert.IsTrue(locationsProperty.IsReadOnly);
            Assert.IsTrue(locationsProperty.IsBrowsable);
            Assert.AreEqual(expectedLocationsDisplayName, locationsProperty.DisplayName);
            Assert.AreEqual(expectedLocationsDescription, locationsProperty.Description);
            Assert.AreEqual(expectedLocationsCategory, locationsProperty.Category);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            HydraulicBoundaryLocation location = TestHydraulicBoundaryLocation.CreateWaveHeightCalculated(1.2);
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>
            {
                location
            };

            // Call
            var properties = new GrassCoverErosionOutwardsWaveHeightLocationsProperties(hydraulicBoundaryLocations,
                                                                                        hbl => hbl.WaveHeightCalculation1);

            // Assert
            Assert.AreEqual(1, properties.Locations.Length);
            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsWaveHeightLocationsProperties, ExpandableArrayConverter>(
                nameof(GrassCoverErosionOutwardsWaveHeightLocationsProperties.Locations));
            GrassCoverErosionOutwardsWaveHeightLocationProperties locationProperties = properties.Locations.First();
            Assert.AreEqual(location.Name, locationProperties.Name);
            Assert.AreEqual(location.Id, locationProperties.Id);
            Assert.AreEqual(location.Location, locationProperties.Location);

            RoundedDouble locationWaveHeight = location.WaveHeightCalculation1.Output.Result;
            Assert.AreEqual(locationWaveHeight, locationProperties.WaveHeight, locationWaveHeight.GetAccuracy());

            Assert.AreEqual("Ja", locationProperties.Convergence);
        }
    }
}