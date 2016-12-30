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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data.Properties;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int lengthEffectPropertyIndex = 2;
        private const int aPropertyIndex = 3;
        private const int bPropertyIndex = 4;
        private const int cPropertyIndex = 5;

        [Test]
        public void Constructor_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = CreateSimpleHandler(mockRepository);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsFailureMechanismProperties(null, changeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithoutChangeHandler_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsFailureMechanismProperties(new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("changeHandler", paramName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = CreateSimpleHandler(mockRepository);
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionOutwardsFailureMechanism>>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(Resources.GrassCoverErosionOutwardsFailureMechanism_DisplayName, properties.Name);
            Assert.AreEqual(Resources.GrassCoverErosionOutwardsFailureMechanism_Code, properties.Code);
            Assert.AreEqual(failureMechanism.GeneralInput.GeneralWaveConditionsInput.A, properties.A);
            Assert.AreEqual(failureMechanism.GeneralInput.GeneralWaveConditionsInput.B, properties.B);
            Assert.AreEqual(failureMechanism.GeneralInput.GeneralWaveConditionsInput.C, properties.C);
            Assert.AreEqual(2, properties.LengthEffect);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void LengthEffect_SetValidValue_UpdateDataAndNotifyObservers(int newLengthEffect)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var observableMock = mockRepository.StrictMock<IObservable>();
            observableMock.Expect(o => o.NotifyObservers());

            var changeHandler = mockRepository.StrictMock<IGrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler>();
            changeHandler.Expect(h => h.ConfirmPropertyChange()).Return(true);
            changeHandler.Expect(h => h.PropertyChanged(Arg<GrassCoverErosionOutwardsFailureMechanism>.Is.NotNull)).Return(new[] { observableMock });

            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.Attach(observerMock);

            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.AreEqual(newLengthEffect, failureMechanism.GeneralInput.N);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void LengthEffect_SetValidValueNoConfirmation_NoValueChangeNoUpdates(int newLengthEffect)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();

            var changeHandler = mockRepository.StrictMock<IGrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler>();
            changeHandler.Expect(h => h.ConfirmPropertyChange()).Return(false);

            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.Attach(observerMock);

            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(failureMechanism, changeHandler);
            var oldValue = properties.LengthEffect;

            // Call
            properties.LengthEffect = newLengthEffect;

            // Assert
            Assert.AreEqual(oldValue, failureMechanism.GeneralInput.N);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = CreateSimpleHandler(mockRepository);
            mockRepository.ReplayAll();

            // Call
            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(
                new GrassCoverErosionOutwardsFailureMechanism(),
                changeHandler);

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });
            Assert.AreEqual(6, dynamicProperties.Count);

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
            Assert.AreEqual("De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in een semi-probabilistische beoordeling.", lengthEffectProperty.Description);

            PropertyDescriptor aProperty = dynamicProperties[aPropertyIndex];
            Assert.IsNotNull(aProperty);
            Assert.IsTrue(aProperty.IsReadOnly);
            Assert.AreEqual("Modelinstellingen", aProperty.Category);
            Assert.AreEqual("a", aProperty.DisplayName);
            Assert.AreEqual("De waarde van de parameter 'a' in de berekening voor golf condities.", aProperty.Description);

            PropertyDescriptor bProperty = dynamicProperties[bPropertyIndex];
            Assert.IsNotNull(bProperty);
            Assert.IsTrue(bProperty.IsReadOnly);
            Assert.AreEqual("Modelinstellingen", bProperty.Category);
            Assert.AreEqual("b", bProperty.DisplayName);
            Assert.AreEqual("De waarde van de parameter 'b' in de berekening voor golf condities.", bProperty.Description);

            PropertyDescriptor cProperty = dynamicProperties[cPropertyIndex];
            Assert.IsNotNull(cProperty);
            Assert.IsTrue(cProperty.IsReadOnly);
            Assert.AreEqual("Modelinstellingen", cProperty.Category);
            Assert.AreEqual("c", cProperty.DisplayName);
            Assert.AreEqual("De waarde van de parameter 'c' in de berekening voor golf condities.", cProperty.Description);
            mockRepository.VerifyAll();
        }
        
        private IGrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler CreateSimpleHandler(MockRepository mockRepository)
        {
            var handler = mockRepository.Stub<IGrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler>();
            handler.Stub(h => h.ConfirmPropertyChange()).Return(true);
            handler.Stub(h => h.PropertyChanged(Arg<GrassCoverErosionOutwardsFailureMechanism>.Is.NotNull)).Return(Enumerable.Empty<IObservable>());

            return handler;
        }
    }
}