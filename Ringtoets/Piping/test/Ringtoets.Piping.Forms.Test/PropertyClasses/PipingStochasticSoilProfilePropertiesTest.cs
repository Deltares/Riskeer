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
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingStochasticSoilProfilePropertiesTest
    {
        [Test]
        public void Constructor_StochasticSoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingStochasticSoilProfileProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochasticSoilProfile", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidStochasticSoilProfile_ExpectedValues()
        {
            // Setup
            var stochasticSoilProfile = new PipingStochasticSoilProfile(1, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            // Call
            var properties = new PipingStochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingStochasticSoilProfile>>(properties);
            TestHelper.AssertTypeConverter<PipingStochasticSoilProfileProperties,
                ExpandableArrayConverter>(nameof(PipingStochasticSoilProfileProperties.Layers));
            Assert.AreSame(stochasticSoilProfile, properties.Data);
        }

        [Test]
        [TestCase(0.142, "14,2")]
        [TestCase(1.0, "100")]
        [TestCase(0.5 + 1e-6, "50")]
        [SetCulture("nl-NL")]
        public void GetProperties_WithDataAndDutchLocale_ReturnExpectedValues(double probability, string expectedProbability)
        {
            GetProperties_WithData_ReturnExpectedValues(probability, expectedProbability);
        }

        [Test]
        [TestCase(0.142, "14.2")]
        [TestCase(1.0, "100")]
        [TestCase(0.5 + 1e-6, "50")]
        [SetCulture("en-US")]
        public void GetProperties_WithDataAndEnglishLocale_ReturnExpectedValues(double probability, string expectedProbability)
        {
            GetProperties_WithData_ReturnExpectedValues(probability, expectedProbability);
        }

        [Test]
        [TestCase(SoilProfileType.SoilProfile1D, "1D profiel")]
        [TestCase(SoilProfileType.SoilProfile2D, "2D profiel")]
        public void Type_WithDifferentSoilProfileTypes_ReturnsExpectedValues(SoilProfileType soilProfileType,
                                                                             string expectedValue)
        {
            // Setup
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.0, new PipingSoilProfile("", 0.0, new[]
            {
                new PipingSoilLayer(10.0)
            }, soilProfileType));

            var properties = new PipingStochasticSoilProfileProperties(stochasticSoilProfile);

            // Call
            string typeValue = properties.Type;

            // Assert
            Assert.AreEqual(expectedValue, typeValue);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var pipingStochasticSoilProfile = new PipingStochasticSoilProfile(0.0, new PipingSoilProfile("", 0.0, new[]
            {
                new PipingSoilLayer(10.0)
            }, SoilProfileType.SoilProfile1D));

            // Call
            var properties = new PipingStochasticSoilProfileProperties(pipingStochasticSoilProfile);

            // Assert
            const string generalCategoryName = "Algemeen";
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

        private static void GetProperties_WithData_ReturnExpectedValues(double probability, string expectedProbability)
        {
            // Setup
            var layerOne = new PipingSoilLayer(-2);
            var layerTwo = new PipingSoilLayer(-4);

            IEnumerable<PipingSoilLayer> layers = new[]
            {
                layerOne,
                layerTwo
            };

            var soilProfile = new PipingSoilProfile("<some name>", -5.0, layers, SoilProfileType.SoilProfile1D);
            var stochasticSoilProfile = new PipingStochasticSoilProfile(probability, soilProfile);

            // Call
            var properties = new PipingStochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            Assert.AreEqual(soilProfile.Name, properties.Name);
            Assert.AreEqual(soilProfile.Name, properties.ToString());
            Assert.AreEqual(2, properties.Layers.Length);
            Assert.AreSame(layerOne, properties.Layers[0].Data);
            Assert.AreSame(layerTwo, properties.Layers[1].Data);

            Assert.AreEqual(soilProfile.Bottom, properties.Bottom);
            Assert.AreEqual(expectedProbability, properties.Probability);
        }
    }
}