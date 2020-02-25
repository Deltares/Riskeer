// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.Forms.PropertyClasses;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsOutputPropertiesTest
    {
        private const int waveRunUpPropertyIndex = 0;
        private const int waveImpactPropertyIndex = 1;
        private const int tailorMadeWaveImpactPropertyIndex = 2;

        [Test]
        public void Constructor_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsWaveConditionsOutputProperties(null,
                                                                                                  new GrassCoverErosionOutwardsWaveConditionsInput());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Constructor_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsOutput output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsWaveConditionsOutputProperties(output, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties(output, new GrassCoverErosionOutwardsWaveConditionsInput());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionOutwardsWaveConditionsOutput>>(properties);
            Assert.AreSame(output, properties.Data);
        }

        [Test]
        public void Data_WithCalculationOutput_ReturnsExpectedValues()
        {
            // Setup
            var waveRunUpOutput = new[]
            {
                new TestWaveConditionsOutput()
            };

            var waveImpactOutput = new[]
            {
                new TestWaveConditionsOutput()
            };

            var tailorMadeWaveImpactOutput = new[]
            {
                new TestWaveConditionsOutput()
            };

            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(waveRunUpOutput, waveImpactOutput, tailorMadeWaveImpactOutput);

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties(output, new GrassCoverErosionOutwardsWaveConditionsInput());

            // Assert 
            CollectionAssert.AllItemsAreInstancesOfType(properties.WaveRunUpOutput, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(waveRunUpOutput.Length, properties.WaveRunUpOutput.Length);
            WaveConditionsOutputProperties waveRunUpProperty = properties.WaveRunUpOutput[0];
            Assert.AreSame(waveRunUpOutput[0], waveRunUpProperty.Data);

            CollectionAssert.AllItemsAreInstancesOfType(properties.WaveImpactOutput, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(waveImpactOutput.Length, properties.WaveImpactOutput.Length);
            WaveConditionsOutputProperties waveImpactProperties = properties.WaveImpactOutput[0];
            Assert.AreSame(waveImpactOutput[0], waveImpactProperties.Data);

            CollectionAssert.AllItemsAreInstancesOfType(properties.TailorMadeWaveImpactOutput, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(tailorMadeWaveImpactOutput.Length, properties.TailorMadeWaveImpactOutput.Length);
            WaveConditionsOutputProperties tailorMadeWaveImpactProperties = properties.TailorMadeWaveImpactOutput[0];
            Assert.AreSame(tailorMadeWaveImpactOutput[0], tailorMadeWaveImpactProperties.Data);
        }

        [Test]
        public void Constructor_CalculationTypeAll_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsOutput output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();
            var input = new GrassCoverErosionOutwardsWaveConditionsInput
            {
                CalculationType = GrassCoverErosionOutwardsWaveConditionsCalculationType.All
            };

            // Precondition
            Assert.AreEqual(GrassCoverErosionOutwardsWaveConditionsCalculationType.All, input.CalculationType);

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties(output, input);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor waveRunUpProperty = dynamicProperties[waveRunUpPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveRunUpProperty,
                                                                            "Resultaat",
                                                                            "Hydraulische belastingen voor golfoploop",
                                                                            "Berekende hydraulische belastingen voor golfoploop.",
                                                                            true);

            PropertyDescriptor waveImpactProperty = dynamicProperties[waveImpactPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveImpactProperty,
                                                                            "Resultaat",
                                                                            "Hydraulische belastingen voor golfklap",
                                                                            "Berekende hydraulische belastingen voor golfklap.",
                                                                            true);

            PropertyDescriptor tailorMadeWaveImpactProperty = dynamicProperties[tailorMadeWaveImpactPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(tailorMadeWaveImpactProperty,
                                                                            "Resultaat",
                                                                            "Hydraulische belastingen voor golfklap toets op maat",
                                                                            "Berekende hydraulische belastingen voor golfklap toets op maat.",
                                                                            true);
        }

        [Test]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact)]
        public void Constructor_CalculationTypeWaveRunUpOrWaveImpact_PropertiesHaveExpectedAttributesValues(
            GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType)
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsOutput output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();
            var input = new GrassCoverErosionOutwardsWaveConditionsInput
            {
                CalculationType = calculationType
            };

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties(output, input);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(1, dynamicProperties.Count);

            string revetmentType = null;
            if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp)
            {
                revetmentType = "golfoploop";
            }

            if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact)
            {
                revetmentType = "golfklap";
            }

            if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact)
            {
                revetmentType = "golfklap toets op maat";
            }

            PropertyDescriptor outputProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(outputProperty,
                                                                            "Resultaat",
                                                                            $"Hydraulische belastingen voor {revetmentType}",
                                                                            $"Berekende hydraulische belastingen voor {revetmentType}.",
                                                                            true);
        }

        [Test]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact, true, true, false)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp, true, false, false)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact, false, true, false)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact, false, false, true)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact, true, false, true)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.All, true, true, true)]
        public void DynamicVisibleValidationMethod_DependingOnRelevancy_ReturnExpectedVisibility(
            GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType, bool waveRunUpVisible, bool waveImpactVisible, bool tailorMadeWaveImpactVisible)
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsOutput output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();
            var input = new GrassCoverErosionOutwardsWaveConditionsInput
            {
                CalculationType = calculationType
            };

            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties(output, input);

            // Call & Assert
            Assert.AreEqual(waveRunUpVisible, properties.DynamicVisibleValidationMethod(nameof(properties.WaveRunUpOutput)));
            Assert.AreEqual(waveImpactVisible, properties.DynamicVisibleValidationMethod(nameof(properties.WaveImpactOutput)));
            Assert.AreEqual(tailorMadeWaveImpactVisible, properties.DynamicVisibleValidationMethod(nameof(properties.TailorMadeWaveImpactOutput)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(null));
        }
    }
}