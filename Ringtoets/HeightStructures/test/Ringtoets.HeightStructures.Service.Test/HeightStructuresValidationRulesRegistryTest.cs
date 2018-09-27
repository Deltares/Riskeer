// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.Service.Test
{
    [TestFixture]
    public class HeightStructuresValidationRulesRegistryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var registry = new HeightStructuresValidationRulesRegistry();

            // Assert
            Assert.IsInstanceOf<IStructuresValidationRulesRegistry<HeightStructuresInput, HeightStructure>>(registry);
        }

        [Test]
        public void GetValidationRules_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new HeightStructuresValidationRulesRegistry();

            // Call
            TestDelegate test = () => registry.GetValidationRules(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void GetValidationRules_WithInput_ReturnsValidationRules()
        {
            // Setup
            var registry = new HeightStructuresValidationRulesRegistry();

            // Call
            IEnumerable<ValidationRule> rules = registry.GetValidationRules(new HeightStructuresInput());

            // Assert
            ValidationRule[] validationRules = rules.ToArray();
            Assert.AreEqual(10, validationRules.Length);

            Assert.IsInstanceOf<UseBreakWaterRule>(validationRules[0]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[1]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[2]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[3]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[4]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[5]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[6]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[7]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[8]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[9]);
        }
    }
}