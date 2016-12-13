// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingOutputContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingOutputContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingOutputContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(22);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            double upliftEffectiveStress = random.NextDouble();
            double heaveGradient = random.NextDouble();
            double sellmeijerCreepCoefficient = random.NextDouble();
            double sellmeijerCriticalFall = random.NextDouble();
            double sellmeijerReducedFall = random.NextDouble();

            var semiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            var output = new PipingOutput(new PipingOutput.ConstructionProperties
            {
                UpliftEffectiveStress = upliftEffectiveStress,
                HeaveGradient = heaveGradient,
                SellmeijerCreepCoefficient = sellmeijerCreepCoefficient,
                SellmeijerCriticalFall = sellmeijerCriticalFall,
                SellmeijerReducedFall = sellmeijerReducedFall
            });

            // Call
            var properties = new PipingOutputContextProperties
            {
                Data = new PipingOutputContext(output, semiProbabilisticOutput)
            };

            // Call & Assert
            var probabilityFormat = "1/{0:n0}";
            Assert.AreEqual(upliftFactorOfSafety, properties.UpliftFactorOfSafety, properties.UpliftFactorOfSafety.GetAccuracy());
            Assert.AreEqual(upliftReliability, properties.UpliftReliability, properties.UpliftReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, 1.0/upliftProbability), properties.UpliftProbability);
            Assert.AreEqual(heaveFactorOfSafety, properties.HeaveFactorOfSafety, properties.HeaveFactorOfSafety.GetAccuracy());
            Assert.AreEqual(heaveReliability, properties.HeaveReliability, properties.HeaveReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, 1.0/heaveProbability), properties.HeaveProbability);
            Assert.AreEqual(sellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety.GetAccuracy());
            Assert.AreEqual(sellmeijerReliability, properties.SellmeijerReliability, properties.SellmeijerReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, 1.0/sellmeijerProbability), properties.SellmeijerProbability);
            Assert.AreEqual(string.Format(probabilityFormat, 1.0/requiredProbability), properties.RequiredProbability);
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, 1.0/pipingProbability), properties.PipingProbability);
            Assert.AreEqual(pipingReliability, properties.PipingReliability, properties.PipingReliability.GetAccuracy());
            Assert.AreEqual(pipingFactorOfSafety, properties.PipingFactorOfSafety, properties.PipingFactorOfSafety.GetAccuracy());

            Assert.AreEqual(upliftEffectiveStress, properties.UpliftEffectiveStress, properties.UpliftEffectiveStress.GetAccuracy());
            Assert.AreEqual(heaveGradient, properties.HeaveGradient, properties.HeaveGradient.GetAccuracy());
            Assert.AreEqual(sellmeijerCreepCoefficient, properties.SellmeijerCreepCoefficient, properties.SellmeijerCreepCoefficient.GetAccuracy());
            Assert.AreEqual(sellmeijerCriticalFall, properties.SellmeijerCriticalFall, properties.SellmeijerCriticalFall.GetAccuracy());
            Assert.AreEqual(sellmeijerReducedFall, properties.SellmeijerReducedFall, properties.SellmeijerReducedFall.GetAccuracy());
        }

        [Test]
        public void GetProperties_WithZeroValues_ReturnTranslatedFormat()
        {
            // Setup
            var random = new Random(22);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            double upliftProbability = 0;
            double heaveProbability = 0;
            double sellmeijerProbability = 0;
            double requiredProbability = 0;
            double pipingProbability = 0;

            // Call
            var semiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            var properties = new PipingOutputContextProperties
            {
                Data = new PipingOutputContext(new TestPipingOutput(), semiProbabilisticOutput)
            };

            // Call & Assert
            var probability = "1/Oneindig";
            Assert.AreEqual(probability, properties.UpliftProbability);
            Assert.AreEqual(probability, properties.HeaveProbability);
            Assert.AreEqual(probability, properties.SellmeijerProbability);
            Assert.AreEqual(probability, properties.RequiredProbability);
            Assert.AreEqual(probability, properties.PipingProbability);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var random = new Random(22);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            var semiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety,
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety,
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            // Call
            var properties = new PipingOutputContextProperties
            {
                Data = new PipingOutputContext(new TestPipingOutput(), semiProbabilisticOutput)
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });
            Assert.AreEqual(19, dynamicProperties.Count);

            var heaveCategory = "\t\tHeave";
            var upliftCategory = "\t\t\tOpbarsten";
            var sellmeijerCategory = "\tTerugschrijdende erosie (Sellmeijer)";
            var pipingCategory = "Piping";

            PropertyDescriptor upliftEffectiveStress = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upliftEffectiveStress,
                                                                            upliftCategory,
                                                                            "Gewicht van de deklaag [kN/m²]",
                                                                            "Het effectieve gewicht van de deklaag.",
                                                                            true);

            PropertyDescriptor upliftFactorOfSafetyProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upliftFactorOfSafetyProperty,
                                                                            upliftCategory,
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor het submechanisme opbarsten voor deze berekening.",
                                                                            true);

            PropertyDescriptor upliftReliabilityProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upliftReliabilityProperty,
                                                                            upliftCategory,
                                                                            "Betrouwbaarheidsindex [-]",
                                                                            "De betrouwbaarheidsindex voor het submechanisme opbarsten voor deze berekening.",
                                                                            true);

            PropertyDescriptor upliftProbabilityProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(upliftProbabilityProperty,
                                                                            upliftCategory,
                                                                            "Kans van voorkomen [1/jaar]",
                                                                            "De kans dat het submechanisme opbarsten optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor heaveGradientProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(heaveGradientProperty,
                                                                            heaveCategory,
                                                                            "Heave gradiënt [-]",
                                                                            "De optredende verticale gradiënt in het opbarstkanaal.",
                                                                            true);

            PropertyDescriptor heaveFactorOfSafetyProperty = dynamicProperties[5];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(heaveFactorOfSafetyProperty,
                                                                            heaveCategory,
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor het submechanisme heave voor deze berekening.",
                                                                            true);

            PropertyDescriptor heaveReliabilityProperty = dynamicProperties[6];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(heaveReliabilityProperty,
                                                                            heaveCategory,
                                                                            "Betrouwbaarheidsindex [-]",
                                                                            "De betrouwbaarheidsindex voor het submechanisme heave voor deze berekening.",
                                                                            true);

            PropertyDescriptor heaveProbabilityProperty = dynamicProperties[7];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(heaveProbabilityProperty,
                                                                            heaveCategory,
                                                                            "Kans van voorkomen [1/jaar]",
                                                                            "De kans dat het submechanisme heave optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor sellmeijerCreepCoefficientProperty = dynamicProperties[8];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sellmeijerCreepCoefficientProperty,
                                                                            sellmeijerCategory,
                                                                            "Creep coëfficiënt [-]",
                                                                            "De verhouding tussen de kwelweglengte en het berekende kritieke verval op basis van de regel van Sellmeijer (analoog aan de vuistregel van Bligh).",
                                                                            true);

            PropertyDescriptor sellmeijerCriticalFallProperty = dynamicProperties[9];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sellmeijerCriticalFallProperty,
                                                                            sellmeijerCategory,
                                                                            "Kritiek verval [m]",
                                                                            "Het kritieke verval over de waterkering.",
                                                                            true);

            PropertyDescriptor sellmeijerReducedFallProperty = dynamicProperties[10];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sellmeijerReducedFallProperty,
                                                                            sellmeijerCategory,
                                                                            "Gereduceerd verval [m]",
                                                                            "Het verschil tussen de buitenwaterstand en de binnenwaterstand, gecorrigeerd voor de drukval in het opbarstkanaal.",
                                                                            true);

            PropertyDescriptor sellmeijerFactorOfSafetyProperty = dynamicProperties[11];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sellmeijerFactorOfSafetyProperty,
                                                                            sellmeijerCategory,
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor het submechanisme terugschrijdende erosie (Sellmeijer) voor deze berekening.",
                                                                            true);

            PropertyDescriptor sellmeijerReliabilityProperty = dynamicProperties[12];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sellmeijerReliabilityProperty,
                                                                            sellmeijerCategory,
                                                                            "Betrouwbaarheidsindex [-]",
                                                                            "De betrouwbaarheidsindex voor het submechanisme terugschrijdende erosie (Sellmeijer) voor deze berekening.",
                                                                            true);

            PropertyDescriptor sellmeijerProbabilityProperty = dynamicProperties[13];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(sellmeijerProbabilityProperty,
                                                                            sellmeijerCategory,
                                                                            "Kans van voorkomen [1/jaar]",
                                                                            "De kans dat het submechanisme terugschrijdende erosie (Sellmeijer) optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[14];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredProbabilityProperty,
                                                                            pipingCategory,
                                                                            "Faalkanseis [1/jaar]",
                                                                            "De maximaal toegestane kans dat het toetsspoor piping optreedt.",
                                                                            true);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[15];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(requiredReliabilityProperty,
                                                                            pipingCategory,
                                                                            "Betrouwbaarheidsindex faalkanseis [-]",
                                                                            "De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor piping.",
                                                                            true);

            PropertyDescriptor pipingProbabilityProperty = dynamicProperties[16];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(pipingProbabilityProperty,
                                                                            pipingCategory,
                                                                            "Benaderde faalkans [1/jaar]",
                                                                            "De benaderde kans dat het toetsspoor piping optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor pipingReliabilityProperty = dynamicProperties[17];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(pipingReliabilityProperty,
                                                                            pipingCategory,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);

            PropertyDescriptor pipingFactorOfSafetyProperty = dynamicProperties[18];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(pipingFactorOfSafetyProperty,
                                                                            pipingCategory,
                                                                            "Veiligheidsfactor [-]",
                                                                            "De veiligheidsfactor voor deze berekening.",
                                                                            true);
        }
    }
}