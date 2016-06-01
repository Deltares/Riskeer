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

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class BreakWaterPropertiesTest
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
            var properties = new BreakWaterProperties();

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
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(new ProbabilityAssessmentInput());
            var inputMock = mockRepository.StrictMock<GrassCoverErosionInwardsInput>();
            mockRepository.ReplayAll();

            var properties = new BreakWaterProperties();

            // Call
            properties.Data = new GrassCoverErosionInwardsInputContext(inputMock, calculationMock, failureMechanismMock, assessmentSectionMock);

            // Assert
            Assert.IsFalse(properties.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Caisson, properties.BreakWaterType);
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
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>(new ProbabilityAssessmentInput());
            mockRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            input.Attach(observerMock);
            var properties = new BreakWaterProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(input, calculationMock, failureMechanismMock, assessmentSectionMock)
            };

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
            var properties = new BreakWaterProperties
            {
                Data = new GrassCoverErosionInwardsInputContext(inputMock, calculationMock, failureMechanismMock, assessmentSectionMock)
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            Assert.IsNotNull(useBreakWaterProperty);
            Assert.IsFalse(useBreakWaterProperty.IsReadOnly);
            Assert.AreEqual("Aanwezig", useBreakWaterProperty.DisplayName);
            Assert.AreEqual("Is er een havendam aanwezig?", useBreakWaterProperty.Description);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsNotNull(breakWaterTypeProperty);
            Assert.IsFalse(breakWaterTypeProperty.IsReadOnly);
            Assert.AreEqual("Type", breakWaterTypeProperty.DisplayName);
            Assert.AreEqual("Het type van de havendam.", breakWaterTypeProperty.Description);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            Assert.IsNotNull(breakWaterHeightProperty);
            Assert.IsFalse(breakWaterHeightProperty.IsReadOnly);
            Assert.AreEqual("Hoogte [m+NAP]", breakWaterHeightProperty.DisplayName);
            Assert.AreEqual("De hoogte van de havendam [m+NAP].", breakWaterHeightProperty.Description);
            mockRepository.VerifyAll();
        }
    }
}