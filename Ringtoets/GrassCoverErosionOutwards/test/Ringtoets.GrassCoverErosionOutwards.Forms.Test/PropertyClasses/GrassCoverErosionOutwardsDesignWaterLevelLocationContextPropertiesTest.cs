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

using System;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationContextPropertiesTest
    {
        private const int idPropertyIndex = 0;
        private const int namePropertyIndex = 1;
        private const int coordinatesPropertyIndex = 2;
        private const int designWaterLevelPropertyIndex = 3;
        private const int targetProbabilityPropertyIndex = 4;
        private const int targetReliabilityPropertyIndex = 5;
        private const int calculatedProbabilityPropertyIndex = 6;
        private const int calculatedReliabilityPropertyIndex = 7;
        private const int convergencePropertyIndex = 8;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties();

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties
            {
                Data = new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(new ObservableList<HydraulicBoundaryLocation>
                {
                    hydraulicBoundaryLocation
                }, hydraulicBoundaryLocation)
            };

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocation.Id, properties.Id);
            Assert.AreEqual(hydraulicBoundaryLocation.Name, properties.Name);
            Assert.AreEqual(hydraulicBoundaryLocation.Location, properties.Location);
            Assert.IsNaN(properties.DesignWaterLevel);
            Assert.IsTrue(TypeUtils.HasTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties,
                              NoValueRoundedDoubleConverter>(p => p.DesignWaterLevel));
            Assert.AreEqual(double.NaN, properties.TargetProbability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties,
                              FailureMechanismSectionResultNoProbabilityValueDoubleConverter>(
                                  p => p.TargetProbability));
            Assert.IsNaN(properties.TargetReliability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties,
                              NoValueRoundedDoubleConverter>(p => p.TargetReliability));
            Assert.AreEqual(double.NaN, properties.CalculatedProbability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties,
                              FailureMechanismSectionResultNoProbabilityValueDoubleConverter>(
                                  p => p.CalculatedProbability));
            Assert.IsNaN(properties.CalculatedReliability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties,
                              NoValueRoundedDoubleConverter>(p => p.CalculatedReliability));
            Assert.AreEqual(string.Empty, properties.Convergence);
        }

        [Test]
        [TestCase(CalculationConvergence.CalculatedConverged, "Ja")]
        [TestCase(CalculationConvergence.CalculatedNotConverged, "Nee")]
        [TestCase(CalculationConvergence.NotCalculated, "")]
        public void GetProperties_ValidData_ReturnsExpectedValues(CalculationConvergence convergenceReached, string expectedConvergenceValue)
        {
            // Setup
            var random = new Random();
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";

            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            double designWaterLevel = random.NextDouble();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y)
            {
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(designWaterLevel, targetProbability,
                                                                             targetReliability,
                                                                             calculatedProbability,
                                                                             calculatedReliability,
                                                                             convergenceReached)
            };
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties
            {
                Data = context
            };

            // Assert
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            Point2D coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, properties.Location);
            Assert.AreEqual(designWaterLevel, properties.DesignWaterLevel, properties.DesignWaterLevel.GetAccuracy());

            Assert.AreEqual(targetProbability, properties.TargetProbability);
            Assert.AreEqual(targetReliability, properties.TargetReliability, properties.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, properties.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, properties.CalculatedReliability, properties.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(expectedConvergenceValue, properties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties
            {
                Data = new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(new ObservableList<HydraulicBoundaryLocation>
                {
                    hydraulicBoundaryLocation
                }, hydraulicBoundaryLocation)
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            var propertyBag = new DynamicPropertyBag(properties);

            PropertyDescriptorCollection dynamicProperties = propertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            Assert.AreEqual(9, dynamicProperties.Count);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            PropertyDescriptor idProperty = dynamicProperties[idPropertyIndex];
            Assert.IsNotNull(idProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            "Algemeen",
                                                                            "ID",
                                                                            "ID van de hydraulische randvoorwaardenlocatie in de database.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            Assert.IsNotNull(nameProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Naam",
                                                                            "Naam van de hydraulische randvoorwaardenlocatie.",
                                                                            true);

            PropertyDescriptor coordinatesProperty = dynamicProperties[coordinatesPropertyIndex];
            Assert.IsNotNull(coordinatesProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coordinatesProperty,
                                                                            "Algemeen",
                                                                            "Coördinaten [m]",
                                                                            "Coördinaten van de hydraulische randvoorwaardenlocatie.",
                                                                            true);

            PropertyDescriptor designWaterLevelProperty = dynamicProperties[designWaterLevelPropertyIndex];
            Assert.IsNotNull(designWaterLevelProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(designWaterLevelProperty,
                                                                            "Resultaat",
                                                                            "Waterstand bij doorsnede-eis [m+NAP]",
                                                                            "Berekende waterstand bij doorsnede-eis.",
                                                                            true);

            PropertyDescriptor targetProbabilityProperty = dynamicProperties[targetProbabilityPropertyIndex];
            Assert.IsNotNull(targetProbabilityProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetProbabilityProperty,
                                                                            "Resultaat",
                                                                            "Doelkans [1/jaar]",
                                                                            "De ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor targetReliabilityProperty = dynamicProperties[targetReliabilityPropertyIndex];
            Assert.IsNotNull(targetReliabilityProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetReliabilityProperty,
                                                                            "Resultaat",
                                                                            "Betrouwbaarheidsindex doelkans [-]",
                                                                            "Betrouwbaarheidsindex van de ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor calculatedProbabilityProperty = dynamicProperties[calculatedProbabilityPropertyIndex];
            Assert.IsNotNull(calculatedProbabilityProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedProbabilityProperty,
                                                                            "Resultaat",
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor calculatedReliabilityProperty = dynamicProperties[calculatedReliabilityPropertyIndex];
            Assert.IsNotNull(calculatedReliabilityProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedReliabilityProperty,
                                                                            "Resultaat",
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor convergenceProperty = dynamicProperties[convergencePropertyIndex];
            Assert.IsNotNull(convergenceProperty);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(convergenceProperty,
                                                                            "Resultaat",
                                                                            "Convergentie",
                                                                            "Is convergentie bereikt in de berekening van de waterstand bij doorsnede-eis?",
                                                                            true);
        }
    }
}