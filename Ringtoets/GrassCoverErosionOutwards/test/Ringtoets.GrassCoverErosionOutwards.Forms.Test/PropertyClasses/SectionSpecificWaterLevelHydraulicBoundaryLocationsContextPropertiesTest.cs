// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class SectionSpecificWaterLevelHydraulicBoundaryLocationsContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new SectionSpecificWaterLevelHydraulicBoundaryLocationsContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            RoundedDouble sectionSpecificWaterLevel = (RoundedDouble) 12.34;
            var location = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "name", 1.0, 2.0))
            {
                SectionSpecificWaterLevel = sectionSpecificWaterLevel
            };

            // Call
            SectionSpecificWaterLevelHydraulicBoundaryLocationsContextProperties properties = new SectionSpecificWaterLevelHydraulicBoundaryLocationsContextProperties
            {
                Data = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
                {
                    location
                }
            };

            // Assert
            Assert.AreEqual(1, properties.Locations.Length);

            SectionSpecificWaterLevelHydraulicBoundaryLocationContextProperties locationProperties = properties.Locations[0];
            Assert.AreEqual(location.HydraulicBoundaryLocation.Name, locationProperties.Name);
            Assert.AreEqual(location.HydraulicBoundaryLocation.Id, locationProperties.Id);
            Assert.AreEqual(location.HydraulicBoundaryLocation.Location, locationProperties.Location);
            Assert.AreEqual(sectionSpecificWaterLevel, locationProperties.SectionSpecificWaterLevel, location.SectionSpecificWaterLevel.GetAccuracy());
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var properties = new SectionSpecificWaterLevelHydraulicBoundaryLocationsContextProperties();

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            const string expectedLocationsDisplayName = "Locaties";
            const string expectedLocationsDescription = "Locaties uit de hydraulische randvoorwaardendatabase.";
            const string expectedLocationsCategory = "Algemeen";

            // Call
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);

            // Assert
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
    }
}