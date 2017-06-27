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
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DikeHeightOutputPropertiesTest
    {
        private const int dikeHeightPropertyIndex = 0;
        private const int targetProbabilityPropertyIndex = 1;
        private const int targetReliabilityPropertyIndex = 2;
        private const int calculatedProbabilityPropertyIndex = 3;
        private const int calculatedReliabilityPropertyIndex = 4;
        private const int convergencePropertyIndex = 5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new DikeHeightOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<DikeHeightOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewValue_ReturnCorrectPropertyValues()
        {
            // Setup
            var random = new Random();
            double dikeHeight = random.NextDouble();
            double dikeHeightTargetProbability = random.NextDouble();
            double dikeHeightTargetReliability = random.NextDouble();
            double dikeHeightCalculatedProbability = random.NextDouble();
            double dikeHeightCalculatedReliability = random.NextDouble();
            var dikeHeightConvergence = random.NextEnumValue<CalculationConvergence>();

            var output = new DikeHeightOutput(dikeHeight,
                                              dikeHeightTargetProbability,
                                              dikeHeightTargetReliability,
                                              dikeHeightCalculatedProbability,
                                              dikeHeightCalculatedReliability,
                                              dikeHeightConvergence);
            // Call
            var properties = new DikeHeightOutputProperties
            {
                Data = output
            };

            // Assert
            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(dikeHeight, properties.DikeHeight, properties.DikeHeight.GetAccuracy());
            Assert.AreEqual(dikeHeightTargetProbability, properties.DikeHeightTargetProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.DikeHeightTargetProbability));
            Assert.AreEqual(dikeHeightTargetReliability, properties.DikeHeightTargetReliability, properties.DikeHeightTargetReliability.GetAccuracy());
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.DikeHeightTargetReliability));
            Assert.AreEqual(dikeHeightCalculatedProbability, properties.DikeHeightCalculatedProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.DikeHeightCalculatedProbability));
            Assert.AreEqual(dikeHeightCalculatedReliability, properties.DikeHeightCalculatedReliability, properties.DikeHeightCalculatedReliability.GetAccuracy());
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.DikeHeightCalculatedReliability));

            string dikeHeightConvergenceValue = new EnumDisplayWrapper<CalculationConvergence>(dikeHeightConvergence).DisplayName;
            Assert.AreEqual(dikeHeightConvergenceValue, properties.DikeHeightConvergence);
        }

        [Test]
        public void PropertyAttributes_Always_ReturnExpectedValues()
        {
            // Setup
            var output = new TestDikeHeightOutput(10);

            // Call
            var properties = new DikeHeightOutputProperties
            {
                Data = output
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string category = "HBN";
            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightProperty,
                                                                            category,
                                                                            "HBN [m+NAP]",
                                                                            "Het berekende Hydraulisch Belasting Niveau (HBN).",
                                                                            true);

            PropertyDescriptor targetProbability = dynamicProperties[targetProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetProbability,
                                                                            category,
                                                                            "Doelkans [1/jaar]",
                                                                            "De ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor targetReliability = dynamicProperties[targetReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetReliability,
                                                                            category,
                                                                            "Betrouwbaarheidsindex doelkans [-]",
                                                                            "Betrouwbaarheidsindex van de ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor calculatedProbability = dynamicProperties[calculatedProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedProbability,
                                                                            category,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor calculatedReliability = dynamicProperties[calculatedReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedReliability,
                                                                            category,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor calculationConvergence = dynamicProperties[convergencePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculationConvergence,
                                                                            category,
                                                                            "Convergentie",
                                                                            "Is convergentie bereikt in de HBN berekening?",
                                                                            true);
        }
    }
}