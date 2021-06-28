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
    public class GrassCoverErosionInwardsCalculationsPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int nPropertyIndex = 4;
        private const int frunupModelFactorPropertyIndex = 5;
        private const int fbFactorPropertyIndex = 6;
        private const int fnFactorPropertyIndex = 7;
        private const int fshallowModelFactorPropertyIndex = 8;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }
        
        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            var properties = new GrassCoverErosionInwardsCalculationsProperties(
                failureMechanism,
                handler);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionInwardsFailureMechanismProperties>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);

            GeneralGrassCoverErosionInwardsInput generalInput = failureMechanism.GeneralInput;

            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(generalInput.N, properties.N, properties.N.GetAccuracy());

            Assert.AreEqual(generalInput.FbFactor.Mean, properties.FbFactor.Mean);
            Assert.AreEqual(generalInput.FbFactor.StandardDeviation, properties.FbFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FnFactor.Mean, properties.FnFactor.Mean);
            Assert.AreEqual(generalInput.FnFactor.StandardDeviation, properties.FnFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FrunupModelFactor.Mean, properties.FrunupModelFactor.Mean);
            Assert.AreEqual(generalInput.FrunupModelFactor.StandardDeviation, properties.FrunupModelFactor.StandardDeviation);

            Assert.AreEqual(generalInput.FshallowModelFactor.Mean, properties.FshallowModelFactor.Mean);
            Assert.AreEqual(generalInput.FshallowModelFactor.StandardDeviation, properties.FshallowModelFactor.StandardDeviation);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var handler = mocks.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionInwardsFailureMechanism>>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            var properties = new GrassCoverErosionInwardsCalculationsProperties(
                failureMechanism,
                handler);

            // Assert
            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect parameters";
            const string modelSettingsCategory = "Modelinstellingen";

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(9, dynamicProperties.Count);

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

            PropertyDescriptor nProperty = dynamicProperties[nPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling.");

            PropertyDescriptor frunupModelFactorProperty = dynamicProperties[frunupModelFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(frunupModelFactorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(frunupModelFactorProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Frunup [-]",
                                                                            "De parameter 'Frunup' die gebruikt wordt in de berekening.",
                                                                            true);

            PropertyDescriptor fbModelProperty = dynamicProperties[fbFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(fbModelProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fbModelProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Fb [-]",
                                                                            "De parameter 'Fb' die gebruikt wordt in de berekening.",
                                                                            true);

            PropertyDescriptor fnFactorProperty = dynamicProperties[fnFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(fnFactorProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fnFactorProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Fn [-]",
                                                                            "De parameter 'Fn' die gebruikt wordt in de berekening.",
                                                                            true);

            PropertyDescriptor fshallowProperty = dynamicProperties[fshallowModelFactorPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(fshallowProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(fshallowProperty,
                                                                            modelSettingsCategory,
                                                                            "Modelfactor Fondiep [-]",
                                                                            "De parameter 'Fondiep' die gebruikt wordt in de berekening.",
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

            var properties = new GrassCoverErosionInwardsCalculationsProperties(
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

            var properties = new GrassCoverErosionInwardsCalculationsProperties(
                failureMechanism,
                changeHandler);

            // Call
            properties.N = (RoundedDouble) newN;

            // Assert
            Assert.AreEqual(newN, failureMechanism.GeneralInput.N);
            Assert.IsTrue(changeHandler.Called);

            mocks.VerifyAll();
        }
    }
}