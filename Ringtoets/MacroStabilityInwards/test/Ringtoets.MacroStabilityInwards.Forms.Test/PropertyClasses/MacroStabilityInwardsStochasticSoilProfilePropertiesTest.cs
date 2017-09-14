﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilProfilePropertiesTest
    {
        private const string generalCategoryName = "Algemeen";

        [Test]
        public void Constructor_StochasticSoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsStochasticSoilProfileProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochasticSoilProfile", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidStochasticSoilProfile_ExpectedValues()
        {
            // Setup
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(1.0, new TestMacroStabilityInwardsSoilProfile1D());

            // Call
            var properties = new MacroStabilityInwardsStochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsStochasticSoilProfile>>(properties);
            TestHelper.AssertTypeConverter<MacroStabilityInwardsStochasticSoilProfileProperties,
                ExpandableArrayConverter>(nameof(MacroStabilityInwardsStochasticSoilProfileProperties.Layers1D));
            TestHelper.AssertTypeConverter<MacroStabilityInwardsStochasticSoilProfileProperties,
                ExpandableArrayConverter>(nameof(MacroStabilityInwardsStochasticSoilProfileProperties.Layers2D));
            Assert.AreSame(stochasticSoilProfile, properties.Data);
        }

        [Test]
        [TestCase(0.142, "14,2")]
        [TestCase(1.0, "100")]
        [TestCase(0.5 + 1e-6, "50")]
        [SetCulture("nl-NL")]
        public void GetProperties_WithSoilProfile1DAndDutchLocale_ReturnExpectedValues(double probability, string expectedProbability)
        {
            GetProperties_WithSoilProfile1D_ReturnExpectedValues(probability, expectedProbability);
        }

        [Test]
        [TestCase(0.142, "14.2")]
        [TestCase(1.0, "100")]
        [TestCase(0.5 + 1e-6, "50")]
        [SetCulture("en-US")]
        public void GetProperties_WithData1DAndEnglishLocale_ReturnExpectedValues(double probability, string expectedProbability)
        {
            GetProperties_WithSoilProfile1D_ReturnExpectedValues(probability, expectedProbability);
        }

        [Test]
        [TestCase(0.142, "14,2")]
        [TestCase(1.0, "100")]
        [TestCase(0.5 + 1e-6, "50")]
        [SetCulture("nl-NL")]
        public void GetProperties_WithSoilProfile2DAndDutchLocale_ReturnExpectedValues(double probability, string expectedProbability)
        {
            GetProperties_WithSoilProfile2D_ReturnExpectedValues(probability, expectedProbability);
        }

        [Test]
        [TestCase(0.142, "14.2")]
        [TestCase(1.0, "100")]
        [TestCase(0.5 + 1e-6, "50")]
        [SetCulture("en-US")]
        public void GetProperties_WithSoilProfile2DAndEnglishLocale_ReturnExpectedValues(double probability, string expectedProbability)
        {
            GetProperties_WithSoilProfile2D_ReturnExpectedValues(probability, expectedProbability);
        }

        [Test]
        public void DynamicVisibleValidationMethod_WithSoilProfile1D_Only1DPropertiesVisible()
        {
            // Setup
            IEnumerable<MacroStabilityInwardsSoilLayer1D> layers = new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-2)
            };

            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", -5.0, layers);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);
            var properties = new MacroStabilityInwardsStochasticSoilProfileProperties(stochasticSoilProfile);

            // Call
            bool bottomVisible = properties.DynamicVisibleValidationMethod("Bottom");
            bool layers1DVisible = properties.DynamicVisibleValidationMethod("Layers1D");
            bool layers2DVisible = properties.DynamicVisibleValidationMethod("Layers2D");
            bool preconsolidationStressesVisible = properties.DynamicVisibleValidationMethod("PresonsolidationStresses");

            // Assert
            Assert.IsTrue(bottomVisible);
            Assert.IsTrue(layers1DVisible);
            Assert.IsFalse(layers2DVisible);
            Assert.IsFalse(preconsolidationStressesVisible);
        }

        [Test]
        public void DynamicVisibleValidationMethod_WithSoilProfile2D_Only2DPropertiesVisible()
        {
            // Setup
            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers = new[]
            {
                CreateMacroStabilityInwardsSoilLayer2D()
            };

            var soilProfile = new MacroStabilityInwardsSoilProfile2D("name", layers, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);
            var properties = new MacroStabilityInwardsStochasticSoilProfileProperties(stochasticSoilProfile);

            // Call
            bool bottomVisible = properties.DynamicVisibleValidationMethod("Bottom");
            bool layers1DVisible = properties.DynamicVisibleValidationMethod("Layers1D");
            bool layers2DVisible = properties.DynamicVisibleValidationMethod("Layers2D");
            bool preconsolidationStressesVisible = properties.DynamicVisibleValidationMethod("PreconsolidationStresses");

            // Assert
            Assert.IsFalse(bottomVisible);
            Assert.IsFalse(layers1DVisible);
            Assert.IsTrue(layers2DVisible);
            Assert.IsTrue(preconsolidationStressesVisible);
        }

        [Test]
        public void DynamicVisibleValidationMethod_AnyOtherProperty_ReturnsFalse()
        {
            // Setup

            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", -5.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-2)
            });
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);
            var properties = new MacroStabilityInwardsStochasticSoilProfileProperties(stochasticSoilProfile);

            // Call
            bool otherVisible = properties.DynamicVisibleValidationMethod(string.Empty);

            // Assert
            Assert.IsFalse(otherVisible);
        }

        [Test]
        public void VisibleProperties_WithSoilProfile1D_ExpectedAttributesValues()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer1D(-2);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.142, new MacroStabilityInwardsSoilProfile1D("<some name>", -5.0, new[]
            {
                layer
            }));

            // Call
            var properties = new MacroStabilityInwardsStochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Naam",
                                                                            "De naam van de ondergrondschematisatie.",
                                                                            true);

            PropertyDescriptor contributionProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(contributionProperty,
                                                                            generalCategoryName,
                                                                            "Aandeel [%]",
                                                                            "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel.",
                                                                            true);

            PropertyDescriptor layersProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(layersProperty,
                                                                            generalCategoryName,
                                                                            "Grondlagen",
                                                                            "",
                                                                            true);

            PropertyDescriptor bottomProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(bottomProperty,
                                                                            generalCategoryName,
                                                                            "Bodemniveau",
                                                                            "Het niveau van de onderkant van de ondergrondschematisatie.",
                                                                            true);

            PropertyDescriptor typeProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(typeProperty,
                                                                            generalCategoryName,
                                                                            "Type",
                                                                            "Het type van de ondergrondschematisatie.",
                                                                            true);
        }

        [Test]
        public void VisibleProperties_WithSoilProfile2D_ExpectedAttributesValues()
        {
            // Setup
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.142, new MacroStabilityInwardsSoilProfile2D("<some name>", new[]
            {
                CreateMacroStabilityInwardsSoilLayer2D()
            }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>()));

            // Call
            var properties = new MacroStabilityInwardsStochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(5, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Naam",
                                                                            "De naam van de ondergrondschematisatie.",
                                                                            true);

            PropertyDescriptor contributionProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(contributionProperty,
                                                                            generalCategoryName,
                                                                            "Aandeel [%]",
                                                                            "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel.",
                                                                            true);

            PropertyDescriptor layersProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(layersProperty,
                                                                            generalCategoryName,
                                                                            "Grondlagen",
                                                                            "",
                                                                            true);

            PropertyDescriptor typeProperty = dynamicProperties[3];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(typeProperty,
                                                                            generalCategoryName,
                                                                            "Type",
                                                                            "Het type van de ondergrondschematisatie.",
                                                                            true);

            PropertyDescriptor preconsolidationProperty = dynamicProperties[4];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(preconsolidationProperty,
                                                                            generalCategoryName,
                                                                            "Grensspanningen",
                                                                            "De grensspanningen in de ondergrondschematisatie.",
                                                                            true);
        }

        [Test]
        public void GetProperties_WithUnsupportedSoilProfile_ThrowsNotSupportedException()
        {
            // Setup
            var soilProfile = new UnsupportedSoilProfile();
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);

            var properties = new MacroStabilityInwardsStochasticSoilProfileProperties(stochasticSoilProfile);

            string type = null;

            //Call
            TestDelegate call = () => type = properties.Type;

            // Assert
            var exception = Assert.Throws<NotSupportedException>(call);
            string expectedMessage = $"{soilProfile.GetType()} is not supported. Supported types: " +
                                     $"{nameof(MacroStabilityInwardsSoilProfile1D)} and {nameof(MacroStabilityInwardsSoilProfile2D)}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        private static void GetProperties_WithSoilProfile1D_ReturnExpectedValues(double probability, string expectedProbability)
        {
            // Setup
            var layerOne = new MacroStabilityInwardsSoilLayer1D(-2);
            var layerTwo = new MacroStabilityInwardsSoilLayer1D(-4);
            IEnumerable<MacroStabilityInwardsSoilLayer1D> layers = new[]
            {
                layerOne,
                layerTwo
            };

            var soilProfile = new MacroStabilityInwardsSoilProfile1D("<some name>", -5.0, layers);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, soilProfile);

            // Call
            var properties = new MacroStabilityInwardsStochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            Assert.AreEqual(soilProfile.Name, properties.Name);
            Assert.AreEqual(soilProfile.Name, properties.ToString());

            Assert.AreEqual(2, properties.Layers1D.Length);
            Assert.AreSame(layerOne, properties.Layers1D[0].Data);
            Assert.AreSame(layerTwo, properties.Layers1D[1].Data);

            CollectionAssert.IsEmpty(properties.Layers2D);
            Assert.AreEqual(soilProfile.Bottom.ToString(CultureInfo.CurrentCulture), properties.Bottom);
            Assert.AreEqual(expectedProbability, properties.Probability);
            Assert.AreEqual("1D profiel", properties.Type);
        }

        private static void GetProperties_WithSoilProfile2D_ReturnExpectedValues(double probability, string expectedProbability)
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layerOne = CreateMacroStabilityInwardsSoilLayer2D();
            MacroStabilityInwardsSoilLayer2D layerTwo = CreateMacroStabilityInwardsSoilLayer2D();
            var layers = new[]
            {
                layerOne,
                layerTwo
            };

            var stresses = new[]
            {
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress()
            };

            var soilProfile = new MacroStabilityInwardsSoilProfile2D("<some name>", layers, stresses);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, soilProfile);

            // Call
            var properties = new MacroStabilityInwardsStochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            Assert.AreEqual(soilProfile.Name, properties.Name);
            Assert.AreEqual(soilProfile.Name, properties.ToString());

            Assert.AreEqual(layers.Length, properties.Layers2D.Length);
            Assert.AreSame(layerOne, properties.Layers2D[0].Data);
            Assert.AreSame(layerTwo, properties.Layers2D[1].Data);

            CollectionAssert.IsEmpty(properties.Layers1D);
            Assert.AreEqual(double.NaN.ToString(CultureInfo.CurrentCulture), properties.Bottom);

            Assert.AreEqual(expectedProbability, properties.Probability);
            Assert.AreEqual("2D profiel", properties.Type);

            TestHelper.AssertTypeConverter<MacroStabilityInwardsStochasticSoilProfileProperties,
                ExpandableArrayConverter>(nameof(MacroStabilityInwardsStochasticSoilProfileProperties.PreconsolidationStresses));
            Assert.AreEqual(stresses.Length, properties.PreconsolidationStresses.Length);
            Assert.AreSame(stresses[0], properties.PreconsolidationStresses[0].Data);
        }

        private static MacroStabilityInwardsSoilLayer2D CreateMacroStabilityInwardsSoilLayer2D()
        {
            return new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                        {
                                                            new Point2D(20.210230, 26.00001),
                                                            new Point2D(3.830, 1.040506)
                                                        }),
                                                        new Ring[0]);
        }

        private class UnsupportedSoilProfile : IMacroStabilityInwardsSoilProfile
        {
            public string Name { get; }
        }
    }
}