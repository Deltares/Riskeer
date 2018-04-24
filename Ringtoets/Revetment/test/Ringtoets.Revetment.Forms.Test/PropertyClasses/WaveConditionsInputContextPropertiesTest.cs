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
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.Revetment.Forms.TestUtil;

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

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new TestWaveConditionsInputContextProperties(null,
                                                                                   AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
                                                                                   handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GetNormativeAssessmentLevelFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            var context = new TestWaveConditionsInputContext(
                new WaveConditionsInput(),
                Enumerable.Empty<ForeshoreProfile>(),
                assessmentSection);

            // Call
            TestDelegate test = () => new TestWaveConditionsInputContextProperties(context, null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("getNormativeAssessmentLevelFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PropertyChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var context = new TestWaveConditionsInputContext(
                new WaveConditionsInput(),
                Enumerable.Empty<ForeshoreProfile>(),
                new AssessmentSectionStub());

            // Call
            TestDelegate test = () => new TestWaveConditionsInputContextProperties(context,
                                                                                   AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
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
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            // Call
            var properties = new TestWaveConditionsInputContextProperties(inputContext, () => assessmentLevel, handler);

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

            var input = new WaveConditionsInput();
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
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
                                                                          handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(15, dynamicProperties.Count);

            const string hydraulicParametersCategory = "Hydraulische gegevens";
            const string schematizationCategory = "Schematisatie";

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

            var input = new WaveConditionsInput
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
            RoundedDouble upperBoundaryDesignWaterLevel = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(
                properties => properties.Orientation = upperBoundaryDesignWaterLevel);
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

            var input = new WaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
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

            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
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
            var input = new WaveConditionsInput
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
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
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

            var input = new WaveConditionsInput
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
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
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

            var input = new WaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
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

            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile()
            };

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(locations);

            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
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

            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile()
            };

            var calculation = new TestWaveConditionsCalculation();
            var inputContext = new TestWaveConditionsInputContext(input, calculation, assessmentSection, new ForeshoreProfile[0]);

            var otherProfile = new TestForeshoreProfile(new Point2D(0, 190));
            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
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

            var input = new WaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, foreshoreProfiles, assessmentSection);

            var properties = new TestWaveConditionsInputContextProperties(inputContext,
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
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

            var calculation = new TestWaveConditionsCalculation();
            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile()
            };

            var context = new TestWaveConditionsInputContext(input,
                                                             calculation,
                                                             assessmentSection,
                                                             new ForeshoreProfile[0]);

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new TestWaveConditionsInputContextProperties(context,
                                                                          AssessmentSectionHelper.GetTestNormativeAssessmentLevel,
                                                                          customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(customHandler.Called);
            mocks.VerifyAll();
        }

        private class TestWaveConditionsInputContextProperties : WaveConditionsInputContextProperties<WaveConditionsInputContext>
        {
            public TestWaveConditionsInputContextProperties(WaveConditionsInputContext context,
                                                            Func<RoundedDouble> getNormativeAssessmentLevelFunc,
                                                            IObservablePropertyChangeHandler handler)
                : base(context, getNormativeAssessmentLevelFunc, handler) {}

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