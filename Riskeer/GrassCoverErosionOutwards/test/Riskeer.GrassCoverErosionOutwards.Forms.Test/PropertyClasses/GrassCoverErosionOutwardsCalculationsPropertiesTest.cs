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
    public class GrassCoverErosionOutwardsCalculationsPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int nPropertyIndex = 4;
        private const int waveRunUpPropertyIndex = 5;
        private const int waveImpactPropertyIndex = 6;
        private const int tailorMadeWaveImpactPropertyIndex = 7;

        [Test]
        public void Constructor_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new GrassCoverErosionOutwardsCalculationsProperties(null, changeHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithoutChangeHandler_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionOutwardsCalculationsProperties(new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            var properties = new GrassCoverErosionOutwardsCalculationsProperties(failureMechanism, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionOutwardsFailureMechanism>>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);

            GeneralGrassCoverErosionOutwardsInput generalInput = failureMechanism.GeneralInput;
            Assert.AreSame(generalInput.GeneralWaveRunUpWaveConditionsInput, properties.WaveRunUp.Data);
            Assert.AreSame(generalInput.GeneralWaveImpactWaveConditionsInput, properties.WaveImpact.Data);
            Assert.AreSame(generalInput.GeneralTailorMadeWaveImpactWaveConditionsInput, properties.TailorMadeWaveImpact.Data);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var changeHandler = mockRepository.Stub<IFailureMechanismPropertyChangeHandler<GrassCoverErosionOutwardsFailureMechanism>>();
            mockRepository.ReplayAll();

            // Call
            var properties = new GrassCoverErosionOutwardsCalculationsProperties(
                new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = true
                },
                changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string modelSettingsCategory = "Modelinstellingen";

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
                                                                            "Lengte-effect parameters",
                                                                            "N [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect mee te nemen in de beoordeling.");

            PropertyDescriptor waveRunUpProperty = dynamicProperties[waveRunUpPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveRunUpProperty,
                                                                            modelSettingsCategory,
                                                                            "Golfoploop",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor golfoploop.",
                                                                            true);

            PropertyDescriptor waveImpactProperty = dynamicProperties[waveImpactPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveImpactProperty,
                                                                            modelSettingsCategory,
                                                                            "Golfklap",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor golfklap zonder invloed van de golfinvalshoek.",
                                                                            true);

            PropertyDescriptor tailorMadeWaveImpactProperty = dynamicProperties[tailorMadeWaveImpactPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(tailorMadeWaveImpactProperty,
                                                                            modelSettingsCategory,
                                                                            "Golfklap voor toets op maat",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor golfklap met invloed van de golfinvalshoek, voor toets op maat.",
                                                                            true);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(-1.0)]
        [TestCase(-20.0)]
        public void N_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(double newN)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observable = mockRepository.StrictMock<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<GrassCoverErosionOutwardsFailureMechanism, double>(
                failureMechanism,
                newN,
                new[]
                {
                    observable
                });

            var properties = new GrassCoverErosionOutwardsCalculationsProperties(failureMechanism, changeHandler);

            // Call
            void Call() => properties.N = (RoundedDouble) newN;

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Call);
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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var changeHandler = new FailureMechanismSetPropertyValueAfterConfirmationParameterTester<GrassCoverErosionOutwardsFailureMechanism, double>(
                failureMechanism,
                newN,
                new[]
                {
                    observable
                });

            var properties = new GrassCoverErosionOutwardsCalculationsProperties(failureMechanism, changeHandler);

            // Call
            properties.N = (RoundedDouble) newN;

            // Assert
            Assert.AreEqual(newN, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());
            Assert.IsTrue(changeHandler.Called);
            mockRepository.VerifyAll();
        }
    }
}