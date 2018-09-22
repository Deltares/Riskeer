// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.Probability;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismSectionProbabilityAssessmentPropertiesTest
    {
        [Test]
        public void Constructor_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            TestDelegate call = () => new FailureMechanismSectionProbabilityAssessmentProperties(section, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("probabilityAssessmentInput", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(0.5, 100);
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var properties = new FailureMechanismSectionProbabilityAssessmentProperties(section, probabilityAssessmentInput);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionProperties>(properties);
            Assert.AreSame(section, properties.Data);
            TestHelper.AssertTypeConverter<FailureMechanismSectionProbabilityAssessmentProperties, ExpandableObjectConverter>();

            Assert.AreEqual(2, properties.N.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, properties.N, properties.N.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var probabilityAssessmentInput = new TestProbabilityAssessmentInput(0.5, 100);
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var properties = new FailureMechanismSectionProbabilityAssessmentProperties(section, probabilityAssessmentInput);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Vaknaam",
                                                                            "De naam van het vak.",
                                                                            true);
            PropertyDescriptor sectionStartDistanceProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionStartDistanceProperty,
                                                                            "Algemeen",
                                                                            "Metrering* van [m]",
                                                                            "De afstand tussen het beginpunt van het vak en het begin van het traject, gemeten langs het traject in meters (afgerond).",
                                                                            true);
            PropertyDescriptor sectionEndDistanceProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sectionEndDistanceProperty,
                                                                            "Algemeen",
                                                                            "Metrering* tot [m]",
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

            PropertyDescriptor nProperty = dynamicProperties[6];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nProperty,
                                                                            "Algemeen",
                                                                            "Nvak* [-]",
                                                                            "Vakspecifieke waarde voor het in rekening brengen van " +
                                                                            "het lengte-effect tijdens assemblage (afgerond).",
                                                                            true);
        }
    }
}