// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PropertyClasses;

namespace Riskeer.DuneErosion.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DuneErosionFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int isRelevantPropertyIndex = 4;
        private const int nPropertyIndex = 5;

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ExpectedValues(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = isRelevant
            };

            // Call
            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<DuneErosionFailureMechanism>>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);
            Assert.AreEqual(isRelevant, properties.IsRelevant);

            Assert.AreEqual(failureMechanism.GeneralInput.N, properties.N);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DuneErosionFailureMechanismProperties(null, changeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("data", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PropertyChangeHandlerNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate call = () => new DuneErosionFailureMechanismProperties(failureMechanism, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("propertyChangeHandler", paramName);
        }

        [Test]
        public void Constructor_IsRelevantTrue_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = true
            };

            // Call
            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string lengthEffectParameterCategory = "Lengte-effect parameters";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor groupProperty = dynamicProperties[groupPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(groupProperty,
                                                                            generalCategory,
                                                                            "Groep",
                                                                            "De groep waar het toetsspoor toe behoort.",
                                                                            true);

            PropertyDescriptor contributionProperty = dynamicProperties[contributionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(contributionProperty,
                                                                            generalCategory,
                                                                            "Faalkansbijdrage [%]",
                                                                            "Procentuele bijdrage van dit toetsspoor aan de totale overstromingskans van het traject.",
                                                                            true);

            PropertyDescriptor isRelevantProperty = dynamicProperties[isRelevantPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);

            PropertyDescriptor nProperty = dynamicProperties[nPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectParameterCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling.");
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_IsRelevantFalse_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = false
            };

            // Call
            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor codeProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(codeProperty,
                                                                            generalCategory,
                                                                            "Label",
                                                                            "Het label van het toetsspoor.",
                                                                            true);

            PropertyDescriptor groupProperty = dynamicProperties[groupPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(groupProperty,
                                                                            generalCategory,
                                                                            "Groep",
                                                                            "De groep waar het toetsspoor toe behoort.",
                                                                            true);

            PropertyDescriptor isRelevantProperty = dynamicProperties[isRelevantPropertyIndex - 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isRelevantProperty,
                                                                            generalCategory,
                                                                            "Is relevant",
                                                                            "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                            true);
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(0.0)]
        [TestCase(-1.0)]
        [TestCase(-20.0)]
        public void N_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(double newN)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observable = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<DuneErosionFailureMechanism, RoundedDouble>(
                failureMechanism,
                (RoundedDouble) newN,
                new[]
                {
                    observable
                });

            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            TestDelegate test = () => properties.N = (RoundedDouble) newN;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        public void N_SetValidValue_UpdateDataAndNotifyObservers(double newN)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observable = mockRepository.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<DuneErosionFailureMechanism, RoundedDouble>(
                failureMechanism,
                (RoundedDouble) newN,
                new[]
                {
                    observable
                });

            var properties = new DuneErosionFailureMechanismProperties(failureMechanism, changeHandler);

            // Call
            properties.N = (RoundedDouble) newN;

            // Assert
            Assert.AreEqual(newN, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnRelevancy_ReturnExpectedVisibility(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IFailureMechanismPropertyChangeHandler<DuneErosionFailureMechanism>>();
            mocks.ReplayAll();

            var properties = new DuneErosionFailureMechanismProperties(
                new DuneErosionFailureMechanism
                {
                    IsRelevant = isRelevant
                },
                changeHandler);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Group)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.IsRelevant)));

            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.Contribution)));
            Assert.AreEqual(isRelevant, properties.DynamicVisibleValidationMethod(nameof(properties.N)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));

            mocks.VerifyAll();
        }
    }
}