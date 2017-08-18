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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StochasticSoilProfilePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new StochasticSoilProfileProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsStochasticSoilProfile>>(properties);
            Assert.IsNull(properties.Data);
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

        private static void GetProperties_WithData_ReturnExpectedValues(double probability, string expectedProbability)
        {
            // Setup
            const string expectedName = "<some name>";
            IEnumerable<MacroStabilityInwardsSoilLayer1D> layers = new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-2),
                new MacroStabilityInwardsSoilLayer1D(-4)
                {
                    Properties =
                    {
                        IsAquifer = true
                    }
                }
            };

            var soilProfile = new MacroStabilityInwardsSoilProfile1D(expectedName, -5.0, layers, SoilProfileType.SoilProfile1D, 0);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, 1234L)
            {
                SoilProfile = soilProfile
            };

            // Call
            var properties = new StochasticSoilProfileProperties
            {
                Data = stochasticSoilProfile
            };

            // Assert
            Assert.AreEqual(expectedName, properties.Name);
            Assert.AreEqual(expectedName, properties.ToString());
            CollectionAssert.AreEqual(soilProfile.Layers.Select(l => l.Top), properties.TopLevels);
            Assert.AreEqual(soilProfile.Bottom, properties.Bottom);
            Assert.AreEqual(expectedProbability, properties.Probability);
            Assert.AreEqual("1D profiel", properties.Type);
        }
    }
}