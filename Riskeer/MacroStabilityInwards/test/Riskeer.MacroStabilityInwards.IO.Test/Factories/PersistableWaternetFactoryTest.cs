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
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableWaternetFactoryTest
    {
        [Test]
        public void Create_DailyWaternetNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetFactory.Create(null, new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0], new MacroStabilityInwardsWaternetLine[0]),
                                                             new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dailyWaternet", exception.ParamName);
        }
        [Test]
        public void Create_ExtremeWaternetNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetFactory.Create(new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0], new MacroStabilityInwardsWaternetLine[0]),
                                                             null, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("extremeWaternet", exception.ParamName);
        }

        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Setup
            var waternet = new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0], new MacroStabilityInwardsWaternetLine[0]);

            // Call
            void Call() => PersistableWaternetFactory.Create(waternet, waternet, null, new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var waternet = new MacroStabilityInwardsWaternet(new MacroStabilityInwardsPhreaticLine[0], new MacroStabilityInwardsWaternetLine[0]);

            // Call
            void Call() => PersistableWaternetFactory.Create(waternet, waternet, new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }
    }
}