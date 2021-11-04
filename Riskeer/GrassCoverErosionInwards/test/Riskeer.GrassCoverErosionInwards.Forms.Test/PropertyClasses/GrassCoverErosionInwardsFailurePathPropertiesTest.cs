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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailurePathPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int inAssemblyPropertyIndex = 4;
        private const int nPropertyIndex = 5;

        [Test]
        public void Constructor_ChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionInwardsFailurePathProperties(new GrassCoverErosionInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism>>();
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                InAssembly = random.NextBoolean()
            };

            // Call
            var properties = new GrassCoverErosionInwardsFailurePathProperties(failureMechanism, handler);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionInwardsFailureMechanismProperties>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);
            Assert.AreEqual(failureMechanism.InAssembly, properties.InAssembly);

            GeneralGrassCoverErosionInwardsInput generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(generalInput.N,
                            properties.N,
                            properties.N.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InAssemblyTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                InAssembly = true
            };

            // Call
            var properties = new GrassCoverErosionInwardsFailurePathProperties(failureMechanism, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect parameters";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van het toetsspoor.",
                                                                            true);

            PropertyDescriptor labelProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(labelProperty,
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

            PropertyDescriptor inAssemblyProperty = dynamicProperties[inAssemblyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inAssemblyProperty,
                                                                            generalCategory,
                                                                            "In assemblage",
                                                                            "Geeft aan of dit faalpad wordt meegenomen in de assemblage.",
                                                                            true);

            PropertyDescriptor nProperty = dynamicProperties[nPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling.");
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InAssemblyFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                InAssembly = false
            };

            // Call
            var properties = new GrassCoverErosionInwardsFailurePathProperties(failureMechanism, handler);

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

            PropertyDescriptor labelProperty = dynamicProperties[codePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(labelProperty,
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

            PropertyDescriptor inAssemblyProperty = dynamicProperties[inAssemblyPropertyIndex - 1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inAssemblyProperty,
                                                                            generalCategory,
                                                                            "In assemblage",
                                                                            "Geeft aan of dit faalpad wordt meegenomen in de assemblage.",
                                                                            true);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(0.0)]
        [TestCase(-1.0)]
        [TestCase(-20.0)]
        public void N_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(double newN)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<GrassCoverErosionInwardsFailureMechanism, double>(
                failureMechanism,
                newN,
                new[]
                {
                    observable
                });

            var properties = new GrassCoverErosionInwardsFailurePathProperties(
                failureMechanism,
                changeHandler);

            // Call
            void Call() => properties.N = (RoundedDouble) newN;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
            Assert.IsTrue(changeHandler.Called);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(10.0)]
        [TestCase(20.0)]
        public void N_SetValidValue_UpdateDataAndNotifyObservers(double newN)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<GrassCoverErosionInwardsFailureMechanism, double>(
                failureMechanism,
                newN,
                new[]
                {
                    observable
                });

            var properties = new GrassCoverErosionInwardsFailurePathProperties(
                failureMechanism,
                changeHandler);

            // Call
            properties.N = (RoundedDouble) newN;

            // Assert
            Assert.AreEqual(newN, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
            Assert.IsTrue(changeHandler.Called);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnInAssembly_ReturnExpectedVisibility(bool inAssembly)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism>>();
            mocks.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                InAssembly = inAssembly
            };
            var properties = new GrassCoverErosionInwardsFailurePathProperties(grassCoverErosionInwardsFailureMechanism, handler);

            // Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Group)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.InAssembly)));
            
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.N)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.Contribution)));

            mocks.VerifyAll();
        }
    }
}