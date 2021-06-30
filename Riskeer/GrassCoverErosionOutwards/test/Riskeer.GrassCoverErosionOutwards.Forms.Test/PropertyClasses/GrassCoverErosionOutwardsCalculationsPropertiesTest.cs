﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.ComponentModel;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.PropertyClasses;
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
            var properties = new GrassCoverErosionOutwardsCalculationsProperties(new GrassCoverErosionOutwardsFailureMechanism(), changeHandler);

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
            Assert.IsInstanceOf<ExpandableObjectConverter>(waveRunUpProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveRunUpProperty,
                                                                            modelSettingsCategory,
                                                                            "Golfoploop",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor golfoploop.",
                                                                            true);

            PropertyDescriptor waveImpactProperty = dynamicProperties[waveImpactPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(waveImpactProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveImpactProperty,
                                                                            modelSettingsCategory,
                                                                            "Golfklap",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor golfklap zonder invloed van de golfinvalshoek.",
                                                                            true);

            PropertyDescriptor tailorMadeWaveImpactProperty = dynamicProperties[tailorMadeWaveImpactPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(tailorMadeWaveImpactProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(tailorMadeWaveImpactProperty,
                                                                            modelSettingsCategory,
                                                                            "Golfklap voor toets op maat",
                                                                            "De modelinstellingen voor het berekenen van golfcondities voor golfklap met invloed van de golfinvalshoek, voor toets op maat.",
                                                                            true);
        }
    }
}