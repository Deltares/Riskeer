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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses;

namespace Riskeer.WaveImpactAsphaltCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailurePathPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int inAssemblyPropertyIndex = 4;
        private const int sectionLengthPropertyIndex = 5;
        private const int deltaLPropertyIndex = 6;
        private const int nPropertyIndex = 7;
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
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new WaveImpactAsphaltCoverFailurePathProperties(new WaveImpactAsphaltCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                InAssembly = random.NextBoolean()
            };

            // Call
            var properties = new WaveImpactAsphaltCoverFailurePathProperties(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<WaveImpactAsphaltCoverFailureMechanismProperties>(properties);
            Assert.AreSame(failureMechanism, properties.Data);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual(failureMechanism.Contribution, properties.Contribution);
            Assert.AreEqual(failureMechanism.InAssembly, properties.InAssembly);

            Assert.AreEqual(2, properties.SectionLength.NumberOfDecimalPlaces);
            Assert.AreEqual(assessmentSection.ReferenceLine.Length,
                            properties.SectionLength,
                            properties.SectionLength.GetAccuracy());

            GeneralWaveImpactAsphaltCoverInput generalWaveImpactAsphaltCoverInput = failureMechanism.GeneralWaveImpactAsphaltCoverInput;
            Assert.AreEqual(2, properties.DeltaL.NumberOfDecimalPlaces);
            Assert.AreEqual(generalWaveImpactAsphaltCoverInput.DeltaL,
                            properties.DeltaL,
                            properties.DeltaL.GetAccuracy());

            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(generalWaveImpactAsphaltCoverInput.GetN(assessmentSection.ReferenceLine.Length),
                            properties.N,
                            properties.N.GetAccuracy());
        }

        [Test]
        public void Constructor_InAssemblyTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var properties = new WaveImpactAsphaltCoverFailurePathProperties(new WaveImpactAsphaltCoverFailureMechanism(), assessmentSection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect parameters";

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

            PropertyDescriptor inAssemblyProperty = dynamicProperties[inAssemblyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(inAssemblyProperty,
                                                                            generalCategory,
                                                                            "In assemblage",
                                                                            "Geeft aan of dit faalpad wordt meegenomen in de assemblage.",
                                                                            true);

            PropertyDescriptor sectionLength = dynamicProperties[sectionLengthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionLength,
                                                                            lengthEffectCategory,
                                                                            "Lengte* [m]",
                                                                            "Totale lengte van het traject in meters (afgerond).",
                                                                            true);

            PropertyDescriptor deltaLProperty = dynamicProperties[deltaLPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(deltaLProperty,
                                                                            lengthEffectCategory,
                                                                            "ΔL [m]",
                                                                            "Lengte van onafhankelijke dijkstrekkingen voor dit toetsspoor.");

            PropertyDescriptor nProperty = dynamicProperties[nPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            lengthEffectCategory,
                                                                            "N* [-]",
                                                                            "De parameter 'N' die gebruikt wordt om het lengte-effect " +
                                                                            "mee te nemen in de beoordeling (afgerond).",
                                                                            true);
        }

        [Test]
        public void Constructor_InAssemblyFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                InAssembly = false
            };

            // Call
            var properties = new WaveImpactAsphaltCoverFailurePathProperties(failureMechanism, assessmentSection);

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
        public void DeltaL_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(double newN)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new WaveImpactAsphaltCoverFailurePathProperties(
                failureMechanism,
                assessmentSection);

            // Call
            void Call() => properties.DeltaL = (RoundedDouble) newN;

            // Assert
            const string expectedMessage = "De waarde voor 'ΔL' moet groter zijn dan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(10.0)]
        [TestCase(20.0)]
        public void DeltaL_SetValidValue_UpdateDataAndNotifyObservers(double newN)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.Attach(observer);

            var properties = new WaveImpactAsphaltCoverFailurePathProperties(
                failureMechanism,
                assessmentSection);

            // Call
            properties.DeltaL = (RoundedDouble) newN;

            // Assert
            Assert.AreEqual(newN, failureMechanism.GeneralWaveImpactAsphaltCoverInput.DeltaL);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnInAssembly_ReturnExpectedVisibility(bool inAssembly)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                InAssembly = inAssembly
            };
            var properties = new WaveImpactAsphaltCoverFailurePathProperties(failureMechanism, assessmentSection);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Group)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.InAssembly)));

            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.Contribution)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.DeltaL)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.SectionLength)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.N)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}