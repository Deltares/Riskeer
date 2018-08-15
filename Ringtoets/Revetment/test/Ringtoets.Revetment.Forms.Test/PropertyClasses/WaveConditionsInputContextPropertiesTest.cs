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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.Revetment.Forms.TestUtil;

namespace Ringtoets.Revetment.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveConditionsInputContextPropertiesTest
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 0;
        private const int categoryTypePropertyIndex = 1;
        private const int assessmentLevelPropertyIndex = 2;
        private const int upperBoundaryAssessmentLevelPropertyIndex = 3;
        private const int upperBoundaryRevetmentPropertyIndex = 4;
        private const int lowerBoundaryRevetmentPropertyIndex = 5;
        private const int upperBoundaryWaterLevelsPropertyIndex = 6;
        private const int lowerBoundaryWaterLevelsPropertyIndex = 7;
        private const int stepSizePropertyIndex = 8;
        private const int waterLevelsPropertyIndex = 9;

        private const int foreshoreProfilePropertyIndex = 10;
        private const int worldReferencePointPropertyIndex = 11;
        private const int orientationPropertyIndex = 12;
        private const int breakWaterPropertyIndex = 13;
        private const int foreshoreGeometryPropertyIndex = 14;
        private const int revetmentTypePropertyIndex = 15;

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new TestWaveConditionsInputContextProperties(null,
                                                                                   AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                                   handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GetAssessmentLevelFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var context = new TestWaveConditionsInputContext(
                new TestWaveConditionsInput(),
                Enumerable.Empty<ForeshoreProfile>(),
                assessmentSection);

            // Call
            TestDelegate test = () => new TestWaveConditionsInputContextProperties(context, null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("getAssessmentLevelFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PropertyChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var context = new TestWaveConditionsInputContext(
                new TestWaveConditionsInput(),
                Enumerable.Empty<ForeshoreProfile>(),
                new AssessmentSectionStub());

            // Call
            TestDelegate test = () => new TestWaveConditionsInputContextProperties(context,
                                                                                   AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("propertyChangeHandler", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var random = new Random(21);
            RoundedDouble assessmentLevel = random.NextRoundedDouble();
            RoundedDouble lowerBoundaryRevetment = random.NextRoundedDouble();
            RoundedDouble lowerBoundaryWaterLevels = random.NextRoundedDouble();
            RoundedDouble upperBoundaryRevetment = lowerBoundaryRevetment + random.NextRoundedDouble();
            RoundedDouble upperBoundaryWaterLevels = lowerBoundaryWaterLevels + random.NextRoundedDouble();
            const WaveConditionsInputStepSize stepSize = WaveConditionsInputStepSize.Half;

            RoundedDouble worldX = random.NextRoundedDouble();
            RoundedDouble worldY = random.NextRoundedDouble();
            RoundedDouble damHeight = random.NextRoundedDouble();
            RoundedDouble foreshoreProfileOrientation = random.NextRoundedDouble();

            var foreshoreProfile = new ForeshoreProfile(
                new Point2D(worldX, worldY),
                Enumerable.Empty<Point2D>(),
                new BreakWater(BreakWaterType.Dam, damHeight),
                new ForeshoreProfile.ConstructionProperties
                {
                    Id = "id",
                    Name = string.Empty,
                    Orientation = foreshoreProfileOrientation,
                    X0 = -3
                });
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var input = new TestWaveConditionsInput
            {
                ForeshoreProfile = foreshoreProfile,
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = stepSize
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            // Call
            var properties = new TestWaveConditionsInputContextProperties(inputContext, () => assessmentLevel, handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<TestWaveConditionsInputContext>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.IsInstanceOf<IHasForeshoreProfileProperty>(properties);

            Assert.AreEqual(assessmentLevel - 0.01, properties.UpperBoundaryAssessmentLevel.Value, properties.UpperBoundaryAssessmentLevel.GetAccuracy());
            Assert.AreEqual(2, properties.UpperBoundaryAssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(upperBoundaryRevetment, properties.UpperBoundaryRevetment.Value, properties.UpperBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(2, properties.UpperBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.AreEqual(lowerBoundaryRevetment, properties.LowerBoundaryRevetment.Value, properties.LowerBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(2, properties.LowerBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.AreEqual(upperBoundaryWaterLevels, properties.UpperBoundaryWaterLevels.Value, properties.UpperBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(2, properties.UpperBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(lowerBoundaryWaterLevels, properties.LowerBoundaryWaterLevels.Value, properties.LowerBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(2, properties.LowerBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, properties.StepSize.AsValue());
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.BreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.ForeshoreGeometry);
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
            CollectionAssert.IsEmpty(properties.ForeshoreGeometry.Coordinates);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues(
            [Values(true, false)] bool withForeshoreProfile)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var input = new TestWaveConditionsInput();
            var foreshoreProfile = new TestForeshoreProfile();

            if (withForeshoreProfile)
            {
                input.ForeshoreProfile = foreshoreProfile;
            }

            var inputContext = new TestWaveConditionsInputContext(input, new[]
            {
                foreshoreProfile
            }, assessmentSection);

            // Call
            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(16, dynamicProperties.Count);

            const string hydraulicParametersCategory = "Hydraulische gegevens";
            const string schematizationCategory = "Schematisatie";

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[hydraulicBoundaryLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hydraulicBoundaryLocationProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Locatie met hydraulische randvoorwaarden",
                                                                            "De locatie met hydraulische randvoorwaarden.");

            PropertyDescriptor categoryTypeProperty = dynamicProperties[categoryTypePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(categoryTypeProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Categoriegrens",
                                                                            "Categoriegrens (kans) waarvoor de berekening moet worden uitgevoerd.");

            PropertyDescriptor assessmentLevelProperty = dynamicProperties[assessmentLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(assessmentLevelProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Waterstand bij categoriegrens [m+NAP]",
                                                                            "Waterstand bij de geselecteerde categoriegrens.",
                                                                            true);

            PropertyDescriptor upperBoundaryAssessmentLevelProperty = dynamicProperties[upperBoundaryAssessmentLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upperBoundaryAssessmentLevelProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Bovengrens op basis van waterstand bij categoriegrens [m+NAP]",
                                                                            "Bovengrens bepaald aan de hand van de waterstand bij categoriegrens voor de geselecteerde hydraulische locatie.",
                                                                            true);

            PropertyDescriptor upperBoundaryRevetmentProperty = dynamicProperties[upperBoundaryRevetmentPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upperBoundaryRevetmentProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Bovengrens bekleding [m+NAP]",
                                                                            "Bovengrens van de bekleding.");

            PropertyDescriptor lowerBoundaryRevetmentProperty = dynamicProperties[lowerBoundaryRevetmentPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lowerBoundaryRevetmentProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Ondergrens bekleding [m+NAP]",
                                                                            "Ondergrens van de bekleding.");

            PropertyDescriptor upperBoundaryWaterLevelsProperty = dynamicProperties[upperBoundaryWaterLevelsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upperBoundaryWaterLevelsProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Bovengrens waterstanden [m+NAP]",
                                                                            "Een aangepaste bovengrens voor de waterstanden.");

            PropertyDescriptor lowerBoundaryWaterLevelsProperty = dynamicProperties[lowerBoundaryWaterLevelsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lowerBoundaryWaterLevelsProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Ondergrens waterstanden [m+NAP]",
                                                                            "Een aangepaste ondergrens voor de waterstanden.");

            PropertyDescriptor stepSizeProperty = dynamicProperties[stepSizePropertyIndex];
            Assert.IsInstanceOf<EnumTypeConverter>(stepSizeProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(stepSizeProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Stapgrootte [m]",
                                                                            "Grootte van de stappen waarmee de waterstanden in de berekening worden bepaald.");

            PropertyDescriptor waterLevelsProperty = dynamicProperties[waterLevelsPropertyIndex];
            Assert.IsInstanceOf<ExpandableReadOnlyArrayConverter>(waterLevelsProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waterLevelsProperty,
                                                                            hydraulicParametersCategory,
                                                                            "Waterstanden in berekening [m+NAP]",
                                                                            "De waterstanden waarvoor gerekend moet worden. Deze zijn afgeleid van de opgegeven boven- en ondergrenzen, en van de stapgrootte.",
                                                                            true);

            PropertyDescriptor foreshoreProfileProperty = dynamicProperties[foreshoreProfilePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(foreshoreProfileProperty,
                                                                            schematizationCategory,
                                                                            "Voorlandprofiel",
                                                                            "De schematisatie van het voorlandprofiel.");

            PropertyDescriptor worldReferencePointProperty = dynamicProperties[worldReferencePointPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(worldReferencePointProperty,
                                                                            schematizationCategory,
                                                                            "Locatie (RD) [m]",
                                                                            "De coördinaten van de locatie van het voorlandprofiel in het Rijksdriehoeksstelsel.",
                                                                            true);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(orientationProperty,
                                                                            schematizationCategory,
                                                                            "Oriëntatie [°]",
                                                                            "Oriëntatie van de dijknormaal ten opzichte van het noorden.");

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(breakWaterProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterProperty,
                                                                            schematizationCategory,
                                                                            "Dam",
                                                                            "Eigenschappen van de dam.",
                                                                            true);

            PropertyDescriptor foreshoreGeometryProperty = dynamicProperties[foreshoreGeometryPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreGeometryProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(foreshoreGeometryProperty,
                                                                            schematizationCategory,
                                                                            "Voorlandgeometrie",
                                                                            "Eigenschappen van de voorlandgeometrie.",
                                                                            true);

            PropertyDescriptor revetmentTypeProperty = dynamicProperties[revetmentTypePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(revetmentTypeProperty,
                                                                            schematizationCategory,
                                                                            "Type bekleding",
                                                                            "Het type van de bekleding waarvoor berekend wordt.",
                                                                            true);

            mocks.VerifyAll();
        }

        [Test]
        public void WaterLevels_WithValidData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var assessmentLevel = (RoundedDouble) 5.99;
            var lowerBoundaryRevetment = (RoundedDouble) 3.58;
            var lowerBoundaryWaterLevels = (RoundedDouble) 3.40;
            var upperBoundaryRevetment = (RoundedDouble) 6.10;
            var upperBoundaryWaterLevels = (RoundedDouble) 5.88;
            const WaveConditionsInputStepSize stepSize = WaveConditionsInputStepSize.Half;

            var input = new TestWaveConditionsInput
            {
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = stepSize
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            // Call
            var properties = new TestWaveConditionsInputContextProperties(inputContext, () => assessmentLevel, handler);

            // Assert
            Assert.IsNotEmpty(properties.WaterLevels);
            CollectionAssert.AreEqual(input.GetWaterLevels(assessmentLevel), properties.WaterLevels);
            mocks.VerifyAll();
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_Always_InputChangedAndObservablesNotified()
        {
            var propertiesSelectedHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(
                new TestHydraulicBoundaryLocation(), new Point2D(0, 0));
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.SelectedHydraulicBoundaryLocation = propertiesSelectedHydraulicBoundaryLocation);
        }

        [Test]
        public void CategoryType_Always_InputChangedAndObservablesNotified()
        {
            var categoryType = new Random(21).NextEnumValue<TestCategoryType>();

            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.CategoryType = categoryType);
        }

        [Test]
        public void ForeshoreProfile_Always_InputChangedAndObservablesNotified()
        {
            var foreshoreProfile = new TestForeshoreProfile();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.ForeshoreProfile = foreshoreProfile);
        }

        [Test]
        public void LowerBoundaryRevetment_Always_InputChangedAndObservablesNotified()
        {
            RoundedDouble lowerBoundaryRevetment = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.LowerBoundaryRevetment = lowerBoundaryRevetment);
        }

        [Test]
        public void UpperBoundaryRevetment_Always_InputChangedAndObservablesNotified()
        {
            RoundedDouble upperBoundaryRevetment = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.UpperBoundaryRevetment = upperBoundaryRevetment);
        }

        [Test]
        public void LowerBoundaryWaterLevels_Always_InputChangedAndObservablesNotified()
        {
            RoundedDouble lowerBoundaryWaterLevels = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.LowerBoundaryWaterLevels = lowerBoundaryWaterLevels);
        }

        [Test]
        public void UpperBoundaryWaterLevels_Always_InputChangedAndObservablesNotified()
        {
            RoundedDouble upperBoundaryWaterLevels = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.UpperBoundaryWaterLevels = upperBoundaryWaterLevels);
        }

        [Test]
        public void StepSize_Always_InputChangedAndObservablesNotified()
        {
            var waveConditionsInputStepSize = new Random(21).NextEnumValue<WaveConditionsInputStepSize>();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.StepSize = waveConditionsInputStepSize);
        }

        [Test]
        public void Orientation_Always_InputChangedAndObservablesNotified()
        {
            RoundedDouble orientation = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.Orientation = orientation);
        }

        [Test]
        public void UseBreakWater_Always_InputChangedAndObservablesNotified()
        {
            bool breakWaterUseBreakWater = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.BreakWater.UseBreakWater = breakWaterUseBreakWater);
        }

        [Test]
        public void UseForeshore_Always_InputChangedAndObservablesNotified()
        {
            bool foreshoreGeometryUseForeshore = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.ForeshoreGeometry.UseForeshore = foreshoreGeometryUseForeshore);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputNoLocation_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var input = new TestWaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          handler);

            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = null;

            // Call
            TestDelegate call = () => selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsNull(selectedHydraulicBoundaryLocation);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsForeshoreProfile_CalculatesDistanceWithCorrectReferencePoint()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var input = new TestWaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          handler);

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
            mocks.VerifyAll();
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputWithLocationsForeshoreProfile_CalculatesDistanceWithCorrectReferencePoint()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(new Point2D(200620.173572981, 503401.652985217))
            };

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);
            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          handler);

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
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithForeshoreProfileAndLocations_WhenSelectingLocation_ThenSelectedLocationDistanceSameAsLocationItem()
        {
            // Given
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);

            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(new Point2D(200620.173572981, 503401.652985217))
            };

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          handler);

            // When
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();
            SelectableHydraulicBoundaryLocation selectedLocation = properties.SelectedHydraulicBoundaryLocation;

            // Then
            SelectableHydraulicBoundaryLocation hydraulicBoundaryLocationItem = availableHydraulicBoundaryLocations.ToArray()[0];
            Assert.AreEqual(selectedLocation.Distance, hydraulicBoundaryLocationItem.Distance,
                            hydraulicBoundaryLocationItem.Distance.GetAccuracy());
            mocks.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsNoReferencePoint_ReturnsLocationsSortedById()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var locations = new List<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "A", 0, 1),
                new HydraulicBoundaryLocation(4, "C", 0, 2),
                new HydraulicBoundaryLocation(3, "D", 0, 3),
                new HydraulicBoundaryLocation(2, "B", 0, 4)
            };

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(locations);

            var input = new TestWaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          handler);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                locations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, null))
                         .OrderBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsAndReferencePoint_ReturnsLocationsSortedByDistanceThenById()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var locations = new List<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "A", 0, 10),
                new HydraulicBoundaryLocation(4, "E", 0, 500),
                new HydraulicBoundaryLocation(5, "F", 0, 100),
                new HydraulicBoundaryLocation(6, "D", 0, 200),
                new HydraulicBoundaryLocation(3, "C", 0, 200),
                new HydraulicBoundaryLocation(2, "B", 0, 200)
            };

            var input = new TestWaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile()
            };

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(locations);

            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          handler);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                locations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, input.ForeshoreProfile.WorldReferencePoint))
                         .OrderBy(hbl => hbl.Distance)
                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Name);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mocks.VerifyAll();
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

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(locations);

            var input = new TestWaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile()
            };

            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(input);
            var inputContext = new TestWaveConditionsInputContext(input, calculation, assessmentSection, new ForeshoreProfile[0]);

            var otherProfile = new TestForeshoreProfile(new Point2D(0, 190));
            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          customHandler);

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
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var foreshoreProfiles = new List<ForeshoreProfile>
            {
                new TestForeshoreProfile()
            };

            var input = new TestWaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, foreshoreProfiles, assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          handler);

            // Call
            IEnumerable<ForeshoreProfile> availableForeshoreProfiles = properties.GetAvailableForeshoreProfiles();

            // Assert
            Assert.AreSame(foreshoreProfiles, availableForeshoreProfiles);
            mocks.VerifyAll();
        }

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(Action<TestWaveConditionsInputContextProperties> setProperty)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var input = new TestWaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile()
            };
            var calculation = new TestWaveConditionsCalculation<TestWaveConditionsInput>(input);

            var context = new TestWaveConditionsInputContext(input,
                                                             calculation,
                                                             assessmentSection,
                                                             new ForeshoreProfile[0]);

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new TestWaveConditionsInputContextProperties(context,
                                                                          AssessmentSectionHelper.GetTestAssessmentLevel,
                                                                          customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(customHandler.Called);
            mocks.VerifyAll();
        }

        private class TestWaveConditionsInputContextProperties : WaveConditionsInputContextProperties<TestWaveConditionsInputContext, TestWaveConditionsInput, TestCategoryType>
        {
            public TestWaveConditionsInputContextProperties(TestWaveConditionsInputContext context,
                                                            Func<RoundedDouble> getAssessmentLevelFunc,
                                                            IObservablePropertyChangeHandler handler)
                : base(context, getAssessmentLevelFunc, handler) {}

            public override string RevetmentType
            {
                get
                {
                    return "Test";
                }
            }

            protected override TestCategoryType GetCategoryType()
            {
                return 0;
            }

            protected override void SetCategoryType(TestCategoryType categoryType) {}
        }

        private enum TestCategoryType
        {
            Type1,
            Type2
        }
    }
}