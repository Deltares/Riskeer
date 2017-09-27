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

using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer2DTestFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsSoilLayer2D_ReturnsExpectedMacroStabilityInwardsSoilLayer2D()
        {
            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D();

            // Assert
            Assert.IsNotNull(soilLayer);
            Assert.AreEqual(typeof(MacroStabilityInwardsSoilLayer2D), soilLayer.GetType());
            Assert.AreEqual(new Ring(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }), soilLayer.OuterRing);
            CollectionAssert.IsEmpty(soilLayer.Holes);
        }
    }
}