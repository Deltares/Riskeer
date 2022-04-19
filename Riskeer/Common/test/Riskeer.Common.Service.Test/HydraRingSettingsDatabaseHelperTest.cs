﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class HydraRingSettingsDatabaseHelperTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.Service);

        [Test]
        public void AssignSettingsFromDatabase_FileWithInvalidCharacters_ThrowsArgumentException()
        {
            // Call
            void Call() => HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(new TestHydraRingCalculationInput(), ">", false);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void AssignSettingsFromDatabase_FileWithoutSettingsDatabase_ThrowsCriticalFileReadException()
        {
            // Call
            void Call() => HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(new TestHydraRingCalculationInput(), "NoConfig.sqlite", false);

            // Assert
            Assert.Throws<CriticalFileReadException>(Call);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssignSettingsFromDatabase_FileWithEmptySettingsDatabase_DefaultsSettingsAdded(bool usePreprocessor)
        {
            // Setup
            var input = new TestHydraRingCalculationInput();

            // Call
            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(input, Path.Combine(testDataPath, "hrd.sqlite"), usePreprocessor);

            // Assert
            Assert.NotNull(input.DesignTablesSetting);
            Assert.NotNull(input.NumericsSettings);
            Assert.NotNull(input.TimeIntegrationSetting);
            Assert.NotNull(input.PreprocessorSetting);
            Assert.AreEqual(usePreprocessor, input.PreprocessorSetting.RunPreprocessor);
        }
    }

    public class TestHydraRingCalculationInput : HydraRingCalculationInput
    {
        public TestHydraRingCalculationInput() : base(2) {}

        public override HydraRingFailureMechanismType FailureMechanismType => HydraRingFailureMechanismType.AssessmentLevel;

        public override int CalculationTypeId => 1;

        public override int VariableId => 2;

        public override int FaultTreeModelId => 3;

        public override HydraRingSection Section => null;
    }
}