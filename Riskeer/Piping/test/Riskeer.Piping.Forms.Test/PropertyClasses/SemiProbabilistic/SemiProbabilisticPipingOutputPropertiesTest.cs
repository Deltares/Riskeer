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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PropertyClasses.SemiProbabilistic;

namespace Riskeer.Piping.Forms.Test.PropertyClasses.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingOutputPropertiesTest
    {
        [Test]
        public void Constructor_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new SemiProbabilisticPipingOutputProperties(null, 0.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            SemiProbabilisticPipingOutput output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();

            // Call
            var properties = new SemiProbabilisticPipingOutputProperties(output, 0.1);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<SemiProbabilisticPipingOutput>>(properties);
            Assert.AreSame(output, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const double norm = 0.1;

            var random = new Random(22);
            double upliftEffectiveStress = random.NextDouble();
            double heaveGradient = random.NextDouble();
            double sellmeijerCreepCoefficient = random.NextDouble();
            double sellmeijerCriticalFall = random.NextDouble();
            double sellmeijerReducedFall = random.NextDouble();
            double upliftFactorOfSafety = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();

            var output = new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties
            {
                UpliftEffectiveStress = upliftEffectiveStress,
                HeaveGradient = heaveGradient,
                SellmeijerCreepCoefficient = sellmeijerCreepCoefficient,
                SellmeijerCriticalFall = sellmeijerCriticalFall,
                SellmeijerReducedFall = sellmeijerReducedFall,
                UpliftFactorOfSafety = upliftFactorOfSafety,
                HeaveFactorOfSafety = heaveFactorOfSafety,
                SellmeijerFactorOfSafety = sellmeijerFactorOfSafety
            });

            // Call
            var properties = new SemiProbabilisticPipingOutputProperties(output, norm);

            // Assert
            DerivedSemiProbabilisticPipingOutput expectedDerivedOutput = DerivedSemiProbabilisticPipingOutputFactory.Create(output, norm);
            Assert.AreEqual(upliftFactorOfSafety, properties.UpliftFactorOfSafety, properties.UpliftFactorOfSafety.GetAccuracy());
            Assert.AreEqual(expectedDerivedOutput.UpliftReliability, properties.UpliftReliability, properties.UpliftReliability.GetAccuracy());
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.UpliftProbability), properties.UpliftProbability);
            Assert.AreEqual(heaveFactorOfSafety, properties.HeaveFactorOfSafety, properties.HeaveFactorOfSafety.GetAccuracy());
            Assert.AreEqual(expectedDerivedOutput.HeaveReliability, properties.HeaveReliability, properties.HeaveReliability.GetAccuracy());
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.HeaveProbability), properties.HeaveProbability);
            Assert.AreEqual(sellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety.GetAccuracy());
            Assert.AreEqual(expectedDerivedOutput.SellmeijerReliability, properties.SellmeijerReliability, properties.SellmeijerReliability.GetAccuracy());
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.SellmeijerProbability), properties.SellmeijerProbability);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.PipingProbability), properties.PipingProbability);
            Assert.AreEqual(expectedDerivedOutput.PipingReliability, properties.PipingReliability, properties.PipingReliability.GetAccuracy());

            Assert.AreEqual(upliftEffectiveStress, properties.UpliftEffectiveStress, properties.UpliftEffectiveStress.GetAccuracy());
            Assert.AreEqual(heaveGradient, properties.HeaveGradient, properties.HeaveGradient.GetAccuracy());
            Assert.AreEqual(sellmeijerCreepCoefficient, properties.SellmeijerCreepCoefficient, properties.SellmeijerCreepCoefficient.GetAccuracy());
            Assert.AreEqual(sellmeijerCriticalFall, properties.SellmeijerCriticalFall, properties.SellmeijerCriticalFall.GetAccuracy());
            Assert.AreEqual(sellmeijerReducedFall, properties.SellmeijerReducedFall, properties.SellmeijerReducedFall.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new SemiProbabilisticPipingOutputProperties(PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput(), 0.1);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(16, dynamicProperties.Count);

            const string heaveCategory = "\t\tHeave";
            const string upliftCategory = "\t\t\tOpbarsten";
            const string sellmeijerCategory = "\tTerugschrijdende erosie (Sellmeijer)";
            const string pipingCategory = "Piping";

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

            PropertyDescriptor pipingProbabilityProperty = dynamicProperties[14];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(pipingProbabilityProperty,
                                                                            pipingCategory,
                                                                            "Benaderde faalkans [1/jaar]",
                                                                            "De benaderde kans dat het faalmechanisme piping optreedt voor deze berekening.",
                                                                            true);

            PropertyDescriptor pipingReliabilityProperty = dynamicProperties[15];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(pipingReliabilityProperty,
                                                                            pipingCategory,
                                                                            "Betrouwbaarheidsindex faalkans [-]",
                                                                            "De betrouwbaarheidsindex van de faalkans voor deze berekening.",
                                                                            true);
        }
    }
}