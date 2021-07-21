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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsOutputPropertiesTest
    {
        [Test]
        public void Constructor_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsOutputProperties(null, 1.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            // Call
            var properties = new MacroStabilityInwardsOutputProperties(output, 1.1);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsOutput>>(properties);
            Assert.AreSame(output, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const double modelFactor = 1.1;

            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            // Call
            var properties = new MacroStabilityInwardsOutputProperties(output, modelFactor);

            // Assert
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(output, modelFactor);

            Assert.AreEqual(expectedDerivedOutput.FactorOfStability, properties.MacroStabilityInwardsFactorOfStability,
                            properties.MacroStabilityInwardsFactorOfStability.GetAccuracy());

            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.MacroStabilityInwardsProbability), properties.MacroStabilityInwardsProbability);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsReliability, properties.MacroStabilityInwardsReliability, properties.MacroStabilityInwardsReliability.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            MacroStabilityInwardsOutput output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            // Call
            var properties = new MacroStabilityInwardsOutputProperties(output, 1.1);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string macroStabilityInwardsCategory = "Macrostabiliteit binnenwaarts";

            PropertyDescriptor stabilityFactorProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(stabilityFactorProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Stabiliteitsfactor [-]",
                                                                            "Het quotiënt van de weerstandbiedende- en aandrijvende krachten langs een glijvlak.",
                                                                            true);

            PropertyDescriptor macroStabilityInwardsProbabilityProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(macroStabilityInwardsProbabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Benaderde faalkans [1/jaar]",
                                                                            "De benaderde kans dat het toetsspoor macrostabiliteit binnenwaarts optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor macroStabilityInwardsReliabilityProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(macroStabilityInwardsReliabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);
        }
    }
}