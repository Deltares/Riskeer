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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsOutputContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new MacroStabilityInwardsOutputContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsOutputContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(22);
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double macroStabilityInwardsProbability = random.NextDouble();
            double macroStabilityInwardsReliability = random.NextDouble();
            double macroStabilityInwardsFactorOfSafety = random.NextDouble();

            var semiProbabilisticOutput = new MacroStabilityInwardsSemiProbabilisticOutput(
                requiredProbability,
                requiredReliability,
                macroStabilityInwardsProbability,
                macroStabilityInwardsReliability,
                macroStabilityInwardsFactorOfSafety);

            var output = new MacroStabilityInwardsOutput();

            // Call
            var properties = new MacroStabilityInwardsOutputContextProperties
            {
                Data = new MacroStabilityInwardsOutputContext(output, semiProbabilisticOutput)
            };

            // Call & Assert
            const string probabilityFormat = "1/{0:n0}";
            Assert.AreEqual(string.Format(probabilityFormat, 1.0 / requiredProbability), properties.RequiredProbability);
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, 1.0 / macroStabilityInwardsProbability), properties.MacroStabilityInwardsProbability);
            Assert.AreEqual(macroStabilityInwardsReliability, properties.MacroStabilityInwardsReliability, properties.MacroStabilityInwardsReliability.GetAccuracy());
            Assert.AreEqual(macroStabilityInwardsFactorOfSafety, properties.MacroStabilityInwardsFactorOfSafety, properties.MacroStabilityInwardsFactorOfSafety.GetAccuracy());
        }

        [Test]
        public void GetProperties_WithZeroValues_ReturnTranslatedFormat()
        {
            // Setup
            var random = new Random(22);
            double requiredReliability = random.NextDouble();
            double macroStabilityInwardsReliability = random.NextDouble();
            double macroStabilityInwardsFactorOfSafety = random.NextDouble();

            const double requiredProbability = 0;
            const double macroStabilityInwardsProbability = 0;

            // Call
            var semiProbabilisticOutput = new MacroStabilityInwardsSemiProbabilisticOutput(
                requiredProbability,
                requiredReliability,
                macroStabilityInwardsProbability,
                macroStabilityInwardsReliability,
                macroStabilityInwardsFactorOfSafety);

            var properties = new MacroStabilityInwardsOutputContextProperties
            {
                Data = new MacroStabilityInwardsOutputContext(new TestMacroStabilityInwardsOutput(), semiProbabilisticOutput)
            };

            // Call & Assert
            const string probability = "1/Oneindig";
            Assert.AreEqual(probability, properties.RequiredProbability);
            Assert.AreEqual(probability, properties.MacroStabilityInwardsProbability);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var random = new Random(22);
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double macroStabilityInwardsProbability = random.NextDouble();
            double macroStabilityInwardsReliability = random.NextDouble();
            double macroStabilityInwardsFactorOfSafety = random.NextDouble();

            var semiProbabilisticOutput = new MacroStabilityInwardsSemiProbabilisticOutput(
                requiredProbability,
                requiredReliability,
                macroStabilityInwardsProbability,
                macroStabilityInwardsReliability,
                macroStabilityInwardsFactorOfSafety);

            // Call
            var properties = new MacroStabilityInwardsOutputContextProperties
            {
                Data = new MacroStabilityInwardsOutputContext(new TestMacroStabilityInwardsOutput(), semiProbabilisticOutput)
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

            const string macroStabilityInwardsCategory = "Macrostabiliteit binnenwaarts";

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredProbabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Faalkanseis [1/jaar]",
                                                                            "De maximaal toegestane kans dat het toetsspoor macrostabiliteit binnenwaarts optreedt.",
                                                                            true);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredReliabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Betrouwbaarheidsindex faalkanseis [-]",
                                                                            "De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor macrostabiliteit binnenwaarts.",
                                                                            true);

            PropertyDescriptor macroStabilityInwardsProbabilityProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(macroStabilityInwardsProbabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Benaderde faalkans [1/jaar]",
                                                                            "De benaderde kans dat het toetsspoor macrostabiliteit binnenwaarts optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor macroStabilityInwardsReliabilityProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(macroStabilityInwardsReliabilityProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);

            PropertyDescriptor macroStabilityInwardsFactorOfSafetyProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(macroStabilityInwardsFactorOfSafetyProperty,
                                                                            macroStabilityInwardsCategory,
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor deze berekening.",
                                                                            true);
        }
    }
}