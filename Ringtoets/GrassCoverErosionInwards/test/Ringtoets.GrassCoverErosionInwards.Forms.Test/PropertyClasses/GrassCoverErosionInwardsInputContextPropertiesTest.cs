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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

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
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(generalInput);
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput(generalInput);

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

            var dikeHeight = new RoundedDouble(2).Value.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual(dikeHeight, properties.DikeHeight);

            var foreshoreProperties = new ForeshoreProperties
            {
                Data = inputContext
            };
            Assert.AreEqual(foreshoreProperties.UseForeshore, properties.Foreshore.UseForeshore);
            Assert.AreEqual(foreshoreProperties.NumberOfCoordinates, properties.Foreshore.NumberOfCoordinates);

            var orientation = new RoundedDouble(2).Value.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual(orientation, properties.Orientation);

            var breakWaterProperties = new BreakWaterProperties
            {
                Data = inputContext
            };
            Assert.AreEqual(breakWaterProperties.UseBreakWater, properties.BreakWater.UseBreakWater);
            Assert.AreEqual(breakWaterProperties.BreakWaterHeight, properties.BreakWater.BreakWaterHeight);
            Assert.AreEqual(breakWaterProperties.BreakWaterType, properties.BreakWater.BreakWaterType);
            VerifyRoundedDoubleString(input.CriticalFlowRate.Mean, properties.CriticalFlowRate.Mean);
            VerifyRoundedDoubleString(input.CriticalFlowRate.StandardDeviation, properties.CriticalFlowRate.StandardDeviation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 2;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(generalInput);
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput(generalInput);
            input.Attach(observerMock);
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculationMock, failureMechanismMock, assessmentSectionMock)
            };

            var newDikeHeight = new RoundedDouble(2, 9);
            var newOrientation = new RoundedDouble(2, 5);

            // Call
            properties.DikeHeight = newDikeHeight.ToString();
            properties.Orientation = newOrientation.Value.ToString(CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual(newDikeHeight, input.DikeHeight);
            Assert.AreEqual(newOrientation, input.Orientation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(generalInput);
            var inputMock = mockRepository.StrictMock<GrassCoverErosionInwardsInput>(generalInput);
            mockRepository.ReplayAll();

            // Call
            var properties = new GrassCoverErosionInwardsInputContextProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(inputMock, calculationMock, failureMechanismMock, assessmentSectionMock)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(7, dynamicProperties.Count);

            PropertyDescriptor dikeGeometryProperty = dynamicProperties[dikeGeometryPropertyIndex];
            Assert.IsNotNull(dikeGeometryProperty);
            Assert.IsTrue(dikeGeometryProperty.IsReadOnly);
            Assert.AreEqual("Dijkgeometrie", dikeGeometryProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dijkgeometrie.", dikeGeometryProperty.Description);

            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightPropertyIndex];
            Assert.IsNotNull(dikeHeightProperty);
            Assert.IsFalse(dikeHeightProperty.IsReadOnly);
            Assert.AreEqual("Dijkhoogte [m+NAP]", dikeHeightProperty.DisplayName);
            Assert.AreEqual("De hoogte van de dijk [m+NAP].", dikeHeightProperty.Description);

            PropertyDescriptor foreshoreProperty = dynamicProperties[foreshorePropertyIndex];
            Assert.IsNotNull(foreshoreProperty);
            Assert.IsTrue(foreshoreProperty.IsReadOnly);
            Assert.AreEqual("Voorland", foreshoreProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van het voorland.", foreshoreProperty.Description);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            Assert.IsNotNull(orientationProperty);
            Assert.IsFalse(orientationProperty.IsReadOnly);
            Assert.AreEqual("Oriëntatie [º]", orientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de dijk.", orientationProperty.Description);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsNotNull(breakWaterProperty);
            Assert.IsTrue(breakWaterProperty.IsReadOnly);
            Assert.AreEqual("Havendam", breakWaterProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de havendam.", breakWaterProperty.Description);

            PropertyDescriptor criticalFlowRateProperty = dynamicProperties[criticalFlowRatePropertyIndex];
            Assert.IsNotNull(criticalFlowRateProperty);
            Assert.IsTrue(criticalFlowRateProperty.IsReadOnly);
            Assert.AreEqual("Kritisch overslagdebiet [m3/m/s]", criticalFlowRateProperty.DisplayName);
            Assert.AreEqual("Het kritische overslagdebiet.", criticalFlowRateProperty.Description);
            mockRepository.VerifyAll();
        }

        private static void VerifyRoundedDoubleString(RoundedDouble roundedDouble, string expectedString)
        {
            var stringValue = new RoundedDouble(2, roundedDouble).Value.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual(expectedString, stringValue);
        }

        private const int dikeGeometryPropertyIndex = 0;
        private const int dikeHeightPropertyIndex = 1;
        private const int foreshorePropertyIndex = 2;
        private const int orientationPropertyIndex = 3;
        private const int breakWaterPropertyIndex = 4;
        private const int criticalFlowRatePropertyIndex = 5;
    }
}