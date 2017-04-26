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
using System.Globalization;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.PropertyClasses;

namespace Ringtoets.DuneErosion.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DuneLocationsContextPropertiesTest
    {
        private const int requiredLocationsPropertyIndex = 0;

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var location = new TestDuneLocation();
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocations.Add(location);
            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            // Call
            var properties = new DuneLocationsContextProperties(context.WrappedData);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(properties.Locations, typeof(DuneLocationContextProperties));
            Assert.AreEqual(1, properties.Locations.Length);
            TestHelper.AssertTypeConverter<DuneLocationsContextProperties, ExpandableArrayConverter>(
                              nameof(DuneLocationsContextProperties.Locations));

            DuneLocationContextProperties duneLocationContextProperties = properties.Locations.First();
            Assert.AreEqual(location.Id, duneLocationContextProperties.Id);
            Assert.AreEqual(location.Name, duneLocationContextProperties.Name);
            Assert.AreEqual(location.CoastalAreaId, duneLocationContextProperties.CoastalAreaId);
            Assert.AreEqual(location.Offset.ToString("0.#", CultureInfo.InvariantCulture), duneLocationContextProperties.Offset);
            Assert.AreEqual(location.Location, duneLocationContextProperties.Location);

            Assert.IsNaN(duneLocationContextProperties.WaterLevel);
            Assert.IsNaN(duneLocationContextProperties.WaveHeight);
            Assert.IsNaN(duneLocationContextProperties.WavePeriod);
            Assert.AreEqual(location.D50, duneLocationContextProperties.D50);

            Assert.IsNaN(duneLocationContextProperties.TargetProbability);
            Assert.IsNaN(duneLocationContextProperties.TargetReliability);
            Assert.IsNaN(duneLocationContextProperties.CalculatedProbability);
            Assert.IsNaN(duneLocationContextProperties.CalculatedReliability);

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(CalculationConvergence.NotCalculated).DisplayName;
            Assert.AreEqual(convergenceValue, duneLocationContextProperties.Convergence);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocations.Add(new TestDuneLocation());
            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            // Call
            var properties = new DuneLocationsContextProperties(context.WrappedData);

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
    }
}