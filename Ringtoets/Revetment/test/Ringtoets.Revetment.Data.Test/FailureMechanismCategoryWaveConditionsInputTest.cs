﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Revetment.Data.TestUtil;

namespace Ringtoets.Revetment.Data.Test
{
    [TestFixture]
    public class FailureMechanismCategoryWaveConditionsInputTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var waveConditionsInput = new FailureMechanismCategoryWaveConditionsInput();

            // Assert
            Assert.IsInstanceOf<WaveConditionsInput>(waveConditionsInput);
            Assert.AreEqual((FailureMechanismCategoryType) 0, waveConditionsInput.CategoryType);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new FailureMechanismCategoryWaveConditionsInput();

            WaveConditionsTestDataGenerator.SetRandomDataToWaveConditionsInput(original);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, WaveConditionsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new FailureMechanismCategoryWaveConditionsInput();

            WaveConditionsTestDataGenerator.SetRandomDataToWaveConditionsInput(original);

            original.ForeshoreProfile = null;
            original.HydraulicBoundaryLocation = null;

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, WaveConditionsCloneAssert.AreClones);
        }
    }
}