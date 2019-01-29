// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsOutputPropertiesTest
    {
        [Test]
        public void Constructor_OutputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsOutputProperties(null, new MacroStabilityInwardsFailureMechanism(),
                                                                                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsOutputProperties(output, null,
                                                                                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsOutputProperties(output, new MacroStabilityInwardsFailureMechanism(),
                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            // Call
            var properties = new MacroStabilityInwardsOutputProperties(output, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsOutput>>(properties);
            Assert.AreSame(output, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                Contribution = 10
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            // Call
            var properties = new MacroStabilityInwardsOutputProperties(output, failureMechanism, assessmentSection);

            // Assert
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(
                output, failureMechanism, assessmentSection);

            Assert.AreEqual(expectedDerivedOutput.FactorOfStability, properties.MacroStabilityInwardsFactorOfStability,
                            properties.MacroStabilityInwardsFactorOfStability.GetAccuracy());

            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.RequiredProbability), properties.RequiredProbability);
            Assert.AreEqual(expectedDerivedOutput.RequiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.MacroStabilityInwardsProbability), properties.MacroStabilityInwardsProbability);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsReliability, properties.MacroStabilityInwardsReliability, properties.MacroStabilityInwardsReliability.GetAccuracy());
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsFactorOfSafety, properties.MacroStabilityInwardsFactorOfSafety, properties.MacroStabilityInwardsFactorOfSafety.GetAccuracy());
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            // Call
            var properties = new MacroStabilityInwardsOutputProperties(output, failureMechanism, assessmentSection);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string macroStabilityInwardsCategory = "Macrostabiliteit binnenwaarts";

            PropertyDescriptor stabilityFactorProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(stabilityFactorProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Stabiliteitsfactor [-]",
                                                                            "Het quotiënt van de weerstandbiedende- en aandrijvende krachten langs een glijvlak.",
                                                                            true);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredProbabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Faalkanseis [1/jaar]",
                                                                            "De maximaal toegestane kans dat het toetsspoor macrostabiliteit binnenwaarts optreedt.",
                                                                            true);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredReliabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Betrouwbaarheidsindex faalkanseis [-]",
                                                                            "De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor macrostabiliteit binnenwaarts.",
                                                                            true);

            PropertyDescriptor macroStabilityInwardsProbabilityProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(macroStabilityInwardsProbabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Benaderde faalkans [1/jaar]",
                                                                            "De benaderde kans dat het toetsspoor macrostabiliteit binnenwaarts optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor macroStabilityInwardsReliabilityProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(macroStabilityInwardsReliabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);

            PropertyDescriptor macroStabilityInwardsFactorOfSafetyProperty = dynamicProperties[5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(macroStabilityInwardsFactorOfSafetyProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor deze berekening.",
                                                                            true);
            mocks.VerifyAll();
        }
    }
}