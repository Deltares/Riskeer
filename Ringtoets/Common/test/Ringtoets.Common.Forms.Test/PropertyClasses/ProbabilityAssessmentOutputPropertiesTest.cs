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
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ProbabilityAssessmentOutputPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ProbabilityAssessmentOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ProbabilityAssessmentOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(22);
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double probability = random.NextDouble();
            double reliability = random.NextDouble();
            double factorOfSafety = random.NextDouble();

            var output = new ProbabilityAssessmentOutput(requiredProbability, requiredReliability, probability, reliability, factorOfSafety);

            // Call
            var properties = new ProbabilityAssessmentOutputProperties
            {
                Data = output
            };

            // Assert
            Assert.AreEqual(ProbabilityFormattingHelper.Format(requiredProbability), properties.RequiredProbability);
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, 1e-3);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(probability), properties.Probability);
            Assert.AreEqual(reliability, properties.Reliability, 1e-3);
            Assert.AreEqual(factorOfSafety, properties.FactorOfSafety, 1e-3);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var output = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            var properties = new ProbabilityAssessmentOutputProperties
            {
                Data = output
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(5, dynamicProperties.Count);

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
        }

        private const int requiredProbabilityPropertyIndex = 0;
        private const int requiredReliabilityPropertyIndex = 1;
        private const int probabilityPropertyIndex = 2;
        private const int reliabilityPropertyIndex = 3;
        private const int factorOfSafetyPropertyIndex = 4;
    }
}