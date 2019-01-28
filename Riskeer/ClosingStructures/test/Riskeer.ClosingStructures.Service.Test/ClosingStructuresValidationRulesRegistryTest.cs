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
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Service.ValidationRules;

namespace Ringtoets.ClosingStructures.Service.Test
{
    [TestFixture]
    public class ClosingStructuresValidationRulesRegistryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var registry = new ClosingStructuresValidationRulesRegistry();

            // Assert
            Assert.IsInstanceOf<IStructuresValidationRulesRegistry<ClosingStructuresInput, ClosingStructure>>(registry);
        }

        [Test]
        public void GetValidationRules_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new ClosingStructuresValidationRulesRegistry();

            // Call
            TestDelegate test = () => registry.GetValidationRules(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void GetValidationRules_InvalidInflowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var registry = new ClosingStructuresValidationRulesRegistry();

            // Call
            TestDelegate test = () => registry.GetValidationRules(new ClosingStructuresInput
            {
                InflowModelType = (ClosingStructureInflowModelType) 99
            });

            // Assert
            const string message = "The value of argument 'input' (99) is invalid for Enum type 'ClosingStructureInflowModelType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        public void GetValidationRules_InflowModelTypeVerticalWall_ReturnsValidationRules()
        {
            // Setup
            var registry = new ClosingStructuresValidationRulesRegistry();

            // Call
            IEnumerable<ValidationRule> rules = registry.GetValidationRules(new ClosingStructuresInput
            {
                InflowModelType = ClosingStructureInflowModelType.VerticalWall
            });

            // Assert
            ValidationRule[] validationRules = rules.ToArray();

            Assert.AreEqual(11, validationRules.Length);

            Assert.IsInstanceOf<UseBreakWaterRule>(validationRules[0]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[1]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[2]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[3]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[4]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[5]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[6]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[7]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[8]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[9]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[10]);
        }

        [Test]
        public void GetValidationRules_InflowModelTypeLowSill_ReturnsValidationRules()
        {
            // Setup
            var registry = new ClosingStructuresValidationRulesRegistry();

            // Call
            IEnumerable<ValidationRule> rules = registry.GetValidationRules(new ClosingStructuresInput
            {
                InflowModelType = ClosingStructureInflowModelType.LowSill
            });

            // Assert
            ValidationRule[] validationRules = rules.ToArray();

            Assert.AreEqual(11, validationRules.Length);

            Assert.IsInstanceOf<UseBreakWaterRule>(validationRules[0]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[1]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[2]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[3]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[4]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[5]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[6]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[7]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[8]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[9]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[10]);
        }

        [Test]
        public void GetValidationRules_InflowModelTypeFloodedCulvert_ReturnsValidationRules()
        {
            // Setup
            var registry = new ClosingStructuresValidationRulesRegistry();

            // Call
            IEnumerable<ValidationRule> rules = registry.GetValidationRules(new ClosingStructuresInput
            {
                InflowModelType = ClosingStructureInflowModelType.FloodedCulvert
            });

            // Assert
            ValidationRule[] validationRules = rules.ToArray();

            Assert.AreEqual(10, validationRules.Length);

            Assert.IsInstanceOf<UseBreakWaterRule>(validationRules[0]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[1]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[2]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[3]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[4]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[5]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[6]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[7]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[8]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[9]);
        }
    }
}