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

using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
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
            Assert.IsNull(properties.DikeProfile);
            Assert.AreEqual(0.0, properties.Orientation.Value);
            Assert.AreSame(inputContext, properties.BreakWater.Data);
            Assert.AreSame(inputContext, properties.Foreshore.Data);
            Assert.AreSame(inputContext, properties.DikeGeometry.Data);
            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, properties.DikeHeight.Value);
            Assert.AreEqual(input.CriticalFlowRate.Mean, properties.CriticalFlowRate.Mean);
            Assert.AreEqual(input.CriticalFlowRate.StandardDeviation, properties.CriticalFlowRate.StandardDeviation);
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);
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
                DikeProfile = new DikeProfile(new Point2D(12.34, 56.78), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties()),
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
            Assert.AreSame(inputContext, properties.BreakWater.Data);
            Assert.AreSame(inputContext, properties.Foreshore.Data);
            Assert.AreSame(inputContext, properties.DikeGeometry.Data);
            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, properties.DikeHeight.Value);
            Assert.AreEqual(input.CriticalFlowRate.Mean, properties.CriticalFlowRate.Mean);
            Assert.AreEqual(input.CriticalFlowRate.StandardDeviation, properties.CriticalFlowRate.StandardDeviation);
            Assert.AreSame(input.HydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);
            Assert.AreEqual(input.CalculateDikeHeight, properties.CalculateDikeHeight);
            Assert.AreEqual(new Point2D(12, 57), properties.WorldReferencePoint);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 5;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
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

            var newDikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                 null, new DikeProfile.ConstructionProperties());
            var newDikeHeight = new RoundedDouble(2, 9);
            var newOrientation = new RoundedDouble(2, 5);
            var newHydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "name", 0.0, 1.1);

            // Call
            properties.DikeProfile = newDikeProfile;
            properties.Orientation = newOrientation;
            properties.DikeHeight = newDikeHeight;
            properties.HydraulicBoundaryLocation = newHydraulicBoundaryLocation;
            properties.CalculateDikeHeight = true;

            // Assert
            Assert.AreSame(newDikeProfile, input.DikeProfile);
            Assert.AreEqual(newOrientation, input.Orientation);
            Assert.AreEqual(newDikeHeight, input.DikeHeight);
            Assert.AreSame(newHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.IsTrue(input.CalculateDikeHeight);
            mockRepository.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PropertyAttributes_ReturnExpectedValues(bool withDikeProfile)
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();

            if (withDikeProfile)
            {
                input.DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                    null, new DikeProfile.ConstructionProperties());
            }

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(11, dynamicProperties.Count);

            PropertyDescriptor dikeProfileProperty = dynamicProperties[dikeProfilePropertyIndex];
            Assert.IsNotNull(dikeProfileProperty);
            Assert.IsFalse(dikeProfileProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", dikeProfileProperty.Category);
            Assert.AreEqual("Dijkprofiel", dikeProfileProperty.DisplayName);
            Assert.AreEqual("De schematisatie van het dijkprofiel.", dikeProfileProperty.Description);

            PropertyDescriptor worldReferencePointProperty = dynamicProperties[worldReferencePointPropertyIndex];
            Assert.IsNotNull(worldReferencePointProperty);
            Assert.IsTrue(worldReferencePointProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", worldReferencePointProperty.Category);
            Assert.AreEqual("Locatie (RD) [m]", worldReferencePointProperty.DisplayName);
            Assert.AreEqual("De coördinaten van de locatie van de dijk in het Rijksdriehoeksstelsel.", worldReferencePointProperty.Description);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            Assert.IsNotNull(orientationProperty);
            Assert.AreEqual(!withDikeProfile, orientationProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", orientationProperty.Category);
            Assert.AreEqual("Oriëntatie [°]", orientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de dijk.", orientationProperty.Description);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsNotNull(breakWaterProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(breakWaterProperty.Converter);
            Assert.IsTrue(breakWaterProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", breakWaterProperty.Category);
            Assert.AreEqual("Dam", breakWaterProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dam.", breakWaterProperty.Description);

            PropertyDescriptor foreshoreProperty = dynamicProperties[foreshorePropertyIndex];
            Assert.IsNotNull(foreshoreProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreProperty.Converter);
            Assert.IsTrue(foreshoreProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", foreshoreProperty.Category);
            Assert.AreEqual("Voorlandgeometrie", foreshoreProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de voorlandgeometrie.", foreshoreProperty.Description);

            PropertyDescriptor dikeGeometryProperty = dynamicProperties[dikeGeometryPropertyIndex];
            Assert.IsNotNull(dikeGeometryProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(dikeGeometryProperty.Converter);
            Assert.IsTrue(dikeGeometryProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", dikeGeometryProperty.Category);
            Assert.AreEqual("Dijkgeometrie", dikeGeometryProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dijkgeometrie.", dikeGeometryProperty.Description);

            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightPropertyIndex];
            Assert.IsNotNull(dikeHeightProperty);
            Assert.AreEqual(!withDikeProfile, dikeHeightProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", dikeHeightProperty.Category);
            Assert.AreEqual("Dijkhoogte [m+NAP]", dikeHeightProperty.DisplayName);
            Assert.AreEqual("De hoogte van de dijk.", dikeHeightProperty.Description);

            PropertyDescriptor criticalFlowRateProperty = dynamicProperties[criticalFlowRatePropertyIndex];
            Assert.IsNotNull(criticalFlowRateProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(criticalFlowRateProperty.Converter);
            Assert.IsTrue(criticalFlowRateProperty.IsReadOnly);
            Assert.AreEqual("Toetseisen", criticalFlowRateProperty.Category);
            Assert.AreEqual("Kritisch overslagdebiet [m³/s/m]", criticalFlowRateProperty.DisplayName);
            Assert.AreEqual("Kritisch overslagdebiet per strekkende meter.", criticalFlowRateProperty.Description);

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[hydraulicBoundaryLocationPropertyIndex];
            Assert.IsNotNull(hydraulicBoundaryLocationProperty);
            Assert.IsFalse(hydraulicBoundaryLocationProperty.IsReadOnly);
            Assert.AreEqual("Hydraulische gegevens", hydraulicBoundaryLocationProperty.Category);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", hydraulicBoundaryLocationProperty.DisplayName);
            Assert.AreEqual("De locatie met hydraulische randvoorwaarden.", hydraulicBoundaryLocationProperty.Description);

            Assert.IsNotNull(dynamicProperties[calculateDikeHeightPropertyIndex]);
            Assert.AreEqual("Schematisatie", dynamicProperties[calculateDikeHeightPropertyIndex].Category);
            Assert.AreEqual("HBN berekenen", dynamicProperties[calculateDikeHeightPropertyIndex].DisplayName);
            Assert.AreEqual("Geeft aan of ook het Hydraulisch Belasting Niveau (HBN) moet worden berekend.", dynamicProperties[calculateDikeHeightPropertyIndex].Description);

            mockRepository.VerifyAll();
        }
    }
}