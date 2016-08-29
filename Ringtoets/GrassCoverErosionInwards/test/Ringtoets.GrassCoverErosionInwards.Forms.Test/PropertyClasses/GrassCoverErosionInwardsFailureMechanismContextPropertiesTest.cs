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
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismContextPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int lengthEffectPropertyIndex = 2;
        private const int frunupModelFactorPropertyIndex = 3;
        private const int fbFactorPropertyIndex = 4;
        private const int fnFactorPropertyIndex = 5;
        private const int fshallowModelFactorPropertyIndex = 6;
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
            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsFailureMechanismContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewFailureMechanismContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties();

            // Call
            properties.Data = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Assert
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayName, properties.Name);
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayCode, properties.Code);
            Assert.AreEqual(2, properties.LengthEffect);
            var generalInput = new GeneralGrassCoverErosionInwardsInput();

            Assert.AreEqual(generalInput.FbFactor.Mean, properties.FbFactor.Mean);
            Assert.AreEqual(generalInput.FbFactor.StandardDeviation, properties.FbFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FnFactor.Mean, properties.FnFactor.Mean);
            Assert.AreEqual(generalInput.FnFactor.StandardDeviation, properties.FnFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FrunupModelFactor.Mean, properties.FrunupModelFactor.Mean);
            Assert.AreEqual(generalInput.FrunupModelFactor.StandardDeviation, properties.FrunupModelFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FshallowModelFactor.Mean, properties.FshallowModelFactor.Mean);
            Assert.AreEqual(generalInput.FshallowModelFactor.StandardDeviation, properties.FshallowModelFactor.StandardDeviation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            int numberProperties = 1;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.Attach(observerMock);
            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties
            {
                Data = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock)
            };
            const int newLengthEffect = 10;

            // Call
            properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.AreEqual(newLengthEffect, failureMechanism.GeneralInput.N);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            var properties = new GrassCoverErosionInwardsFailureMechanismContextProperties
            {
                Data = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(8, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("De naam van het toetsspoor.", nameProperty.Description);

            PropertyDescriptor codeProperty = dynamicProperties[codePropertyIndex];
            Assert.IsNotNull(codeProperty);
            Assert.IsTrue(codeProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", codeProperty.Category);
            Assert.AreEqual("Label", codeProperty.DisplayName);
            Assert.AreEqual("Het label van het toetsspoor.", codeProperty.Description);

            PropertyDescriptor lengthEffectProperty = dynamicProperties[lengthEffectPropertyIndex];
            Assert.IsNotNull(lengthEffectProperty);
            Assert.IsFalse(lengthEffectProperty.IsReadOnly);
            Assert.AreEqual("Lengte-effect parameters", lengthEffectProperty.Category);
            Assert.AreEqual("N [-]", lengthEffectProperty.DisplayName);
            Assert.AreEqual("De parameter 'N' die gebruikt wordt om het lengte effect te bepalen in een semi-probabilistische beoordeling.", lengthEffectProperty.Description);

            PropertyDescriptor frunupModelFactorProperty = dynamicProperties[frunupModelFactorPropertyIndex];
            Assert.IsNotNull(frunupModelFactorProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(frunupModelFactorProperty.Converter);
            Assert.IsTrue(frunupModelFactorProperty.IsReadOnly);
            Assert.AreEqual("Modelfactoren", frunupModelFactorProperty.Category);
            Assert.AreEqual("Modelfactor Frunup [-]", frunupModelFactorProperty.DisplayName);
            Assert.AreEqual("De parameter 'Frunup' die gebruikt wordt in de berekening.", frunupModelFactorProperty.Description);

            PropertyDescriptor fbModelProperty = dynamicProperties[fbFactorPropertyIndex];
            Assert.IsNotNull(fbModelProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(fbModelProperty.Converter);
            Assert.IsTrue(fbModelProperty.IsReadOnly);
            Assert.AreEqual("Modelfactoren", fbModelProperty.Category);
            Assert.AreEqual("Modelfactor Fb [-]", fbModelProperty.DisplayName);
            Assert.AreEqual("De parameter 'Fb' die gebruikt wordt in de berekening.", fbModelProperty.Description);

            PropertyDescriptor fnFactorProperty = dynamicProperties[fnFactorPropertyIndex];
            Assert.IsNotNull(fnFactorProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(fnFactorProperty.Converter);
            Assert.IsTrue(fnFactorProperty.IsReadOnly);
            Assert.AreEqual("Modelfactoren", fnFactorProperty.Category);
            Assert.AreEqual("Modelfactor Fn [-]", fnFactorProperty.DisplayName);
            Assert.AreEqual("De parameter 'Fn' die gebruikt wordt in de berekening.", fnFactorProperty.Description);

            PropertyDescriptor fshallowProperty = dynamicProperties[fshallowModelFactorPropertyIndex];
            Assert.IsNotNull(fshallowProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(fshallowProperty.Converter);
            Assert.IsTrue(fshallowProperty.IsReadOnly);
            Assert.AreEqual("Modelfactoren", fshallowProperty.Category);
            Assert.AreEqual("Modelfactor Fondiep [-]", fshallowProperty.DisplayName);
            Assert.AreEqual("De parameter 'Fondiep' die gebruikt wordt in de berekening.", fshallowProperty.Description);

            mockRepository.VerifyAll();
        }
    }
}