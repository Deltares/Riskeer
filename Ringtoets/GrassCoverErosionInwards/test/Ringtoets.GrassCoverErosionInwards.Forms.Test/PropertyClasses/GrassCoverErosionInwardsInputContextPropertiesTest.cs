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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextPropertiesTest
    {
        private const int dikeProfilePropertyIndex = 0;
        private const int worldReferencePointPropertyIndex = 1;
        private const int orientationPropertyIndex = 2;
        private const int breakWaterPropertyIndex = 3;
        private const int foreshorePropertyIndex = 4;
        private const int dikeGeometryPropertyIndex = 5;
        private const int dikeHeightPropertyIndex = 6;
        private const int criticalFlowRatePropertyIndex = 7;
        private const int hydraulicBoundaryLocationPropertyIndex = 8;
        private const int calculateDikeHeightPropertyIndex = 9;
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsInputContext>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>();
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput
            {
                DikeHeightCalculationType = DikeHeightCalculationType.NoCalculation
            };
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculationMock, failureMechanismMock, assessmentSectionMock);

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.IsNull(properties.DikeProfile);
            Assert.IsNaN(properties.Orientation.Value);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.BreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.Foreshore);
            Assert.AreSame(inputContext, properties.DikeGeometry.Data);
            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.IsNaN(properties.DikeHeight);
            Assert.AreEqual(input.CriticalFlowRate.Mean, properties.CriticalFlowRate.Mean);
            Assert.AreEqual(input.CriticalFlowRate.StandardDeviation, properties.CriticalFlowRate.StandardDeviation);
            Assert.IsNull(properties.SelectedHydraulicBoundaryLocation);
            Assert.AreEqual(input.DikeHeightCalculationType, properties.DikeHeightCalculationType);
            Assert.IsTrue(TypeUtils.HasTypeConverter<GrassCoverErosionInwardsInputContextProperties,
                              EnumTypeConverter>(p => p.DikeHeightCalculationType));
            Assert.IsNull(properties.WorldReferencePoint);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Data_SetNewInputContextInstanceWithDikeProfile_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>();
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile(new Point2D(12.34, 56.78)),
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "", 0, 0)
            };
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculationMock, failureMechanismMock, assessmentSectionMock);

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreSame(input.DikeProfile, properties.DikeProfile);
            Assert.AreEqual(0.0, properties.Orientation.Value);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.BreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.Foreshore);
            Assert.AreSame(inputContext, properties.DikeGeometry.Data);
            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, properties.DikeHeight.Value);
            Assert.AreEqual(input.CriticalFlowRate.Mean, properties.CriticalFlowRate.Mean);
            Assert.AreEqual(input.CriticalFlowRate.StandardDeviation, properties.CriticalFlowRate.StandardDeviation);
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.AreEqual(input.DikeHeightCalculationType, properties.DikeHeightCalculationType);
            Assert.AreEqual(new Point2D(12, 57), properties.WorldReferencePoint);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(
                    input,
                    new GrassCoverErosionInwardsCalculation(),
                    new GrassCoverErosionInwardsFailureMechanism(),
                    assessmentSectionMock)
            };

            DikeProfile newDikeProfile = new TestDikeProfile();
            var newDikeHeight = new RoundedDouble(2, 9);
            var newOrientation = new RoundedDouble(2, 5);
            var newSelectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(
                new HydraulicBoundaryLocation(0, "name", 0.0, 1.1), null);
            DikeHeightCalculationType dikeHeightCalculationType = DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability;

            // Call
            properties.DikeProfile = newDikeProfile;
            properties.Orientation = newOrientation;
            properties.DikeHeight = newDikeHeight;
            properties.SelectedHydraulicBoundaryLocation = newSelectableHydraulicBoundaryLocation;
            properties.DikeHeightCalculationType = dikeHeightCalculationType;

            // Assert
            Assert.AreSame(newDikeProfile, input.DikeProfile);
            Assert.AreEqual(newOrientation, input.Orientation);
            Assert.AreEqual(newDikeHeight, input.DikeHeight);
            Assert.AreSame(newSelectableHydraulicBoundaryLocation.HydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(dikeHeightCalculationType, input.DikeHeightCalculationType);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Orientation_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.Orientation = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DikeHeight_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.DikeHeight = new Random(21).NextRoundedDouble());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DikeHeightCalculationType_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.DikeHeightCalculationType = new Random(21).NextEnumValue<DikeHeightCalculationType>());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DikeProfile_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(hasOutput, properties => properties.DikeProfile = new TestDikeProfile());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SelectedHydraulicBoundaryLocation_WithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(
                hasOutput,
                properties => properties.SelectedHydraulicBoundaryLocation =
                              new SelectableHydraulicBoundaryLocation(new TestHydraulicBoundaryLocation(), new Point2D(0, 0)));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void BreakWater_UseBreakWaterChangedWithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(
                hasOutput,
                properties => properties.BreakWater.UseBreakWater = new Random(21).NextBoolean());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CriticalFlowRate_MeanChangedWithOrWithoutOutput_HasOutputFalseInputNotifiedAndCalculationNotifiedWhenHadOutput(bool hasOutput)
        {
            SetPropertyAndVerifyNotifcationsAndOutput(
                hasOutput,
                properties => properties.CriticalFlowRate.Mean = new Random(21).NextRoundedDouble());
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputNoLocation_ReturnsNull()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculationInput = new GrassCoverErosionInwardsInput();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var inputContext = new GrassCoverErosionInwardsInputContext(calculationInput,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = inputContext
            };

            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = null;

            // Call
            TestDelegate call = () => selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsNull(selectedHydraulicBoundaryLocation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableLocations_InputWithLocationsNoDikeProfile_ReturnsLocationsSortedById()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 1),
                    new HydraulicBoundaryLocation(4, "C", 0, 2),
                    new HydraulicBoundaryLocation(3, "D", 0, 3),
                    new HydraulicBoundaryLocation(2, "B", 0, 4),
                }
            };
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionStub)
            };

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                assessmentSectionStub.HydraulicBoundaryDatabase.Locations
                                     .Select(location =>
                                             new SelectableHydraulicBoundaryLocation(location, null))
                                     .OrderBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsAndNoDikeProfile_ReturnsLocationsSortedByDistanceThenById()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 10),
                    new HydraulicBoundaryLocation(4, "E", 0, 500),
                    new HydraulicBoundaryLocation(6, "F", 0, 100),
                    new HydraulicBoundaryLocation(5, "D", 0, 200),
                    new HydraulicBoundaryLocation(3, "C", 0, 200),
                    new HydraulicBoundaryLocation(2, "B", 0, 200)
                }
            };
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput()
            {
                DikeProfile = new TestDikeProfile()
            };
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionStub)
            };

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                assessmentSectionStub.HydraulicBoundaryDatabase.Locations
                                     .Select(location =>
                                             new SelectableHydraulicBoundaryLocation(
                                                 location, input.DikeProfile.WorldReferencePoint))
                                     .OrderBy(hbl => hbl.Distance)
                                     .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenLocationAndReferencePoint_WhenUpdatingDikeProfile_ThenUpdateSelectableBoundaryLocations()
        {
            // Given
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 10),
                    new HydraulicBoundaryLocation(4, "E", 0, 500),
                    new HydraulicBoundaryLocation(6, "F", 0, 100),
                    new HydraulicBoundaryLocation(5, "D", 0, 200),
                    new HydraulicBoundaryLocation(3, "C", 0, 200),
                    new HydraulicBoundaryLocation(2, "B", 0, 200)
                }
            };
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput()
            {
                DikeProfile = new TestDikeProfile()
            };
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionStub)
            };

            IEnumerable<SelectableHydraulicBoundaryLocation> originalList =
                properties.GetSelectableHydraulicBoundaryLocations().ToList();

            // When 
            properties.DikeProfile = new TestDikeProfile(new Point2D(0.0, 190.0));

            // Then
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations().ToList();
            CollectionAssert.AreNotEqual(originalList, availableHydraulicBoundaryLocations);

            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                assessmentSectionStub.HydraulicBoundaryDatabase.Locations
                                     .Select(hbl =>
                                             new SelectableHydraulicBoundaryLocation(hbl,
                                                                                     properties.DikeProfile.WorldReferencePoint))
                                     .OrderBy(hbl => hbl.Distance)
                                     .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableDikeProfiles_InputWithDikeProfiles_ReturnsDikeProfiles()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            var calculation = new GrassCoverErosionInwardsCalculation();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism()
            {
                DikeProfiles =
                {
                    new TestDikeProfile()
                }
            };

            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionStub)
            };

            // Call
            var availableDikeProfiles = properties.GetAvailableDikeProfiles();

            // Assert
            List<DikeProfile> expectedHydraulicBoundaryLocations = failureMechanism.DikeProfiles;
            Assert.AreSame(expectedHydraulicBoundaryLocations, availableDikeProfiles);
            mockRepository.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues(bool withDikeProfile)
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();

            if (withDikeProfile)
            {
                input.DikeProfile = new TestDikeProfile();
            }

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock)
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(10, dynamicProperties.Count);

            PropertyDescriptor dikeProfileProperty = dynamicProperties[dikeProfilePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeProfileProperty,
                                                                            "Schematisatie",
                                                                            "Dijkprofiel",
                                                                            "De schematisatie van het dijkprofiel.");

            PropertyDescriptor worldReferencePointProperty = dynamicProperties[worldReferencePointPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(worldReferencePointProperty,
                                                                            "Schematisatie",
                                                                            "Locatie (RD) [m]",
                                                                            "De coördinaten van de locatie van de dijk in het Rijksdriehoeksstelsel.",
                                                                            true);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(orientationProperty,
                                                                            "Schematisatie",
                                                                            "Oriëntatie [°]",
                                                                            "Oriëntatie van de dijknormaal ten opzichte van het noorden.",
                                                                            !withDikeProfile);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(breakWaterProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterProperty,
                                                                            "Schematisatie",
                                                                            "Dam",
                                                                            "Eigenschappen van de dam.",
                                                                            true);

            PropertyDescriptor foreshoreProperty = dynamicProperties[foreshorePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(foreshoreProperty,
                                                                            "Schematisatie",
                                                                            "Voorlandgeometrie",
                                                                            "Eigenschappen van de voorlandgeometrie.",
                                                                            true);

            PropertyDescriptor dikeGeometryProperty = dynamicProperties[dikeGeometryPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(dikeGeometryProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeGeometryProperty,
                                                                            "Schematisatie",
                                                                            "Dijkgeometrie",
                                                                            "Eigenschappen van de dijkgeometrie.",
                                                                            true);

            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightProperty,
                                                                            "Schematisatie",
                                                                            "Dijkhoogte [m+NAP]",
                                                                            "De hoogte van de dijk.",
                                                                            !withDikeProfile);

            PropertyDescriptor criticalFlowRateProperty = dynamicProperties[criticalFlowRatePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalFlowRateProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(criticalFlowRateProperty,
                                                                            "Toetseisen",
                                                                            "Kritisch overslagdebiet [m³/s/m]",
                                                                            "Kritisch overslagdebiet per strekkende meter.",
                                                                            true);

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[hydraulicBoundaryLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hydraulicBoundaryLocationProperty,
                                                                            "Hydraulische gegevens",
                                                                            "Locatie met hydraulische randvoorwaarden",
                                                                            "De locatie met hydraulische randvoorwaarden.");

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[calculateDikeHeightPropertyIndex],
                                                                            "Schematisatie",
                                                                            "HBN berekenen",
                                                                            "Geeft aan of ook het Hydraulisch Belasting Niveau (HBN) moet worden berekend.");

            mockRepository.VerifyAll();
        }

        private void SetPropertyAndVerifyNotifcationsAndOutput(
            bool hasOutput,
            Action<GrassCoverErosionInwardsInputContextProperties> setProperty)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var inputObserver = mocks.StrictMock<IObserver>();
            int numberOfChangedProperties = hasOutput ? 1 : 0;
            calculationObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            inputObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            if (hasOutput)
            {
                calculation.Output = new TestGrassCoverErosionInwardsOutput();
            }
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            GrassCoverErosionInwardsInput inputParameters = calculation.InputParameters;
            calculation.Attach(calculationObserver);
            inputParameters.Attach(inputObserver);

            inputParameters.DikeProfile = new TestDikeProfile();

            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(inputParameters,
                                                                calculation,
                                                                failureMechanism,
                                                                assessmentSection)
            };

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

            mocks.VerifyAll();
        }
    }
}