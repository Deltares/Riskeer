﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

        [Test]
        public void Constructor_WithoutContext_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestWaveConditionsInputContextProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Constructor_WithContext_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentLevel = (RoundedDouble)random.NextDouble();
            var lowerBoundaryRevetment = (RoundedDouble)random.NextDouble();
            var lowerBoundaryWaterLevels = (RoundedDouble)random.NextDouble();
            var upperBoundaryRevetment = lowerBoundaryRevetment + (RoundedDouble)random.NextDouble();
            var upperBoundaryWaterLevels = lowerBoundaryWaterLevels + (RoundedDouble)random.NextDouble();
            var stepSize = WaveConditionsInputStepSize.Half;

            var worldX = (RoundedDouble)random.NextDouble();
            var worldY = (RoundedDouble)random.NextDouble();
            var damHeight = (RoundedDouble)random.NextDouble();
            var foreshoreProfileOrientation = (RoundedDouble)random.NextDouble();

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
            var hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(assessmentLevel);
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
            var properties = new TestWaveConditionsInputContextProperties(inputContext);

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
            var properties = new TestWaveConditionsInputContextProperties(inputContext);

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
                                                                                              {
                                                                                                  BrowsableAttribute.Yes
                                                                                              });
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
        [TestCase(true)]
        [TestCase(false)]
        public void SelectedHydraulicBoundaryLocation_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.SelectedHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(
                new TestHydraulicBoundaryLocation(), new Point2D(0, 0)));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ForeshoreProfile_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.ForeshoreProfile = new TestForeshoreProfile());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void LowerBoundaryRevetment_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.LowerBoundaryRevetment = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UpperBoundaryRevetment_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.UpperBoundaryRevetment = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void LowerBoundaryWaterLevels_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.LowerBoundaryWaterLevels = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UpperBoundaryWaterLevels_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.UpperBoundaryWaterLevels = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void StepSize_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.StepSize = new Random(21).NextEnumValue<WaveConditionsInputStepSize>());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UpperBoundaryDesignWaterLevel_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.Orientation = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UseBreakWater_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(
                hasOutput,
                properties => properties.BreakWater.UseBreakWater = new Random(21).NextBoolean());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UseForeshore_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(
                hasOutput,
                properties => properties.ForeshoreGeometry.UseForeshore = true);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputNoLocation_ReturnsNull()
        {
            var input = new WaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], new HydraulicBoundaryLocation[0]);

            var properties = new TestWaveConditionsInputContextProperties(inputContext);

            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = null;

            // Call
            TestDelegate call = () => selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsNull(selectedHydraulicBoundaryLocation);
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsNoReferencePoint_ReturnsLocationsSortedById()
        {
            // Setup
            var locations = new List<HydraulicBoundaryLocation>()
            {
                new HydraulicBoundaryLocation(1, "A", 0, 1),
                new HydraulicBoundaryLocation(4, "C", 0, 2),
                new HydraulicBoundaryLocation(3, "D", 0, 3),
                new HydraulicBoundaryLocation(2, "B", 0, 4)
            };

            var input = new WaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], locations);

            var properties = new TestWaveConditionsInputContextProperties(inputContext);

            // Call
            var availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

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
            var locations = new List<HydraulicBoundaryLocation>()
            {
                new HydraulicBoundaryLocation(1, "A", 0, 10),
                new HydraulicBoundaryLocation(4, "E", 0, 500),
                new HydraulicBoundaryLocation(5, "F", 0, 100),
                new HydraulicBoundaryLocation(6, "D", 0, 200),
                new HydraulicBoundaryLocation(3, "C", 0, 200),
                new HydraulicBoundaryLocation(2, "B", 0, 200)
            };

            var input = new WaveConditionsInput()
            {
                ForeshoreProfile = new TestForeshoreProfile(string.Empty)
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], locations);

            var properties = new TestWaveConditionsInputContextProperties(inputContext);

            // Call
            var availableHydraulicBoundaryLocations = properties.GetSelectableHydraulicBoundaryLocations();

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
            var locations = new List<HydraulicBoundaryLocation>()
            {
                new HydraulicBoundaryLocation(1, "A", 0, 10),
                new HydraulicBoundaryLocation(3, "E", 0, 500),
                new HydraulicBoundaryLocation(6, "F", 0, 100),
                new HydraulicBoundaryLocation(5, "D", 0, 200),
                new HydraulicBoundaryLocation(4, "C", 0, 200),
                new HydraulicBoundaryLocation(2, "B", 0, 200)
            };

            var input = new WaveConditionsInput()
            {
                ForeshoreProfile = new TestForeshoreProfile(string.Empty)
            };
            var inputContext = new TestWaveConditionsInputContext(input, new ForeshoreProfile[0], locations);

            var properties = new TestWaveConditionsInputContextProperties(inputContext);

            IEnumerable<SelectableHydraulicBoundaryLocation> originalList = properties.GetSelectableHydraulicBoundaryLocations()
                                                                                      .ToList();

            // When
            properties.ForeshoreProfile = new TestForeshoreProfile(new Point2D(0, 190));

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
            var locations = new List<ForeshoreProfile>()
            {
                new TestForeshoreProfile()
            };

            var input = new WaveConditionsInput();
            var inputContext = new TestWaveConditionsInputContext(input, locations, new HydraulicBoundaryLocation[0]);

            var properties = new TestWaveConditionsInputContextProperties(inputContext);

            // Call
            var availableForeshoreProfiles = properties.GetAvailableForeshoreProfiles();

            // Assert
            Assert.AreSame(locations, availableForeshoreProfiles);
        }

        private void SetPropertyAndVerifyNotifcationsAndOutput(bool hasOutput, Action<TestWaveConditionsInputContextProperties> setProperty)
        {
            // Setup
            var mocks = new MockRepository();

            var calculationObserver = mocks.StrictMock<IObserver>();
            int numberOfChangedProperties = hasOutput ? 1 : 0;
            calculationObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);

            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var calculation = new TestCalculation();

            if (hasOutput)
            {
                calculation.Output = new object();
            }
            calculation.Attach(calculationObserver);

            var input = new WaveConditionsInput();
            input.ForeshoreProfile = new TestForeshoreProfile();
            input.Attach(inputObserver);

            var properties = new TestWaveConditionsInputContextProperties(
                new TestWaveConditionsInputContext(input, calculation, new ForeshoreProfile[0], new HydraulicBoundaryLocation[0]));

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

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
            public TestWaveConditionsInputContextProperties(WaveConditionsInputContext context) : base(context) {}

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