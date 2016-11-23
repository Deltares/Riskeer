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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsInputContextPropertiesTest
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
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new StabilityStoneCoverWaveConditionsInputContextProperties();

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContextProperties<StabilityStoneCoverWaveConditionsInputContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetDefaultInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new WaveConditionsInput();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var inputContext = new StabilityStoneCoverWaveConditionsInputContext(input,
                                                                                 failureMechanism.ForeshoreProfiles,
                                                                                 assessmentSection);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            Assert.IsNaN(properties.AssessmentLevel.Value);
            Assert.IsNaN(properties.UpperBoundaryDesignWaterLevel.Value);
            Assert.AreEqual(2, properties.UpperBoundaryDesignWaterLevel.NumberOfDecimalPlaces);
            Assert.IsNaN(properties.UpperBoundaryRevetment.Value);
            Assert.AreEqual(2, properties.UpperBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.IsNaN(properties.LowerBoundaryRevetment.Value);
            Assert.AreEqual(2, properties.LowerBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.IsNaN(properties.UpperBoundaryWaterLevels.Value);
            Assert.AreEqual(2, properties.UpperBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.IsNaN(properties.LowerBoundaryWaterLevels.Value);
            Assert.AreEqual(2, properties.LowerBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, properties.StepSize.AsValue());
            CollectionAssert.AreEqual(input.WaterLevels, properties.WaterLevels);

            Assert.IsNull(properties.ForeshoreProfile);
            Assert.IsNull(properties.WorldReferencePoint);
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.IsNaN(properties.Orientation);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.BreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.ForeshoreGeometry);
            Assert.AreEqual("Steen (blokken en zuilen)", properties.RevetmentType);
            mocks.VerifyAll();
        }

        [Test]
        public void Data_SetNewInputContextInstanceWithForeshoreProfile_ReturnCorrectPropertyValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(21);
            var assessmentLevel = (RoundedDouble) random.NextDouble();
            var lowerBoundaryRevetment = (RoundedDouble) random.NextDouble();
            var lowerBoundaryWaterLevels = (RoundedDouble) random.NextDouble();
            var upperBoundaryRevetment = lowerBoundaryRevetment + (RoundedDouble) random.NextDouble();
            var upperBoundaryWaterLevels = lowerBoundaryWaterLevels + (RoundedDouble) random.NextDouble();
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
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            {
                DesignWaterLevel = assessmentLevel
            };
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var input = new WaveConditionsInput()
            {
                ForeshoreProfile = foreshoreProfile,
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = stepSize
            };

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.ForeshoreProfiles.Add(foreshoreProfile);

            var inputContext = new StabilityStoneCoverWaveConditionsInputContext(input,
                                                                                 failureMechanism.ForeshoreProfiles,
                                                                                 assessmentSection);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsInputContextProperties
            {
                Data = inputContext
            };

            // Assert
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
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            const int numberOfChangedProperties = 8;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(21);
            var orientation = (RoundedDouble) random.NextDouble();
            var assessmentLevel = (RoundedDouble) random.NextDouble();
            var newLowerBoundaryRevetment = (RoundedDouble) random.NextDouble();
            var newLowerBoundaryWaterLevels = (RoundedDouble) random.NextDouble();
            var newUpperBoundaryRevetment = newLowerBoundaryRevetment + (RoundedDouble) random.NextDouble();
            var newUpperBoundaryWaterLevels = newLowerBoundaryWaterLevels + (RoundedDouble) random.NextDouble();
            var newStepSize = WaveConditionsInputStepSize.Half;

            var newHydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "name", 0.0, 1.1)
            {
                DesignWaterLevel = assessmentLevel
            };
            var newSelectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(newHydraulicBoundaryLocation, null);

            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    newHydraulicBoundaryLocation
                }
            };

            var input = new WaveConditionsInput();
            input.Attach(observerMock);

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var inputContext = new StabilityStoneCoverWaveConditionsInputContext(input,
                                                                                 failureMechanism.ForeshoreProfiles,
                                                                                 assessmentSection);

            var newForeshoreProfile = new ForeshoreProfile(
                new Point2D(
                    (RoundedDouble) random.NextDouble(),
                    (RoundedDouble) random.NextDouble()),
                Enumerable.Empty<Point2D>(),
                new BreakWater(BreakWaterType.Dam, (RoundedDouble) random.NextDouble()),
                new ForeshoreProfile.ConstructionProperties());

            var properties = new StabilityStoneCoverWaveConditionsInputContextProperties
            {
                Data = inputContext
            };

            // Call
            properties.ForeshoreProfile = newForeshoreProfile;
            properties.UpperBoundaryRevetment = newUpperBoundaryRevetment;
            properties.LowerBoundaryRevetment = newLowerBoundaryRevetment;
            properties.UpperBoundaryWaterLevels = newUpperBoundaryWaterLevels;
            properties.LowerBoundaryWaterLevels = newLowerBoundaryWaterLevels;
            properties.StepSize = newStepSize;
            properties.SelectedHydraulicBoundaryLocation = newSelectableHydraulicBoundaryLocation;
            properties.Orientation = orientation;

            // Assert
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.AreEqual(input.HydraulicBoundaryLocation.DesignWaterLevel.Value, properties.AssessmentLevel.Value);
            Assert.AreEqual(assessmentLevel - 0.01, properties.UpperBoundaryDesignWaterLevel.Value, properties.UpperBoundaryDesignWaterLevel.GetAccuracy());
            Assert.AreEqual(2, properties.UpperBoundaryDesignWaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(newUpperBoundaryRevetment.Value, properties.UpperBoundaryRevetment.Value, properties.UpperBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(2, properties.UpperBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.AreEqual(newLowerBoundaryRevetment.Value, properties.LowerBoundaryRevetment.Value, properties.LowerBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(2, properties.LowerBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.AreEqual(newUpperBoundaryWaterLevels.Value, properties.UpperBoundaryWaterLevels.Value, properties.UpperBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(2, properties.UpperBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(newLowerBoundaryWaterLevels.Value, properties.LowerBoundaryWaterLevels.Value, properties.LowerBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(2, properties.LowerBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(orientation, properties.Orientation.Value, properties.Orientation.GetAccuracy());
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(newStepSize, properties.StepSize);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues(
            [Values(true, false)] bool withForeshoreProfile)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var inputContext = new StabilityStoneCoverWaveConditionsInputContext(input, failureMechanism.ForeshoreProfiles, assessmentSection);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsInputContextProperties
            {
                Data = inputContext
            };

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
            mocks.VerifyAll();
        }
    }
}