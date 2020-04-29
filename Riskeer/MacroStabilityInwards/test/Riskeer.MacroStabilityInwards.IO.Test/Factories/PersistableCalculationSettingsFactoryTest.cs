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
using System.Linq;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableCalculationSettingsFactoryTest
    {
        [Test]
        public void Create_SlidingCurveNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableCalculationSettingsFactory.Create(null, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("slidingCurve", exception.ParamName);
        }

        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableCalculationSettingsFactory.Create(MacroStabilityInwardsOutputTestFactory.CreateOutput().SlidingCurve,
                                                                        null, new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableCalculationSettingsFactory.Create(MacroStabilityInwardsOutputTestFactory.CreateOutput().SlidingCurve,
                                                                        new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableCalculationSettingsCollection()
        {
            // Setup
            MacroStabilityInwardsSlidingCurve slidingCurve = MacroStabilityInwardsOutputTestFactory.CreateOutput().SlidingCurve;
            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            // Call
            IEnumerable<PersistableCalculationSettings> settingsCollection = PersistableCalculationSettingsFactory.Create(slidingCurve, idFactory, registry);

            // Assert
            PersistableDataModelTestHelper.AssertCalculationSettings(slidingCurve, settingsCollection);

            var stages = new[]
            {
                MacroStabilityInwardsExportStageType.Daily,
                MacroStabilityInwardsExportStageType.Extreme
            };

            Assert.AreEqual(2, registry.Settings.Count);

            for (var i = 0; i < stages.Length; i++)
            {
                Assert.AreEqual(registry.Settings[stages[i]], settingsCollection.ElementAt(i).Id);
            }
        }
    }
}