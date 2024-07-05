﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableDataModelFactoryTest
    {
        [Test]
        public void Create_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableDataModelFactory.Create(null, new GeneralMacroStabilityInwardsInput(), AssessmentSectionTestHelper.GetTestAssessmentLevel, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Create_GeneralInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableDataModelFactory.Create(new MacroStabilityInwardsCalculation(), null, AssessmentSectionTestHelper.GetTestAssessmentLevel, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
        }

        [Test]
        public void Create_GetAssessmentLevelFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableDataModelFactory.Create(new MacroStabilityInwardsCalculation(), new GeneralMacroStabilityInwardsInput(), null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getNormativeAssessmentLevelFunc", exception.ParamName);
        }

        [Test]
        public void Create_CalculationWithoutOutput_ThrowsInvalidOperationException()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());

            // Call
            void Call() => PersistableDataModelFactory.Create(calculation, new GeneralMacroStabilityInwardsInput(), AssessmentSectionTestHelper.GetTestAssessmentLevel, string.Empty);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(Call);
            Assert.AreEqual("Calculation must have output.", exception.Message);
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableDataModel()
        {
            // Setup
            const string filePath = "ValidFilePath";
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                PersistableDataModel persistableDataModel = PersistableDataModelFactory.Create(calculation, new GeneralMacroStabilityInwardsInput(), AssessmentSectionTestHelper.GetTestAssessmentLevel, filePath);

                // Assert
                PersistableDataModelTestHelper.AssertPersistableDataModel(calculation, filePath, persistableDataModel);
            }
        }
    }
}