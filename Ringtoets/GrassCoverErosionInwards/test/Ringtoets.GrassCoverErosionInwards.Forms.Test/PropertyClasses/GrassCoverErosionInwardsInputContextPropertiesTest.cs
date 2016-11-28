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

using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.HydraRing.Data;

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

            var input = new GrassCoverErosionInwardsInput();
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
            Assert.AreEqual(0.0, properties.DikeHeight.Value);
            Assert.AreEqual(input.CriticalFlowRate.Mean, properties.CriticalFlowRate.Mean);
            Assert.AreEqual(input.CriticalFlowRate.StandardDeviation, properties.CriticalFlowRate.StandardDeviation);
            Assert.IsNull(properties.SelectedHydraulicBoundaryLocation);
            Assert.AreEqual(input.CalculateDikeHeight, properties.CalculateDikeHeight);
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
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation);
            Assert.AreEqual(input.CalculateDikeHeight, properties.CalculateDikeHeight);
            Assert.AreEqual(new Point2D(12, 57), properties.WorldReferencePoint);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberOfChangedProperties = 5;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>();
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            input.Attach(observerMock);
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculationMock, failureMechanismMock, assessmentSectionMock)
            };

            DikeProfile newDikeProfile = new TestDikeProfile();
            var newDikeHeight = new RoundedDouble(2, 9);
            var newOrientation = new RoundedDouble(2, 5);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "name", 0.0, 1.1);

            // Call
            properties.DikeProfile = newDikeProfile;
            properties.Orientation = newOrientation;
            properties.DikeHeight = newDikeHeight;
            properties.SelectedHydraulicBoundaryLocation = hydraulicBoundaryLocation;
            properties.CalculateDikeHeight = true;

            // Assert
            Assert.AreSame(newDikeProfile, input.DikeProfile);
            Assert.AreEqual(newOrientation, input.Orientation);
            Assert.AreEqual(newDikeHeight, input.DikeHeight);
            Assert.AreSame(hydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.IsTrue(input.CalculateDikeHeight);
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

        [Test]
        public void GetHydraulicBoundaryLocations_InputWithLocations_ReturnsLocations()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(0, "A", 0, 10),
                    new HydraulicBoundaryLocation(0, "E", 0, 500),
                    new HydraulicBoundaryLocation(0, "F", 0, 100),
                    new HydraulicBoundaryLocation(0, "D", 0, 200),
                    new HydraulicBoundaryLocation(0, "C", 0, 200),
                    new HydraulicBoundaryLocation(0, "B", 0, 200)
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
            var availableHydraulicBoundaryLocations = properties.GetHydraulicBoundaryLocations();

            // Assert
            Assert.AreSame(assessmentSectionStub.HydraulicBoundaryDatabase.Locations, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }


        [Test]
        public void GetReferenceLocation_InputWithoutDikeProfile_ReturnsNull()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionStub)
            };

            // Call
            Point2D referencePoint = properties.GetReferenceLocation();

            // Assert
            Assert.IsNull(referencePoint);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetReferenceLocation_InputWithDikeProfile_ReturnsReferencePoint()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
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
            Point2D referencePoint = properties.GetReferenceLocation();

            // Assert
            Assert.AreSame(input.DikeProfile.WorldReferencePoint, referencePoint);
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
    }
}