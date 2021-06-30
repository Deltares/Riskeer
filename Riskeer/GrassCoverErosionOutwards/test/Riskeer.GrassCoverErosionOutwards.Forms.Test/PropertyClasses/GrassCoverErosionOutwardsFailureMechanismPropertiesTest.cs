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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PropertyClasses;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismPropertiesTest
    {
        private const int namePropertyIndex = 4;
        private const int codePropertyIndex = 3;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 1;
        private const int nPropertyIndex = 0;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Setup
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            void Call() => new GrassCoverErosionOutwardsFailureMechanismProperties(null, new GrassCoverErosionOutwardsFailureMechanismProperties.ConstructionProperties(), handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("data", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            void Call() => new GrassCoverErosionOutwardsFailureMechanismProperties(new GrassCoverErosionOutwardsFailureMechanism(), null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ChangeHandlerNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionOutwardsFailureMechanismProperties(new GrassCoverErosionOutwardsFailureMechanism(), new GrassCoverErosionOutwardsFailureMechanismProperties.ConstructionProperties(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(failureMechanism, new GrassCoverErosionOutwardsFailureMechanismProperties.ConstructionProperties(), handler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionOutwardsFailureMechanism>>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);

            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(failureMechanism.GeneralInput.N,
                            properties.N,
                            properties.N.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(failureMechanism, new GrassCoverErosionOutwardsFailureMechanismProperties.ConstructionProperties
            {
                NamePropertyIndex = namePropertyIndex,
                CodePropertyIndex = codePropertyIndex,
                GroupPropertyIndex = groupPropertyIndex,
                ContributionPropertyIndex = contributionPropertyIndex,
                NPropertyIndex = nPropertyIndex
            }, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

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

            PropertyDescriptor nProperty = dynamicProperties[nPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling.");

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
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var handler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<GrassCoverErosionOutwardsFailureMechanism, double>(
                failureMechanism,
                newN,
                new[]
                {
                    observable
                });

            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(
                failureMechanism,
                new GrassCoverErosionOutwardsFailureMechanismProperties.ConstructionProperties(), handler);

            // Call
            void Call() => properties.N = (RoundedDouble) newN;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
            Assert.IsTrue(handler.Called);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(10.0)]
        [TestCase(20.0)]
        public void N_SetValidValue_UpdateDataAndNotifyObservers(double newN)
        {
            // Setup
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var handler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<GrassCoverErosionOutwardsFailureMechanism, double>(
                failureMechanism,
                newN,
                new[]
                {
                    observable
                });

            var properties = new GrassCoverErosionOutwardsFailureMechanismProperties(
                failureMechanism,
                new GrassCoverErosionOutwardsFailureMechanismProperties.ConstructionProperties(), handler);

            // Call
            properties.N = (RoundedDouble) newN;

            // Assert
            Assert.AreEqual(newN, failureMechanism.GeneralInput.N);
            Assert.IsTrue(handler.Called);

            mocks.VerifyAll();
        }
    }
}