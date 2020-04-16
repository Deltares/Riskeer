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
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableStageFactoryTest
    {
        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableStageFactory.Create(null, new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableStageFactory.Create(new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithValidData_ReturnsStages()
        {
            // Setup
            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            var stageTypes = new[]
            {
                MacroStabilityInwardsExportStageType.Daily,
                MacroStabilityInwardsExportStageType.Extreme
            };

            var settingsList = new List<PersistableCalculationSettings>();

            foreach (MacroStabilityInwardsExportStageType stageType in stageTypes)
            {
                var settings = new PersistableCalculationSettings
                {
                    Id = idFactory.Create()
                };
                settingsList.Add(settings);
                registry.Add(stageType, settings.Id);
            }

            // Call
            IEnumerable<PersistableStage> stages = PersistableStageFactory.Create(idFactory, registry);

            // Assert
            PersistableDataModelTestHelper.AssertStages(stages, settingsList);
        }
    }
}