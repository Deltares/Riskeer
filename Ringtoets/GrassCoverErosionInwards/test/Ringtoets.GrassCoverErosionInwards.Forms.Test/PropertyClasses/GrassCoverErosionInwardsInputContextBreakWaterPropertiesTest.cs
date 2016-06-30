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
using Core.Common.Base.Geometry;
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
    public class GrassCoverErosionInwardsInputContextBreakWaterPropertiesTest
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
            // Setup & Call
            var properties = new GrassCoverErosionInwardsInputContextBreakWaterProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsInputContext>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();
            var properties = new GrassCoverErosionInwardsInputContextBreakWaterProperties();

            // Call
            properties.Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock);

            // Assert
            Assert.IsFalse(properties.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, properties.BreakWaterType);
            Assert.AreEqual(0.0, properties.BreakWaterHeight.Value);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 3;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();
            var properties = new GrassCoverErosionInwardsInputContextBreakWaterProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock)
            };

            input.Attach(observerMock);

            RoundedDouble newBreakWaterHeight = new RoundedDouble(2, 9);
            const BreakWaterType newBreakWaterType = BreakWaterType.Wall;

            // Call
            properties.BreakWaterHeight = newBreakWaterHeight;
            properties.BreakWaterType = newBreakWaterType;
            properties.UseBreakWater = false;

            // Assert
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(newBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(newBreakWaterHeight, input.BreakWater.Height);
            mockRepository.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PropertyAttributes_Always_ReturnExpectedValues(bool withDikeProfile)
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();

            if (withDikeProfile)
            {
                input.DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0])
                {
                    BreakWater = new BreakWater(BreakWaterType.Caisson, 1.1)
                };
            }

            // Call
            var properties = new GrassCoverErosionInwardsInputContextBreakWaterProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            Assert.IsNotNull(useBreakWaterProperty);
            Assert.AreEqual(!withDikeProfile, useBreakWaterProperty.IsReadOnly);
            Assert.AreEqual("Gebruik", useBreakWaterProperty.DisplayName);
            Assert.AreEqual("Moet de dam worden gebruikt tijdens de berekening?", useBreakWaterProperty.Description);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsNotNull(breakWaterTypeProperty);
            Assert.AreEqual(!withDikeProfile, breakWaterTypeProperty.IsReadOnly);
            Assert.AreEqual("Type", breakWaterTypeProperty.DisplayName);
            Assert.AreEqual("Het type van de dam.", breakWaterTypeProperty.Description);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            Assert.IsNotNull(breakWaterHeightProperty);
            Assert.AreEqual(!withDikeProfile, breakWaterHeightProperty.IsReadOnly);
            Assert.AreEqual("Hoogte [m+NAP]", breakWaterHeightProperty.DisplayName);
            Assert.AreEqual("De hoogte van de dam [m+NAP].", breakWaterHeightProperty.Description);
            mockRepository.VerifyAll();
        }

        [TestCase(true, TestName = "PropertyAttributes_WithDikeProfileAndWaterState_ReturnValues(true)")]
        [TestCase(false, TestName = "PropertyAttributes_WithDikeProfileAndWaterState_ReturnValues(false)")]
        public void PropertyAttributes_WithDikeProfileAndSpecificUseBreakWaterState_ReturnExpectedValues(bool useBreakWaterState)
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var input = new GrassCoverErosionInwardsInput();

            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            if (useBreakWaterState)
            {
                dikeProfile.BreakWater = new BreakWater(BreakWaterType.Caisson, 1.1);
            }

            input.DikeProfile = dikeProfile;

            // Precondition
            Assert.AreEqual(useBreakWaterState, input.UseBreakWater);

            // Call
            var properties = new GrassCoverErosionInwardsInputContextBreakWaterProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSectionMock)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            Assert.IsNotNull(useBreakWaterProperty);
            Assert.IsFalse(useBreakWaterProperty.IsReadOnly);
            Assert.AreEqual("Gebruik", useBreakWaterProperty.DisplayName);
            Assert.AreEqual("Moet de dam worden gebruikt tijdens de berekening?", useBreakWaterProperty.Description);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsNotNull(breakWaterTypeProperty);
            Assert.AreEqual(!useBreakWaterState, breakWaterTypeProperty.IsReadOnly);
            Assert.AreEqual("Type", breakWaterTypeProperty.DisplayName);
            Assert.AreEqual("Het type van de dam.", breakWaterTypeProperty.Description);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            Assert.IsNotNull(breakWaterHeightProperty);
            Assert.AreEqual(!useBreakWaterState, breakWaterHeightProperty.IsReadOnly);
            Assert.AreEqual("Hoogte [m+NAP]", breakWaterHeightProperty.DisplayName);
            Assert.AreEqual("De hoogte van de dam [m+NAP].", breakWaterHeightProperty.Description);
            mockRepository.VerifyAll();
        }
    }
}