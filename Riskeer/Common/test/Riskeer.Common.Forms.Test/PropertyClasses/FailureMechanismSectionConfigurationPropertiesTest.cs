// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismSectionConfigurationPropertiesTest
    {
        [Test]
        public void Constructor_SectionConfigurationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismSectionConfigurationProperties(null, double.NaN, double.NaN, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionConfiguration", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            double b = random.NextDouble();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new FailureMechanismSectionConfiguration(section);

            // Call
            var properties = new FailureMechanismSectionConfigurationProperties(sectionConfiguration, sectionStart, sectionEnd, b);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionProperties>(properties);
            Assert.AreSame(section, properties.Data);
            TestHelper.AssertTypeConverter<FailureMechanismSectionConfigurationProperties, ExpandableObjectConverter>();

            Assert.AreEqual(section.Name, properties.Name);
            Assert.AreEqual(section.Length, properties.Length, properties.Length.GetAccuracy());
            Assert.AreEqual(section.StartPoint, properties.StartPoint);
            Assert.AreEqual(section.EndPoint, properties.EndPoint);

            Assert.AreEqual(sectionStart, properties.SectionStart, properties.SectionStart.GetAccuracy());
            Assert.AreEqual(sectionEnd, properties.SectionEnd, properties.SectionEnd.GetAccuracy());

            Assert.AreEqual(sectionConfiguration.A, properties.ParameterA);
            Assert.AreEqual(2, properties.LengthEffectNRounded.NumberOfDecimalPlaces);
            Assert.AreEqual(sectionConfiguration.GetN(b), properties.LengthEffectNRounded, properties.LengthEffectNRounded.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new FailureMechanismSectionConfiguration(section);

            // Call
            var properties = new FailureMechanismSectionConfigurationProperties(sectionConfiguration, double.NaN, double.NaN, double.NaN);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Vaknaam",
                                                                            "De naam van het vak.",
                                                                            true);
            PropertyDescriptor sectionStartDistanceProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionStartDistanceProperty,
                                                                            "Algemeen",
                                                                            "Metrering van* [m]",
                                                                            "De afstand tussen het beginpunt van het vak en het begin van het traject, gemeten langs het traject in meters (afgerond).",
                                                                            true);
            PropertyDescriptor sectionEndDistanceProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionEndDistanceProperty,
                                                                            "Algemeen",
                                                                            "Metrering tot* [m]",
                                                                            "De afstand tussen het eindpunt van het vak en het begin van het traject, gemeten langs het traject in meters (afgerond).",
                                                                            true);
            PropertyDescriptor lengthProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthProperty,
                                                                            "Algemeen",
                                                                            "Lengte* [m]",
                                                                            "De totale lengte van het vak in meters (afgerond).",
                                                                            true);
            PropertyDescriptor startPointProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(startPointProperty,
                                                                            "Algemeen",
                                                                            "Beginpunt",
                                                                            "Beginpunt van het vak (X-coördinaat, Y-coördinaat).",
                                                                            true);
            PropertyDescriptor endPointProperty = dynamicProperties[5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(endPointProperty,
                                                                            "Algemeen",
                                                                            "Eindpunt",
                                                                            "Eindpunt van het vak (X-coördinaat, Y-coördinaat).",
                                                                            true);

            PropertyDescriptor parameterAProperty = dynamicProperties[6];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(parameterAProperty,
                                                                            "Algemeen",
                                                                            "a [-]",
                                                                            "Mechanismegevoelige fractie van het dijkvak.");

            PropertyDescriptor lengthEffectNRoundedProperty = dynamicProperties[7];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(lengthEffectNRoundedProperty,
                                                                            "Algemeen",
                                                                            "Nvak* [-]",
                                                                            "De parameter 'Nvak*' die het lengte-effect beschrijft in de berekening van de faalkans per vak in de semi-probabilistische toets.",
                                                                            true);
        }

        [Test]
        public void ParameterA_SetNewValue_NotifyObservers()
        {
            // Setup
            var random = new Random(21);
            RoundedDouble newParameterAValue = random.NextRoundedDouble();

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new FailureMechanismSectionConfiguration(section);
            sectionConfiguration.Attach(observer);

            var properties = new FailureMechanismSectionConfigurationProperties(sectionConfiguration, double.NaN, double.NaN, double.NaN);

            // Call
            properties.ParameterA = newParameterAValue;

            // Assert
            Assert.AreEqual(newParameterAValue, sectionConfiguration.A, sectionConfiguration.A.GetAccuracy());
            mocks.VerifyAll();
        }
    }
}