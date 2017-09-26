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
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressPropertiesTest
    {
        [Test]
        public void Constructor_PreconsolidationStressNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsPreconsolidationStressProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("preconsolidationStress", exception.ParamName);
        }

        [Test]
        public void Constructor_WithPreconsolidationStress_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var stress = new MacroStabilityInwardsPreconsolidationStress(random.NextDouble(),
                                                                         random.NextDouble(),
                                                                         random.NextDouble(),
                                                                         random.NextDouble());

            // Call
            var properties = new MacroStabilityInwardsPreconsolidationStressProperties(stress);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsPreconsolidationStress>>(properties);
            Assert.AreSame(stress, properties.Data);

            Assert.AreEqual(2, properties.XCoordinate.NumberOfDecimalPlaces);
            Assert.AreEqual(stress.Location.X, properties.XCoordinate, properties.XCoordinate.GetAccuracy());
            Assert.AreEqual(2, properties.ZCoordinate.NumberOfDecimalPlaces);
            Assert.AreEqual(stress.Location.Y, properties.ZCoordinate, properties.ZCoordinate.GetAccuracy());

            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionProperties>(properties.PreconsolidationStress);
            Assert.AreSame(stress.Stress, properties.PreconsolidationStress.Data);
        }

        [Test]
        public void Constructor_WithPreconsolidationStress_PropertiesHaveExpectedAttributeValues()
        {
            // Call
            var properties = new MacroStabilityInwardsPreconsolidationStressProperties(
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[0],
                generalCategory,
                "X [m]",
                "Lokale x-coördinaat van het punt waarvoor de grensspanning is opgegeven.",
                true);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[1],
                generalCategory,
                "Z [m+NAP]",
                "Lokale z-coördinaat van het punt waarvoor de grensspanning is opgegeven.",
                true);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[2],
                generalCategory,
                "Grensspanning [kN/m²]",
                "Grensspanning op de aangegeven locatie.",
                true);
        }

        [Test]
        public void ToString_Always_ReturnsEmptyString()
        {
            // Setup
            var properties = new MacroStabilityInwardsPreconsolidationStressProperties(
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress());

            // Call
            string result = properties.ToString();

            // Assert
            Assert.IsEmpty(result);
        }
    }
}