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
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.GrassCoverErosionInwards.Data;
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
        private IObservablePropertyChangeHandler handler;
        private IAssessmentSection assessmentSection;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            assessmentSection = mockRepository.Stub<IAssessmentSection>();
        }

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsInputContextProperties(null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculationItem = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var inputParameters = new GrassCoverErosionInwardsInput();

            var context = new GrassCoverErosionInwardsInputContext(inputParameters,
                                                                   calculationItem,
                                                                   failureMechanism,
                                                                   assessmentSection);

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsInputContextProperties(context, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("handler", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculationItem = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var inputParameters = new GrassCoverErosionInwardsInput();

            var context = new GrassCoverErosionInwardsInputContext(inputParameters,
                                                                   calculationItem,
                                                                   failureMechanism,
                                                                   assessmentSection);

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties(context, handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsInputContext>>(properties);
            Assert.IsInstanceOf<IHasHydraulicBoundaryLocationProperty>(properties);
            Assert.AreSame(context, properties.Data);
        }

        [Test]
        public void Constructor_WithInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var failureMechanism = mockRepository.Stub<GrassCoverErosionInwardsFailureMechanism>();
            var calculation = mockRepository.Stub<GrassCoverErosionInwardsCalculation>();
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput
            {
                DikeHeightCalculationType = DikeHeightCalculationType.NoCalculation
            };
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Assert
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.IsNull(properties.DikeProfile);
            Assert.IsNaN(properties.Orientation.Value);
            Assert.IsInstanceOf<UseBreakWaterProperties>(
                properties.BreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(
                properties.Foreshore);
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
        public void Constructor_WithInputContextInstanceWithDikeProfile_ReturnCorrectPropertyValues()
        {
            // Setup
            var failureMechanism = mockRepository.Stub<GrassCoverErosionInwardsFailureMechanism>();
            var calculation = mockRepository.Stub<GrassCoverErosionInwardsCalculation>();
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile(new Point2D(12.34, 56.78)),
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "", 0, 0)
            };
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Assert
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreSame(input.DikeProfile, properties.DikeProfile);
            Assert.AreEqual(0.0, properties.Orientation.Value);
            Assert.IsInstanceOf<UseBreakWaterProperties>(
                properties.BreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(
                properties.Foreshore);
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
        public void Orientation_Always_InputChangedAndObsevablesNotified()
        {
            // Setup
            RoundedDouble orientation = new Random(21).NextRoundedDouble();

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutput(properties => properties.Orientation = orientation);
        }

        [Test]
        public void DikeHeight_Always_InputChangedAndObsevablesNotified()
        {
            RoundedDouble dikeHeight = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(properties => properties.DikeHeight = dikeHeight);
        }

        [Test]
        public void DikeHeightCalculationType_Always_InputChangedAndObsevablesNotified()
        {
            var dikeHeightCalculationType = new Random(21).NextEnumValue<DikeHeightCalculationType>();
            SetPropertyAndVerifyNotifcationsAndOutput(properties => properties.DikeHeightCalculationType = dikeHeightCalculationType);
        }

        [Test]
        public void DikeProfile_Always_InputChangedAndObsevablesNotified()
        {
            var dikeProfile = new TestDikeProfile();
            SetPropertyAndVerifyNotifcationsAndOutput(properties => properties.DikeProfile = dikeProfile);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_Always_InputChangedAndObsevablesNotified()
        {
            var selectableLocation = new SelectableHydraulicBoundaryLocation(new TestHydraulicBoundaryLocation(), new Point2D(0, 0));
            SetPropertyAndVerifyNotifcationsAndOutput(
                properties => properties.SelectedHydraulicBoundaryLocation = selectableLocation);
        }

        [Test]
        public void BreakWater_UseBreakWaterChangedAlways_InputChangedAndObsevablesNotified()
        {
            bool useBreakWater = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotifcationsAndOutput(properties => properties.BreakWater.UseBreakWater = useBreakWater);
        }

        [Test]
        public void UseForeshore_Always_InputChangedAndObsevablesNotified()
        {
            bool useForeshore = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotifcationsAndOutput(properties => properties.Foreshore.UseForeshore = useForeshore);
        }

        [Test]
        public void CriticalFlowRate_MeanChanged_InputChangedAndObsevablesNotified()
        {
            RoundedDouble criticalFlowMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotifcationsAndOutput(properties => properties.CriticalFlowRate.Mean = criticalFlowMean);
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
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = null;

            // Call
            TestDelegate call = () => selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            Assert.DoesNotThrow(call);
            Assert.IsNull(selectedHydraulicBoundaryLocation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableHydraulicBoundaryLocations_InputWithLocationsDikeProfile_CalculatesDistanceWithCorrectReferencePoint()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Call 
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            double distanceToPropertiesWorldReferencePoint =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(properties.WorldReferencePoint);
            double distanceToDikeProfileWorldReferencePoint =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(input.DikeProfile.WorldReferencePoint);
            Assert.AreEqual(59, distanceToPropertiesWorldReferencePoint, 1);
            Assert.AreEqual(60, distanceToDikeProfileWorldReferencePoint, 1);

            SelectableHydraulicBoundaryLocation hydraulicBoundaryLocationItem = availableHydraulicBoundaryLocations.ToArray()[0];
            RoundedDouble itemDistance = hydraulicBoundaryLocationItem.Distance;
            Assert.AreEqual(distanceToDikeProfileWorldReferencePoint, itemDistance, itemDistance.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputWithLocationsDikeProfile_CalculatesDistanceWithCorrectReferencePoint()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);

            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                DikeProfile = new TestDikeProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Call 
            SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            double distanceToPropertiesWorldReferencePoint =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(properties.WorldReferencePoint);
            double distanceToDikeProfileWorldReferencePoint =
                hydraulicBoundaryLocation.Location.GetEuclideanDistanceTo(input.DikeProfile.WorldReferencePoint);
            Assert.AreEqual(59, distanceToPropertiesWorldReferencePoint, 1);
            Assert.AreEqual(60, distanceToDikeProfileWorldReferencePoint, 1);

            RoundedDouble selectedLocationDistance = selectedHydraulicBoundaryLocation.Distance;
            Assert.AreEqual(distanceToDikeProfileWorldReferencePoint, selectedLocationDistance, selectedLocationDistance.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithDikeProfileAndLocations_WhenSelectingLocation_ThenSelectedLocationDistanceSameAsLocationItem()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 200643.312, 503347.25);
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                DikeProfile = new TestDikeProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();
            SelectableHydraulicBoundaryLocation selectedLocation = properties.SelectedHydraulicBoundaryLocation;

            // Assert
            SelectableHydraulicBoundaryLocation hydraulicBoundaryLocationItem = availableHydraulicBoundaryLocations.ToArray()[0];
            Assert.AreEqual(selectedLocation.Distance, hydraulicBoundaryLocationItem.Distance,
                            hydraulicBoundaryLocationItem.Distance.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSelectableLocations_InputWithLocationsNoDikeProfile_ReturnsLocationsSortedById()
        {
            // Setup
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
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
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                assessmentSection.HydraulicBoundaryDatabase.Locations
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
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
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
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations();

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                assessmentSection.HydraulicBoundaryDatabase.Locations
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
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
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
            var otherProfile = new TestDikeProfile(new Point2D(0.0, 190.0));

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);

            var customHandler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, customHandler);

            IEnumerable<SelectableHydraulicBoundaryLocation> originalList =
                properties.GetSelectableHydraulicBoundaryLocations().ToList();

            // When 
            properties.DikeProfile = otherProfile;

            // Then
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                properties.GetSelectableHydraulicBoundaryLocations().ToList();
            CollectionAssert.AreNotEqual(originalList, availableHydraulicBoundaryLocations);

            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                assessmentSection.HydraulicBoundaryDatabase.Locations
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

            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<DikeProfile> availableDikeProfiles = properties.GetAvailableDikeProfiles();

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
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();

            if (withDikeProfile)
            {
                input.DikeProfile = new TestDikeProfile();
            }

            // Call
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

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

        private void SetPropertyAndVerifyNotifcationsAndOutput(Action<GrassCoverErosionInwardsInputContextProperties> setProperty)
        {
            // Setup
            var observable = mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsInput input = calculation.InputParameters;
            input.DikeProfile = new TestDikeProfile();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var customHandler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, customHandler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsFalse(calculation.HasOutput);

            mockRepository.VerifyAll();
        }
    }
}