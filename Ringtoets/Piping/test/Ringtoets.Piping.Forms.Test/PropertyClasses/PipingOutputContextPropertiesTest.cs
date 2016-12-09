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
            Assert.AreEqual(18, dynamicProperties.Count);

            var heaveCategory = "\t\tHeave";
            var upliftCategory = "\t\t\tOpbarsten";
            var sellmeijerCategory = "\tTerugschrijdende erosie (Sellmeijer)";
            var pipingCategory = "Piping";

            PropertyDescriptor upliftFactorOfSafetyProperty = dynamicProperties[0];
            Assert.IsNotNull(upliftFactorOfSafetyProperty);
            Assert.IsTrue(upliftFactorOfSafetyProperty.IsReadOnly);
            Assert.AreEqual(upliftCategory, upliftFactorOfSafetyProperty.Category);
            Assert.AreEqual("Veiligheidsfactor [-]", upliftFactorOfSafetyProperty.DisplayName);
            Assert.AreEqual("De veiligheidsfactor voor het submechanisme opbarsten voor deze berekening.", upliftFactorOfSafetyProperty.Description);

            PropertyDescriptor upliftReliabilityProperty = dynamicProperties[1];
            Assert.IsNotNull(upliftReliabilityProperty);
            Assert.IsTrue(upliftReliabilityProperty.IsReadOnly);
            Assert.AreEqual(upliftCategory, upliftReliabilityProperty.Category);
            Assert.AreEqual("Betrouwbaarheidsindex [-]", upliftReliabilityProperty.DisplayName);
            Assert.AreEqual("De betrouwbaarheidsindex voor het submechanisme opbarsten voor deze berekening.", upliftReliabilityProperty.Description);

            PropertyDescriptor upliftProbabilityProperty = dynamicProperties[2];
            Assert.IsNotNull(upliftProbabilityProperty);
            Assert.IsTrue(upliftProbabilityProperty.IsReadOnly);
            Assert.AreEqual(upliftCategory, upliftProbabilityProperty.Category);
            Assert.AreEqual("Kans van voorkomen [1/jaar]", upliftProbabilityProperty.DisplayName);
            Assert.AreEqual("De kans dat het submechanisme opbarsten optreedt voor deze berekening.", upliftProbabilityProperty.Description);

            PropertyDescriptor heaveGradientProperty = dynamicProperties[3];
            Assert.IsNotNull(heaveGradientProperty);
            Assert.IsTrue(heaveGradientProperty.IsReadOnly);
            Assert.AreEqual(heaveCategory, heaveGradientProperty.Category);
            Assert.AreEqual("Heave gradiënt [-]", heaveGradientProperty.DisplayName);
            Assert.AreEqual("De optredende verticale gradiënt in het opbarstkanaal.", heaveGradientProperty.Description);

            PropertyDescriptor heaveFactorOfSafetyProperty = dynamicProperties[4];
            Assert.IsNotNull(heaveFactorOfSafetyProperty);
            Assert.IsTrue(heaveFactorOfSafetyProperty.IsReadOnly);
            Assert.AreEqual(heaveCategory, heaveFactorOfSafetyProperty.Category);
            Assert.AreEqual("Veiligheidsfactor [-]", heaveFactorOfSafetyProperty.DisplayName);
            Assert.AreEqual("De veiligheidsfactor voor het submechanisme heave voor deze berekening.", heaveFactorOfSafetyProperty.Description);

            PropertyDescriptor heaveReliabilityProperty = dynamicProperties[5];
            Assert.IsNotNull(heaveReliabilityProperty);
            Assert.IsTrue(heaveReliabilityProperty.IsReadOnly);
            Assert.AreEqual(heaveCategory, heaveReliabilityProperty.Category);
            Assert.AreEqual("Betrouwbaarheidsindex [-]", heaveReliabilityProperty.DisplayName);
            Assert.AreEqual("De betrouwbaarheidsindex voor het submechanisme heave voor deze berekening.", heaveReliabilityProperty.Description);

            PropertyDescriptor heaveProbabilityProperty = dynamicProperties[6];
            Assert.IsNotNull(heaveProbabilityProperty);
            Assert.IsTrue(heaveProbabilityProperty.IsReadOnly);
            Assert.AreEqual(heaveCategory, heaveProbabilityProperty.Category);
            Assert.AreEqual("Kans van voorkomen [1/jaar]", heaveProbabilityProperty.DisplayName);
            Assert.AreEqual("De kans dat het submechanisme heave optreedt voor deze berekening.", heaveProbabilityProperty.Description);

            PropertyDescriptor sellmeijerCreepCoefficientProperty = dynamicProperties[7];
            Assert.IsNotNull(sellmeijerCreepCoefficientProperty);
            Assert.IsTrue(sellmeijerCreepCoefficientProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, sellmeijerCreepCoefficientProperty.Category);
            Assert.AreEqual("Creep coëfficiënt [-]", sellmeijerCreepCoefficientProperty.DisplayName);
            Assert.AreEqual("De verhouding tussen de kwelweglengte en het berekende kritieke verval op basis van de regel van Sellmeijer (analoog aan de vuistregel van Bligh).", 
                sellmeijerCreepCoefficientProperty.Description);

            PropertyDescriptor sellmeijerCriticalFallProperty = dynamicProperties[8];
            Assert.IsNotNull(sellmeijerCriticalFallProperty);
            Assert.IsTrue(sellmeijerCriticalFallProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, sellmeijerCriticalFallProperty.Category);
            Assert.AreEqual("Kritiek verval [m]", sellmeijerCriticalFallProperty.DisplayName);
            Assert.AreEqual("Het kritieke verval over de waterkering.", sellmeijerCriticalFallProperty.Description);

            PropertyDescriptor sellmeijerReducedFallProperty = dynamicProperties[9];
            Assert.IsNotNull(sellmeijerReducedFallProperty);
            Assert.IsTrue(sellmeijerReducedFallProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, sellmeijerReducedFallProperty.Category);
            Assert.AreEqual("Gereduceerd verval [m]", sellmeijerReducedFallProperty.DisplayName);
            Assert.AreEqual("Het verschil tussen de buitenwaterstand en de binnenwaterstand, gecorrigeerd voor de drukval in het opbarstkanaal.", sellmeijerReducedFallProperty.Description);

            PropertyDescriptor sellmeijerFactorOfSafetyProperty = dynamicProperties[10];
            Assert.IsNotNull(sellmeijerFactorOfSafetyProperty);
            Assert.IsTrue(sellmeijerFactorOfSafetyProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, sellmeijerFactorOfSafetyProperty.Category);
            Assert.AreEqual("Veiligheidsfactor [-]", sellmeijerFactorOfSafetyProperty.DisplayName);
            Assert.AreEqual("De veiligheidsfactor voor het submechanisme terugschrijdende erosie (Sellmeijer) voor deze berekening.", sellmeijerFactorOfSafetyProperty.Description);

            PropertyDescriptor sellmeijerReliabilityProperty = dynamicProperties[11];
            Assert.IsNotNull(sellmeijerReliabilityProperty);
            Assert.IsTrue(sellmeijerReliabilityProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, sellmeijerReliabilityProperty.Category);
            Assert.AreEqual("Betrouwbaarheidsindex [-]", sellmeijerReliabilityProperty.DisplayName);
            Assert.AreEqual("De betrouwbaarheidsindex voor het submechanisme terugschrijdende erosie (Sellmeijer) voor deze berekening.", sellmeijerReliabilityProperty.Description);

            PropertyDescriptor sellmeijerProbabilityProperty = dynamicProperties[12];
            Assert.IsNotNull(sellmeijerProbabilityProperty);
            Assert.IsTrue(sellmeijerProbabilityProperty.IsReadOnly);
            Assert.AreEqual(sellmeijerCategory, sellmeijerProbabilityProperty.Category);
            Assert.AreEqual("Kans van voorkomen [1/jaar]", sellmeijerProbabilityProperty.DisplayName);
            Assert.AreEqual("De kans dat het submechanisme terugschrijdende erosie (Sellmeijer) optreedt voor deze berekening.", sellmeijerProbabilityProperty.Description);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[13];
            Assert.IsNotNull(requiredProbabilityProperty);
            Assert.IsTrue(requiredProbabilityProperty.IsReadOnly);
            Assert.AreEqual(pipingCategory, requiredProbabilityProperty.Category);
            Assert.AreEqual("Faalkanseis [1/jaar]", requiredProbabilityProperty.DisplayName);
            Assert.AreEqual("De maximaal toegestane kans dat het toetsspoor piping optreedt.", requiredProbabilityProperty.Description);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[14];
            Assert.IsNotNull(requiredReliabilityProperty);
            Assert.IsTrue(requiredReliabilityProperty.IsReadOnly);
            Assert.AreEqual(pipingCategory, requiredReliabilityProperty.Category);
            Assert.AreEqual("Betrouwbaarheidsindex faalkanseis [-]", requiredReliabilityProperty.DisplayName);
            Assert.AreEqual("De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor piping.", requiredReliabilityProperty.Description);

            PropertyDescriptor pipingProbabilityProperty = dynamicProperties[15];
            Assert.IsNotNull(pipingProbabilityProperty);
            Assert.IsTrue(pipingProbabilityProperty.IsReadOnly);
            Assert.AreEqual(pipingCategory, pipingProbabilityProperty.Category);
            Assert.AreEqual("Benaderde faalkans [1/jaar]", pipingProbabilityProperty.DisplayName);
            Assert.AreEqual("De benaderde kans dat het toetsspoor piping optreedt voor deze berekening.", pipingProbabilityProperty.Description);

            PropertyDescriptor pipingReliabilityProperty = dynamicProperties[16];
            Assert.IsNotNull(pipingReliabilityProperty);
            Assert.IsTrue(pipingReliabilityProperty.IsReadOnly);
            Assert.AreEqual(pipingCategory, pipingReliabilityProperty.Category);
            Assert.AreEqual("Betrouwbaarheidsindex faalkans [-]", pipingReliabilityProperty.DisplayName);
            Assert.AreEqual("De betrouwbaarheidsindex van de faalkans voor deze berekening.", pipingReliabilityProperty.Description);

            PropertyDescriptor pipingFactorOfSafetyProperty = dynamicProperties[17];
            Assert.IsNotNull(pipingFactorOfSafetyProperty);
            Assert.IsTrue(pipingFactorOfSafetyProperty.IsReadOnly);
            Assert.AreEqual(pipingCategory, pipingFactorOfSafetyProperty.Category);
            Assert.AreEqual("Veiligheidsfactor [-]", pipingFactorOfSafetyProperty.DisplayName);
            Assert.AreEqual("De veiligheidsfactor voor deze berekening.", pipingFactorOfSafetyProperty.Description);
        }
    }
}