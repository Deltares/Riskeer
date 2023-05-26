﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.TestUtil;
using Core.Common.Util.Enums;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PropertyClasses;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Forms.Test.PropertyClasses
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
            TestHelper.AssertTypeConverter<PipingStochasticSoilProfileProperties,
                EnumTypeConverter>(nameof(PipingStochasticSoilProfileProperties.Type));
            Assert.AreSame(stochasticSoilProfile, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var layerOne = new PipingSoilLayer(4);
            var layerTwo = new PipingSoilLayer(2);

            IEnumerable<PipingSoilLayer> layers = new[]
            {
                layerOne,
                layerTwo
            };

            var random = new Random(21);
            double bottom = random.NextDouble();
            double probability = random.NextDouble();
            var soilProfileType = random.NextEnumValue<SoilProfileType>();
            var soilProfile = new PipingSoilProfile("<some name>", bottom, layers, soilProfileType);
            var stochasticSoilProfile = new PipingStochasticSoilProfile(probability, soilProfile);

            // Call
            var properties = new PipingStochasticSoilProfileProperties(stochasticSoilProfile);

            // Assert
            Assert.AreEqual(soilProfile.Name, properties.Name);
            Assert.AreEqual(soilProfile.Name, properties.ToString());

            Assert.AreEqual(2, properties.Layers.Length);
            Assert.AreSame(layerOne, properties.Layers[0].Data);
            Assert.AreSame(layerTwo, properties.Layers[1].Data);

            Assert.AreEqual(2, properties.Bottom.NumberOfDecimalPlaces);
            Assert.AreEqual(bottom, properties.Bottom, properties.Bottom.GetAccuracy());
            Assert.AreEqual(2, properties.Probability.NumberOfDecimalPlaces);
            Assert.AreEqual(probability * 100, properties.Probability, properties.Probability.GetAccuracy());
            Assert.AreEqual(soilProfileType, properties.Type);
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
    }
}