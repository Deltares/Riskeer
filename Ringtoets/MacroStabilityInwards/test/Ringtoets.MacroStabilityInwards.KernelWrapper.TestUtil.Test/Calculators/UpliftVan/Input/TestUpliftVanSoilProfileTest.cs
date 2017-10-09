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

using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Input;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Calculators.UpliftVan.Input
{
    [TestFixture]
    public class TestUpliftVanSoilProfileTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var profile = new TestUpliftVanSoilProfile();

            // Assert
            Assert.IsInstanceOf<UpliftVanSoilProfile>(profile);
            Assert.AreEqual(1, profile.Layers.Count());
            UpliftVanSoilLayer layer = profile.Layers.First();
            CollectionAssert.IsEmpty(layer.OuterRing);
            CollectionAssert.IsEmpty(layer.Holes);
            CollectionAssert.IsEmpty(profile.PreconsolidationStresses);
        }
    }
}