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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StochasticSoilProfilePropertiesTest
    {
        private const string generalCategoryName = "Algemeen";

        [Test]
        public void Constructor_StochasticSoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StochasticSoilProfileProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochasticSoilProfile", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidStochasticSoilProfile_ExpectedValues()
        {
            // Setup
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(1.0, SoilProfileType.SoilProfile2D, 1234L);

            // Call
            var properties = new StochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsStochasticSoilProfile>>(properties);
            TestHelper.AssertTypeConverter<StochasticSoilProfileProperties, ExpandableArrayConverter>(nameof(StochasticSoilProfileProperties.Layers1D));
            TestHelper.AssertTypeConverter<StochasticSoilProfileProperties, ExpandableArrayConverter>(nameof(StochasticSoilProfileProperties.Layers2D));
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

            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", -5.0, layers, SoilProfileType.SoilProfile1D, 0);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1234L)
            {
                SoilProfile = soilProfile
            };
            var properties = new StochasticSoilProfileProperties(stochasticSoilProfile);

            // Call
            bool bottomVisible = properties.DynamicVisibleValidationMethod("Bottom");
            bool layers1DVisible = properties.DynamicVisibleValidationMethod("Layers1D");
            bool layers2DVisible = properties.DynamicVisibleValidationMethod("Layers2D");
            bool otherVisible = properties.DynamicVisibleValidationMethod("");

            // Assert
            Assert.IsTrue(bottomVisible);
            Assert.IsTrue(layers1DVisible);
            Assert.IsFalse(layers2DVisible);
            Assert.IsFalse(otherVisible);
        }

        [Test]
        public void DynamicVisibleValidationMethod_WithSoilProfile2D_Only2DPropertiesVisible()
        {
            // Setup
            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers = new[]
            {
                new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(3.0, 1.0)
                }), new Ring[0])
            };

            var soilProfile = new MacroStabilityInwardsSoilProfile2D("name", layers, SoilProfileType.SoilProfile2D, 1234L);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1234L)
            {
                SoilProfile = soilProfile
            };
            var properties = new StochasticSoilProfileProperties(stochasticSoilProfile);

            // Call
            bool bottomVisible = properties.DynamicVisibleValidationMethod("Bottom");
            bool layers1DVisible = properties.DynamicVisibleValidationMethod("Layers1D");
            bool layers2DVisible = properties.DynamicVisibleValidationMethod("Layers2D");
            bool otherVisible = properties.DynamicVisibleValidationMethod("");

            // Assert
            Assert.IsFalse(bottomVisible);
            Assert.IsFalse(layers1DVisible);
            Assert.IsTrue(layers2DVisible);
            Assert.IsFalse(otherVisible);
        }

        [Test]
        public void VisibleProperties_WithSoilProfile1D_ExpectedAttributesValues()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer1D(-2);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.142, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D("<some name>", -5.0, new[]
                {
                    layer
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            var properties = new StochasticSoilProfileProperties(stochasticSoilProfile);

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
            var layer = new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                             {
                                                                 new Point2D(20.210230, 26.00001),
                                                                 new Point2D(3.830, 1.040506)
                                                             }),
                                                             new Ring[0]);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.142, SoilProfileType.SoilProfile2D, 0)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile2D("<some name>", new[]
                {
                    layer
                }, SoilProfileType.SoilProfile2D, 0)
            };

            // Call
            var properties = new StochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

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
        }

        [Test]
        public void GetProperties_WithUnsupportedSoilProfile_ThrowsNotSupportedException()
        {
            // Setup
            var soilProfile = new UnsupportedSoilProfile();
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1234L)
            {
                SoilProfile = soilProfile
            };

            // Call
            var properties = new StochasticSoilProfileProperties(stochasticSoilProfile);

            string type = null;

            // Assert
            TestDelegate call = () => type = properties.Type;

            var exception = Assert.Throws<NotSupportedException>(call);
            Assert.AreEqual("Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses.StochasticSoilProfilePropertiesTest+UnsupportedSoilProfile " +
                            "is not supported. Supported types: MacroStabilityInwardsSoilProfile1D and MacroStabilityInwardsSoilProfile2D.", 
                            exception.Message);
        }

        private static void GetProperties_WithSoilProfile1D_ReturnExpectedValues(double probability, string expectedProbability)
        {
            // Setup
            const string expectedName = "<some name>";
            var layerOne = new MacroStabilityInwardsSoilLayer1D(-2);
            var layerTwo = new MacroStabilityInwardsSoilLayer1D(-4);
            IEnumerable<MacroStabilityInwardsSoilLayer1D> layers = new[]
            {
                layerOne,
                layerTwo
            };

            var soilProfile = new MacroStabilityInwardsSoilProfile1D(expectedName, -5.0, layers, SoilProfileType.SoilProfile1D, 0);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, 1234L)
            {
                SoilProfile = soilProfile
            };

            // Call
            var properties = new StochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            Assert.AreEqual(expectedName, properties.Name);
            Assert.AreEqual(expectedName, properties.ToString());

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
            const string expectedName = "<some name>";
            var layerOne = new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                                {
                                                                    new Point2D(20.210230, 26.00001),
                                                                    new Point2D(3.830, 1.040506)
                                                                }),
                                                                new Ring[0]);

            var layerTwo = new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                                {
                                                                    new Point2D(20.210230, 26.00001),
                                                                    new Point2D(3.830, 1.040506)
                                                                }),
                                                                new Ring[0]);
            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers = new[]
            {
                layerOne,
                layerTwo
            };

            var soilProfile = new MacroStabilityInwardsSoilProfile2D(expectedName, layers, SoilProfileType.SoilProfile2D, 0);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, SoilProfileType.SoilProfile2D, 0)
            {
                SoilProfile = soilProfile
            };

            // Call
            var properties = new StochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            Assert.AreEqual(expectedName, properties.Name);
            Assert.AreEqual(expectedName, properties.ToString());

            Assert.AreEqual(2, properties.Layers2D.Length);
            Assert.AreSame(layerOne, properties.Layers2D[0].Data);
            Assert.AreSame(layerTwo, properties.Layers2D[1].Data);

            CollectionAssert.IsEmpty(properties.Layers1D);
            Assert.AreEqual(double.NaN.ToString(CultureInfo.CurrentCulture), properties.Bottom);

            Assert.AreEqual(expectedProbability, properties.Probability);
            Assert.AreEqual("2D profiel", properties.Type);
        }

        private class UnsupportedSoilProfile : IMacroStabilityInwardsSoilProfile
        {
            public string Name { get; }
        }
    }
}