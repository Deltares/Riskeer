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

using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextPropertiesTest
    {
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
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(new ProbabilityAssessmentInput());
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();

            var properties = new GrassCoverErosionInwardsInputContextProperties();

            // Call
            var inputContext = new GrassCoverErosionInwardsInputContext(input, calculationMock, failureMechanismMock, assessmentSectionMock);
            properties.Data = inputContext;

            // Assert
            var dikeGeometryProperties = new DikeGeometryProperties
            {
                Data = inputContext
            };
            Assert.AreEqual(dikeGeometryProperties.Coordinates, properties.DikeGeometry.Coordinates);
            Assert.AreEqual(dikeGeometryProperties.Roughness, properties.DikeGeometry.Roughness);

            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(0.00, properties.DikeHeight.Value);

            var foreshoreProperties = new ForeshoreProperties
            {
                Data = inputContext
            };
            Assert.AreEqual(foreshoreProperties.UseForeshore, properties.Foreshore.UseForeshore);
            Assert.AreEqual(foreshoreProperties.NumberOfCoordinates, properties.Foreshore.NumberOfCoordinates);

            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.00, properties.Orientation.Value);

            var breakWaterProperties = new BreakWaterProperties
            {
                Data = inputContext
            };
            Assert.AreEqual(breakWaterProperties.UseBreakWater, properties.BreakWater.UseBreakWater);
            Assert.AreEqual(breakWaterProperties.BreakWaterHeight, properties.BreakWater.BreakWaterHeight);
            Assert.AreEqual(breakWaterProperties.BreakWaterType, properties.BreakWater.BreakWaterType);
            Assert.AreEqual(input.CriticalFlowRate.Mean, properties.CriticalFlowRate.Mean);
            Assert.AreEqual(input.CriticalFlowRate.StandardDeviation, properties.CriticalFlowRate.StandardDeviation);
            Assert.AreEqual(input.HydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 3;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "name", 0.0, 1.1);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(new ProbabilityAssessmentInput());
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            input.Attach(observerMock);
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculationMock, failureMechanismMock, assessmentSectionMock)
            };

            var newDikeHeight = new RoundedDouble(2, 9);
            var newOrientation = new RoundedDouble(2, 5);

            // Call
            properties.DikeHeight = newDikeHeight;
            properties.Orientation = newOrientation;
            properties.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Assert
            Assert.AreEqual(newDikeHeight, input.DikeHeight);
            Assert.AreEqual(newOrientation, input.Orientation);
            Assert.AreEqual(hydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(new ProbabilityAssessmentInput());
            var inputMock = mockRepository.StrictMock<GrassCoverErosionInwardsInput>();
            mockRepository.ReplayAll();

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(inputMock, calculationMock, failureMechanismMock, assessmentSectionMock)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(8, dynamicProperties.Count);

            PropertyDescriptor dikeGeometryProperty = dynamicProperties[dikeGeometryPropertyIndex];
            Assert.IsNotNull(dikeGeometryProperty);
            Assert.IsTrue(dikeGeometryProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", dikeGeometryProperty.Category);
            Assert.AreEqual("Dijkgeometrie", dikeGeometryProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dijkgeometrie.", dikeGeometryProperty.Description);

            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightPropertyIndex];
            Assert.IsNotNull(dikeHeightProperty);
            Assert.IsFalse(dikeHeightProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", dikeHeightProperty.Category);
            Assert.AreEqual("Dijkhoogte [m+NAP]", dikeHeightProperty.DisplayName);
            Assert.AreEqual("De hoogte van de dijk [m+NAP].", dikeHeightProperty.Description);

            PropertyDescriptor foreshoreProperty = dynamicProperties[foreshorePropertyIndex];
            Assert.IsNotNull(foreshoreProperty);
            Assert.IsTrue(foreshoreProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", foreshoreProperty.Category);
            Assert.AreEqual("Voorland", foreshoreProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van het voorland.", foreshoreProperty.Description);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            Assert.IsNotNull(orientationProperty);
            Assert.IsFalse(orientationProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", orientationProperty.Category);
            Assert.AreEqual("Oriëntatie [º]", orientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de dijk.", orientationProperty.Description);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsNotNull(breakWaterProperty);
            Assert.IsTrue(breakWaterProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", breakWaterProperty.Category);
            Assert.AreEqual("Havendam", breakWaterProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de havendam.", breakWaterProperty.Description);

            PropertyDescriptor criticalFlowRateProperty = dynamicProperties[criticalFlowRatePropertyIndex];
            Assert.IsNotNull(criticalFlowRateProperty);
            Assert.IsTrue(criticalFlowRateProperty.IsReadOnly);
            Assert.AreEqual("Toetseisen", criticalFlowRateProperty.Category);
            Assert.AreEqual("Kritisch overslagdebiet [m³/s/m]", criticalFlowRateProperty.DisplayName);
            Assert.AreEqual("Kritisch overslagdebiet per strekkende meter.", criticalFlowRateProperty.Description);

            PropertyDescriptor hydraulicBoundaryLocationProperty = dynamicProperties[hydraulicBoundaryLocationPropertyIndex];
            Assert.IsNotNull(hydraulicBoundaryLocationProperty);
            Assert.IsFalse(hydraulicBoundaryLocationProperty.IsReadOnly);
            Assert.AreEqual("Hydraulische gegevens", hydraulicBoundaryLocationProperty.Category);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", hydraulicBoundaryLocationProperty.DisplayName);
            Assert.AreEqual("De locatie met hydraulische randvoorwaarden die gebruikt wordt tijdens de berekening.", hydraulicBoundaryLocationProperty.Description);
            mockRepository.VerifyAll();
        }

        private const int dikeGeometryPropertyIndex = 0;
        private const int dikeHeightPropertyIndex = 1;
        private const int foreshorePropertyIndex = 2;
        private const int orientationPropertyIndex = 3;
        private const int breakWaterPropertyIndex = 4;
        private const int criticalFlowRatePropertyIndex = 5;
        private const int hydraulicBoundaryLocationPropertyIndex = 6;
    }
}