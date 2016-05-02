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

            // Call
            const int newLengthEffect = 10;
            properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.AreEqual(newLengthEffect, failureMechanism.NormProbabilityInput.N);
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
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("De naam van het toetsspoor.", nameProperty.Description);

            PropertyDescriptor codeProperty = dynamicProperties[codePropertyIndex];
            Assert.IsNotNull(codeProperty);
            Assert.IsTrue(codeProperty.IsReadOnly);
            Assert.AreEqual("Label", codeProperty.DisplayName);
            Assert.AreEqual("Het label van het toetsspoor.", codeProperty.Description);

            PropertyDescriptor lengthEffectProperty = dynamicProperties[lengthEffectPropertyIndex];
            Assert.IsNotNull(lengthEffectProperty);
            Assert.IsFalse(lengthEffectProperty.IsReadOnly);
            Assert.AreEqual("N", lengthEffectProperty.DisplayName);
            Assert.AreEqual("De parameter 'N' die gebruikt wordt voor het lengte effect in de berekening.", lengthEffectProperty.Description);

            PropertyDescriptor mz2Property = dynamicProperties[mz2PropertyIndex];
            Assert.IsNotNull(mz2Property);
            Assert.IsTrue(mz2Property.IsReadOnly);
            Assert.AreEqual("mz2 [-]", mz2Property.DisplayName);
            Assert.AreEqual("De parameter 'mz2' die gebruikt wordt in de berekening.", mz2Property.Description);

            PropertyDescriptor fbProperty = dynamicProperties[fbPropertyIndex];
            Assert.IsNotNull(fbProperty);
            Assert.IsTrue(fbProperty.IsReadOnly);
            Assert.AreEqual("fb [-]", fbProperty.DisplayName);
            Assert.AreEqual("De parameter 'fb' die gebruikt wordt in de berekening.", fbProperty.Description);

            PropertyDescriptor fnProperty = dynamicProperties[fnPropertyIndex];
            Assert.IsNotNull(fnProperty);
            Assert.IsTrue(fnProperty.IsReadOnly);
            Assert.AreEqual("fn [-]", fnProperty.DisplayName);
            Assert.AreEqual("De parameter 'fn' die gebruikt wordt in de berekening.", fnProperty.Description);

            PropertyDescriptor fshallowProperty = dynamicProperties[fshallowPropertyIndex];
            Assert.IsNotNull(fshallowProperty);
            Assert.IsTrue(fshallowProperty.IsReadOnly);
            Assert.AreEqual("f ondiep [-]", fshallowProperty.DisplayName);
            Assert.AreEqual("De parameter 'f ondiep' die gebruikt wordt in de berekening.", fshallowProperty.Description);
        }

        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int lengthEffectPropertyIndex = 2;
        private const int mz2PropertyIndex = 3;
        private const int fbPropertyIndex = 4;
        private const int fnPropertyIndex = 5;
        private const int fshallowPropertyIndex = 6;
    }
}