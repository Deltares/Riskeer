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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.Revetment.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveConditionsInputContextPropertiesTest
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 0;
        private const int assessmentLevelPropertyIndex = 1;
        private const int upperBoundaryDesignWaterLevelPropertyIndex = 2;
        private const int upperBoundaryRevetmentPropertyIndex = 3;
        private const int lowerBoundaryRevetmentPropertyIndex = 4;
        private const int upperBoundaryWaterLevelsPropertyIndex = 5;
        private const int lowerBoundaryWaterLevelsPropertyIndex = 6;
        private const int stepSizePropertyIndex = 7;
        private const int waterLevelsPropertyIndex = 8;

        private const int foreshoreProfilePropertyIndex = 9;
        private const int worldReferencePointPropertyIndex = 10;
        private const int orientationPropertyIndex = 11;
        private const int breakWaterPropertyIndex = 12;
        private const int foreshoreGeometryPropertyIndex = 13;
        private const int revetmentTypePropertyIndex = 14;

        private MockRepository mockRepository;
        private ICalculationInputPropertyChangeHandler handler;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            handler = mockRepository.Stub<ICalculationInputPropertyChangeHandler>();
        }

        [Test]
        public void Constructor_WithoutContext_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestWaveConditionsInputContextProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Constructor_WithoutHandler_ThrowsArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            var context = new TestWaveConditionsInputContext(
                new WaveConditionsInput(),
                Enumerable.Empty<ForeshoreProfile>(),
                Enumerable.Empty<HydraulicBoundaryLocation>());

            // Call
            TestDelegate test = () => new TestWaveConditionsInputContextProperties(context, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("handler", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithContext_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentLevel = (RoundedDouble) random.NextDouble();
            var lowerBoundaryRevetment = (RoundedDouble) random.NextDouble();
            var lowerBoundaryWaterLevels = (RoundedDouble) random.NextDouble();
            RoundedDouble upperBoundaryRevetment = lowerBoundaryRevetment + (RoundedDouble) random.NextDouble();
            RoundedDouble upperBoundaryWaterLevels = lowerBoundaryWaterLevels + (RoundedDouble) random.NextDouble();
            var stepSize = WaveConditionsInputStepSize.Half;

            var worldX = (RoundedDouble) random.NextDouble();
            var worldY = (RoundedDouble) random.NextDouble();
            var damHeight = (RoundedDouble) random.NextDouble();
            var foreshoreProfileOrientation = (RoundedDouble) random.NextDouble();

            var foreshoreProfile = new ForeshoreProfile(
                new Point2D(worldX, worldY),
                Enumerable.Empty<Point2D>(),
                new BreakWater(BreakWaterType.Dam, damHeight),
                new ForeshoreProfile.ConstructionProperties
                {
                    Name = string.Empty,
                    Orientation = foreshoreProfileOrientation,
                    X0 = -3
                });
            TestHydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(assessmentLevel);
            var input = new WaveConditionsInput
            {
                ForeshoreProfile = foreshoreProfile,
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = stepSize
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], new HydraulicBoundaryLocation[0]);

            // Call
            var properties = new TestWaveConditionsInputContextProperties(inputContext, handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WaveConditionsInputContext>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.IsInstanceOf<IHasForeshoreProfileProperty>(properties);

            Assert.AreEqual(assessmentLevel - 0.01, properties.UpperBoundaryDesignWaterLevel.Value, properties.UpperBoundaryDesignWaterLevel.GetAccuracy());
            Assert.AreEqual(2, properties.UpperBoundaryDesignWaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(upperBoundaryRevetment, properties.UpperBoundaryRevetment.Value, properties.UpperBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(2, properties.UpperBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.AreEqual(lowerBoundaryRevetment, properties.LowerBoundaryRevetment.Value, properties.LowerBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(2, properties.LowerBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.AreEqual(upperBoundaryWaterLevels, properties.UpperBoundaryWaterLevels.Value, properties.UpperBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(2, properties.UpperBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(lowerBoundaryWaterLevels, properties.LowerBoundaryWaterLevels.Value, properties.LowerBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(2, properties.LowerBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, properties.StepSize.AsValue());
            CollectionAssert.AreEqual(input.WaterLevels, properties.WaterLevels);
            Assert.IsInstanceOf<ConfirmingUseBreakWaterProperties<WaveConditionsInput>>(properties.BreakWater);
            Assert.IsInstanceOf<ConfirmingUseForeshoreProperties<WaveConditionsInput>>(properties.ForeshoreGeometry);
            Assert.AreEqual("Test", properties.RevetmentType);

            Assert.AreSame(hydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.AreEqual(assessmentLevel.Value, properties.AssessmentLevel.Value, properties.AssessmentLevel.GetAccuracy());
            Assert.AreSame(foreshoreProfile, properties.ForeshoreProfile);
            Assert.AreEqual(worldX, properties.WorldReferencePoint.X, 0.5);
            Assert.AreEqual(worldY, properties.WorldReferencePoint.Y, 0.5);
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(foreshoreProfileOrientation, properties.Orientation.Value, properties.Orientation.GetAccuracy());
            Assert.AreEqual(BreakWaterType.Dam, properties.BreakWater.BreakWaterType);
            Assert.AreEqual(damHeight, properties.BreakWater.BreakWaterHeight.Value, properties.BreakWater.BreakWaterHeight.GetAccuracy());
            Assert.IsEmpty(properties.ForeshoreGeometry.Coordinates);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues(
            [Values(true, false)] bool withForeshoreProfile)
        {
            // Setup
            var input = new WaveConditionsInput();
            var foreshoreProfile = new ForeshoreProfile(
                new Point2D(0, 0),
                Enumerable.Empty<Point2D>(),
                null,
                new ForeshoreProfile.ConstructionProperties());

            if (withForeshoreProfile)
            {
                input.ForeshoreProfile = foreshoreProfile;
            }
            var inputContext = new TestWaveConditionsInputContext(input, new[]
            {
                foreshoreProfile
            }, new HydraulicBoundaryLocation[0]);

            // Call
            var properties = new TestWaveConditionsInputContextProperties(inputContext, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(15, dynamicProperties.Count);

            var hydraulicParametersCategory = "Hydraulische gegevens";
            var schematizationCategory = "Schematisatie";

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[hydraulicBoundaryLocationPropertyIndex];
            Assert.IsNotNull(hydraulicBoundaryLocationProperty);
            Assert.IsFalse(hydraulicBoundaryLocationProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, hydraulicBoundaryLocationProperty.Category);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", hydraulicBoundaryLocationProperty.DisplayName);
            Assert.AreEqual("De locatie met hydraulische randvoorwaarden.", hydraulicBoundaryLocationProperty.Description);

            PropertyDescriptor assessmentLevelProperty = dynamicProperties[assessmentLevelPropertyIndex];
            Assert.IsNotNull(assessmentLevelProperty);
            Assert.IsTrue(assessmentLevelProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, assessmentLevelProperty.Category);
            Assert.AreEqual("Toetspeil [m+NAP]", assessmentLevelProperty.DisplayName);
            Assert.AreEqual("Waterstand met een overschrijdingsfrequentie gelijk aan de trajectnorm.", assessmentLevelProperty.Description);

            PropertyDescriptor upperBoundaryDesignWaterLevelProperty = dynamicProperties[upperBoundaryDesignWaterLevelPropertyIndex];
            Assert.IsNotNull(upperBoundaryDesignWaterLevelProperty);
            Assert.IsTrue(upperBoundaryDesignWaterLevelProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, upperBoundaryDesignWaterLevelProperty.Category);
            Assert.AreEqual("Bovengrens op basis van toetspeil [m+NAP]", upperBoundaryDesignWaterLevelProperty.DisplayName);
            Assert.AreEqual("Bovengrens bepaald aan de hand van de waarde van het toetspeil op de geselecteerde hydraulische locatie.", upperBoundaryDesignWaterLevelProperty.Description);

            PropertyDescriptor upperBoundaryRevetmentProperty = dynamicProperties[upperBoundaryRevetmentPropertyIndex];
            Assert.IsNotNull(upperBoundaryRevetmentProperty);
            Assert.IsTrue(upperBoundaryDesignWaterLevelProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, upperBoundaryRevetmentProperty.Category);
            Assert.AreEqual("Bovengrens bekleding [m+NAP]", upperBoundaryRevetmentProperty.DisplayName);
            Assert.AreEqual("Bovengrens van de bekleding.", upperBoundaryRevetmentProperty.Description);

            PropertyDescriptor lowerBoundaryRevetmentProperty = dynamicProperties[lowerBoundaryRevetmentPropertyIndex];
            Assert.IsNotNull(lowerBoundaryRevetmentProperty);
            Assert.IsFalse(lowerBoundaryRevetmentProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, lowerBoundaryRevetmentProperty.Category);
            Assert.AreEqual("Ondergrens bekleding [m+NAP]", lowerBoundaryRevetmentProperty.DisplayName);
            Assert.AreEqual("Ondergrens van de bekleding.", lowerBoundaryRevetmentProperty.Description);

            PropertyDescriptor upperBoundaryWaterLevelsProperty = dynamicProperties[upperBoundaryWaterLevelsPropertyIndex];
            Assert.IsNotNull(upperBoundaryWaterLevelsProperty);
            Assert.IsFalse(upperBoundaryWaterLevelsProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, upperBoundaryWaterLevelsProperty.Category);
            Assert.AreEqual("Bovengrens waterstanden [m+NAP]", upperBoundaryWaterLevelsProperty.DisplayName);
            Assert.AreEqual("Een aangepaste bovengrens voor de waterstanden.", upperBoundaryWaterLevelsProperty.Description);

            PropertyDescriptor lowerBoundaryWaterLevelsProperty = dynamicProperties[lowerBoundaryWaterLevelsPropertyIndex];
            Assert.IsNotNull(lowerBoundaryWaterLevelsProperty);
            Assert.IsFalse(lowerBoundaryWaterLevelsProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, lowerBoundaryWaterLevelsProperty.Category);
            Assert.AreEqual("Ondergrens waterstanden [m+NAP]", lowerBoundaryWaterLevelsProperty.DisplayName);
            Assert.AreEqual("Een aangepaste ondergrens voor de waterstanden.", lowerBoundaryWaterLevelsProperty.Description);

            PropertyDescriptor stepSizeProperty = dynamicProperties[stepSizePropertyIndex];
            Assert.IsNotNull(stepSizeProperty);
            Assert.IsInstanceOf<EnumTypeConverter>(stepSizeProperty.Converter);
            Assert.IsFalse(stepSizeProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, stepSizeProperty.Category);
            Assert.AreEqual("Stapgrootte [m]", stepSizeProperty.DisplayName);
            Assert.AreEqual("Grootte van de stappen waarmee de waterstanden in de berekening worden bepaald.", stepSizeProperty.Description);

            PropertyDescriptor waterLevelsProperty = dynamicProperties[waterLevelsPropertyIndex];
            Assert.IsNotNull(waterLevelsProperty);
            Assert.IsInstanceOf<ExpandableReadOnlyArrayConverter>(waterLevelsProperty.Converter);
            Assert.IsTrue(waterLevelsProperty.IsReadOnly);
            Assert.AreEqual(hydraulicParametersCategory, waterLevelsProperty.Category);
            Assert.AreEqual("Waterstanden in berekening [m+NAP]", waterLevelsProperty.DisplayName);
            Assert.AreEqual("De waterstanden waarvoor gerekend moet worden. Deze zijn afgeleid van de opgegeven boven- en ondergrenzen, en van de stapgrootte.", waterLevelsProperty.Description);

            PropertyDescriptor foreshoreProfileProperty = dynamicProperties[foreshoreProfilePropertyIndex];
            Assert.IsNotNull(foreshoreProfileProperty);
            Assert.IsFalse(foreshoreProfileProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, foreshoreProfileProperty.Category);
            Assert.AreEqual("Voorlandprofiel", foreshoreProfileProperty.DisplayName);
            Assert.AreEqual("De schematisatie van het voorlandprofiel.", foreshoreProfileProperty.Description);

            PropertyDescriptor worldReferencePointProperty = dynamicProperties[worldReferencePointPropertyIndex];
            Assert.IsNotNull(worldReferencePointProperty);
            Assert.IsTrue(worldReferencePointProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, worldReferencePointProperty.Category);
            Assert.AreEqual("Locatie (RD) [m]", worldReferencePointProperty.DisplayName);
            Assert.AreEqual("De coördinaten van de locatie van het voorlandprofiel in het Rijksdriehoeksstelsel.", worldReferencePointProperty.Description);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            Assert.IsNotNull(orientationProperty);
            Assert.IsFalse(orientationProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, orientationProperty.Category);
            Assert.AreEqual("Oriëntatie [°]", orientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de dijknormaal ten opzichte van het noorden.", orientationProperty.Description);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsNotNull(breakWaterProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(breakWaterProperty.Converter);
            Assert.IsTrue(breakWaterProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, breakWaterProperty.Category);
            Assert.AreEqual("Dam", breakWaterProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dam.", breakWaterProperty.Description);

            PropertyDescriptor foreshoreGeometryProperty = dynamicProperties[foreshoreGeometryPropertyIndex];
            Assert.IsNotNull(foreshoreGeometryProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreGeometryProperty.Converter);
            Assert.IsTrue(foreshoreGeometryProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, foreshoreGeometryProperty.Category);
            Assert.AreEqual("Voorlandgeometrie", foreshoreGeometryProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de voorlandgeometrie.", foreshoreGeometryProperty.Description);

            PropertyDescriptor revetmentTypeProperty = dynamicProperties[revetmentTypePropertyIndex];
            Assert.IsNotNull(revetmentTypeProperty);
            Assert.IsTrue(revetmentTypeProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, revetmentTypeProperty.Category);
            Assert.AreEqual("Type bekleding", revetmentTypeProperty.DisplayName);
            Assert.AreEqual("Het type van de bekleding waarvoor berekend wordt.", revetmentTypeProperty.Description);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            var propertiesSelectedHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(
                new TestHydraulicBoundaryLocation(), new Point2D(0, 0));
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.SelectedHydraulicBoundaryLocation = propertiesSelectedHydraulicBoundaryLocation,
                propertiesSelectedHydraulicBoundaryLocation);
        }

        [Test]
        public void ForeshoreProfile_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            var foreshoreProfile = new TestForeshoreProfile();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.ForeshoreProfile = foreshoreProfile,
                foreshoreProfile);
        }

        [Test]
        public void LowerBoundaryRevetment_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            RoundedDouble lowerBoundaryRevetment = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.LowerBoundaryRevetment = lowerBoundaryRevetment,
                lowerBoundaryRevetment);
        }

        [Test]
        public void UpperBoundaryRevetment_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            RoundedDouble upperBoundaryRevetment = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.UpperBoundaryRevetment = upperBoundaryRevetment,
                upperBoundaryRevetment);
        }

        [Test]
        public void LowerBoundaryWaterLevels_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            RoundedDouble lowerBoundaryWaterLevels = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                lowerBoundaryWaterLevels);
        }

        [Test]
        public void UpperBoundaryWaterLevels_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            RoundedDouble upperBoundaryWaterLevels = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                upperBoundaryWaterLevels);
        }

        [Test]
        public void StepSize_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            var waveConditionsInputStepSize = new Random(21).NextEnumValue<WaveConditionsInputStepSize>();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.StepSize = waveConditionsInputStepSize,
                waveConditionsInputStepSize);
        }

        [Test]
        public void Orientation_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            RoundedDouble upperBoundaryDesignWaterLevel = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.Orientation = upperBoundaryDesignWaterLevel,
                upperBoundaryDesignWaterLevel);
        }

        [Test]
        public void UseBreakWater_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            bool breakWaterUseBreakWater = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.BreakWater.UseBreakWater = breakWaterUseBreakWater,
                breakWaterUseBreakWater);
        }

        [Test]
        public void UseForeshore_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput()
        {
            bool foreshoreGeometryUseForeshore = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
                properties => properties.ForeshoreGeometry.UseForeshore = foreshoreGeometryUseForeshore,
                foreshoreGeometryUseForeshore);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputNoLocation_ReturnsNull()
        {
            var input = new WaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], new HydraulicBoundaryLocation[0]);

            var properties = new TestWaveConditionsInputContextProperties(inputContext, handler);

            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = null;

            // Call
            TestDelegate call = () => selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsNull(selectedHydraulicBoundaryLocation);
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsForeshoreProfile_CalculatesDistanceWithCorrectReferencePoint()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            var locations = new List<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };

            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], locations);

            var properties = new TestWaveConditionsInputContextProperties(inputContext, handler);

            // Call 
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            double distanceToPropertiesWorldReferencePoint =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(properties.WorldReferencePoint);
            double distanceToForeshoreProfileReferencePoint =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(input.ForeshoreProfile.WorldReferencePoint);
            Assert.AreEqual(59, distanceToPropertiesWorldReferencePoint, 1);
            Assert.AreEqual(60, distanceToForeshoreProfileReferencePoint, 1);

            SelectableHydraulicBoundaryLocation hydraulicBoundaryLocationItem = availableHydraulicBoundaryLocations.ToArray()[0];
            RoundedDouble itemDistance = hydraulicBoundaryLocationItem.Distance;
            Assert.AreEqual(distanceToForeshoreProfileReferencePoint, itemDistance, itemDistance.GetAccuracy());
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputWithLocationsForeshoreProfile_CalculatesDistanceWithCorrectReferencePoint()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], new HydraulicBoundaryLocation[0]);
            var properties = new TestWaveConditionsInputContextProperties(inputContext, handler);

            // Call 
            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            double distanceToPropertiesWorldReferencePoint =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(properties.WorldReferencePoint);
            double distanceToForeshoreProfileReferencePoint =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(input.ForeshoreProfile.WorldReferencePoint);
            Assert.AreEqual(59, distanceToPropertiesWorldReferencePoint, 1);
            Assert.AreEqual(60, distanceToForeshoreProfileReferencePoint, 1);

            RoundedDouble selectedLocationDistance = selectedHydraulicBoundaryLocation.Distance;
            Assert.AreEqual(distanceToForeshoreProfileReferencePoint, selectedLocationDistance, selectedLocationDistance.GetAccuracy());
        }

        [Test]
        public void GivenPropertiesWithForeshoreProfileAndLocations_WhenSelectingLocation_ThenSelectedLocationDistanceSameAsLocationItem()
        {
            // Given
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            var locations = new List<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };

            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], locations);

            var properties = new TestWaveConditionsInputContextProperties(inputContext, handler);

            // When
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();
            SelectableHydraulicBoundaryLocation selectedLocation = properties.SelectedHydraulicBoundaryLocation;

            // Then
            SelectableHydraulicBoundaryLocation hydraulicBoundaryLocationItem = availableHydraulicBoundaryLocations.ToArray()[0];
            Assert.AreEqual(selectedLocation.Distance, hydraulicBoundaryLocationItem.Distance,
                            hydraulicBoundaryLocationItem.Distance.GetAccuracy());
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsNoReferencePoint_ReturnsLocationsSortedById()
        {
            // Setup
            var locations = new List<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "A", 0, 1),
                new HydraulicBoundaryLocation(4, "C", 0, 2),
                new HydraulicBoundaryLocation(3, "D", 0, 3),
                new HydraulicBoundaryLocation(2, "B", 0, 4)
            };

            var input = new WaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], locations);

            var properties = new TestWaveConditionsInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                locations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, null))
                         .OrderBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsAndReferencePoint_ReturnsLocationsSortedByDistanceThenById()
        {
            // Setup
            var locations = new List<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "A", 0, 10),
                new HydraulicBoundaryLocation(4, "E", 0, 500),
                new HydraulicBoundaryLocation(5, "F", 0, 100),
                new HydraulicBoundaryLocation(6, "D", 0, 200),
                new HydraulicBoundaryLocation(3, "C", 0, 200),
                new HydraulicBoundaryLocation(2, "B", 0, 200)
            };

            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile(string.Empty)
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], locations);

            var properties = new TestWaveConditionsInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                locations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, input.ForeshoreProfile.WorldReferencePoint))
                         .OrderBy(hbl => hbl.Distance)
                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Name);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
        }

        [Test]
        public void GivenLocationAndReferencePoint_WhenUpdatingForeshoreProfile_ThenUpdateSelectableBoundaryLocations()
        {
            // Given
            var locations = new List<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "A", 0, 10),
                new HydraulicBoundaryLocation(3, "E", 0, 500),
                new HydraulicBoundaryLocation(6, "F", 0, 100),
                new HydraulicBoundaryLocation(5, "D", 0, 200),
                new HydraulicBoundaryLocation(4, "C", 0, 200),
                new HydraulicBoundaryLocation(2, "B", 0, 200)
            };

            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile(string.Empty)
            };

            var calculation = new TestCalculation();
            var inputContext = new TestWaveConditionsInputContext(input, calculation, new ForeshoreProfile[0], locations);

            var otherProfile = new TestForeshoreProfile(new Point2D(0, 190));
            var customHandler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester<ForeshoreProfile>(input, calculation, otherProfile, Enumerable.Empty<IObservable>());
            var properties = new TestWaveConditionsInputContextProperties(inputContext, customHandler);

            IEnumerable<SelectableHydraulicBoundaryLocation> originalList = properties.GetSelectableHydraulicBoundaryLocations()
                                                                                      .ToList();

            // When
            properties.ForeshoreProfile = otherProfile;

            // Then
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations().ToList();
            CollectionAssert.AreNotEqual(originalList, availableHydraulicBoundaryLocations);

            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                locations.Select(hbl =>
                                     new SelectableHydraulicBoundaryLocation(hbl,
                                                                             properties.ForeshoreProfile.WorldReferencePoint))
                         .OrderBy(hbl => hbl.Distance)
                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
        }

        [Test]
        public void GetAvailableForeshoreProfiles_InputWithLocations_ReturnsLocations()
        {
            // Setup
            var locations = new List<ForeshoreProfile>
            {
                new TestForeshoreProfile()
            };

            var input = new WaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, locations, new HydraulicBoundaryLocation[0]);

            var properties = new TestWaveConditionsInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<ForeshoreProfile> availableForeshoreProfiles = properties.GetAvailableForeshoreProfiles();

            // Assert
            Assert.AreSame(locations, availableForeshoreProfiles);
        }
        
        private void SetPropertyAndVerifyNotifcationsAndOutputForCalculation<TPropertyValue>(
            Action<TestWaveConditionsInputContextProperties> setProperty,
            TPropertyValue expectedValueSet)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var input = new WaveConditionsInput();
            input.ForeshoreProfile = new TestForeshoreProfile();

            var context = new TestWaveConditionsInputContext(input,
                                                 calculation, 
                                                 new ForeshoreProfile[0], 
                                                 new HydraulicBoundaryLocation[0]);

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester<TPropertyValue>(
                input,
                calculation,
                expectedValueSet,
                new[]
                {
                    observable
                });

            var properties = new TestWaveConditionsInputContextProperties(context, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private class TestWaveConditionsInputContext : WaveConditionsInputContext
        {
            public TestWaveConditionsInputContext(WaveConditionsInput wrappedData,
                                                  IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                                  IEnumerable<HydraulicBoundaryLocation> locations)
                : this(wrappedData, new TestCalculation(), foreshoreProfiles, locations) {}

            public TestWaveConditionsInputContext(WaveConditionsInput wrappedData,
                                                  ICalculation calculation,
                                                  IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                                  IEnumerable<HydraulicBoundaryLocation> locations)
                : base(wrappedData, calculation)
            {
                ForeshoreProfiles = foreshoreProfiles;
                HydraulicBoundaryLocations = locations;
            }

            public override IEnumerable<HydraulicBoundaryLocation> HydraulicBoundaryLocations { get; }

            public override IEnumerable<ForeshoreProfile> ForeshoreProfiles { get; }
        }

        private class TestWaveConditionsInputContextProperties : WaveConditionsInputContextProperties<WaveConditionsInputContext>
        {
            public TestWaveConditionsInputContextProperties(WaveConditionsInputContext context, ICalculationInputPropertyChangeHandler handler) : base(context, handler) {}

            public override string RevetmentType
            {
                get
                {
                    return "Test";
                }
            }
        }
    }
}