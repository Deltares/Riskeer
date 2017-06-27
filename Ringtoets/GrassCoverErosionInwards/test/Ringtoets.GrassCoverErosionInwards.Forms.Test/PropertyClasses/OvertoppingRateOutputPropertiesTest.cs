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
    public class OvertoppingRateOutputPropertiesTest
    {
        private const int overtoppingRatePropertyIndex = 0;
        private const int targetProbabilityPropertyIndex = 1;
        private const int targetReliabilityPropertyIndex = 2;
        private const int calculatedProbabilityPropertyIndex = 3;
        private const int calculatedReliabilityPropertyIndex = 4;
        private const int convergencePropertyIndex = 5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new OvertoppingRateOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<OvertoppingRateOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var random = new Random();
            double overtoppingRate = random.NextDouble();
            double overtoppingRateTargetProbability = random.NextDouble();
            double overtoppingRateTargetReliability = random.NextDouble();
            double overtoppingRateCalculatedProbability = random.NextDouble();
            double overtoppingRateCalculatedReliability = random.NextDouble();
            var overtoppingRateConvergence = random.NextEnumValue<CalculationConvergence>();

            var output = new OvertoppingRateOutput(overtoppingRate,
                                                   overtoppingRateTargetProbability,
                                                   overtoppingRateTargetReliability,
                                                   overtoppingRateCalculatedProbability,
                                                   overtoppingRateCalculatedReliability,
                                                   overtoppingRateConvergence);

            // Call
            var properties = new OvertoppingRateOutputProperties
            {
                Data = output
            };

            // Assert
            Assert.AreEqual(2, properties.OvertoppingRate.NumberOfDecimalPlaces);
            Assert.AreEqual(overtoppingRate * 1000, properties.OvertoppingRate, properties.OvertoppingRate.GetAccuracy());
            Assert.AreEqual(overtoppingRateTargetProbability, properties.OvertoppingRateTargetProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.OvertoppingRateTargetProbability));
            Assert.AreEqual(overtoppingRateTargetReliability, properties.OvertoppingRateTargetReliability, properties.OvertoppingRateTargetReliability.GetAccuracy());
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.OvertoppingRateTargetReliability));
            Assert.AreEqual(overtoppingRateCalculatedProbability, properties.OvertoppingRateCalculatedProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.OvertoppingRateCalculatedProbability));
            Assert.AreEqual(overtoppingRateCalculatedReliability, properties.OvertoppingRateCalculatedReliability, properties.OvertoppingRateCalculatedReliability.GetAccuracy());
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionInwardsOutputProperties.OvertoppingRateCalculatedReliability));

            string overtoppingRateConvergenceValue = new EnumDisplayWrapper<CalculationConvergence>(overtoppingRateConvergence).DisplayName;
            Assert.AreEqual(overtoppingRateConvergenceValue, properties.OvertoppingRateConvergence);
        }

        [Test]
        public void PropertyAttributes_Always_ReturnExpectedValues()
        {
            // Setup
            var output = new TestOvertoppingRateOutput(10);

            // Call
            var properties = new OvertoppingRateOutputProperties
            {
                Data = output
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            const string category = "Overslagdebiet";
            PropertyDescriptor overtoppingRateProperty = dynamicProperties[overtoppingRatePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(overtoppingRateProperty,
                                                                            category,
                                                                            "Overslagdebiet [l/m/s]",
                                                                            "Het berekende overslagdebiet.",
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
                                                                            "Is convergentie bereikt in de overslagdebiet berekening?",
                                                                            true);
        }
    }
}