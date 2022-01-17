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

using System;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Forms.PropertyClasses.StandAlone;

namespace Riskeer.Integration.Forms.Test.PropertyClasses.StandAlone
{
    public class StrengthStabilityLengthwiseConstructionFailurePathPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int codePropertyIndex = 1;
        private const int groupPropertyIndex = 2;
        private const int contributionPropertyIndex = 3;
        private const int inAssemblyPropertyIndex = 4;
        private const int nPropertyIndex = 5;
        private const int applyLengthEffectInSectionPropertyIndex = 6;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new StrengthStabilityLengthwiseConstructionFailurePathProperties(null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new StrengthStabilityLengthwiseConstructionFailurePathProperties(new StrengthStabilityLengthwiseConstructionFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double otherContribution = random.NextDouble();
            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism
            {
                InAssembly = random.NextBoolean()
            };
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetContributingFailureMechanisms()).Return(new IFailureMechanism[]
            {
                failureMechanism,
                new OtherFailureMechanism
                {
                    Contribution = otherContribution
                }
            });
            mocks.ReplayAll();

            // Call
            var properties = new StrengthStabilityLengthwiseConstructionFailurePathProperties(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StrengthStabilityLengthwiseConstructionFailureMechanism>>(properties);
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            Assert.AreEqual(failureMechanism.Group, properties.Group);
            Assert.AreEqual($"Overig ({otherContribution})", properties.Contribution);
            Assert.AreEqual(failureMechanism.InAssembly, properties.InAssembly);

            GeneralInput generalInput = failureMechanism.GeneralInput;
            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(generalInput.N,
                            properties.N,
                            properties.N.GetAccuracy());
            Assert.AreEqual(generalInput.ApplyLengthEffectInSection, properties.ApplyLengthEffectInSection);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InAssemblyTrue_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism
            {
                InAssembly = true
            };

            // Call
            var properties = new StrengthStabilityLengthwiseConstructionFailurePathProperties(failureMechanism, assessmentSection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string lengthEffectCategory = "Lengte-effect";

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

            PropertyDescriptor applySectionLengthInSectionProperty = dynamicProperties[applyLengthEffectInSectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(applySectionLengthInSectionProperty,
                                                                            lengthEffectCategory,
                                                                            "Toepassen lengte-effect binnen vak",
                                                                            "Geeft aan of het lengte-effect binnen een vak toegepast wordt.",
                                                                            true);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InAssemblyFalse_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism
            {
                InAssembly = false
            };

            // Call
            var properties = new StrengthStabilityLengthwiseConstructionFailurePathProperties(failureMechanism, assessmentSection);

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

            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(0.9)]
        [TestCase(20.1)]
        [TestCase(30)]
        public void N_SetInvalidValue_ThrowsArgumentOutOfRangeExceptionNoNotifications(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism();
            failureMechanism.Attach(observer);
            var properties = new StrengthStabilityLengthwiseConstructionFailurePathProperties(failureMechanism, assessmentSection);

            // Call
            void Call() => properties.N = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "De waarde voor 'N' moet in het bereik [1,00, 20,00] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(12)]
        [TestCase(20)]
        public void N_SetValidValue_SetsValueAndUpdatesObserver(double value)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism();
            failureMechanism.Attach(observer);
            var properties = new StrengthStabilityLengthwiseConstructionFailurePathProperties(failureMechanism, assessmentSection);

            // Call
            properties.N = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(value, failureMechanism.GeneralInput.N, failureMechanism.GeneralInput.N.GetAccuracy());

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicVisibleValidationMethod_DependingOnInAssembly_ReturnExpectedVisibility(bool inAssembly)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism
            {
                InAssembly = inAssembly
            };
            var properties = new StrengthStabilityLengthwiseConstructionFailurePathProperties(failureMechanism, assessmentSection);

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Name)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Code)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.Group)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(nameof(properties.InAssembly)));

            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.Contribution)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.N)));
            Assert.AreEqual(inAssembly, properties.DynamicVisibleValidationMethod(nameof(properties.ApplyLengthEffectInSection)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));

            mocks.VerifyAll();
        }
    }
}