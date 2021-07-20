// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.UITypeEditors;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextPropertiesTest
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 0;
        private const int dikeProfilePropertyIndex = 1;
        private const int worldReferencePointPropertyIndex = 2;
        private const int orientationPropertyIndex = 3;
        private const int breakWaterPropertyIndex = 4;
        private const int foreshorePropertyIndex = 5;
        private const int dikeGeometryPropertyIndex = 6;
        private const int dikeHeightPropertyIndex = 7;
        private const int criticalFlowRatePropertyIndex = 8;
        private const int shouldOvertoppingOutputIllustrationPointsBeCalculatedPropertyIndex = 9;
        private const int shouldDikeHeightBeCalculatedPropertyIndex = 10;
        private const int shouldDikeHeightIllustrationPointsBeCalculatedPropertyIndex = 11;
        private const int shouldOvertoppingRateBeCalculatedPropertyIndex = 12;
        private const int shouldOvertoppingRateIllustrationPointsBeCalculatedPropertyIndex = 13;

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
            void Call() => new GrassCoverErosionInwardsInputContextProperties(null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("data", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowArgumentNullException()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var inputParameters = new GrassCoverErosionInwardsInput(double.NaN);

            var context = new GrassCoverErosionInwardsInputContext(inputParameters,
                                                                   calculation,
                                                                   failureMechanism,
                                                                   assessmentSection);

            // Call
            void Call() => new GrassCoverErosionInwardsInputContextProperties(context, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("handler", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var inputParameters = new GrassCoverErosionInwardsInput(double.NaN);

            var context = new GrassCoverErosionInwardsInputContext(inputParameters,
                                                                   calculation,
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
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var input = new GrassCoverErosionInwardsInput(double.NaN);
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Assert
            Assert.IsNull(properties.SelectedHydraulicBoundaryLocation);
            Assert.IsNull(properties.DikeProfile);
            Assert.IsNull(properties.WorldReferencePoint);
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.IsNaN(properties.Orientation.Value);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.BreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.Foreshore);
            Assert.AreSame(inputContext, properties.DikeGeometry.Data);
            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.IsNaN(properties.DikeHeight);
            Assert.AreEqual(input.CriticalFlowRate.Mean, properties.CriticalFlowRate.Mean);
            Assert.AreEqual(input.CriticalFlowRate.StandardDeviation, properties.CriticalFlowRate.StandardDeviation);
            Assert.IsFalse(properties.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(input.ShouldDikeHeightBeCalculated, properties.ShouldDikeHeightBeCalculated);
            Assert.IsFalse(properties.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.AreEqual(input.ShouldOvertoppingRateBeCalculated, properties.ShouldOvertoppingRateBeCalculated);
            Assert.IsFalse(properties.ShouldOvertoppingRateIllustrationPointsBeCalculated);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithInputContextInstanceWithDikeProfile_ReturnCorrectPropertyValues()
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(12.34, 56.78)),
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "", 0, 0)
            };
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Assert
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.SelectedHydraulicBoundaryLocation.HydraulicBoundaryLocation);
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreSame(input.DikeProfile, properties.DikeProfile);
            Assert.AreEqual(new Point2D(12, 57), properties.WorldReferencePoint);
            Assert.AreEqual(0.0, properties.Orientation.Value);
            Assert.IsInstanceOf<UseBreakWaterProperties>(properties.BreakWater);
            Assert.IsInstanceOf<UseForeshoreProperties>(properties.Foreshore);
            Assert.AreSame(inputContext, properties.DikeGeometry.Data);
            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, properties.DikeHeight.Value);
            Assert.AreEqual(input.CriticalFlowRate.Mean, properties.CriticalFlowRate.Mean);
            Assert.AreEqual(input.CriticalFlowRate.StandardDeviation, properties.CriticalFlowRate.StandardDeviation);
            Assert.IsFalse(properties.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(input.ShouldDikeHeightBeCalculated, properties.ShouldDikeHeightBeCalculated);
            Assert.IsFalse(properties.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.AreEqual(input.ShouldOvertoppingRateBeCalculated, properties.ShouldOvertoppingRateBeCalculated);
            Assert.IsFalse(properties.ShouldOvertoppingRateIllustrationPointsBeCalculated);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DikeProfile_Always_InputChangedAndObservablesNotified()
        {
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();
            SetPropertyAndVerifyNotificationsAndOutput(properties => properties.DikeProfile = dikeProfile);
        }

        [Test]
        public void Orientation_Always_InputChangedAndObservablesNotified()
        {
            // Setup
            RoundedDouble orientation = new Random(21).NextRoundedDouble();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutput(properties => properties.Orientation = orientation);
        }

        [Test]
        public void DikeHeight_Always_InputChangedAndObservablesNotified()
        {
            RoundedDouble dikeHeight = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(properties => properties.DikeHeight = dikeHeight);
        }

        [Test]
        public void ShouldDikeHeightBeCalculated_Always_InputChangedAndObservablesNotified()
        {
            bool shouldDikeHeightBeCalculated = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutput(properties => properties.ShouldDikeHeightBeCalculated = shouldDikeHeightBeCalculated);
        }

        [Test]
        public void ShouldOvertoppingRateBeCalculated_Always_InputChangedAndObservablesNotified()
        {
            bool shouldOvertoppingRateBeCalculated = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutput(properties => properties.ShouldOvertoppingRateBeCalculated = shouldOvertoppingRateBeCalculated);
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_Always_InputChangedAndObservablesNotified()
        {
            var selectableLocation = new SelectableHydraulicBoundaryLocation(new TestHydraulicBoundaryLocation(), new Point2D(0, 0));
            SetPropertyAndVerifyNotificationsAndOutput(
                properties => properties.SelectedHydraulicBoundaryLocation = selectableLocation);
        }

        [Test]
        public void BreakWater_UseBreakWaterChangedAlways_InputChangedAndObservablesNotified()
        {
            bool useBreakWater = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutput(properties => properties.BreakWater.UseBreakWater = useBreakWater);
        }

        [Test]
        public void UseForeshore_Always_InputChangedAndObservablesNotified()
        {
            bool useForeshore = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutput(properties => properties.Foreshore.UseForeshore = useForeshore);
        }

        [Test]
        public void CriticalFlowRate_MeanChanged_InputChangedAndObservablesNotified()
        {
            RoundedDouble criticalFlowMean = new Random(21).NextRoundedDouble();
            SetPropertyAndVerifyNotificationsAndOutput(properties => properties.CriticalFlowRate.Mean = criticalFlowMean);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 3;
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm
            };
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);
            inputContext.Attach(observer);

            // Call
            properties.ShouldDikeHeightIllustrationPointsBeCalculated = true;
            properties.ShouldOvertoppingOutputIllustrationPointsBeCalculated = true;
            properties.ShouldOvertoppingRateIllustrationPointsBeCalculated = true;

            // Assert
            Assert.IsTrue(properties.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.IsTrue(properties.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.IsTrue(properties.ShouldOvertoppingRateIllustrationPointsBeCalculated);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SelectedHydraulicBoundaryLocation_InputNoLocation_ReturnsNull()
        {
            // Setup
            mockRepository.ReplayAll();

            var calculationInput = new GrassCoverErosionInwardsInput(double.NaN);
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var inputContext = new GrassCoverErosionInwardsInputContext(calculationInput,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSection);
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

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            });

            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
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

            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
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

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            });

            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                DikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(200620.173572981, 503401.652985217))
            };
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
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
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "A", 0, 1),
                    new HydraulicBoundaryLocation(4, "C", 0, 2),
                    new HydraulicBoundaryLocation(3, "D", 0, 3),
                    new HydraulicBoundaryLocation(2, "B", 0, 4)
                }
            });

            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput(double.NaN);
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
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
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
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
            });

            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
            };
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
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
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
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
            });

            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
            };
            DikeProfile otherProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 190.0));

            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(Enumerable.Empty<IObservable>());
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

            var input = new GrassCoverErosionInwardsInput(double.NaN);
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile()
            }, "path");

            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Call
            IEnumerable<DikeProfile> availableDikeProfiles = properties.GetAvailableDikeProfiles();

            // Assert
            DikeProfileCollection expectedHydraulicBoundaryLocations = failureMechanism.DikeProfiles;
            Assert.AreSame(expectedHydraulicBoundaryLocations, availableDikeProfiles);
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues([Values(true, false)] bool withDikeProfile,
                                                                              [Values(true, false)] bool calculationsEnabled)
        {
            // Setup
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                DikeProfile = withDikeProfile
                                  ? DikeProfileTestFactory.CreateDikeProfile()
                                  : null,
                ShouldDikeHeightBeCalculated = calculationsEnabled,
                ShouldOvertoppingRateBeCalculated = calculationsEnabled
            };

            // Call
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);
            var properties = new GrassCoverErosionInwardsInputContextProperties(inputContext, handler);

            // Assert
            const string hydraulicDataCategoryName = "\t\t\t\t\tHydraulische gegevens";
            const string schematizationCategoryName = "\t\t\t\tSchematisatie";
            const string criticalValuesCategoryName = "\t\t\tToetseisen";
            const string overtoppingOutputCategoryName = "\t\tSterkte berekening";
            const string dikeHeightCategoryName = "\tHBN";
            const string overtoppingRateCategoryName = "Overslagdebiet";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(14, dynamicProperties.Count);

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[hydraulicBoundaryLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(hydraulicBoundaryLocationProperty,
                                                                            hydraulicDataCategoryName,
                                                                            "Hydraulische belastingenlocatie",
                                                                            "De hydraulische belastingenlocatie.");

            PropertyDescriptor dikeProfileProperty = dynamicProperties[dikeProfilePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeProfileProperty,
                                                                            schematizationCategoryName,
                                                                            "Dijkprofiel",
                                                                            "De schematisatie van het dijkprofiel.");

            PropertyDescriptor worldReferencePointProperty = dynamicProperties[worldReferencePointPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(worldReferencePointProperty,
                                                                            schematizationCategoryName,
                                                                            "Locatie (RD) [m]",
                                                                            "De coördinaten van de locatie van de dijk in het Rijksdriehoeksstelsel.",
                                                                            true);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(orientationProperty,
                                                                            schematizationCategoryName,
                                                                            "Oriëntatie [°]",
                                                                            "Oriëntatie van de dijknormaal ten opzichte van het noorden.",
                                                                            !withDikeProfile);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(breakWaterProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterProperty,
                                                                            schematizationCategoryName,
                                                                            "Dam",
                                                                            "Eigenschappen van de dam.",
                                                                            true);

            PropertyDescriptor foreshoreProperty = dynamicProperties[foreshorePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(foreshoreProperty,
                                                                            schematizationCategoryName,
                                                                            "Voorlandgeometrie",
                                                                            "Eigenschappen van de voorlandgeometrie.",
                                                                            true);

            PropertyDescriptor dikeGeometryProperty = dynamicProperties[dikeGeometryPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(dikeGeometryProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeGeometryProperty,
                                                                            schematizationCategoryName,
                                                                            "Dijkgeometrie",
                                                                            "Eigenschappen van de dijkgeometrie.",
                                                                            true);

            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightProperty,
                                                                            schematizationCategoryName,
                                                                            "Dijkhoogte [m+NAP]",
                                                                            "De hoogte van de dijk.",
                                                                            !withDikeProfile);

            PropertyDescriptor criticalFlowRateProperty = dynamicProperties[criticalFlowRatePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalFlowRateProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(criticalFlowRateProperty,
                                                                            criticalValuesCategoryName,
                                                                            "Kritiek overslagdebiet [m³/s/m]",
                                                                            "Kritiek overslagdebiet per strekkende meter.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[shouldOvertoppingOutputIllustrationPointsBeCalculatedPropertyIndex],
                                                                            overtoppingOutputCategoryName,
                                                                            "Illustratiepunten inlezen   ",
                                                                            "Neem de informatie over de illustratiepunten op in het berekeningsresultaat.");

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[shouldDikeHeightBeCalculatedPropertyIndex],
                                                                            dikeHeightCategoryName,
                                                                            "HBN berekenen",
                                                                            "Geeft aan of ook het Hydraulisch Belasting Niveau (HBN) moet worden berekend.");

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[shouldDikeHeightIllustrationPointsBeCalculatedPropertyIndex],
                                                                            dikeHeightCategoryName,
                                                                            "Illustratiepunten inlezen ",
                                                                            "Neem de informatie over de illustratiepunten op in het berekeningsresultaat.",
                                                                            !calculationsEnabled);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[shouldOvertoppingRateBeCalculatedPropertyIndex],
                                                                            overtoppingRateCategoryName,
                                                                            "Overslagdebiet berekenen",
                                                                            "Geeft aan of ook het overslagdebiet moet worden berekend.");

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[shouldOvertoppingRateIllustrationPointsBeCalculatedPropertyIndex],
                                                                            overtoppingRateCategoryName,
                                                                            "Illustratiepunten inlezen  ",
                                                                            "Neem de informatie over de illustratiepunten op in het berekeningsresultaat.",
                                                                            !calculationsEnabled);
            mockRepository.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnly_ShouldDikeHeightIllustrationPointsBeCalculated_ReturnsExpectedResult(bool shouldDikeHeightBeCalculated)
        {
            // Setup
            var changeHandler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                ShouldDikeHeightBeCalculated = shouldDikeHeightBeCalculated
            };

            var context = new GrassCoverErosionInwardsInputContext(input,
                                                                   calculation,
                                                                   failureMechanism,
                                                                   assessmentSection);

            var properties = new GrassCoverErosionInwardsInputContextProperties(context, changeHandler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod(nameof(properties.ShouldDikeHeightIllustrationPointsBeCalculated));

            // Assert
            Assert.AreEqual(!shouldDikeHeightBeCalculated, result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnly_ShouldOvertoppingRateIllustrationPointsBeCalculated_ReturnsExpectedResult(bool shouldOvertoppingRateBeCalculated)
        {
            // Setup
            var changeHandler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                ShouldOvertoppingRateBeCalculated = shouldOvertoppingRateBeCalculated
            };

            var context = new GrassCoverErosionInwardsInputContext(input,
                                                                   calculation,
                                                                   failureMechanism,
                                                                   assessmentSection);

            var properties = new GrassCoverErosionInwardsInputContextProperties(context, changeHandler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod(nameof(properties.ShouldOvertoppingRateIllustrationPointsBeCalculated));

            // Assert
            Assert.AreEqual(!shouldOvertoppingRateBeCalculated, result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnly_Orientation_ReturnsExpectedResult(bool hasDikeProfile)
        {
            // Setup
            var changeHandler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                DikeProfile = hasDikeProfile
                                  ? DikeProfileTestFactory.CreateDikeProfile()
                                  : null
            };

            var context = new GrassCoverErosionInwardsInputContext(input,
                                                                   calculation,
                                                                   failureMechanism,
                                                                   assessmentSection);

            var properties = new GrassCoverErosionInwardsInputContextProperties(context, changeHandler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod(nameof(properties.Orientation));

            // Assert
            Assert.AreEqual(!hasDikeProfile, result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnly_DikeHeight_ReturnsExpectedResult(bool hasDikeProfile)
        {
            // Setup
            var changeHandler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var input = new GrassCoverErosionInwardsInput(double.NaN)
            {
                DikeProfile = hasDikeProfile
                                  ? DikeProfileTestFactory.CreateDikeProfile()
                                  : null
            };

            var context = new GrassCoverErosionInwardsInputContext(input,
                                                                   calculation,
                                                                   failureMechanism,
                                                                   assessmentSection);

            var properties = new GrassCoverErosionInwardsInputContextProperties(context, changeHandler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod(nameof(properties.DikeHeight));

            // Assert
            Assert.AreEqual(!hasDikeProfile, result);
        }

        private void SetPropertyAndVerifyNotificationsAndOutput(Action<GrassCoverErosionInwardsInputContextProperties> setProperty)
        {
            // Setup
            var observable = mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation(double.NaN);
            GrassCoverErosionInwardsInput input = calculation.InputParameters;
            input.DikeProfile = DikeProfileTestFactory.CreateDikeProfile();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var customHandler = new SetPropertyValueAfterConfirmationParameterTester(new[]
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