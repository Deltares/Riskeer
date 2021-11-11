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
        private const int waveImpactWithWaveDirectionPropertyIndex = 2;

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

            var waveImpactWithWaveDirectionOutput = new[]
            {
                new TestWaveConditionsOutput()
            };

            GrassCoverErosionOutwardsWaveConditionsOutput output =
                GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(waveRunUpOutput, waveImpactOutput, waveImpactWithWaveDirectionOutput);

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

            CollectionAssert.AllItemsAreInstancesOfType(properties.WaveImpactWithWaveDirectionOutput, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(waveImpactWithWaveDirectionOutput.Length, properties.WaveImpactWithWaveDirectionOutput.Length);
            WaveConditionsOutputProperties waveImpactWithWaveDirectionOutputProperties = properties.WaveImpactWithWaveDirectionOutput[0];
            Assert.AreSame(waveImpactWithWaveDirectionOutput[0], waveImpactWithWaveDirectionOutputProperties.Data);
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
                                                                            "Berekende hydraulische belastingen voor golfklap zonder invloed van de golfinvalshoek.",
                                                                            true);

            PropertyDescriptor waveImpactWithWaveDirectionProperty = dynamicProperties[waveImpactWithWaveDirectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveImpactWithWaveDirectionProperty,
                                                                            "Resultaat",
                                                                            "Hydraulische belastingen voor golfklap met golfrichting",
                                                                            "Berekende hydraulische belastingen voor golfklap met invloed van de golfinvalshoek.",
                                                                            true);
        }

        [Test]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp, "Hydraulische belastingen voor golfoploop", "Berekende hydraulische belastingen voor golfoploop.")]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact, "Hydraulische belastingen voor golfklap", "Berekende hydraulische belastingen voor golfklap zonder invloed van de golfinvalshoek.")]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpactWithWaveDirection, "Hydraulische belastingen voor golfklap met golfrichting", "Berekende hydraulische belastingen voor golfklap met invloed van de golfinvalshoek.")]
        public void Constructor_CalculationTypeWaveRunUpOrWaveImpact_PropertiesHaveExpectedAttributesValues(
            GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType, string expectedDisplayName, string expectedDescription)
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

            PropertyDescriptor outputProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(outputProperty,
                                                                            "Resultaat",
                                                                            expectedDisplayName,
                                                                            expectedDescription,
                                                                            true);
        }

        [Test]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact, true, true, false)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp, true, false, false)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact, false, true, false)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpactWithWaveDirection, false, false, true)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpactWithWaveDirection, true, false, true)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.All, true, true, true)]
        public void DynamicVisibleValidationMethod_DependingOnRelevancy_ReturnExpectedVisibility(
            GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType, bool waveRunUpVisible, bool waveImpactVisible, bool waveImpactWithWaveDirectionVisible)
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
            Assert.AreEqual(waveImpactWithWaveDirectionVisible, properties.DynamicVisibleValidationMethod(nameof(properties.WaveImpactWithWaveDirectionOutput)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(null));
        }
    }
}