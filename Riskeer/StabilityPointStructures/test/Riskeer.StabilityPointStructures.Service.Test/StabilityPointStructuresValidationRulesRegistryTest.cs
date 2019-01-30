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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Service.ValidationRules;
using Riskeer.StabilityPointStructures.Data;

namespace Riskeer.StabilityPointStructures.Service.Test
{
    [TestFixture]
    public class StabilityPointStructuresValidationRulesRegistryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var registry = new StabilityPointStructuresValidationRulesRegistry();

            // Assert
            Assert.IsInstanceOf<IStructuresValidationRulesRegistry<StabilityPointStructuresInput, StabilityPointStructure>>(registry);
        }

        [Test]
        public void GetValidationRules_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new StabilityPointStructuresValidationRulesRegistry();

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
            var registry = new StabilityPointStructuresValidationRulesRegistry();

            // Call
            TestDelegate test = () => registry.GetValidationRules(new StabilityPointStructuresInput
            {
                InflowModelType = (StabilityPointStructureInflowModelType) 99
            });

            // Assert
            const string message = "The value of argument 'input' (99) is invalid for Enum type 'StabilityPointStructureInflowModelType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        public void GetValidationRules_InvalidLoadSchematizationType_ThrowsInvalidEnumArgumentException(StabilityPointStructureInflowModelType inflowModelType)
        {
            // Setup
            var registry = new StabilityPointStructuresValidationRulesRegistry();

            // Call
            TestDelegate test = () => registry.GetValidationRules(new StabilityPointStructuresInput
            {
                InflowModelType = inflowModelType,
                LoadSchematizationType = (LoadSchematizationType) 99
            });

            // Assert
            const string message = "The value of argument 'input' (99) is invalid for Enum type 'LoadSchematizationType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void GetValidationRules_InflowModelTypeLowSill_ReturnValidationRules(LoadSchematizationType loadSchematizationType)
        {
            // Setup
            var registry = new StabilityPointStructuresValidationRulesRegistry();

            // Call
            IEnumerable<ValidationRule> rules = registry.GetValidationRules(new StabilityPointStructuresInput
            {
                InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                LoadSchematizationType = loadSchematizationType
            });

            // Assert
            ValidationRule[] validationRules = rules.ToArray();

            Assert.AreEqual(23, validationRules.Length);

            Assert.IsInstanceOf<UseBreakWaterRule>(validationRules[0]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[1]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[2]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[3]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[4]);
            Assert.IsInstanceOf<VariationCoefficientNormalDistributionRule>(validationRules[5]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[6]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[7]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[8]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[9]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[10]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[11]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[12]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[13]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[14]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[15]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[16]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[17]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[18]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[19]);
            Assert.IsInstanceOf<VariationCoefficientNormalDistributionRule>(validationRules[20]);
            Assert.IsInstanceOf<VariationCoefficientNormalDistributionRule>(validationRules[21]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[22]);
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void GetValidationRules_InflowModelTypeFloodedCulvert_ReturnValidationRules(LoadSchematizationType loadSchematizationType)
        {
            // Setup
            var registry = new StabilityPointStructuresValidationRulesRegistry();

            // Call
            IEnumerable<ValidationRule> rules = registry.GetValidationRules(new StabilityPointStructuresInput
            {
                InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                LoadSchematizationType = loadSchematizationType
            });

            // Assert
            ValidationRule[] validationRules = rules.ToArray();

            Assert.AreEqual(24, validationRules.Length);

            Assert.IsInstanceOf<UseBreakWaterRule>(validationRules[0]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[1]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[2]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[3]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[4]);
            Assert.IsInstanceOf<VariationCoefficientNormalDistributionRule>(validationRules[5]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[6]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[7]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[8]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[9]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[10]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[11]);
            Assert.IsInstanceOf<LogNormalDistributionRule>(validationRules[12]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[13]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[14]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[15]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[16]);
            Assert.IsInstanceOf<NormalDistributionRule>(validationRules[17]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[18]);
            Assert.IsInstanceOf<NumericInputRule>(validationRules[19]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[20]);
            Assert.IsInstanceOf<VariationCoefficientNormalDistributionRule>(validationRules[21]);
            Assert.IsInstanceOf<VariationCoefficientNormalDistributionRule>(validationRules[22]);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionRule>(validationRules[23]);
        }
    }
}