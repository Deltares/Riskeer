﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Rhino.Mocks;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputPropertiesTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionInwardsOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var random = new Random(21);
            double waveHeight = random.NextDouble();
            bool isOvertoppingDominant = Convert.ToBoolean(random.Next(0, 2));
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double probability = random.NextDouble();
            double reliability = random.NextDouble();
            double factorOfSafety = random.NextDouble();
            double dikeHeight = random.NextDouble();
            ProbabilityAssessmentOutput probabilityAssessmentOutput = new ProbabilityAssessmentOutput(requiredProbability, requiredReliability, probability, reliability, factorOfSafety);
            var output = new GrassCoverErosionInwardsOutput(waveHeight, isOvertoppingDominant, probabilityAssessmentOutput, dikeHeight);

            // Call
            var properties = new GrassCoverErosionInwardsOutputProperties
            {
                Data = output
            };

            // Assert
            Assert.AreEqual(2, properties.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, properties.WaveHeight, properties.WaveHeight.GetAccuracy());
            Assert.AreEqual(3, properties.Reliability.NumberOfDecimalPlaces);
            Assert.AreEqual(reliability, properties.Reliability, properties.Reliability.GetAccuracy());
            Assert.AreEqual(3, properties.RequiredReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(3, properties.FactorOfSafety.NumberOfDecimalPlaces);
            Assert.AreEqual(factorOfSafety, properties.FactorOfSafety, properties.FactorOfSafety.GetAccuracy());

            Assert.AreEqual(ProbabilityFormattingHelper.Format(requiredProbability), properties.RequiredProbability);
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, 1e-3);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(probability), properties.Probability);

            Assert.AreEqual(isOvertoppingDominant, properties.IsOvertoppingDominant);

            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(dikeHeight, properties.DikeHeight, properties.DikeHeight.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_WithDikeHeightCalculated_ReturnExpectedValues()
        {
            // Setup
            ProbabilityAssessmentOutput probabilityAssessmentOutput = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var output = new GrassCoverErosionInwardsOutput(double.NaN, true, probabilityAssessmentOutput, double.NaN);

            // Call
            var properties = new GrassCoverErosionInwardsOutputProperties
            {
                Data = output
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(8, dynamicProperties.Count);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[requiredProbabilityPropertyIndex];
            Assert.IsTrue(requiredProbabilityProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", requiredProbabilityProperty.Category);
            Assert.AreEqual("Faalkanseis [1/jaar]", requiredProbabilityProperty.DisplayName);
            Assert.AreEqual("De maximaal toegestane faalkanseis voor het toetsspoor.", requiredProbabilityProperty.Description);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[requiredReliabilityPropertyIndex];
            Assert.IsTrue(requiredReliabilityProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", requiredReliabilityProperty.Category);
            Assert.AreEqual("Betrouwbaarheidsindex faalkanseis [-]", requiredReliabilityProperty.DisplayName);
            Assert.AreEqual("De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor.", requiredReliabilityProperty.Description);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            Assert.IsTrue(probabilityProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", probabilityProperty.Category);
            Assert.AreEqual("Faalkans [1/jaar]", probabilityProperty.DisplayName);
            Assert.AreEqual("De kans dat het toetsspoor optreedt voor deze berekening.", probabilityProperty.Description);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            Assert.IsTrue(reliabilityProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", reliabilityProperty.Category);
            Assert.AreEqual("Betrouwbaarheidsindex faalkans [-]", reliabilityProperty.DisplayName);
            Assert.AreEqual("De betrouwbaarheidsindex van de faalkans voor deze berekening.", reliabilityProperty.Description);

            PropertyDescriptor factorOfSafetyProperty = dynamicProperties[factorOfSafetyPropertyIndex];
            Assert.IsTrue(factorOfSafetyProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", factorOfSafetyProperty.Category);
            Assert.AreEqual("Veiligheidsfactor [-]", factorOfSafetyProperty.DisplayName);
            Assert.AreEqual("De veiligheidsfactor voor deze berekening.", factorOfSafetyProperty.Description);

            PropertyDescriptor waveHeightProperty = dynamicProperties[waveHeightIndex];
            Assert.IsTrue(waveHeightProperty.IsReadOnly);
            Assert.AreEqual("Indicatieve golfhoogte", waveHeightProperty.Category);
            Assert.AreEqual("Golfhoogte (Hs) [m]", waveHeightProperty.DisplayName);
            Assert.AreEqual("De golfhoogte van de overslag deelberekening.", waveHeightProperty.Description);

            PropertyDescriptor isDominantProperty = dynamicProperties[isDominantIndex];
            Assert.IsTrue(isDominantProperty.IsReadOnly);
            Assert.AreEqual("Indicatieve golfhoogte", isDominantProperty.Category);
            Assert.AreEqual("Overslag dominant [-]", isDominantProperty.DisplayName);
            Assert.AreEqual("Is het resultaat van de overslag deelberekening dominant over de overloop deelberekening.", isDominantProperty.Description);

            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightIndex];
            Assert.IsTrue(dikeHeightProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", dikeHeightProperty.Category);
            Assert.AreEqual("HBN [m+NAP]", dikeHeightProperty.DisplayName);
            Assert.AreEqual("Het berekende Hydraulisch Belasting Niveau (HBN).", dikeHeightProperty.Description);
        }

        [Test]
        public void PropertyAttributes_WithoutDikeHeightCalculated_ReturnExpectedValues()
        {
            // Setup
            ProbabilityAssessmentOutput probabilityAssessmentOutput = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var output = new GrassCoverErosionInwardsOutput(double.NaN, true, probabilityAssessmentOutput, null);

            // Call
            var properties = new GrassCoverErosionInwardsOutputProperties
            {
                Data = output
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(7, dynamicProperties.Count);

            PropertyDescriptor requiredProbabilityProperty = dynamicProperties[requiredProbabilityPropertyIndex];
            Assert.IsTrue(requiredProbabilityProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", requiredProbabilityProperty.Category);
            Assert.AreEqual("Faalkanseis [1/jaar]", requiredProbabilityProperty.DisplayName);
            Assert.AreEqual("De maximaal toegestane faalkanseis voor het toetsspoor.", requiredProbabilityProperty.Description);

            PropertyDescriptor requiredReliabilityProperty = dynamicProperties[requiredReliabilityPropertyIndex];
            Assert.IsTrue(requiredReliabilityProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", requiredReliabilityProperty.Category);
            Assert.AreEqual("Betrouwbaarheidsindex faalkanseis [-]", requiredReliabilityProperty.DisplayName);
            Assert.AreEqual("De betrouwbaarheidsindex van de faalkanseis voor het toetsspoor.", requiredReliabilityProperty.Description);

            PropertyDescriptor probabilityProperty = dynamicProperties[probabilityPropertyIndex];
            Assert.IsTrue(probabilityProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", probabilityProperty.Category);
            Assert.AreEqual("Faalkans [1/jaar]", probabilityProperty.DisplayName);
            Assert.AreEqual("De kans dat het toetsspoor optreedt voor deze berekening.", probabilityProperty.Description);

            PropertyDescriptor reliabilityProperty = dynamicProperties[reliabilityPropertyIndex];
            Assert.IsTrue(reliabilityProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", reliabilityProperty.Category);
            Assert.AreEqual("Betrouwbaarheidsindex faalkans [-]", reliabilityProperty.DisplayName);
            Assert.AreEqual("De betrouwbaarheidsindex van de faalkans voor deze berekening.", reliabilityProperty.Description);

            PropertyDescriptor factorOfSafetyProperty = dynamicProperties[factorOfSafetyPropertyIndex];
            Assert.IsTrue(factorOfSafetyProperty.IsReadOnly);
            Assert.AreEqual("Resultaat", factorOfSafetyProperty.Category);
            Assert.AreEqual("Veiligheidsfactor [-]", factorOfSafetyProperty.DisplayName);
            Assert.AreEqual("De veiligheidsfactor voor deze berekening.", factorOfSafetyProperty.Description);

            PropertyDescriptor waveHeightProperty = dynamicProperties[waveHeightIndex];
            Assert.IsTrue(waveHeightProperty.IsReadOnly);
            Assert.AreEqual("Indicatieve golfhoogte", waveHeightProperty.Category);
            Assert.AreEqual("Golfhoogte (Hs) [m]", waveHeightProperty.DisplayName);
            Assert.AreEqual("De golfhoogte van de overslag deelberekening.", waveHeightProperty.Description);

            PropertyDescriptor isDominantProperty = dynamicProperties[isDominantIndex];
            Assert.IsTrue(isDominantProperty.IsReadOnly);
            Assert.AreEqual("Indicatieve golfhoogte", isDominantProperty.Category);
            Assert.AreEqual("Overslag dominant [-]", isDominantProperty.DisplayName);
            Assert.AreEqual("Is het resultaat van de overslag deelberekening dominant over de overloop deelberekening.", isDominantProperty.Description);
        }

        private const int requiredProbabilityPropertyIndex = 0;
        private const int requiredReliabilityPropertyIndex = 1;
        private const int probabilityPropertyIndex = 2;
        private const int reliabilityPropertyIndex = 3;
        private const int factorOfSafetyPropertyIndex = 4;
        private const int waveHeightIndex = 5;
        private const int isDominantIndex = 6;
        private const int dikeHeightIndex = 7;
    }
}